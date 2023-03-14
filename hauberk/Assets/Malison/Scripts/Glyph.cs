using System;
using System.Collections.Generic;

namespace Malison
{
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

    public string cssColor => $"rgb({r}, {g}, {b})";

      public Color(int r, int g, int b)
      {
          this.r = r;
          this.g = g;
          this.b = b;
      }

    public int hashCode => r.GetHashCode() ^ g.GetHashCode() ^ b.GetHashCode();

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

    public Color blend(Color other, double fractionOther) {
      var fractionThis = 1.0 - fractionOther;
      return new Color(
          (int)(r * fractionThis + other.r * fractionOther),
          (int)(g * fractionThis + other.g * fractionOther),
          (int)(b * fractionThis + other.b * fractionOther));
    }

    Color blendPercent(Color other, int percentOther) =>
        blend(other, percentOther / 100);
  }

  class Glyph {
    /// The empty glyph: a clear glyph using the default background color
    /// [Color.BLACK].
    public static Glyph clear = new Glyph(CharCode.space);

    public int _char;
    public Color fore;
    public Color back;

    public Glyph(string ch, Color fore = null, Color back = null)
    {
      this._char = ch[0];
      this.fore = fore != null ? fore : Color.white;
      this.back = back != null ? back : Color.black;
    }


    public Glyph(int ch, Color fore = null, Color back = null)
    {
      this._char = ch;
      this.fore = fore != null ? fore : Color.white;
      this.back = back != null ? back : Color.black;
    }

    public static Glyph fromDynamic(object charOrCharCode, Color fore = null, Color back = null) {
      if (charOrCharCode is string) 
        return new Glyph(charOrCharCode.ToString(), fore, back);
      return new Glyph((int)charOrCharCode, fore, back);
    }

    int hashCode => _char.GetHashCode() ^ fore.hashCode ^ back.hashCode;

    public static bool operator ==(Glyph a, object other) {
      if (other is Glyph) {
        var b = other as Glyph;
        return a._char == b._char && a.fore == b.fore && a.back == b.back;
      }

      return false;
    }

    public static bool operator !=(Glyph a, object other) {
      return !(a == other);
    }
  }
}