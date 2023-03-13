using System;
using System.Collections.Generic;
using UnityEngine;

namespace Malison
{
  /// A [RenderableTerminal] that draws to a canvas element using a browser font.
  class CanvasTerminal : RenderableTerminal {
    public Display _display;

    public Font _font;
    public html.CanvasElement _canvas;
    public html.CanvasRenderingContext2D _context;

    /// The drawing scale, used to adapt to Retina displays.
    public int _scale = html.window.devicePixelRatio.toInt();

    Vec get size => _display.size;
    int get width => _display.width;
    int get height => _display.height;

    factory CanvasTerminal(int width, int height, Font font,
        [html.CanvasElement? canvas]) {
      var display = Display(width, height);

      // If not given a canvas, create one and add it to the page.
      if (canvas == null) {
        canvas = html.CanvasElement();
        html.document.body!.append(canvas);
      }

      return CanvasTerminal._(display, font, canvas);
    }

    CanvasTerminal._(this._display, this._font, html.CanvasElement canvas)
        : _canvas = canvas,
          _context = canvas.context2D {
      // Handle high-resolution (i.e. retina) displays.
      var canvasWidth = _font.charWidth * _display.width;
      var canvasHeight = _font.lineHeight * _display.height;
      _canvas.width = canvasWidth * _scale;
      _canvas.height = canvasHeight * _scale;
      _canvas.style.width = '${canvasWidth}px';
      _canvas.style.height = '${canvasHeight}px';
    }

    void drawGlyph(int x, int y, Glyph glyph) {
      _display.setGlyph(x, y, glyph);
    }

    void render() {
      _context.font = '${_font.size * _scale}px ${_font.family}, monospace';

      _display.render((x, y, glyph) {
        var char = glyph.char;

        // Fill the background.
        _context.fillStyle = glyph.back.cssColor;
        _context.fillRect(
            x * _font.charWidth * _scale,
            y * _font.lineHeight * _scale,
            _font.charWidth * _scale,
            _font.lineHeight * _scale);

        // Don't bother drawing empty characters.
        if (char == 0 || char == CharCode.space) return;

        _context.fillStyle = glyph.fore.cssColor;
        _context.fillText(
            String.fromCharCodes([char]),
            (x * _font.charWidth + _font.x) * _scale,
            (y * _font.lineHeight + _font.y) * _scale);
      });
    }

    Vec pixelToChar(Vec pixel) =>
        Vec(pixel.x ~/ _font.charWidth, pixel.y ~/ _font.lineHeight);
  }

  /// Describes a font used by [CanvasTerminal].
  class Font {
    public String family;
    public int size;
    public int charWidth;
    public int lineHeight;
    public int x;
    public int y;

    Font(this.family,
        {required this.size,
        required int w,
        required int h,
        required this.x,
        required this.y})
        : charWidth = w,
          lineHeight = h;
  }
}