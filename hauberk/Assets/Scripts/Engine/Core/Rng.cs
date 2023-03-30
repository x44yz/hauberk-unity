using System;
using System.Collections;
using System.Collections.Generic;
// 
using System.Linq;

/// A singleton instance of Rng globally available.
// public Rng rng = Rng(DateTime.now().millisecondsSinceEpoch);

/// The Random Number God: deliverer of good and ill fortune alike.
public class Rng
{
  public static Rng _rng;
  public static Rng rng {
    get {
      if (_rng == null)
        _rng = new Rng(System.DateTime.Now.Millisecond);
      return _rng;
    }
  }

  System.Random _random;

  public Rng(int seed)
  {
    setSeed(seed);
  }

  /// Resets the random number generator's internal state to [seed].
  void setSeed(int seed)
  {
    Debugger.log($"rng set seed > {seed}");
    _random = new System.Random(seed);
  }

  /// Gets a random int within a given range. If [max] is given, then it is
  /// in the range `[minOrMax, max)`. Otherwise, it is `[0, minOrMax)`. In
  /// other words, `range(3)` returns a `0`, `1`, or `2`, and `range(2, 5)`
  /// returns `2`, `3`, or `4`.
  public int range(int minOrMax, int? max = null)
  {
    if (max == null)
    {
      max = minOrMax;
      minOrMax = 0;
    }

    return _random.Next(max.Value - minOrMax) + minOrMax;
  }

  /// Gets a random int within a given range. If [max] is given, then it is
  /// in the range `[minOrMax, max]`. Otherwise, it is `[0, minOrMax]`. In
  /// other words, `inclusive(2)` returns a `0`, `1`, or `2`, and
  /// `inclusive(2, 4)` returns `2`, `3`, or `4`.
  public int inclusive(int minOrMax, int? max = null)
  {
    if (max == null)
    {
      max = minOrMax;
      minOrMax = 0;
    }

    max++;
    return _random.Next(max.Value - minOrMax) + minOrMax;
  }

  /// Gets a random floating-point value within the given range.
  public double rfloat(double? minOrMax = null, double? max = null)
  {
    if (minOrMax == null)
    {
      return _random.NextDouble();
    }
    else if (max == null)
    {
      return _random.NextDouble() * minOrMax.Value;
    }
    else
    {
      return _random.NextDouble() * (max.Value - minOrMax.Value) + minOrMax.Value;
    }
  }

  /// Gets a random integer count within the given floating point [range].
  ///
  /// The decimal portion of the range is treated as a fractional chance of
  /// returning the next higher integer value. For example:
  ///
  ///     countFromFloat(10.2);
  ///
  /// This has an 80% chance of returning 10 and a 20% chance of returning 11.
  ///
  /// This is particularly useful when the range is less than one, because it
  /// gives you some chance of still producing one instead of always rounding
  /// down to zero.
  int countFromFloat(double range)
  {
    var count = Math.Floor(range);
    if (rng.rfloat(1.0) < range - count) count++;
    return (int)count;
  }

  /// Calculate a random number with a normal distribution.
  ///
  /// Note that this means results may be less than -1.0 or greater than 1.0.
  ///
  /// Uses https://en.wikipedia.org/wiki/Marsaglia_polar_method.
  double normal()
  {
    double u, v, lengthSquared;

    do
    {
      u = rng.rfloat(-1.0, 1.0);
      v = rng.rfloat(-1.0, 1.0);
      lengthSquared = u * u + v * v;
    } while (lengthSquared >= 1.0);

    return u * Math.Sqrt(-2.0 * Math.Log(lengthSquared) / lengthSquared);
  }

  /// Returns `true` if a random int chosen between 1 and chance was 1.
  public bool oneIn(int chance) => range(chance) == 0;

  /// Returns `true` [chance] percent of the time.
  public bool percent(int chance) => range(100) < chance;

  /// Rounds [value] to a nearby integer, randomly rounding up or down based
  /// on the fractional value.
  ///
  /// For example, `round(3.2)` has a 20% chance of returning 3, and an 80%
  /// chance of returning 4.
  public int round(double value)
  {
    var result = Math.Floor(value);
    if ((float)(1.0) < value - result) result++;
    return (int)result;
  }

  /// Gets a random item from the given list.
  public T item<T>(List<T> items) => items[range(items.Count)];

  public T item<T>(Array items)
  {
    var rt = new List<T>();
    foreach (var k in items)
      rt.Add((T)k);
    return item<T>(rt);
  }

  /// Removes a random item from the given list.
  ///
  /// This may not preserve the order of items in the list, but is faster than
  /// [takeOrdered].
  public T take<T>(List<T> items)
  {
    var index = rng.range(items.Count);
    var result = items[index];

    // Replace the removed item with the last item in the list and then discard
    // the last.
    items[index] = items[items.Count - 1];
    items.RemoveAt(items.Count - 1);

    return result;
  }

  /// Removes a random item from the given list, preserving the order of the
  /// remaining items.
  ///
  /// This is O(n) because it must shift forward items after the removed one.
  /// If you don't need to preserve order, use [take].
  T takeOrdered<T>(List<T> items)
  {
    var result = items[items.Count - 1];
    items.RemoveAt(range(items.Count));
    return result;
  }

  /// Randomly re-orders elements in [items].
  public void shuffle<T>(List<T> items)
  {
    items.shuffle(_random);
  }

  /// Gets a random [Vec] within the given [Rect] (half-inclusive).
  public Vec vecInRect(Rect rect)
  {
    return new Vec(range(rect.left, rect.right), range(rect.top, rect.bottom));
  }

  /// Gets a random number centered around [center] with [range] (inclusive)
  /// using a triangular distribution. For example `triangleInt(8, 4)` will
  /// return values between 4 and 12 (inclusive) with greater distribution
  /// towards 8.
  ///
  /// This means output values will range from `(center - range)` to
  /// `(center + range)` inclusive, with most values near the center, but not
  /// with a normal distribution. Think of it as a poor man's bell curve.
  ///
  /// The algorithm works essentially by choosing a random point inside the
  /// triangle, and then calculating the x coordinate of that point. It works
  /// like this:
  ///
  /// Consider Center 4, Range 3:
  ///
  ///             *
  ///           * | *
  ///         * | | | *
  ///       * | | | | | *
  ///     --+-----+-----+--
  ///     0 1 2 3 4 5 6 7 8
  ///      -r     c     r
  ///
  /// Now flip the left half of the triangle (from 1 to 3) vertically and move
  /// it over to the right so that we have a square.
  ///
  ///         .-------.
  ///         |       V
  ///         |
  ///         |   R L L L
  ///         | . R R L L
  ///         . . R R R L
  ///       . . . R R R R
  ///     --+-----+-----+--
  ///     0 1 2 3 4 5 6 7 8
  ///
  /// Choose a point in that square. Figure out which half of the triangle the
  /// point is in, and then remap the point back out to the original triangle.
  /// The result is the *x* coordinate of the point in the original triangle.
  public int triangleInt(int center, int range)
  {
    if (range < 0)
    {
      throw new System.ArgumentException("The argument \"range\" must be zero or greater.");
    }

    // Pick a point in the square.
    int x = inclusive(range);
    int y = inclusive(range);

    // Figure out which triangle we are in.
    if (x <= y)
    {
      // Larger triangle.
      return center + x;
    }
    else
    {
      // Smaller triangle.
      return center - range - 1 + x;
    }
  }

  public int taper(int start, int chanceOfIncrement)
  {
    while (oneIn(chanceOfIncrement)) start++;
    return start;
  }
}
