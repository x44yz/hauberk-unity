using System;
using System.Collections.Generic;

namespace Malison
{
  /// A terminal that draws to a window within another parent terminal.
  class PortTerminal : Terminal {
    public override int width => size.x;
    public override int height => size.y;
    public override Vec size => _size;

    public int _x;
    public int _y;
    public Vec _size;
    public Terminal _root;

    public PortTerminal(int _x, int _y, Vec size, Terminal _root)
    {
      this._x = _x;
      this._y = _y;
      this._size = size;
      this._root = _root;
    }

    public override void drawGlyph(int x, int y, Glyph glyph) {
      if (x < 0) return;
      if (x >= width) return;
      if (y < 0) return;
      if (y >= height) return;

      _root.drawGlyph(_x + x, _y + y, glyph);
    }

    Terminal rect(int x, int y, int width, int height) {
      // TODO: Bounds check.
      // Overridden so we can flatten out nested PortTerminals.
      return new PortTerminal(_x + x, _y + y, new Vec(width, height), _root);
    }
  }
}