using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// TODO: There's a lot of copy/paste between this and Catacomb. Decide if it's
// worth trying to refactor and share some code.

/// Places a number of random rooms.
class Dungeon : RoomArchitecture
{
  /// How much open space it tries to carve.
  public double _density;

  public override PaintStyle paintStyle => PaintStyle.flagstone;

  public Dungeon(double? density = null)
  {
    _density = density ?? 0.3;
  }

  public override IEnumerator build()
  {
    var failed = 0;
    while (carvedDensity < _density && failed < 100)
    {
      var room = Room.create(depth);

      var placed = false;
      for (var i = 0; i < 400; i++)
      {
        // TODO: This puts pretty hard boundaries around the region. Is there
        // a way to more softly distribute the rooms?
        var xMin = 1;
        var xMax = width - room.width;
        var yMin = 1;
        var yMax = height - room.height;

        if (region == Region.everywhere)
        {
          // Do nothing.
        }
        else if (region == Region.n)
        {
          yMax = height / 2 - room.height;
        }
        else if (region == Region.ne)
        {
          xMin = width / 2;
          yMax = height / 2 - room.height;
        }
        else if (region == Region.e)
        {
          xMin = width / 2;
        }
        else if (region == Region.se)
        {
          xMin = width / 2;
          yMin = height / 2;
        }
        else if (region == Region.s)
        {
          yMin = height / 2;
        }
        else if (region == Region.sw)
        {
          xMax = width / 2 - room.width;
          yMin = height / 2;
        }
        else if (region == Region.w)
        {
          xMax = width / 2 - room.width;
        }
        else if (region == Region.nw)
        {
          xMax = width / 2 - room.width;
          yMax = height / 2 - room.height;
        }

        // TODO: Instead of purely random, it would be good if it tried to
        // place rooms as far from other rooms as possible to maximize passage
        // length and more evenly distribute them.
        var x = Rng.rng.range(xMin, xMax);
        var y = Rng.rng.range(yMin, yMax);

        if (_tryPlaceRoom(room, x, y))
        {
          yield return "room";
          placed = true;
          break;
        }
      }

      if (!placed) failed++;
    }
  }

  bool _tryPlaceRoom(Array2D<RoomTile> room, int x, int y)
  {
    if (!canPlaceRoom(room, x, y)) return false;

    foreach (var pos in room.bounds)
    {
      var here = pos.offset(x, y);
      var tile = room[pos];

      if (tile.isTile)
      {
        carve(here.x, here.y, tile.tile);
      }
      else if (tile.isWall)
      {
        preventPassage(here);
      }
    }

    return true;
  }
}
