using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// Uses a cellular automata to carve out rounded open cavernous areas.
class Cavern : Architecture
{
  // TODO: Fields to tune density distribution, thresholds, and number of
  // rounds of smoothing.

  public override IEnumerator build()
  {
    // True is wall, false is floor, null is untouchable tiles that belong to
    // other architectures.
    var cells1 = new Array2D<bool?>(width, height, null);
    var cells2 = new Array2D<bool?>(width, height, null);

    foreach (var pos in cells1.bounds)
    {
      if (!canCarve(pos)) continue;
      cells1[pos] = Rng.rng.rfloat(1.0) < _density(region, pos);
    }

    for (var i = 0; i < 4; i++)
    {
      foreach (var pos in cells1.bounds)
      {
        // Don't touch unavailable cells.
        if (cells1[pos] == null) continue;

        var walls = 0;
        foreach (var here in pos.neighbors)
        {
          if (!cells1.bounds.contains(here) || cells1[here] != false) walls++;
        }

        if (cells1[pos]!.Value)
        {
          // Survival threshold.
          cells2[pos] = walls >= 3;
        }
        else
        {
          // Birth threshold.
          cells2[pos] = walls >= 5;
        }
      }

      var temp = cells1;
      cells1 = cells2;
      cells2 = temp;
      yield return "Round";
    }

    foreach (var pos in cells1.bounds)
    {
      if (cells1[pos] == false) carve(pos.x, pos.y);
    }
  }

  double _density(Region region, Vec pos)
  {
    // TODO: Vary density randomly some.
    double min = 0.3;
    double max = 0.7;

    if (region == Region.everywhere)
      return 0.45;
    else if (region == Region.n)
      return MathUtils.lerpDouble(pos.y, 0, height, min, max);
    else if (region == Region.ne)
    {
      var distance = Math.Max(width - pos.x - 1, pos.y);
      var range = Math.Min(width, height);
      return MathUtils.lerpDouble(distance, 0, range, min, max);
    }
    else if (region == Region.e)
      return MathUtils.lerpDouble(pos.x, 0, width, min, max);
    else if (region == Region.se)
    {
      var distance = Math.Max(width - pos.x - 1, height - pos.y - 1);
      var range = Math.Min(width, height);
      return MathUtils.lerpDouble(distance, 0, range, min, max);
    }
    else if (region == Region.s)
      return MathUtils.lerpDouble(pos.y, 0, height, max, min);
    else if (region == Region.sw)
    {
      var distance = Math.Max(pos.x, height - pos.y - 1);
      var range = Math.Min(width, height);
      return MathUtils.lerpDouble(distance, 0, range, min, max);
    }
    else if (region == Region.w)
      return MathUtils.lerpDouble(pos.x, 0, width, max, min);
    else if (region == Region.nw)
    {
      var distance = Math.Max(pos.x, pos.y);
      var range = Math.Min(width, height);
      return MathUtils.lerpDouble(distance, 0, range, min, max);
    }

    throw new System.Exception("Unreachable.");
  }
}
