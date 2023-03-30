using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;
using num = System.Single;

public static class MathUtils
{
  /// Remaps [value] within the range [min]-[max] to the output range
  /// [outMin]-[outMax].
  public static double lerpDouble(float value, float min, float max, double outMin, double outMax)
  {
    Debugger.assert(min < max);

    if (value <= min) return outMin;
    if (value >= max) return outMax;

    var t = (value - min) / (max - min);
    return outMin + t * (outMax - outMin);
  }

  /// Remaps [value] within the range [min]-[max] to the output range
  /// [outMin]-[outMax].
  public static int lerpInt(int value, int min, int max, int outMin, int outMax) =>
      Mathf.RoundToInt((float)lerpDouble(value, min, max, outMin, outMax));

  // TODO: Not currently used. Delete?
  /// Finds the quadratic curve that goes through [x0],[y0], [x1],[y1], and
  /// [x2],[y2]. Then calculates the y position at [x].
  public static double quadraticInterpolate(num x,
      num x0,
      num y0,
      num x1,
      num y1,
      num x2,
      num y2)
  {
    // From: http://mathonline.wikidot.com/deleted:quadratic-polynomial-interpolation
    var a = ((x - x1) * (x - x2)) / ((x0 - x1) * (x0 - x2));
    var b = ((x - x0) * (x - x2)) / ((x1 - x0) * (x1 - x2));
    var c = ((x - x0) * (x - x1)) / ((x2 - x0) * (x2 - x1));

    return y0 * a + y1 * b + y2 * c;
  }

  /// Produces a psuedo-random 32-bit unsigned integer for the point at [x, y]
  /// using [seed].
  ///
  /// This can be used to associate random values with tiles without having to
  /// store them.
  public static int hashPoint(int x, int y, int seed = 0)
  {
    return (int)(hashInt(hashInt(hashInt(seed) + x) + y) & 0xffffffff);
  }

  // From: https://stackoverflow.com/a/12996028/9457
  public static int hashInt(int n)
  {
    n = (int)((((n >> 16) ^ n) * 0x45d9f3b) & 0xffffffff);
    n = (int)((((n >> 16) ^ n) * 0x45d9f3b) & 0xffffffff);
    n = (n >> 16) ^ n;
    return n;
  }

  // Knuth Durstenfeld
  public static void shuffle<T>(this List<T> list, System.Random random)
  {
    //随机交换
    int currentIndex;
    T tempValue;
    for (int i = list.Count - 1; i >= 0; i--)
    {
      currentIndex = random.Next(0, i + 1);
      tempValue = list[currentIndex];
      list[currentIndex] = list[i];
      list[i] = tempValue;
    }
  }
}
