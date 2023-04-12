using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

/// The main gameplay area of the screen.
class StagePanel : Panel
{
  public static Color[] _dazzleColors = new Color[]{
    Hues.darkCoolGray,
    Hues.coolGray,
    Hues.lightCoolGray,
    Hues.ash,
    Hues.sandal,
    Hues.tan,
    Hues.persimmon,
    Hues.brown,
    Hues.buttermilk,
    Hues.gold,
    Hues.carrot,
    Hues.mint,
    Hues.olive,
    Hues.lima,
    Hues.peaGreen,
    Hues.sherwood,
    Hues.pink,
    Hues.red,
    Hues.maroon,
    Hues.lilac,
    Hues.purple,
    Hues.violet,
    Hues.lightAqua,
    Hues.lightBlue,
    Hues.blue,
    Hues.darkBlue,
  };

  public static int[] _fireChars = new int[] { CharCode.blackUpPointingTriangle, CharCode.caret };
  public static Color[][] _fireColors = new Color[][]{
    new Color[]{Hues.gold, Hues.persimmon},
    new Color[]{Hues.buttermilk, Hues.carrot},
    new Color[]{Hues.tan, Hues.red},
    new Color[]{Hues.red, Hues.brown}
  };

  public GameScreen _gameScreen;

  public List<Effect> _effects = new List<Effect>();

  public List<Monster> visibleMonsters = new List<Monster>();

  bool _hasAnimatedTile = false;

  int _frame = 0;

  /// The portion of the [Stage] currently in view on screen.
  public Rect cameraBounds => _cameraBounds;

  Rect _cameraBounds;

  /// The amount of offset the rendered stage from the top left corner of the
  /// screen.
  ///
  /// This will be zero unless the stage is smaller than the view.
  Vec _renderOffset;

  public StagePanel(GameScreen _gameScreen)
  {
    this._gameScreen = _gameScreen;
  }

  /// Draws [Glyph] at [x], [y] in [Stage] coordinates onto the current view.
  public void drawStageGlyph(Terminal terminal, int x, int y, Glyph glyph)
  {
    _drawStageGlyph(terminal, x + bounds.x, y + bounds.y, glyph);
  }

  void _drawStageGlyph(Terminal terminal, int x, int y, Glyph glyph)
  {
    terminal.WriteAt(x - _cameraBounds.x + _renderOffset.x,
        y - _cameraBounds.y + _renderOffset.y, glyph);
  }

  public bool update(List<Event> events)
  {
    _frame++;

    foreach (var evt in events)
    {
      Effect.addEffects(_effects, evt);
    }

    var hadEffects = _effects.isNotEmpty();
    _effects.RemoveAll((effect) => !effect.update(_gameScreen.game));

    // TODO: Re-rendering the entire screen when only animated tiles have
    // changed is pretty rough on CPU usage. Maybe optimize to only redraw the
    // animated tiles if that's all that happened in a turn?
    return _hasAnimatedTile ||
        hadEffects ||
        _effects.isNotEmpty() ||
        _gameScreen.game.hero.dazzle.isActive;
  }

