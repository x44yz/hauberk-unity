using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


/// Places a number of connected rooms.
class Keep : RoomArchitecture
{
  public static JunctionSet debugJunctions;

  public JunctionSet _junctions;

  int _placedRooms = 0;

  int? _maxRooms;

  public static Keep create(int? maxRooms = null)
  {
    if (maxRooms != null)
    {
      // TODO: For now, small keeps always pack rooms in densely. Have
      // different styles of keep for different monsters?
      return new Keep(Rng.rng.triangleInt(maxRooms.Value, maxRooms.Value / 2), TakeFrom.oldest);
    }
    else
    {
      // TODO: Do we still need this case? Do we want keeps that span the whole
      // dungeon?
      var kk = Enum.GetValues(typeof(TakeFrom));
      return new Keep(null, Rng.rng.item<TakeFrom>(kk));
    }
  }

  public Keep(int? _maxRooms, TakeFrom takeFrom)
  {
    this._maxRooms = _maxRooms;
    _junctions = new JunctionSet(takeFrom);
  }

  // TODO: Different paint styles for different monsters.
  public override PaintStyle paintStyle => PaintStyle.granite;

  public override IEnumerator build()
  {
    debugJunctions = _junctions;

    // If we are covering the whole area, attempt to place multiple rooms.
    // That way, if there disconnected areas (like a river cutting through the
    // stage, we can still hopefully cover it all.
    var startingRooms = 1;
    if (region == Region.everywhere && _maxRooms == null)
    {
      startingRooms = 20;
    }

    for (var i = 0; i < startingRooms; i++)
    {
      yield return Main.Inst.StartCoroutine(_growRooms());
    }
  }

  public override bool spawnMonsters(Painter painter)
  {
    var tiles = painter.ownedTiles
        .Where((pos) => painter.getTile(pos).isWalkable)
        .ToList();
    Rng.rng.shuffle(tiles);

    foreach (var pos in tiles)
    {
      // TODO: Make this tunable?
      if (!Rng.rng.oneIn(20)) continue;

      var group = Rng.rng.item(style.monsterGroups);
      var breed = painter.chooseBreed(painter.depth, tag: group);
      painter.spawnMonster(pos, breed);
    }

    return true;
  }

  IEnumerator _growRooms()
  {
    if (!_tryPlaceStartingRoom()) yield break;

    // Expand outward from it.
    while (_junctions.isNotEmpty)
    {
      var junction = _junctions.takeNext();

      // Make sure the junction is still valid. If other stuff has been placed
      // in its way since then, discard it.
      if (!canCarve(junction.position + junction.direction)) continue;

      if (_tryAttachRoom(junction))
      {
        yield return "Room";

        _placedRooms++;
        if (_maxRooms != null && _placedRooms >= _maxRooms!) break;
      }
      else
      {
        // Couldn't place the room, but maybe try the junction again.
        // TODO: Make tunable.
        if (junction.tries < 5) _junctions.add(junction);
      }
    }
  }

  bool _tryPlaceStartingRoom()
  {
    var room = Room.create(depth);
    for (var i = 0; i < 100; i++)
    {
      var pos = _startLocation(room);
      if (_tryPlaceRoom(room, pos.x, pos.y)) return true;
    }

    return false;
  }

  /// Pick a random location for [room] in [region].
  Vec _startLocation(Array2D<RoomTile> room)
  {
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

    if (region == Region.nw ||
      region == Region.w ||
      region == Region.sw)
      xMax = Math.Max(1, (int)(width * 0.25) - room.width);
    else if (region == Region.ne ||
      region == Region.e ||
      region == Region.se)
      xMin = (int)(width * 0.75);

    if (xMax < xMin) xMax = xMin;
    if (yMax < yMin) yMax = yMin;

    return new Vec(Rng.rng.range(xMin, xMax), Rng.rng.range(yMin, yMax));
  }

