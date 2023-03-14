using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Malison
{
  /// A [RenderableTerminal] that draws to a canvas using the old school DOS
  /// [code page 437][font] font.
  ///
  /// [font]: http://en.wikipedia.org/wiki/Code_page_437
  class RetroTerminal : RenderableTerminal {
    public Display _display;

    // public html.CanvasRenderingContext2D _context;
    // public html.ImageElement _font;
    public UnityEngine.Font _font;

    /// A cache of the tinted font images. Each key is a color, and the image
    /// will is the font in that color.
    // public Map<Color, html.CanvasElement> _fontColorCache = {};

    /// The drawing scale, used to adapt to Retina displays.
    public int _scale;

    bool _imageLoaded = false;

    public int _charWidth;
    public int _charHeight;

    public override int width => _display.width;
    public override int height => _display.height;
    public override Vec size => _display.size;

    /// Creates a new terminal using a built-in DOS-like font.
    public static RetroTerminal dos(int width, int height
            /*html.CanvasElement? canvas*/) =>
            RetroTerminal.create(width, height, "packages/malison/dos.png"
            /*canvas: canvas*/, charWidth: 9, charHeight: 16);

    /// Creates a new terminal using a short built-in DOS-like font.
    public static RetroTerminal shortDos(int width, int height
            /*html.CanvasElement? canvas*/) =>
            RetroTerminal.create(width, height, "packages/malison/dos-short.png",
            /*canvas: canvas,*/ charWidth: 9, charHeight: 13);

    /// Creates a new terminal using a font image at [imageUrl].
    public static RetroTerminal create(int width, int height, string imageUrl,
        /*html.CanvasElement? canvas,*/
        int charWidth,
        int charHeight,
        int scale = 1)
    {
      
      // scale ??= html.window.devicePixelRatio.toInt();

      // // If not given a canvas, create one, automatically size it, and add it to
      // // the page.
      // if (canvas == null) {
      //   canvas = html.CanvasElement();
      //   var canvasWidth = charWidth * width;
      //   var canvasHeight = charHeight * height;
      //   canvas.width = canvasWidth * scale;
      //   canvas.height = canvasHeight * scale;
      //   canvas.style.width = '${canvasWidth}px';
      //   canvas.style.height = '${canvasHeight}px';

      //   html.document.body!.append(canvas);
      // }

      var display = new Display(width, height);

      return new RetroTerminal(display, charWidth, charHeight, /*canvas,*/
          /*html.ImageElement(src: imageUrl),*/ scale);
    }

    RetroTerminal(Display _display, int _charWidth, int _charHeight,
        /*html.CanvasElement canvas, this._font,*/ int _scale)
    {
      this._display = _display;
      this._charWidth = _charWidth;
      this._charHeight = _charHeight;
      this._scale = _scale;

      // _context = canvas.context2D {
      //   _font.onLoad.listen((_) {
      //     _imageLoaded = true;
      //     render();
      //   });
      // }
    }

    public override void drawGlyph(int x, int y, Glyph glyph) {
      _display.setGlyph(x, y, glyph);
    }

    public Array2D<SpriteRenderer> sprs;

    public override void render() {
      // if (!_imageLoaded) return;

      if (sprs == null)
        sprs = new Array2D<SpriteRenderer>(width, height, null);

      _display.render((x, y, glyph) => {
        var _char = glyph._char;

        // Remap it if it's a Unicode character.
        if (UnicodeMap.unicodeMap.ContainsKey(_char))
          _char = UnicodeMap.unicodeMap[_char];

        var sx = (_char % 32) * _charWidth;
        var sy = (_char / 32) * _charHeight;

        // Fill the background.
        // _context.fillStyle = glyph.back.cssColor;
        // _context.fillRect(x * _charWidth * _scale, y * _charHeight * _scale,
        //     _charWidth * _scale, _charHeight * _scale);

        // Don't bother drawing empty characters.
        if (_char == 0 || _char == CharCode.space) return;

        var spr = sprs._get(x, y);
        if (spr == null)
        {
          var obj = new GameObject($"spr{x}x{y}");
          obj.transform.SetParent(MalisonUnity.Inst.glyphsRoot);
          spr = obj.AddComponent<SpriteRenderer>();
          sprs._set(x, y, spr);
        }

        spr.sprite = MalisonUnity.Inst.sprites[_char];
        spr.transform.position = new Vector3(x * _charWidth * _scale / 100.0f, y * _charHeight * _scale / 100.0f, spr.transform.position.z);

        // var color = _getColorFont(glyph.fore);
        // _context.imageSmoothingEnabled = false;
        // _context.drawImageScaledFromSource(
        //     color,
        //     sx,
        //     sy,
        //     _charWidth,
        //     _charHeight,
        //     x * _charWidth * _scale,
        //     y * _charHeight * _scale,
        //     _charWidth * _scale,
        //     _charHeight * _scale);

      });
    }

    public override Vec pixelToChar(Vec pixel) =>
        new Vec(pixel.x / _charWidth, pixel.y / _charHeight);

    // html.CanvasElement _getColorFont(Color color) {
    //   var cached = _fontColorCache[color];
    //   if (cached != null) return cached;

    //   // Create a font using the given color.
    //   var tint = html.CanvasElement(width: _font.width, height: _font.height);
    //   var context = tint.context2D;

    //   // Draw the font.
    //   context.drawImage(_font, 0, 0);

    //   // Tint it by filling in the existing alpha with the color.
    //   context.globalCompositeOperation = 'source-atop';
    //   context.fillStyle = color.cssColor;
    //   context.fillRect(0, 0, _font.width!, _font.height!);

    //   _fontColorCache[color] = tint;
    //   return tint;
    // }
  }
}