  public override void renderPanel(Terminal terminal)
  {
    _positionCamera(new Vec(terminal.width, terminal.height));

    visibleMonsters.Clear();
    _hasAnimatedTile = false;

    var game = _gameScreen.game;
    var hero = game.hero;

    // Draw the tiles and items.
    foreach (var pos in _cameraBounds)
    {
      int? ch = 0;
      var fore = Color.black;
      var back = Color.black;

      // Show tiles containing interesting things more brightly.
      var lightFore = false;
      var lightBack = false;

      // Even if not currently visible, if explored we can see the tile itself.
      var tile = game.stage[pos];
      if (tile.isExplored)
      {
        var tileGlyph = _tileGlyph(pos, tile);
        ch = tileGlyph.ch;
        fore = tileGlyph.fore;
        back = tileGlyph.back;
        lightFore = true;
        lightBack = true;

        // Show the item if the tile has been explored, even if not currently
        // visible.
        // TODO: Should this show what the player last saw when the tile was
        // visible?
        var items = game.stage.itemsAt(pos);
        if (items.isNotEmpty)
        {
          var itemGlyph = items.First().appearance as Glyph;
          ch = itemGlyph.ch;
          fore = itemGlyph.fore;
          lightFore = false;
        }
      }

      // If the tile is currently visible, show any actor on it.
      if (tile.isVisible)
      {
        if (tile.substance != 0)
        {
          if (tile.element == Elements.fire)
          {
            ch = Rng.rng.item<int>(_fireChars);
            var color = Rng.rng.item<Color[]>(_fireColors);
            fore = color[0];
            back = color[1];

            _hasAnimatedTile = true;
          }
          else if (tile.element == Elements.poison)
          {
            var amount = 0.1f + (tile.substance / 255f) * 0.9f;
            back = back.Blend(Hues.lima, amount);
          }
        }
      }

      var actor = game.stage.actorAt(pos);
      var showActor = tile.isVisible ||
          pos == game.hero.pos ||
          Debugger.showAllMonsters ||
          actor != null && hero.canPerceive(actor);

      if (showActor && actor != null)
      {
        var actorGlyph = actor.appearance;
        if (actorGlyph is Glyph)
        {
          var actorGlyph_ = actorGlyph as Glyph;
          ch = actorGlyph_.ch;
          fore = actorGlyph_.fore;
        }
        else
        {
          // Hero.
          ch = CharCode.at;
          fore = _gameScreen.heroColor;
        }
        lightFore = false;

        // If the actor is being targeted, invert its colors.
        if (_gameScreen.currentTargetActor == actor)
        {
          back = fore;
          fore = Hues.darkerCoolGray;
          lightBack = false;
        }

        if (actor is Monster) visibleMonsters.Add(actor as Monster);
      }

      if (hero.dazzle.isActive)
      {
        var chance = Math.Min(90, hero.dazzle.duration * 8);
        if (Rng.rng.percent(chance))
        {
          ch = Rng.rng.percent(chance) ? ch : CharCode.asterisk;
          fore = Rng.rng.item<Color>(_dazzleColors);
        }

        lightFore = false;
        lightBack = false;
      }

      Color multiply(Color a, Color b)
      {
        return new Color(a.r * b.r / 255, a.g * b.g / 255, a.b * b.b / 255);
      }

      // TODO: This could be cached if needed.
      var foreShadow = multiply(fore, new Color(80, 80, 95));
      var backShadow = multiply(back, new Color(40, 40, 55));

      // Apply lighting and visibility to the tile.
      if (tile.isVisible && (lightFore || lightBack))
      {
        Color applyLighting(Color color, Color shadow)
        {
          // Apply a slight brightness curve to either end of the range of
          // floor illumination. We keep most of the middle of the range flat
          // so that there is still a visible ramp down at the dark end and
          // just a small bloom around lights at the bright end.
          var visibility = tile.floorIllumination - tile.fallOff;
          if (visibility < 64)
          {
            // Only blend up to 50% of the shadow color so that there is a
            // clear line between hidden and visible tiles.
            color =
                color.Blend(shadow, (float)MathUtils.lerpDouble(visibility, 0, 64, 0.5, 0.0));
          }
          else if (visibility > 128)
          {
            color = color.Add(Hues.ash, (float)MathUtils.lerpDouble(visibility, 128, 255, 0.0, 0.2));
          }

          if (tile.actorIllumination > 0)
          {
            Color glow = new Color(200, 130, 0);
            color = color.Add(
                glow, (float)MathUtils.lerpDouble(tile.actorIllumination, 0, 255, 0.05, 0.1));
          }

          return color;
        }

        if (lightFore) fore = applyLighting(fore, foreShadow);
        if (lightBack) back = applyLighting(back, backShadow);
      }
      else
      {
        if (lightFore) fore = foreShadow;
        if (lightBack) back = backShadow;
      }

      if (Debugger.showHeroVolume)
      {
        var volume = game.stage.heroVolume(pos);
        if (volume > 0.0) back = back.Blend(Hues.peaGreen, (float)volume);
      }

      if (Debugger.showMonsterAlertness && actor is Monster)
      {
        back = Color.blue.Blend(Color.red, (float)(actor as Monster).alertness);
      }

      if (ch != null)
      {
        var glyph = new Glyph(ch.Value, fore, back);
        _drawStageGlyph(terminal, pos.x, pos.y, glyph);
      }
    }

    // Draw the effects.
    foreach (var effect in _effects)
    {
      // TODO: Allow effects to preserve the tile's existing background color.
      effect.render(game, (x, y, glyph) =>
      {
        _drawStageGlyph(terminal, x, y, glyph);
      });
    }
  }

  /// Gets the [Glyph] to render for [tile].
  Glyph _tileGlyph(Vec pos, Tile tile)
  {
    // If the appearance is a single glyph, it's a normal tile.
    var appearance = tile.type.appearance;
    if (appearance is Glyph) return appearance as Glyph;

    // Otherwise it's an animated tile, like water.
    var glyphs = appearance as List<Glyph>;

    // Ping pong back and forth.
    var period = glyphs.Count * 2 - 2;

    // Calculate a "random" but consistent phase for each position.
    var phase = MathUtils.hashPoint(pos.x, pos.y);
    var frame = (_frame / 8 + phase) % period;
    if (frame >= glyphs.Count)
    {
      frame = glyphs.Count - (frame - glyphs.Count) - 1;
    }

    _hasAnimatedTile = true;
    return glyphs[frame];
  }

  /// Determines which portion of the [Stage] should be in view based on the
  /// position of the [Hero].
  void _positionCamera(Vec size)
  {
    var game = _gameScreen.game;

    // Handle the stage being smaller than the view.
    var rangeWidth = Math.Max(0, game.stage.width - size.x);
    var rangeHeight = Math.Max(0, game.stage.height - size.y);

    var cameraRange = new Rect(0, 0, rangeWidth, rangeHeight);

    var camera = game.hero.pos - size / 2;
    camera = cameraRange.clamp(camera);
    _cameraBounds = new Rect(camera.x, camera.y, Math.Min(size.x, game.stage.width),
        Math.Min(size.y, game.stage.height));

    _renderOffset = new Vec(Math.Max(0, size.x - game.stage.width) / 2,
        Math.Max(0, size.y - game.stage.height) / 2);
  }
}