  /// Determines whether [pos] is within [region], with some randomness.
  bool _regionContains(Vec pos)
  {
    double min = -3.0;
    double max = 2.0;

    double diagonal(int xDistance, int yDistance) =>
        MathUtils.lerpDouble(xDistance + yDistance, 0, width + height, 2.0, -3.0);

    var density = 0.0;
    if (region == Region.everywhere)
      return true;
    else if (region == Region.n)
      density = MathUtils.lerpDouble(pos.y, 0, height, max, min);
    else if (region == Region.ne)
      density = diagonal(width - pos.x - 1, pos.y);
    else if (region == Region.e)
      density = MathUtils.lerpDouble(pos.x, 0, width, min, max);
    else if (region == Region.se)
      density = diagonal(width - pos.x - 1, height - pos.y - 1);
    else if (region == Region.s)
      density = MathUtils.lerpDouble(pos.y, 0, height, min, max);
    else if (region == Region.sw)
      density = diagonal(pos.x, height - pos.y - 1);
    else if (region == Region.w)
      density = MathUtils.lerpDouble(pos.x, 0, width, max, min);
    else if (region == Region.nw)
      density = diagonal(pos.x, pos.y);

    return Rng.rng.rfloat(1.0) < density;
  }

  bool _tryAttachRoom(Junction junction)
  {
    var room = Room.create(depth);

    // Try to find a junction that can mate with this one.
    var direction = junction.direction.rotate180;
    var junctions =
        room.bounds.Where((pos) => room[pos].direction == direction).ToList();
    Rng.rng.shuffle(junctions);

    foreach (var pos in junctions)
    {
      // Calculate the room position by lining up the junctions.
      var roomPos = junction.position - pos;
      if (_tryPlaceRoom(room, roomPos.x, roomPos.y)) return true;
    }

    return false;
  }

  bool _tryPlaceRoom(Array2D<RoomTile> room, int x, int y)
  {
    if (!canPlaceRoom(room, x, y)) return false;

    var junctions = new List<Junction> { };

    foreach (var pos in room.bounds)
    {
      var here = pos.offset(x, y);
      var tile = room[pos];

      if (tile.isJunction)
      {
        // Don't grow outside of the chosen region.
        if (_regionContains(here))
        {
          junctions.Add(new Junction(here, tile.direction));
        }
      }
      else if (tile.isTile)
      {
        carve(here.x, here.y, tile.tile);
      }
      else if (tile.isWall)
      {
        preventPassage(here);
        _junctions.removeAt(here);
      }
    }

    // Shuffle the junctions so that the order we traverse the room tiles
    // doesn't bias the room growth.
    Rng.rng.shuffle(junctions);
    foreach (var junction in junctions)
    {
      // Remove any existing junctions since they now clash with the room.
      _junctions.removeAt(junction.position);
      _junctions.add(junction);
    }

    return true;
  }
}

class Junction
{
  /// The location of the junction.
  public Vec position;

  /// Points from the first room towards where the new room should be attached.
  ///
  /// A room must have an opposing junction in order to match.
  public Direction direction;

  /// How many times we've tried to place something at this junction.
  public int tries = 0;

  public Junction(Vec position, Direction direction)
  {
    this.position = position;
    this.direction = direction;
  }
}

enum TakeFrom { newest, oldest, random }

class JunctionSet
{
  public TakeFrom _takeFrom;
  public Dictionary<Vec, Junction> _byPosition = new Dictionary<Vec, Junction>() { };
  public List<Junction> _junctions = new List<Junction> { };

  public JunctionSet(TakeFrom _takeFrom)
  {
    this._takeFrom = _takeFrom;
  }

  public bool isNotEmpty => _junctions.isNotEmpty<Junction>();

  Junction this[Vec pos] => _byPosition[pos];

  public void add(Junction junction)
  {
    if (_byPosition.ContainsKey(junction.position))
      // Debugger.assert(_byPosition[junction.position] == null);
      Debugger.logError("had contain vec > " + junction.position.ToString());

    _byPosition[junction.position] = junction;
    _junctions.Add(junction);
  }

  public Junction takeNext()
  {
    Junction junction = null;
    switch (_takeFrom)
    {
      case TakeFrom.newest:
        junction = _junctions[_junctions.Count - 1];
        _junctions.RemoveAt(_junctions.Count - 1);
        break;

      case TakeFrom.oldest:
        junction = _junctions[0];
        _junctions.RemoveAt(0);
        break;

      case TakeFrom.random:
        junction = Rng.rng.take(_junctions);
        break;
    }

    _byPosition.Remove(junction.position);
    junction.tries++;

    return junction;
  }

  public void removeAt(Vec pos)
  {
    if (_byPosition.ContainsKey(pos))
    {
      var junction = _byPosition[pos];
      _byPosition.Remove(pos);
      if (junction != null) _junctions.Remove(junction);
    }
  }
}
