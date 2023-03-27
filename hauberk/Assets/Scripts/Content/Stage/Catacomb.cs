using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// Places a number of random blobs.
class Catacomb : Architecture
{
  /// How much open space it tries to carve.
  public double _density;

  /// The minimum chamber size.
  public int _minSize;

  /// The maximum chamber size.
  public int _maxSize;

  public Catacomb(double? density = null, int? minSize = null, int? maxSize = null)
  {
    _density = density ?? 0.3;
    _minSize = minSize ?? 8;
    _maxSize = maxSize ?? 32;
  }


  public override IEnumerator build()
  {
    // Don't try to make chambers bigger than the stage.
    var maxSize = (double)_maxSize;
    maxSize = Math.Min(maxSize, height);
    maxSize = Math.Min(maxSize, width);
    maxSize = Math.Sqrt(maxSize);

    // Make sure the size range isn't backwards.
    var minSize = Math.Sqrt(_minSize);
    minSize = Math.Min(minSize, maxSize);

    var failed = 0;
    while (carvedDensity < _density && failed < 100)
    {
      // Square the size to skew the distribution so that larges ones are
      // rarer than smaller ones.
      var size = (int)Math.Pow(Rng.rng.rfloat(minSize, maxSize), 2.0);
      var cave = Blob.make(size);

      var placed = false;
      for (var i = 0; i < 400; i++)
      {
        // TODO: dungeon.dart has similar code for placing the starting room.
        // Unify.
        // TODO: This puts pretty hard boundaries around the region. Is there
        // a way to more softly distribute the caves?
        var xMin = 1;
        var xMax = width - cave.width;
        var yMin = 1;
        var yMax = height - cave.height;

        if (region == Region.everywhere)
        {
          // Do nothing.
        }
        else if (region == Region.n)
        {
          yMax = height / 2 - cave.height;
        }
        else if (region == Region.ne)
        {
          xMin = width / 2;
          yMax = height / 2 - cave.height;
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
          xMax = width / 2 - cave.width;
          yMin = height / 2;
        }
        else if (region == Region.w)
        {
          xMax = width / 2 - cave.width;
        }
        else if (region == Region.nw)
        {
          xMax = width / 2 - cave.width;
          yMax = height / 2 - cave.height;
        }

        if (xMin >= xMax) continue;
        if (yMin >= yMax) continue;

        var x = Rng.rng.range(xMin, xMax);
        var y = Rng.rng.range(yMin, yMax);

        if (_tryPlaceCave(cave, x, y))
        {
          yield return "cave";
          placed = true;
          break;
        }
      }

      if (!placed) failed++;
    }
  }

  bool _tryPlaceCave(Array2D<bool> cave, int x, int y)
  {
    foreach (var pos in cave.bounds)
    {
      if (cave[pos])
      {
        if (!canCarve(pos.offset(x, y))) return false;
      }
    }

    foreach (var pos in cave.bounds)
    {
      if (cave[pos]) carve(pos.x + x, pos.y + y);
    }

    return true;
  }
}
