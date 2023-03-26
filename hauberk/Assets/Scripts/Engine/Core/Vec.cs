using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;

/// Shared base class of [Vec] and [Direction]. We do this instead of having
/// [Direction] inherit directly from [Vec] so that we can avoid it inheriting
/// an `==` operator, which would prevent it from being used in `switch`
/// statements. Instead, [Direction] uses identity equality.
public class VecBase
{
  public int x;
  public int y;

  public VecBase(int x, int y)
  {
    this.x = x;
    this.y = y;
  }

  /// Gets the area of a [Rect] whose corners are (0, 0) and this Vec.
  ///
  /// Returns a negative area if one of the Vec's coordinates are negative.
  public int area => x * y;

  /// Gets the rook length of the Vec, which is the number of squares a rook on
  /// a chessboard would need to move from (0, 0) to reach the endpoint of the
  /// Vec. Also known as Manhattan or taxicab distance.
  public int rookLength => Mathf.Abs(x) + Mathf.Abs(y);

  /// Gets the king length of the Vec, which is the number of squares a king on
  /// a chessboard would need to move from (0, 0) to reach the endpoint of the
  /// Vec. Also known as Chebyshev distance.
  public int kingLength => Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

  public int lengthSquared => x * x + y * y;

  /// The Cartesian length of the vector.
  ///
  /// If you just need to compare the magnitude of two vectors, prefer using
  /// the comparison operators or [lengthSquared], both of which are faster
  /// than this.
  public float length => Mathf.Sqrt(lengthSquared);

  /// The [Direction] that most closely approximates the angle of this Vec.
  ///
  /// In cases where two directions are equally close, chooses the one that is
  /// clockwise from this Vec's angle.
  ///
  /// In other words, it figures out which octant the vector's angle lies in
  /// (the dotted lines) and chooses the corresponding direction:
  ///
  ///               n
  ///      nw   2.0  -2.0  ne
  ///         \  '  |  '  /
  ///          \    |    /
  ///      0.5  \ ' | ' /   -0.5
  ///         '  \  |  /  '
  ///           ' \'|'/ '
  ///             '\|/'
  ///       w ------0------ e
  ///             '/|\'
  ///           ' /'|'\ '
  ///         '  /  |  \  '
  ///     -0.5  / ' | ' \   0.5
  ///          /    |    \
  ///         /  '  |  '  \
  ///       sw -2.0   2.0  se
  ///               s
  public Direction nearestDirection
  {
    get
    {
      if (x == 0)
      {
        if (y < 0)
        {
          return Direction.n;
        }
        else if (y == 0)
        {
          return Direction.none;
        }
        else
        {
          return Direction.s;
        }
      }

      var slope = y / x;

      if (x < 0)
      {
        if (slope >= 2.0)
        {
          return Direction.n;
        }
        else if (slope >= 0.5)
        {
          return Direction.nw;
        }
        else if (slope >= -0.5)
        {
          return Direction.w;
        }
        else if (slope >= -2.0)
        {
          return Direction.sw;
        }
        else
        {
          return Direction.s;
        }
      }
      else
      {
        if (slope >= 2.0)
        {
          return Direction.s;
        }
        else if (slope >= 0.5)
        {
          return Direction.se;
        }
        else if (slope >= -0.5)
        {
          return Direction.e;
        }
        else if (slope >= -2.0)
        {
          return Direction.ne;
        }
        else
        {
          return Direction.n;
        }
      }
    }
  }

  /// The eight Vecs surrounding this one to the north, south, east, and west
  /// and points in between.
  public List<Vec> neighbors
  {
    get
    {
      var rt = new List<Vec>();
      foreach (var direction in Direction.all)
        rt.Add(this + direction);
      return rt;
    }
  }


  // /// The four Vecs surrounding this one to the north, south, east, and west.
  public List<Vec> cardinalNeighbors
  {
    get
    {
      var rt = new List<Vec>();
      foreach (var direction in Direction.cardinal)
        rt.Add(this + direction);
      return rt;
    }
  }

  // /// The four Vecs surrounding this one to the northeast, southeast, southwest,
  // /// and northwest.
  List<Vec> intercardinalNeighbors
  {
    get
    {
      var rt = new List<Vec>();
      foreach (var direction in Direction.intercardinal)
        rt.Add(this + direction);
      return rt;
    }
  }

  /// Scales this Vec by [other].
  public static Vec operator *(VecBase a, int other) => new Vec(a.x * other, a.y * other);

  /// Scales this Vec by [other].
  public static Vec operator /(VecBase a, int other) => new Vec(a.x / other, a.y / other);

