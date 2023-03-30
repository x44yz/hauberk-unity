using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityTerminal;

public static class DartUtils
{
  public static T2 putIfAbsent<T1, T2>(this Dictionary<T1, T2> map, T1 k) where T2 : new()
  {
    if (map.ContainsKey(k) == false)
      map[k] = new T2();
    return map[k];
  }

  public static bool isOdd(this int k)
  {
    return k % 2 == 1;
  }

  public static bool isEmpty<T>(this List<T> k)
  {
    return k.Count == 0;
  }

  public static bool isNotEmpty<T>(this List<T> k)
  {
    return k.Count > 0;
  }

  // https://api.dart.dev/stable/2.19.4/dart-core/Iterable/firstWhere.html
  public static T firstWhere<T>(this List<T> list, System.Func<T, bool> test, System.Func<T> orElse)
  {
    foreach (var k in list)
    {
      if (test(k))
        return k;
    }
    return orElse == null ? default(T) : orElse();
  }

  // public static Color blend(this Color color, Color other, float fractionOther)
  // {
  //   var fractionThis = 1.0 - fractionOther;
  //   return new Color(
  //       (int)(color.r * fractionThis + other.r * fractionOther),
  //       (int)(color.g * fractionThis + other.g * fractionOther),
  //       (int)(color.b * fractionThis + other.b * fractionOther));
  // }

  // public static Color add(this Color a, Color other, double? fractionOther = null) {
  //   fractionOther ??= 1.0;
  //   return new Color(
  //       (int)(a.r + other.r * fractionOther),
  //       (int)(a.g + other.g * fractionOther),
  //       (int)(a.b + other.b * fractionOther)
  //   );
  // }

  public static bool isEmpty(this string s)
  {
    return string.IsNullOrEmpty(s);
  }

  public static bool isNotEmpty(this string s)
  {
    return !string.IsNullOrEmpty(s);
  }

  public static int fold<T>(this List<T> list, int initialValue, Func<int, T, int> combine)
  {
    int value = initialValue;
    foreach (var element in list)
      value = combine(value, element);
    return value;
  }

  /// Finds the item in [collection] whose score is lowest.
  ///
  /// The score for an item is determined by calling [callback] on it. Returns
  /// `null` if the [collection] is `null` or empty.
  public static T _findLowest<T>(List<T> collection, System.Func<T, int> callback)
  {
    T bestItem = default(T);
    int? bestScore = null;

    foreach (var item in collection)
    {
      var score = callback(item);
      if (bestScore == null || score < bestScore)
      {
        bestItem = item;
        bestScore = score;
      }
    }

    return bestItem;
  }

  /// Finds the item in [collection] whose score is highest.
  ///
  /// The score for an item is determined by calling [callback] on it. Returns
  /// `null` if the [collection] is `null` or empty.
  public static T _findHighest<T>(List<T> collection, System.Func<T, int> callback)
  {
    T bestItem = default(T);
    int? bestScore = null;

    foreach (var item in collection)
    {
      var score = callback(item);
      if (bestScore == null || score > bestScore)
      {
        bestItem = item;
        bestScore = score;
      }
    }

    return bestItem;
  }

  public static T elementAt<T>(this IEnumerable<T> collection, int index)
  {
    return collection.ToList()[index];
  }

  // TODO: Move this elsewhere?
  public static string formatMoney(int price)
  {
    var result = price.ToString();
    if (price > 999999999)
    {
      result = result.Substring(0, result.Length - 9) +
          "," +
          result.Substring(result.Length - 9);
    }

    if (price > 999999)
    {
      result = result.Substring(0, result.Length - 6) +
          "," +
          result.Substring(result.Length - 6);
    }

    if (price > 999)
    {
      result = result.Substring(0, result.Length - 3) +
          "," +
          result.Substring(result.Length - 3);
    }

    return result;
  }

  public static bool isEven(this int v)
  {
    return v % 2 == 0;
  }

  public static StringBuilder sb = new StringBuilder();
  public static string strConcat(string a, int count)
  {
    sb.Clear();
    for (int i = 0; i < count; ++i)
      sb.Append(a);
    return sb.ToString();
  }
}

// public class Color {
//   public static Color black = new Color(0, 0, 0);
//   public static Color white = new Color(255, 255, 255);

//   public static Color lightGray = new Color(192, 192, 192);
//   public static Color gray = new Color(128, 128, 128);
//   public static Color darkGray = new Color(64, 64, 64);

//   public static Color lightRed = new Color(255, 160, 160);
//   public static Color red = new Color(220, 0, 0);
//   public static Color darkRed = new Color(100, 0, 0);

//   public static Color lightOrange = new Color(255, 200, 170);
//   public static Color orange = new Color(255, 128, 0);
//   public static Color darkOrange = new Color(128, 64, 0);

//   public static Color lightGold = new Color(255, 230, 150);
//   public static Color gold = new Color(255, 192, 0);
//   public static Color darkGold = new Color(128, 96, 0);

//   public static Color lightYellow = new Color(255, 255, 150);
//   public static Color yellow = new Color(255, 255, 0);
//   public static Color darkYellow = new Color(128, 128, 0);

//   public static Color lightGreen = new Color(130, 255, 90);
//   public static Color green = new Color(0, 128, 0);
//   public static Color darkGreen = new Color(0, 64, 0);

//   public static Color lightAqua = new Color(128, 255, 255);
//   public static Color aqua = new Color(0, 255, 255);
//   public static Color darkAqua = new Color(0, 128, 128);

//   public static Color lightBlue = new Color(128, 160, 255);
//   public static Color blue = new Color(0, 64, 255);
//   public static Color darkBlue = new Color(0, 37, 168);

