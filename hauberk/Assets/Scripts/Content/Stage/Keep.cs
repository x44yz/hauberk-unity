using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// Places a number of connected rooms.
class Keep : RoomArchitecture {
  public static JunctionSet? debugJunctions;

  public JunctionSet _junctions;

  int _placedRooms = 0;

  int? _maxRooms;

  public static Keep create(int? maxRooms = null) {
    if (maxRooms != null) {
      // TODO: For now, small keeps always pack rooms in densely. Have
      // different styles of keep for different monsters?
      return Keep._(rng.triangleInt(maxRooms, maxRooms ~/ 2), TakeFrom.oldest);
    } else {
      // TODO: Do we still need this case? Do we want keeps that span the whole
      // dungeon?
      return Keep._(null, rng.item(TakeFrom.values));
    }
  }

  Keep(int _maxRooms, TakeFrom takeFrom)
  {
    this._maxRooms = _maxRooms;
    _junctions = JunctionSet(takeFrom);
  }

  // TODO: Different paint styles for different monsters.
  public override PaintStyle paintStyle => PaintStyle.granite;

  public override IEnumerable<string> build() {
    var rt = new List<string>();

    debugJunctions = _junctions;

    // If we are covering the whole area, attempt to place multiple rooms.
    // That way, if there disconnected areas (like a river cutting through the
    // stage, we can still hopefully cover it all.
    var startingRooms = 1;
    if (region == Region.everywhere && _maxRooms == null) {
      startingRooms = 20;
    }

    for (var i = 0; i < startingRooms; i++) {
      rt.Add(_growRooms());
    }

    return rt;
  }

  bool spawnMonsters(Painter painter) {
    var tiles = painter.ownedTiles
        .Where((pos) => painter.getTile(pos).isWalkable)
        .ToList();
    Rng.rng.shuffle(tiles);

    foreach (var pos in tiles) {
      // TODO: Make this tunable?
      if (!Rng.rng.oneIn(20)) continue;

      var group = Rng.rng.item(style.monsterGroups);
      var breed = painter.chooseBreed(painter.depth, tag: group);
      painter.spawnMonster(pos, breed);
    }

    return true;
  }

  IEnumerable<string> _growRooms() {
    var rt = new List<string>();

    if (!_tryPlaceStartingRoom()) return rt;

    // Expand outward from it.
    while (_junctions.isNotEmpty) {
      var junction = _junctions.takeNext();

      // Make sure the junction is still valid. If other stuff has been placed
      // in its way since then, discard it.
      if (!canCarve(junction.position + junction.direction)) continue;

      if (_tryAttachRoom(junction)) {
        rt.Add("Room");

        _placedRooms++;
        if (_maxRooms != null && _placedRooms >= _maxRooms!) break;
      } else {
        // Couldn't place the room, but maybe try the junction again.
        // TODO: Make tunable.
        if (junction.tries < 5) _junctions.Add(junction);
      }
    }

    return rt;
  }

  bool _tryPlaceStartingRoom() {
    var room = Room.create(depth);
    for (var i = 0; i < 100; i++) {
      var pos = _startLocation(room);
      if (_tryPlaceRoom(room, pos.x, pos.y)) return true;
    }

    return false;
  }

  /// Pick a random location for [room] in [region].
  Vec _startLocation(Array2D<RoomTile> room) {
    var xMin = 1;
    var xMax = width - room.width - 1;
    var yMin = 1;
    var yMax = height - room.height - 1;

    if (region == Region.nw ||
      region == Region.n ||
      region == Region.ne)
      {
        yMax = Math.Max(1, (int)(height * 0.25) - room.height);
      }
    else if (region == Region.sw ||
      region == Region.s ||
      region == Region.se)
    {
        yMin = (int)(height * 0.75);
    }

    switch (region) {
      case Region.nw:
      case Region.w:
      case Region.sw:
        xMax = math.max(1, (width * 0.25).toInt() - room.width);
        break;
      case Region.ne:
      case Region.e:
      case Region.se:
        xMin = (width * 0.75).toInt();
        break;
    }

    if (xMax < xMin) xMax = xMin;
    if (yMax < yMin) yMax = yMin;

    return Vec(rng.range(xMin, xMax), rng.range(yMin, yMax));
  }

