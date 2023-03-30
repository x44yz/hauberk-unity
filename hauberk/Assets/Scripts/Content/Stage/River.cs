using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mathf = UnityEngine.Mathf;

/// Uses a cellular automata to carve out rounded open cavernous areas.
class River : Architecture
{
  public override IEnumerator build()
  {
    // TODO: Branching tributaries?

    // Pick the start and end points. Rivers always flow from one edge of the
    // dungeon to another.
    var startSide = Rng.rng.item(Direction.cardinal);
    var kk = Direction.cardinal.ToList();
    kk.Remove(startSide);
    var endSide = Rng.rng.item(kk);

    // Midpoint displacement.
    var mid = _makePoint(Direction.none);
    _displace(_makePoint(startSide), mid);
    _displace(mid, _makePoint(endSide));

    yield break;
  }

  /// Makes a random end- or midpoint for the river. If [side] is a cardinal
  /// direction, it picks a point on that side of the dungeon. If none, it
  /// picks a point near the center.
  _RiverPoint _makePoint(Direction side)
  {
    var x = Rng.rng.rfloat(width * 0.25, width * 0.75);
    var y = Rng.rng.rfloat(height * 0.25, height * 0.75);

    if (side == Direction.none)
      return new _RiverPoint(x, y);
    else if (side == Direction.n)
      return new _RiverPoint(x, -2.0);
    else if (side == Direction.s)
      return new _RiverPoint(x, height + 2.0);
    else if (side == Direction.e)
      return new _RiverPoint(width + 2.0, y);
    else if (side == Direction.w)
      return new _RiverPoint(-2.0, y);

    throw new System.Exception("Unreachable.");
  }

  void _displace(_RiverPoint start, _RiverPoint end)
  {
    var h = start.x - end.x;
    var v = start.y - end.y;

    // Recursively subdivide if the segment is long enough.
    var length = Math.Sqrt(h * h + v * v);
    if (length > 1.0)
    {
      // TODO: Displace along the tangent line between start and end?
      var x = (start.x + end.x) / 2.0 + Rng.rng.rfloat(length / 2.0) - length / 4.0;
      var y = (start.y + end.y) / 2.0 + Rng.rng.rfloat(length / 2.0) - length / 4.0;
      var radius = (start.radius + end.radius) / 2.0;
      var mid = new _RiverPoint(x, y, radius);
      _displace(start, mid);
      _displace(mid, end);
      return;
    }

    var x1 = Math.Floor(start.x - start.radius);
    var y1 = Math.Floor(start.y - start.radius);
    var x2 = Math.Ceiling(start.x + start.radius);
    var y2 = Math.Ceiling(start.y + start.radius);

    // Don't go off the edge of the level. In fact, inset one inside it so
    // that we don't carve walkable tiles up to the edge.
    // TODO: Some sort of different tile types at the edge of the level to
    // look better than the river just stopping?
    x1 = Mathf.Clamp((float)x1, 1, width - 2);
    y1 = Mathf.Clamp((float)y1, 1, height - 2);
    x2 = Mathf.Clamp((float)x2, 1, width - 2);
    y2 = Mathf.Clamp((float)y2, 1, height - 2);

    var radiusSquared = start.radius * start.radius;

    for (var y = y1; y <= y2; y++)
    {
      for (var x = x1; x <= x2; x++)
      {
        var xx = start.x - x;
        var yy = start.y - y;

        var lengthSquared = xx * xx + yy * yy;
        var pos = new Vec((int)x, (int)y);
        if (lengthSquared <= radiusSquared) placeWater(pos);
      }
    }
  }
}

class _RiverPoint
{
  public double x;
  public double y;
  public double radius;

  public _RiverPoint(double x, double y, double? radius = null)
  {
    this.x = x;
    this.y = y;
    this.radius = radius ?? Rng.rng.rfloat(1.0, 3.0);
  }

  public override string ToString() => $"{x},{y} ({radius})";
}