//   public static Color lightPurple = new Color(200, 140, 255);
//   public static Color purple = new Color(128, 0, 255);
//   public static Color darkPurple = new Color(64, 0, 128);

//   public static Color lightBrown = new Color(190, 150, 100);
//   public static Color brown = new Color(160, 110, 60);
//   public static Color darkBrown = new Color(100, 64, 32);

//   public int r;
//   public int g;
//   public int b;

//   string cssColor => $"rgb({r}, {g}, {b})";

//     public Color(int r, int g, int b)
//     {
//         this.r = r;
//         this.g = g;
//         this.b = b;
//     }

//   public int hashCode => r.GetHashCode() ^ g.GetHashCode() ^ b.GetHashCode();

//   public static bool operator ==(Color a, object other) {
//     if (other is Color) {
//         var b = other as Color;
//       return a.r == b.r && a.g == b.g && a.b == b.b;
//     }
//     return false;
//   }

//   public static bool operator !=(Color a, object other) {
//     return !(a == other);
//   }

//   Color add(Color other, float fractionOther = 1.0f) {
//     return new Color(
//         (int)UnityEngine.Mathf.Clamp(r + other.r * fractionOther, 0, 255),
//         (int)UnityEngine.Mathf.Clamp(g + other.g * fractionOther, 0, 255),
//         (int)UnityEngine.Mathf.Clamp(b + other.b * fractionOther, 0, 255));
//   }

//   public Color blend(Color other, double fractionOther) {
//     var fractionThis = 1.0 - fractionOther;
//     return new Color(
//         (int)(r * fractionThis + other.r * fractionOther),
//         (int)(g * fractionThis + other.g * fractionOther),
//         (int)(b * fractionThis + other.b * fractionOther));
//   }

//   Color blendPercent(Color other, int percentOther) =>
//       blend(other, percentOther / 100);
// }

// class Glyph {
//   /// The empty glyph: a clear glyph using the default background color
//   /// [Color.BLACK].
//   public static Glyph clear = new Glyph(CharCode.space);

//   public int _char;
//   public Color fore;
//   public Color back;

//   public Glyph(string ch, Color fore = null, Color back = null)
//   {
//     this._char = ch[0];
//     this.fore = fore != null ? fore : Color.white;
//     this.back = back != null ? back : Color.black;
//   }


//   public Glyph(int ch, Color fore = null, Color back = null)
//   {
//     this._char = ch;
//     this.fore = fore != null ? fore : Color.white;
//     this.back = back != null ? back : Color.black;
//   }

//   public static Glyph fromDynamic(object charOrCharCode, Color fore = null, Color back = null) {
//     if (charOrCharCode is string) 
//       return new Glyph(charOrCharCode.ToString(), fore, back);
//     return new Glyph((int)charOrCharCode, fore, back);
//   }

//   int hashCode => _char.GetHashCode() ^ fore.hashCode ^ back.hashCode;

//   public static bool operator ==(Glyph a, object other) {
//     if (other is Glyph) {
//       var b = other as Glyph;
//       return a._char == b._char && a.fore == b.fore && a.back == b.back;
//     }

//     return false;
//   }

//   public static bool operator !=(Glyph a, object other) {
//     return !(a == other);
//   }
// }

// public class CharCode
// {
//   public const int space = 0x0020;
//   public const int latinCapitalLetterCWithCedilla = 0x00c7;
//   public const int latinSmallLetterUWithDiaeresis = 0x00fc;
//   public const int centSign = 0x00a2;
//   public const int dollarSign = 0x0024;
//   public const int notSign = 0x00ac;
//   public const int vulgarFractionOneQuarter = 0x00bc;
//   public const int invertedExclamationMark = 0x00a1;
//   public const int vulgarFractionOneHalf = 0x00bd;
//   public const int latinSmallLetterAWithGrave = 0x00e0;
//   public const int latinSmallLetterAWithDiaeresis = 0x00e4;
//   public const int latinSmallLetterAWithCircumflex = 0x00e2;
//   public const int latinSmallLetterEWithGrave = 0x00e8;
//   public const int latinSmallLetterEWithDiaeresis = 0x00eb;
//   public const int latinSmallLetterEWithCircumflex = 0x00ea;
//   public const int latinSmallLetterCWithCedilla = 0x00e7;
//   public const int reversedNotSign = 0x2310;
//   public const int invertedQuestionMark = 0x00bf;
//   public const int masculineOrdinalIndicator = 0x00ba;
//   public const int feminineOrdinalIndicator = 0x00aa;
//   public const int latinCapitalLetterNWithTilde = 0x00d1;
//   public const int latinSmallLetterNWithTilde = 0x00f1;
//   public const int latinSmallLetterUWithAcute = 0x00fa;
//   public const int latinSmallLetterOWithAcute = 0x00f3;
//   public const int latinSmallLetterAWithAcute = 0x00e1;
//   public const int latinSmallLetterIWithAcute = 0x00ed;
//   public const int latinSmallLetterIWithGrave = 0x00ec;
//   public const int latinCapitalLetterEWithAcute = 0x00c9;
//   public const int latinSmallLetterOWithCircumflex = 0x00f4;
//   public const int latinSmallLetterOWithDiaeresis = 0x00f6;
//   public const int latinSmallLetterOWithGrave = 0x00f2;
//   public const int latinCapitalLetterAe = 0x00c6;
//   public const int latinCapitalLetterAWithRingAbove = 0x00c5;
//   public const int latinSmallLetterAe = 0x00e6;
//   public const int latinCapitalLetterAWithDiaeresis = 0x00c4;
// }