  /// Determines whether [pos] is within [region], with some randomness.
  bool _regionContains(Vec pos) {
    const min = -3.0;
    const max = 2.0;

    double diagonal(int xDistance, int yDistance) =>
        lerpDouble(xDistance + yDistance, 0, width + height, 2.0, -3.0);

    var density = 0.0;
    switch (region) {
      case Region.everywhere:
        return true;
      case Region.n:
        density = lerpDouble(pos.y, 0, height, max, min);
        break;
      case Region.ne:
        density = diagonal(width - pos.x - 1, pos.y);
        break;
      case Region.e:
        density = lerpDouble(pos.x, 0, width, min, max);
        break;
      case Region.se:
        density = diagonal(width - pos.x - 1, height - pos.y - 1);
        break;
      case Region.s:
        density = lerpDouble(pos.y, 0, height, min, max);
        break;
      case Region.sw:
        density = diagonal(pos.x, height - pos.y - 1);
        break;
      case Region.w:
        density = lerpDouble(pos.x, 0, width, max, min);
        break;
      case Region.nw:
        density = diagonal(pos.x, pos.y);
        break;
    }

    return rng.float(1.0) < density;
  }

  bool _tryAttachRoom(Junction junction) {
    var room = Room.create(depth);

    // Try to find a junction that can mate with this one.
    var direction = junction.direction.rotate180;
    var junctions =
        room.bounds.where((pos) => room[pos].direction == direction).toList();
    rng.shuffle(junctions);

    for (var pos in junctions) {
      // Calculate the room position by lining up the junctions.
      var roomPos = junction.position - pos;
      if (_tryPlaceRoom(room, roomPos.x, roomPos.y)) return true;
    }

    return false;
  }

  bool _tryPlaceRoom(Array2D<RoomTile> room, int x, int y) {
    if (!canPlaceRoom(room, x, y)) return false;

    var junctions = <Junction>[];

    for (var pos in room.bounds) {
      var here = pos.offset(x, y);
      var tile = room[pos];

      if (tile.isJunction) {
        // Don't grow outside of the chosen region.
        if (_regionContains(here)) {
          junctions.add(Junction(here, tile.direction));
        }
      } else if (tile.isTile) {
        carve(here.x, here.y, tile.tile);
      } else if (tile.isWall) {
        preventPassage(here);
        _junctions.removeAt(here);
      }
    }

    // Shuffle the junctions so that the order we traverse the room tiles
    // doesn't bias the room growth.
    rng.shuffle(junctions);
    for (var junction in junctions) {
      // Remove any existing junctions since they now clash with the room.
      _junctions.removeAt(junction.position);
      _junctions.add(junction);
    }

    return true;
  }
}

class Junction {
  /// The location of the junction.
  public Vec position;

  /// Points from the first room towards where the new room should be attached.
  ///
  /// A room must have an opposing junction in order to match.
  public Direction direction;

  /// How many times we've tried to place something at this junction.
  public int tries = 0;

  Junction(this.position, this.direction);
}

enum TakeFrom { newest, oldest, random }

class JunctionSet {
  public TakeFrom _takeFrom;
  public Map<Vec, Junction> _byPosition = {};
  public List<Junction> _junctions = [];

  JunctionSet(this._takeFrom);

  public bool isNotEmpty => _junctions.isNotEmpty;

  Junction? operator [](Vec pos) => _byPosition[pos];

  void add(Junction junction) {
    assert(_byPosition[junction.position] == null);

    _byPosition[junction.position] = junction;
    _junctions.add(junction);
  }

  public Junction takeNext() {
    Junction junction;
    switch (_takeFrom) {
      case TakeFrom.newest:
        junction = _junctions.removeLast();
        break;

      case TakeFrom.oldest:
        junction = _junctions.removeAt(0);
        break;

      case TakeFrom.random:
        junction = rng.take(_junctions);
        break;
    }

    _byPosition.remove(junction.position);
    junction.tries++;

    return junction;
  }

  void removeAt(Vec pos) {
    var junction = _byPosition.remove(pos);
    if (junction != null) _junctions.remove(junction);
  }
}
