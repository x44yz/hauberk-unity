using System;
using System.Collections;
using System.Collections.Generic;


/// Traces a line of integer coordinates from a [start] point to and through an
/// [end] point using Bresenham's line drawing algorithm.
///
/// Useful for line-of-sight calculation, tracing missile paths, etc.
class Line : IEnumerable<Vec>
{
  IEnumerator IEnumerable.GetEnumerator()
  {
    return (IEnumerator)GetEnumerator();
  }

  public IEnumerator<Vec> GetEnumerator()
  {
    return new _LineIterator(start, end);
  }

  public Vec start;
  public Vec end;

  public Line(Vec start, Vec end)
  {
    this.start = start;
    this.end = end;
  }

  public IEnumerator<Vec> iterator => new _LineIterator(start, end);

  int length => throw new System.NotSupportedException("Line iteration is unbounded.");
}

class _LineIterator : IEnumerator<Vec>
{

  object IEnumerator.Current => current;
  public Vec Current => current;
  public bool MoveNext() => moveNext();
  public void Reset()
  {
    this._current = Vec.zero;
    this._error = 0;
  }
  public void Dispose()
  {
  }

  Vec _current;
  Vec current => _current;

  /// Accumulated "error".
  int _error;

  int _primary;

  int _secondary;

  /// Unit vector for the primary direction the line is moving. It advances one
  /// unit this direction each step.
  Vec _primaryStep;

  /// Unit vector for the primary direction the line is moving. It advances one
  /// unit this direction when the accumulated error overflows.
  Vec _secondaryStep;

  public _LineIterator(Vec start, Vec end)
  {
    var delta = end - start;

    // Figure which octant the line is in and increment appropriately.
    var primaryStep = new Vec(Math.Sign(delta.x), 0);
    var secondaryStep = new Vec(0, Math.Sign(delta.y));

    // Discard the signs now that they are accounted for.
    delta = delta.abs();

    // Assume moving horizontally each step.
    var primary = delta.x;
    var secondary = delta.y;

    // Swap the order if the y magnitude is greater.
    if (delta.y > delta.x)
    {
      var temp = primary;
      primary = secondary;
      secondary = temp;

      var tempIncrement = primaryStep;
      primaryStep = secondaryStep;
      secondaryStep = tempIncrement;
    }

    // return new _LineIterator(
    //     start, 0, primary, secondary, primaryStep, secondaryStep);

    this._current = start;
    this._error = 0;
    this._primary = primary;
    this._secondary = secondary;
    this._primaryStep = primaryStep;
    this._secondaryStep = secondaryStep;
  }

  public _LineIterator(Vec _current, int _error, int _primary, int _secondary,
      Vec _primaryStep, Vec _secondaryStep)
  {
    this._current = _current;
    this._error = _error;
    this._primary = _primary;
    this._secondary = _secondary;
    this._primaryStep = _primaryStep;
    this._secondaryStep = _secondaryStep;
  }

  /// Always returns `true` to allow a line to overshoot the end point. Make
  /// sure you terminate iteration yourself.
  bool moveNext()
  {
    _current += _primaryStep;

    // See if we need to step in the secondary direction.
    _error += _secondary;
    if (_error * 2 >= _primary)
    {
      _current += _secondaryStep;
      _error -= _primary;
    }

    return true;
  }
}