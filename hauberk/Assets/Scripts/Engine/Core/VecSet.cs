using System;
using System.Collections;
using System.Collections.Generic;


// TODO: Are there other places we should use this?
/// An optimized set of vectors within a given rectangle.
///
/// It's relatively slow to construct, but can be reused efficiently using
/// [clear].
class VecSet : IEnumerable<Vec>
{
  IEnumerator IEnumerable.GetEnumerator()
  {
    return (IEnumerator)GetEnumerator();
  }

  public IEnumerator<Vec> GetEnumerator()
  {
    return iterator;
  }

  /// The current value at each cell.
  ///
  /// To avoid the expense of clearing every cell when done, we instead use a
  /// sentinel value to indicate which cells are present. To "clear" the array,
  /// we just increment to a new sentinel value.
  public Array2D<int> _values;

  /// The current sentinel value.
  ///
  /// Any cells in [_values] that have this value are in the set.
  int _sentinel = 0;

  /// The bounding box surrounding all set values.
  ///
  /// This lets us efficiently iterate over the present cells without going over
  /// large empty regions.
  int _xMin;
  int _xMax;
  int _yMin;
  int _yMax;

  public VecSet(int width, int height)
  {
    _values = new Array2D<int>(width, height, 0);
    _xMin = width;
    _xMax = 0;
    _yMin = height;
    _yMax = 0;
  }

  IEnumerator<Vec> iterator
  {
    get
    {
      var result = new List<Vec> { };
      for (var y = _yMin; y <= _yMax; y++)
      {
        for (var x = _xMin; x <= _xMax; x++)
        {
          if (_values._get(x, y) == _sentinel) result.Add(new Vec(x, y));
        }
      }

      return result.GetEnumerator();
    }
  }

  public void clear()
  {
    _sentinel++;
    // TODO: Check for overflow?

    _xMin = _values.width;
    _xMax = 0;
    _yMin = _values.height;
    _yMax = 0;
  }

  public void add(Vec pos)
  {
    _values[pos] = _sentinel;
    _xMin = Math.Min(_xMin, pos.x);
    _xMax = Math.Max(_xMax, pos.x);
    _yMin = Math.Min(_yMin, pos.y);
    _yMax = Math.Max(_yMax, pos.y);
  }

  public bool contains(object element) => _values[element as Vec] == _sentinel;
}