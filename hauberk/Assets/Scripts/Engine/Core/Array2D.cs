using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

/// A two-dimensional fixed-size array of elements of type [T].
///
/// This class doesn't follow matrix notation which tends to put the column
/// index before the row. Instead, it mirrors graphics and games where x --
/// the horizontal component -- comes before y.
///
/// Internally, the elements are stored in a single contiguous list in row-major
/// order.
public class Array2D<T>
{
  /// A [Rect] whose bounds cover the full range of valid element indexes.
  public Rect bounds;

  /// The number of elements in a row of the array.
  public int width => bounds.width;

  /// The number of elements in a column of the array.
  public int height => bounds.height;

  public List<T> _elements;

  public Array2D()
  {
  }

  /// Creates a new array with [width], [height] elements initialized to
  /// [value].
  public Array2D(int width, int height, T value)
  {
    bounds = new Rect(0, 0, width, height);
    _elements = new List<T>(width * height);
    for (int i = 0; i < _elements.Capacity; ++i)
      _elements.Add(value);
  }

  /// Creates a new array with [width], [height] elements initialized to the
  /// result of calling [generator] on each element.
  public static Array2D<T> generated(int width, int height, System.Func<Vec, T> generator)
  {
    var a2 = new Array2D<T>();

    a2.bounds = new Rect(0, 0, width, height);
    a2._elements = new List<T>();
    if (width * height > 0)
    {
      for (int i = 0; i < width * height; ++i)
        a2._elements.Add(generator(Vec.zero));
    }

    // Don't call generator() on the first cell twice.
    for (var x = 1; x < width; x++)
    {
      a2._set(x, 0, generator(new Vec(x, 0)));
    }

    for (var y = 1; y < height; y++)
    {
      for (var x = 0; x < width; x++)
      {
        a2._set(x, y, generator(new Vec(x, y)));
      }
    }

    return a2;
  }

  /// Gets the element at [pos].
  public T this[Vec pos]
  {
    get { return _get(pos.x, pos.y); }
    set { _set(pos.x, pos.y, value); }
  }

  /// Sets the element at [pos].
  // public static void operator []=(Vec pos, T value) => set(pos.x, pos.y, value);

  // Store the bounds rect instead of simply the width and height because this
  // is accessed very frequently and avoids allocating a new Rect each time.

  /// The size of the array.
  public Vec size => bounds.size;

  /// Gets the element in the array at [x], [y].
  public T _get(int x, int y)
  {
    if (!_checkBounds(x, y))
      return default(T);
    return _elements[y * width + x];
  }

  /// Sets the element in the array at [x], [y] to [value].
  public void _set(int x, int y, T value)
  {
    if (!_checkBounds(x, y))
      return;
    _elements[y * width + x] = value;
  }

  /// Sets every element to [value].
  public void fill(T value)
  {
    for (int i = 0; i < _elements.Count; ++i)
      _elements[i] = value;
  }

  /// Evaluates [generator] on each position in the array and sets the element
  /// at that position to the result.
  void generate(System.Func<Vec, T> generator)
  {
    foreach (var pos in bounds)
      this[pos] = generator(pos);
  }

  IEnumerator<T> iterator => _elements.GetEnumerator();

  bool _checkBounds(int x, int y)
  {
    if (x < 0 || x >= width) 
      // throw new System.ArgumentOutOfRangeException("x", x.ToString());
      return false;
    if (y < 0 || y >= height)
      // throw new System.ArgumentOutOfRangeException("y", y.ToString());
      return false;
    return true;
  }
}