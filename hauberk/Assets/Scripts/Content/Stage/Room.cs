using System;
using System.Collections.Generic;
using System.Linq;

// TODO: Different kinds of lights.
// TODO: Different architectural styles should lean towards certain lighting
// arrangements.
// TODO: More things to light rooms with:
// - candles on tables
// - candles on floor?
// - torches embedded in wall
// - fireplaces
// - freestanding torches?
// - fire pit

/// Base class for architectures that work with [Room]s.
abstract class RoomArchitecture : Architecture
{
  /// Determines whether [room] can be placed on the stage at [x], [y].
  public bool canPlaceRoom(Array2D<RoomTile> room, int x, int y)
  {
    foreach (var pos in room.bounds)
    {
      var here = pos.offset(x, y);
      var tile = room[pos];

      if (!tile.isUnused && !bounds.contains(here)) return false;
      if (tile.isTile && !canCarve(pos.offset(x, y))) return false;
    }

    return true;
  }
}

/// Generates random rooms.
class Room
{
  public static Array2D<RoomTile> create(int depth)
  {
    // TODO: Instead of picking from these randomly, different architectural
    // styles should prefer certain room shapes.
    // TODO: More room shapes:
    // - Plus
    // - T
    switch (Rng.rng.inclusive(10))
    {
      case 0:
        return _diamond(depth);
      case 1:
        return _octagon(depth);
      case 2:
      case 3:
        return _angled(depth);
      default:
        return _rectangle(depth);
    }
  }

  static Array2D<RoomTile> _rectangle(int depth)
  {
    // Make a randomly-sized room but keep the aspect ratio reasonable.
    var _short = Rng.rng.inclusive(3, 10);
    var _long = Rng.rng.inclusive(_short, Math.Min(16, _short + 4));

    var horizontal = Rng.rng.oneIn(2);
    var width = horizontal ? _long : _short;
    var height = horizontal ? _short : _long;

    var tiles = new Array2D<RoomTile>(width + 2, height + 2, RoomTile.unused);
    for (var y = 0; y < height; y++)
    {
      for (var x = 0; x < width; x++)
      {
        tiles._set(x + 1, y + 1, RoomTile.floor);
      }
    }

    var lights = new List<List<Vec>> { };

    // Center.
    if (_short <= 9 && width.isOdd() && height.isOdd())
    {
      lights.Add(new List<Vec> { new Vec(width / 2 + 1, height / 2 + 1) });
    }

    // Braziers in corners.
    if (_long >= 5)
    {
      for (var i = 0; i < (_short - 1) / 2; i++)
      {
        lights.Add(new List<Vec>{
          new Vec(1 + i, 1 + i),
          new Vec(width - i, 1 + i),
          new Vec(1 + i, height - i),
          new Vec(width - i, height - i)
      });
      }
    }

    // TODO: Row of braziers down center of _long axis.

    _addLights(depth, tiles, lights);
    _calculateEdges(tiles);
    return tiles;
  }

  static Array2D<RoomTile> _angled(int depth)
  {
    // Make a randomly-sized room but keep the aspect ratio reasonable.
    var _short = Rng.rng.inclusive(5, 10);
    var _long = Rng.rng.inclusive(_short, Math.Min(16, _short + 4));

    var horizontal = Rng.rng.oneIn(2);
    var width = horizontal ? _long : _short;
    var height = horizontal ? _short : _long;

    var cutWidth = Rng.rng.inclusive(2, width - 3);
    var cutHeight = Rng.rng.inclusive(2, height - 3);

    var isTop = Rng.rng.oneIn(2);
    var isLeft = Rng.rng.oneIn(2);

    // Open the whole rect.
    var tiles = new Array2D<RoomTile>(width + 2, height + 2, RoomTile.unused);
    for (var y = 0; y < height; y++)
    {
      for (var x = 0; x < width; x++)
      {
        tiles._set(x + 1, y + 1, RoomTile.floor);
      }
    }

    // Fill in the cut.
    var xMin = isLeft ? 0 : width - cutWidth;
    var xMax = isLeft ? cutWidth : width;
    var yMin = isTop ? 0 : height - cutHeight;
    var yMax = isTop ? cutHeight : height;
    for (var y = yMin; y < yMax; y++)
    {
      for (var x = xMin; x < xMax; x++)
      {
        tiles._set(x + 1, y + 1, RoomTile.unused);
      }
    }

    var lights = new List<List<Vec>> { };

    // Braziers in corners.
    var narrowest = Math.Min(width - cutWidth, height - cutHeight);
    for (var i = 0; i < (narrowest - 1) / 2; i++)
    {
      var cornerLights = new List<Vec> { };
      lights.Add(cornerLights);
      if (!isTop || !isLeft) cornerLights.Add(new Vec(1 + i, 1 + i));
      if (!isTop || isLeft) cornerLights.Add(new Vec(width - i, 1 + i));
      if (isTop || !isLeft) cornerLights.Add(new Vec(1 + i, height - i));
      if (isTop || isLeft) cornerLights.Add(new Vec(width - i, height - i));

      if (isTop)
      {
        if (isLeft)
        {
          cornerLights.Add(new Vec(cutWidth + 1 + i, 1 + i));
          cornerLights.Add(new Vec(1 + i, cutHeight + 1 + i));
        }
        else
        {
          cornerLights.Add(new Vec(width - cutWidth - i, 1 + i));
          cornerLights.Add(new Vec(width - i, cutHeight + 1 + i));
        }
      }
      else
      {
        if (isLeft)
        {
          cornerLights.Add(new Vec(cutWidth + 1 + i, height - i));
          cornerLights.Add(new Vec(1 + i, height - cutHeight - i));
        }
        else
        {
          cornerLights.Add(new Vec(width - i, height - cutHeight - i));
          cornerLights.Add(new Vec(width - cutWidth - i, height - i));
        }
      }
    }

    _addLights(depth, tiles, lights);
    _calculateEdges(tiles);
    return tiles;
  }