  /// Adds [other] to this Vec.
  ///
  ///  *  If [other] is a [Vec] or [Direction], adds each pair of coordinates.
  ///  *  If [other] is an [int], adds that value to both coordinates.
  ///
  /// Any other type is an error.
  public static Vec operator +(VecBase a, VecBase other) => new Vec(a.x + other.x, a.y + other.y);
  public static Vec operator +(VecBase a, int other) => new Vec(a.x + other, a.y + other);

  /// Substracts [other] from this Vec.
  ///
  ///  *  If [other] is a [Vec] or [Direction], subtracts each pair of
  ///     coordinates.
  ///  *  If [other] is an [int], subtracts that value from both coordinates.
  ///
  /// Any other type is an error.
  public static Vec operator -(VecBase a, VecBase other) => new Vec(a.x - other.x, a.y - other.y);
  public static Vec operator -(VecBase a, int other) => new Vec(a.x - other, a.y - other);

  /// Returns `true` if the magnitude of this vector is greater than [other].
  public static bool operator >(VecBase a, VecBase other) => a.lengthSquared > other.lengthSquared;
  public static bool operator >(VecBase a, float other) => a.lengthSquared > other * other;

  /// Returns `true` if the magnitude of this vector is greater than or equal
  /// to [other].
  public static bool operator >=(VecBase a, VecBase other) => a.lengthSquared >= other.lengthSquared;
  public static bool operator >=(VecBase a, float other) => a.lengthSquared >= other * other;

  /// Returns `true` if the magnitude of this vector is less than [other].
  public static bool operator <(VecBase a, VecBase other) => a.lengthSquared < other.lengthSquared;
  public static bool operator <(VecBase a, float other) => a.lengthSquared < other * other;

  /// Returns `true` if the magnitude of this vector is less than or equal to
  /// [other].
  public static bool operator <=(VecBase a, VecBase other) => a.lengthSquared <= other.lengthSquared;
  public static bool operator <=(VecBase a, float other) => a.lengthSquared <= other * other;

  /// Returns `true` if [pos] is within a rectangle from (0,0) to this vector
  /// (half-inclusive).
  bool contains(Vec pos)
  {
    var left = Mathf.Min(0, x);
    if (pos.x < left) return false;

    var right = Mathf.Max(0, x);
    if (pos.x >= right) return false;

    var top = Mathf.Min(0, y);
    if (pos.y < top) return false;

    var bottom = Mathf.Max(0, y);
    if (pos.y >= bottom) return false;

    return true;
  }

  /// Returns a new [Vec] with the absolute value of the coordinates of this
  /// one.
  public Vec abs() => new Vec(Mathf.Abs(x), Mathf.Abs(y));

  /// Returns a new [Vec] whose coordinates are this one's translated by [x] and
  /// [y].
  public Vec offset(int x, int y) => new Vec(this.x + x, this.y + y);

  /// Returns a new [Vec] whose coordinates are this one's but with the X
  /// coordinate translated by [x].
  public Vec offsetX(int x) => new Vec(this.x + x, y);

  /// Returns a new [Vec] whose coordinates are this one's but with the Y
  /// coordinate translated by [y].
  public Vec offsetY(int y) => new Vec(x, this.y + y);

  public override string ToString() => $"{x}, {y}";
}

/// A two-dimensional point.
public class Vec : VecBase
{
  public static Vec zero => new Vec(0, 0);

  int hashCode
  {
    get
    {
      // Map negative coordinates to positive and spread out the positive ones to
      // make room for them.
      var a = x >= 0 ? 2 * x : -2 * x - 1;
      var b = y >= 0 ? 2 * y : -2 * y - 1;

      // Cantor pairing function.
      // https://en.wikipedia.org/wiki/Pairing_function
      return (a + b) * (a + b + 1) / 2 + b;
    }
  }

  public Vec(int x, int y) : base(x, y)
  {
  }

  public static bool operator ==(Vec a, Vec b)
  {
    if (a is null && b is null)
      return true;
    if (a is null)
      return false;
    if (b is null)
      return false;
    return a.x == b.x && a.y == b.y;
  }

  public static bool operator !=(Vec a, Vec b)
  {
    if (a is null && b is null)
      return false;
    if (a is null)
      return true;
    if (b is null)
      return true;
    return a.x != b.x || a.y != b.y;
  }

  public override bool Equals(object obj)
  {
    if (obj is Vec)
    {
      var k = obj as Vec;
      return this == k;
    }
    return false;
  }
  public override int GetHashCode()
  {
    return hashCode;
  }
}

