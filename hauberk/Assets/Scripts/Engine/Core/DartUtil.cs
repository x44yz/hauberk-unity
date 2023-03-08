using System;
using System.Collections;
using System.Collections.Generic;

public static class DartUtils
{
    public static void assert(bool v, string msg = null)
    {
        if (msg != null)
            UnityEngine.Debug.Assert(v, msg);
        else
            UnityEngine.Debug.Assert(v);
    }

    public static void print(string s)
    {
        UnityEngine.Debug.Log(s);
    }
}

public class Color {
  public static Color black = new Color(0, 0, 0);
  public static Color white = new Color(255, 255, 255);

  public static Color lightGray = new Color(192, 192, 192);
  public static Color gray = new Color(128, 128, 128);
  public static Color darkGray = new Color(64, 64, 64);

  public static Color lightRed = new Color(255, 160, 160);
  public static Color red = new Color(220, 0, 0);
  public static Color darkRed = new Color(100, 0, 0);

  public static Color lightOrange = new Color(255, 200, 170);
  public static Color orange = new Color(255, 128, 0);
  public static Color darkOrange = new Color(128, 64, 0);

  public static Color lightGold = new Color(255, 230, 150);
  public static Color gold = new Color(255, 192, 0);
  public static Color darkGold = new Color(128, 96, 0);

  public static Color lightYellow = new Color(255, 255, 150);
  public static Color yellow = new Color(255, 255, 0);
  public static Color darkYellow = new Color(128, 128, 0);

  public static Color lightGreen = new Color(130, 255, 90);
  public static Color green = new Color(0, 128, 0);
  public static Color darkGreen = new Color(0, 64, 0);

  public static Color lightAqua = new Color(128, 255, 255);
  public static Color aqua = new Color(0, 255, 255);
  public static Color darkAqua = new Color(0, 128, 128);

  public static Color lightBlue = new Color(128, 160, 255);
  public static Color blue = new Color(0, 64, 255);
  public static Color darkBlue = new Color(0, 37, 168);

  public static Color lightPurple = new Color(200, 140, 255);
  public static Color purple = new Color(128, 0, 255);
  public static Color darkPurple = new Color(64, 0, 128);

  public static Color lightBrown = new Color(190, 150, 100);
  public static Color brown = new Color(160, 110, 60);
  public static Color darkBrown = new Color(100, 64, 32);

  public int r;
  public int g;
  public int b;

  string cssColor => $"rgb({r}, {g}, {b})";

    public Color(int r, int g, int b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }

  int hashCode => r.GetHashCode() ^ g.GetHashCode() ^ b.GetHashCode();

  public static bool operator ==(Color a, object other) {
    if (other is Color) {
        var b = other as Color;
      return a.r == b.r && a.g == b.g && a.b == b.b;
    }
    return false;
  }

  public static bool operator !=(Color a, object other) {
    return !(a == other);
  }

  Color add(Color other, float fractionOther = 1.0f) {
    return new Color(
        (int)UnityEngine.Mathf.Clamp(r + other.r * fractionOther, 0, 255),
        (int)UnityEngine.Mathf.Clamp(g + other.g * fractionOther, 0, 255),
        (int)UnityEngine.Mathf.Clamp(b + other.b * fractionOther, 0, 255));
  }

  Color blend(Color other, double fractionOther) {
    var fractionThis = 1.0 - fractionOther;
    return new Color(
        (int)(r * fractionThis + other.r * fractionOther),
        (int)(g * fractionThis + other.g * fractionOther),
        (int)(b * fractionThis + other.b * fractionOther));
  }

  Color blendPercent(Color other, int percentOther) =>
      blend(other, percentOther / 100);
}

// class Glyph {
//   /// The empty glyph: a clear glyph using the default background color
//   /// [Color.BLACK].
//   public static Color clear = Glyph.fromCharCode(CharCode.space);

//   final int char;
//   final Color fore;
//   final Color back;

//   Glyph(String char, [Color? fore, Color? back])
//       : char = char.codeUnits[0],
//         fore = fore != null ? fore : Color.white,
//         back = back != null ? back : Color.black;

//   const Glyph.fromCharCode(this.char, [Color? fore, Color? back])
//       : fore = fore != null ? fore : Color.white,
//         back = back != null ? back : Color.black;

//   factory Glyph.fromDynamic(Object charOrCharCode, [Color? fore, Color? back]) {
//     if (charOrCharCode is String) return Glyph(charOrCharCode, fore, back);
//     return Glyph.fromCharCode(charOrCharCode as int, fore, back);
//   }

//   int get hashCode => char.hashCode ^ fore.hashCode ^ back.hashCode;

//   operator ==(Object other) {
//     if (other is Glyph) {
//       return char == other.char && fore == other.fore && back == other.back;
//     }

//     return false;
//   }
// }