  static Array2D<RoomTile> _diamond(int depth)
  {
    var size = Rng.rng.inclusive(5, 17);
    return _angledCorners(size, (size - 1) / 2, depth);
  }

  static Array2D<RoomTile> _octagon(int depth)
  {
    var size = Rng.rng.inclusive(6, 13);
    var corner = Rng.rng.inclusive(2, size / 2 - 1);

    return _angledCorners(size, corner, depth);
  }

  static Array2D<RoomTile> _angledCorners(int size, int corner, int depth)
  {
    var tiles = new Array2D<RoomTile>(size + 2, size + 2, RoomTile.unused);
    for (var y = 0; y < size; y++)
    {
      for (var x = 0; x < size; x++)
      {
        if (x + y < corner) continue;
        if (size - x - 1 + y < corner) continue;
        if (x + size - y - 1 < corner) continue;
        if (size - x - 1 + size - y - 1 < corner) continue;

        tiles._set(x + 1, y + 1, RoomTile.floor);
      }
    }

    var lights = new List<List<Vec>> { };

    // Center.
    if (size <= 9 && size.isOdd())
    {
      lights.Add(new List<Vec> { new Vec(size / 2 + 1, size / 2 + 1) });
    }

    // Diamonds.
    if (size.isOdd())
    {
      for (var i = 2; i < size / 2 - 1; i++)
      {
        lights.Add(new List<Vec>{
          new Vec(size / 2 + 1, size / 2 + 1 - i),
          new Vec(size / 2 + 1 + i, size / 2 + 1),
          new Vec(size / 2 + 1, size / 2 + 1 + i),
          new Vec(size / 2 + 1 - i, size / 2 + 1)
      });
      }
    }

    // Squares.
    var maxSquare = (size + 1) / 2 - (corner + 1) / 2 - 3;
    for (var i = 0; i <= maxSquare; i++)
    {
      lights.Add(new List<Vec>{
        new Vec((size - 1) / 2 - i, (size - 1) / 2 - i),
        new Vec((size + 4) / 2 + i, (size - 1) / 2 - i),
        new Vec((size - 1) / 2 - i, (size + 4) / 2 + i),
        new Vec((size + 4) / 2 + i, (size + 4) / 2 + i),
      });
    }

    _addLights(depth, tiles, lights);
    _calculateEdges(tiles);
    return tiles;
  }

  /// Given a set of floor tiles, marks the boundary tiles as junctions or walls
  /// as appropriate.
  static void _calculateEdges(Array2D<RoomTile> room)
  {
    foreach (var pos in room.bounds)
    {
      if (!room[pos].isUnused) continue;

      bool isFloor(Direction dir)
      {
        var here = pos + dir;
        if (!room.bounds.contains(here)) return false;
        return room[here].isTile;
      }

      var cardinalFloors = Direction.cardinal.Where(x => isFloor(x)).ToList();
      var hasCornerFloor = Direction.intercardinal.Any(x => isFloor(x));

      if (cardinalFloors.Count == 1)
      {
        // Place junctions next to floors.
        room[pos] = new RoomTile(cardinalFloors[0].rotate180);
      }
      else if (cardinalFloors.Count > 1)
      {
        // Don't allow junctions at inside corners.
      }
      else if (hasCornerFloor)
      {
        // Don't allow passages at outside corners.
        room[pos] = RoomTile.wall;
      }
    }
  }

  // TODO: This is kind of inefficient because it goes through the trouble to
  // generate every possible lighting setup for a room before picking one or
  // even deciding if the room should be lit.
  static void _addLights(
      int depth, Array2D<RoomTile> room, List<List<Vec>> lights)
  {
    if (lights.isEmpty<List<Vec>>()) return;

    if (!Rng.rng.percent(MathUtils.lerpInt(depth, 1, Option.maxDepth, 90, 20))) return;

    foreach (var light in Rng.rng.item(lights))
    {
      room[light] = new RoomTile(Rng.rng.item(Tiles.braziers));
    }
  }
}

class RoomTile
{
  /// Not part of the room.
  public static RoomTile unused = new RoomTile(Direction.none);

  /// Room floor.
  public static RoomTile floor = new RoomTile(Tiles.open);

  /// Room wall that cannot have a junction or passage through it. Used to
  /// prevent entrances to rooms in corners, which looks weird.
  public static RoomTile wall = new RoomTile(Tiles.solid);

  public TileType tile;
  public Direction direction;

  public RoomTile(Direction direction)
  {
    this.direction = direction;
    tile = null;
  }

  public RoomTile(TileType tile)
  {
    this.tile = tile;
    direction = Direction.none;
  }

  public bool isUnused => tile == null && direction == Direction.none;

  /// Whether the room tile is a floor or other specific tile type.
  public bool isTile => !isUnused && !isWall && !isJunction;

  public bool isWall => tile == Tiles.solid;

  public bool isJunction => direction != Direction.none;
}
