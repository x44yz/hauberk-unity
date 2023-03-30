using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;

/// A two-dimensional immutable rectangle with integer coordinates.
///
/// Many operations treat a [Rect] as a collection of discrete points. In those
/// cases, the boundaries of the rect are two half-open intervals when
/// determining which points are inside the rect. For example, consider the
/// rect whose coordinates are (-1, 1)-(3, 4):
///
///      -2 -1  0  1  2  3  4
///       |  |  |  |  |  |  |
///     0-
///     1-   +-----------+
///     2-   |           |
///     3-   |           |
///     4-   +-----------+
///     5-
///
/// It contains all points within that region except for the ones that lie
/// directly on the right and bottom edges. (It's always right and bottom,
/// even if the rectangle has negative coordinates.) In the above examples,
/// that's these points:
///
///      -2 -1  0  1  2  3  4
///       |  |  |  |  |  |  |
///     0-
///     1-   *--*--*--*--+
///     2-   *  *  *  *  |
///     3-   *  *  *  *  |
///     4-   +-----------+
///     5-
///
/// This seems a bit odd, but does what you want in almost all respects. For
/// example, the width of this rect, determined by subtracting the left
/// coordinate (-1) from the right (3) is 4 and indeed it contains four columns
/// of points.
public class Rect : IEnumerable<Vec>
{
  IEnumerator IEnumerable.GetEnumerator()
  {
    return (IEnumerator)GetEnumerator();
  }

  public IEnumerator<Vec> GetEnumerator()
  {
    return iterator;
  }

  /// Gets the empty rectangle.
  public static Rect empty => Rect.posAndSize(Vec.zero, Vec.zero);

  /// Creates a new rectangle that is the intersection of [a] and [b].
  ///
  ///     .----------.
  ///     | a        |
  ///     | .--------+----.
  ///     | | result |  b |
  ///     | |        |    |
  ///     '-+--------'    |
  ///       |             |
  ///       '-------------'
  public static Rect intersect(Rect a, Rect b)
  {
    var left = Mathf.Max(a.left, b.left);
    var right = Mathf.Min(a.right, b.right);
    var top = Mathf.Max(a.top, b.top);
    var bottom = Mathf.Min(a.bottom, b.bottom);

    var width = Mathf.Max(0, right - left);
    var height = Mathf.Max(0, bottom - top);

    return new Rect(left, top, width, height);
  }

  static Rect centerIn(Rect toCenter, Rect main)
  {
    var pos = main.pos + ((main.size - toCenter.size) / 2);
    return new Rect(pos, toCenter.size);
  }

  public Vec pos;
  public Vec size;

  public int x => pos.x;
  public int y => pos.y;
  public int width => size.x;
  public int height => size.y;

  // Use min and max to handle negative sizes.

  public int left => Mathf.Min(x, x + width);
  public int top => Mathf.Min(y, y + height);
  public int right => Mathf.Max(x, x + width);
  public int bottom => Mathf.Max(y, y + height);

  public Vec topLeft => new Vec(left, top);
  public Vec topRight => new Vec(right, top);
  public Vec bottomLeft => new Vec(left, bottom);
  public Vec bottomRight => new Vec(right, bottom);

  public Vec center => new Vec((left + right) / 2, (top + bottom) / 2);

  int area => size.area;

  public Rect(Vec pos, Vec size)
  {
    this.pos = pos;
    this.size = size;
  }

  public static Rect posAndSize(Vec pos, Vec size)
  {
    return new Rect(pos, size);
  }

  public static Rect leftTopRightBottom(int left, int top, int right, int bottom)
  {
    var pos = new Vec(left, top);
    var size = new Vec(right - left, bottom - top);
    return new Rect(pos, size);
  }

  public Rect(int x, int y, int width, int height)
  {
    pos = new Vec(x, y);
    size = new Vec(width, height);
  }


  /// Creates a new rectangle a single row in height, as wide as [size],
  /// with its top left corner at [pos].
  public static Rect row(int x, int y, int size)
  {
    return new Rect(x, y, size, 1);
  }

