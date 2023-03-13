using System;
using System.Collections.Generic;

namespace Malison
{
  /// A virtual console terminal that can be written onto.
  abstract class Terminal {
    /// The number of columns of characters.
    public int width;

    /// The number of rows of characters.
    public int height;

    /// The number of columns and rows.
    public Vec size;

    /// The default foreground color used when a color is not specified.
    Color foreColor = Color.white;

    /// The default foreground color used when a color is not specified.
    Color backColor = Color.black;

    /// Clears the terminal to [backColor].
    void clear() {
      fill(0, 0, width, height);
    }

    /// Clears and fills the given rectangle with [color].
    void fill(int x, int y, int width, int height, Color? color = null) {
      color ??= backColor;

      var glyph = new Glyph(CharCode.space, foreColor, color);

      for (var py = y; py < y + height; py++) {
        for (var px = x; px < x + width; px++) {
          drawGlyph(px, py, glyph);
        }
      }
    }

    /// Writes [text] starting at column [x], row [y] using [fore] as the text
    /// color and [back] as the background color.
    void writeAt(int x, int y, string text, Color? fore = null, Color? back = null) {
      fore ??= foreColor;
      back ??= backColor;

      // TODO: Bounds check.
      for (var i = 0; i < text.Length; i++) {
        if (x + i >= width) break;
        drawGlyph(x + i, y, new Glyph(text[i], fore, back));
      }
    }

    Terminal rect(int x, int y, int width, int height) {
      // TODO: Bounds check.
      return new PortTerminal(x, y, new Vec(width, height), this);
    }

    /// Writes a one-character string consisting of [charCode] at column [x],
    /// row [y] using [fore] as the text color and [back] as the background color.
    void drawChar(int x, int y, int charCode, Color? fore = null, Color? back = null) {
      drawGlyph(x, y, new Glyph(charCode, fore, back));
    }

    public abstract void drawGlyph(int x, int y, Glyph glyph);
  }

  abstract class RenderableTerminal : Terminal {
    public abstract void render();

    /// Given a point in pixel coordinates, returns the coordinates of the
    /// character that contains that pixel.
    public abstract Vec pixelToChar(Vec pixel);
  }
}

