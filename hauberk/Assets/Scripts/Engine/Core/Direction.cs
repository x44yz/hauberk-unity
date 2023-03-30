using System.Collections;
using System.Collections.Generic;


public class Direction : Vec
{
  public static Direction none = new Direction(0, 0);
  public static Direction n = new Direction(0, -1);
  public static Direction ne = new Direction(1, -1);
  public static Direction e = new Direction(1, 0);
  public static Direction se = new Direction(1, 1);
  public static Direction s = new Direction(0, 1);
  public static Direction sw = new Direction(-1, 1);
  public static Direction w = new Direction(-1, 0);
  public static Direction nw = new Direction(-1, -1);

  /// The eight cardinal and intercardinal directions.
  public static List<Direction> all = new List<Direction>() { n, ne, e, se, s, sw, w, nw };

  /// The four cardinal directions: north, south, east, and west.
  public static List<Direction> cardinal = new List<Direction>() { n, e, s, w };

  /// The four directions between the cardinal ones: northwest, northeast,
  /// southwest and southeast.
  public static List<Direction> intercardinal = new List<Direction>() { ne, se, sw, nw };

  public Direction(int x, int y) : base(x, y)
  {
  }

  public Direction rotateLeft45
  {
    get
    {
      if (this == none) return none;
      else if (this == n) return nw;
      else if (this == ne) return n;
      else if (this == e) return ne;
      else if (this == se) return e;
      else if (this == s) return se;
      else if (this == sw) return s;
      else if (this == w) return sw;
      else if (this == nw) return w;

      Debugger.logError("unreachable > " + this);
      return this;
    }
  }

  public Direction rotateRight45
  {
    get
    {
      if (this == none) return none;
      if (this == n) return ne;
      if (this == ne) return e;
      if (this == e) return se;
      if (this == se) return s;
      if (this == s) return sw;
      if (this == sw) return w;
      if (this == w) return nw;
      if (this == nw) return n;

      Debugger.logError("unreachable > " + this);
      return this;
    }
  }

  public Direction rotateLeft90
  {
    get
    {
      if (this == none) return none;
      else if (this == n) return w;
      else if (this == ne) return nw;
      else if (this == e) return n;
      else if (this == se) return ne;
      else if (this == s) return e;
      else if (this == sw) return se;
      else if (this == w) return s;
      else if (this == nw) return sw;

      Debugger.logError("unreachable > " + this);
      return this;
    }
  }

  public Direction rotateRight90
  {
    get
    {
      if (this == none) return none;
      else if (this == n) return e;
      else if (this == ne) return se;
      else if (this == e) return s;
      else if (this == se) return sw;
      else if (this == s) return w;
      else if (this == sw) return nw;
      else if (this == w) return n;
      else if (this == nw) return ne;

      Debugger.logError("unreachable > " + this);
      return this;
    }
  }

  public Direction rotate180
  {
    get
    {
      if (this == none) return none;
      else if (this == n) return s;
      else if (this == ne) return sw;
      else if (this == e) return w;
      else if (this == se) return nw;
      else if (this == s) return n;
      else if (this == sw) return ne;
      else if (this == w) return e;
      else if (this == nw) return se;

      Debugger.logError("unreachable > " + this);
      return this;
    }
  }

  public override string ToString()
  {
    if (this == none) return "none";
    else if (this == n) return "n";
    else if (this == ne) return "ne";
    else if (this == e) return "e";
    else if (this == se) return "se";
    else if (this == s) return "s";
    else if (this == sw) return "sw";
    else if (this == w) return "w";
    else if (this == nw) return "nw";

    Debugger.logError("unreachable > " + this);
    return "unreachable";
  }
}