  /// Creates a new rectangle a single column in width, as tall as [size],
  /// with its top left corner at [pos].
  public static Rect column(int x, int y, int size)
  {
    return new Rect(x, y, 1, size);
  }

  string tostring() => $"({pos})-({size})";

  public Rect inflate(int distance)
  {
    return new Rect(x - distance, y - distance, width + (distance * 2),
        height + (distance * 2));
  }

  Rect offset(int x, int y) => new Rect(this.x + x, this.y + y, width, height);

  public bool contains(object obj)
  {
    if (!(obj is Vec)) return false;

    var objVec = obj as Vec;
    if (objVec.x < pos.x) return false;
    if (objVec.x >= pos.x + size.x) return false;
    if (objVec.y < pos.y) return false;
    if (objVec.y >= pos.y + size.y) return false;

    return true;
  }

  bool containsRect(Rect rect)
  {
    if (rect.left < left) return false;
    if (rect.right > right) return false;
    if (rect.top < top) return false;
    if (rect.bottom > bottom) return false;

    return true;
  }

  /// Returns a new [Vec] that is as near to [vec] as possible while being in
  /// bounds.
  public Vec clamp(Vec vec)
  {
    var x = (int)Mathf.Clamp(vec.x, left, right);
    var y = (int)Mathf.Clamp(vec.y, top, bottom);
    return new Vec(x, y);
  }

  public RectIterator iterator => new RectIterator(this);

  /// Returns the distance between this Rect and [other]. This is minimum
  /// length that a corridor would have to be to go from one Rect to the other.
  /// If the two Rects are adjacent, returns zero. If they overlap, returns -1.
  int distanceTo(Rect other)
  {
    int vertical;
    if (top >= other.bottom)
    {
      vertical = top - other.bottom;
    }
    else if (bottom <= other.top)
    {
      vertical = other.top - bottom;
    }
    else
    {
      vertical = -1;
    }

    int horizontal;
    if (left >= other.right)
    {
      horizontal = left - other.right;
    }
    else if (right <= other.left)
    {
      horizontal = other.left - right;
    }
    else
    {
      horizontal = -1;
    }

    if ((vertical == -1) && (horizontal == -1)) return -1;
    if (vertical == -1) return horizontal;
    if (horizontal == -1) return vertical;
    return horizontal + vertical;
  }

  /// Iterates over the points along the edge of the Rect.
  public IEnumerable<Vec> trace()
  {
    if ((width > 1) && (height > 1))
    {
      // TODO(bob): Implement an iterator class here if building the list is
      // slow.
      // Trace all four sides.
      var result = new List<Vec>();

      for (var x = left; x < right; x++)
      {
        result.Add(new Vec(x, top));
        result.Add(new Vec(x, bottom - 1));
      }

      for (var y = top + 1; y < bottom - 1; y++)
      {
        result.Add(new Vec(left, y));
        result.Add(new Vec(right - 1, y));
      }

      return result;
    }
    else if ((width > 1) && (height == 1))
    {
      // A single row.
      return Rect.row(left, top, width);
    }
    else if ((height >= 1) && (width == 1))
    {
      // A single column, or one unit
      return Rect.column(left, top, height);
    }

    // Otherwise, the rect doesn't have a positive size, so there's nothing to
    // trace.
    return new List<Vec>();
  }

  // TODO: Equality operator and hashCode.
}

public class RectIterator : IEnumerator<Vec>
{

  object IEnumerator.Current => current;
  public Vec Current => current;
  public bool MoveNext() => moveNext();
  public void Reset()
  {
    _x = _rect.x - 1;
    _y = _rect.y;
  }
  public void Dispose()
  {
  }

  public Rect _rect;
  int _x;
  int _y;

  public RectIterator(Rect _rect)
  {
    this._rect = _rect;
    _x = _rect.x - 1;
    _y = _rect.y;
  }

  Vec current => new Vec(_x, _y);

  bool moveNext()
  {
    _x++;
    if (_x >= _rect.right)
    {
      _x = _rect.x;
      _y++;
    }

    return _y < _rect.bottom;
  }
}
