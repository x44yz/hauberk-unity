using System.Collections;
using System.Collections.Generic;

/// Utility class for handling simple rasterized circles of a relatively small
/// radius.
///
/// Used for lighting, ball spells, etc. Optimized to generate "nice" looking
/// circles at small sizes.
public class Circle : IEnumerable<Vec>
{

  public static int[] _radiiSquared = new int[] { 0, 2, 5, 10, 18, 26, 38 };

  public static int _radiusSquared(int radius)
  {
    // If small enough, use the tuned radius to look best.
    if (radius < _radiiSquared.Length) return _radiiSquared[radius];

    return radius * radius;
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return (IEnumerator)GetEnumerator();
  }

  public IEnumerator<Vec> GetEnumerator()
  {
    return iterator;
  }

  /// The position of the center of the circle.
  public Vec center;

  /// The radius of this Circle.
  public int radius;

  public Circle(Vec center, int radius)
  {
    this.center = center;
    this.radius = radius;
    if (radius < 0) throw new System.Exception("The radius cannot be negative.");
  }

  /// Gets whether [pos] is in the outermost edge of the circle.
  bool isEdge(Vec pos)
  {
    var leadingEdge = true;
    if (radius > 0)
    {
      leadingEdge = (pos - center) > _radiusSquared(radius - 1);
    }

    return leadingEdge;
  }

  IEnumerator<Vec> iterator => _CircleIterator.create(this, edge: false);

  /// Traces the outside edge of the circle.
  public IEnumerable<Vec> edge => new _CircleIterable(_CircleIterator.create(this, edge: true));
}

class _CircleIterable : IEnumerable<Vec>
{
  IEnumerator IEnumerable.GetEnumerator()
  {
    return (IEnumerator)GetEnumerator();
  }

  public IEnumerator<Vec> GetEnumerator()
  {
    return iterator;
  }

  public IEnumerator<Vec> iterator;

  public _CircleIterable(IEnumerator<Vec> iterator)
  {
    this.iterator = iterator;
  }
}

class _CircleIterator : IEnumerator<Vec>
{

  object IEnumerator.Current => current;
  public Vec Current => current;
  public bool MoveNext() => moveNext();
  public void Reset()
  {
    _boundsIterator.Reset();
  }
  public void Dispose()
  {
  }

  Vec current => _boundsIterator.Current + _circle.center;

  public Circle _circle;
  public IEnumerator<Vec> _boundsIterator;
  public bool _edge;

  public static _CircleIterator create(Circle circle, bool edge)
  {
    var size = circle.radius + circle.radius + 1;
    var bounds = new Rect(-circle.radius, -circle.radius, size, size);
    return new _CircleIterator(circle, bounds.iterator, edge: edge);
  }

  public _CircleIterator(Circle _circle, IEnumerator<Vec> _boundsIterator, bool edge)
  {
    this._circle = _circle;
    this._boundsIterator = _boundsIterator;
    this._edge = edge;
  }

  bool moveNext()
  {
    while (true)
    {
      if (!_boundsIterator.MoveNext()) return false;
      var length = _boundsIterator.Current.lengthSquared;

      if (length > Circle._radiusSquared(_circle.radius)) continue;
      if (_edge &&
          _circle.radius > 0 &&
          length < Circle._radiusSquared(_circle.radius - 1)) continue;

      break;
    }

    return true;
  }
}


