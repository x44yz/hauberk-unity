using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

/// Modal dialog for letting the user select a [Direction] to perform a command.
abstract class DirectionDialog : Screen
{
  public const int _numFrames = 8;
  public const int _ticksPerFrame = 5;

  public GameScreen _gameScreen;

  int _animateOffset = 0;

  public override bool isTransparent => true;

  public Game game => _gameScreen.game;

  public virtual string query => "";

  public virtual string helpText => "";

  public DirectionDialog(GameScreen _gameScreen)
  {
    this._gameScreen = _gameScreen;
  }

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == InputX.cancel)
      _select(Direction.none);
    else if (keyCode == InputX.nw)
      _select(Direction.nw);
    else if (keyCode == InputX.n)
      _select(Direction.n);
    else if (keyCode == InputX.ne)
      _select(Direction.ne);
    else if (keyCode == InputX.w)
      _select(Direction.w);
    else if (keyCode == InputX.e)
      _select(Direction.e);
    else if (keyCode == InputX.sw)
      _select(Direction.sw);
    else if (keyCode == InputX.s)
      _select(Direction.s);
    else if (keyCode == InputX.se)
      _select(Direction.se);

    return true;
  }

  public override void Tick(float dt)
  {
    _animateOffset = (_animateOffset + 1) % (_numFrames * _ticksPerFrame);
    if (_animateOffset % _ticksPerFrame == 0) Dirty();
  }

  void draw(int frame, Direction dir, char ch)
  {
    var pos = game.hero.pos + dir;
    if (!canTarget(game.stage[pos])) return;

    Glyph glyph;
    if (_animateOffset / _ticksPerFrame == frame)
    {
      glyph = new Glyph(ch, Hues.gold, Hues.brown);
    }
    else
    {
      // TODO: TargetDialog and GameScreen have similar code. Unify?
      var actor = game.stage.actorAt(pos);
      if (actor != null)
      {
        glyph = actor.appearance as Glyph;
      }
      else
      {
        var items = game.stage.itemsAt(pos);
        if (items.isNotEmpty)
        {
          glyph = items.First().appearance as Glyph;
        }
        else
        {
          var tile = game.stage[pos];
          if (tile.isExplored)
          {
            glyph = tile.type.appearance as Glyph;
          }
          else
          {
            // Since the hero doesn't know what's on the tile, show it as a
            // blank highlighted tile.
            glyph = new Glyph(CharCode.space);
          }

          glyph = game.stage[pos].type.appearance as Glyph;
        }
      }

      glyph = new Glyph(glyph.ch, Hues.gold, Hues.brown);
    }

    _gameScreen.drawStageGlyph(terminal, pos.x, pos.y, glyph);
  }

  public override void Render(Terminal terminal)
  {

    draw(0, Direction.n, '|');
    draw(1, Direction.ne, '/');
    draw(2, Direction.e, '-');
    draw(3, Direction.se, '\\');
    draw(4, Direction.s, '|');
    draw(5, Direction.sw, '/');
    draw(6, Direction.w, '-');
    draw(7, Direction.nw, '\\');

    Draw.helpKeys(terminal,
      new Dictionary<string, string> { { "↕↔", helpText }, { "Esc", "Cancel" } }, query);
  }

  void _select(Direction dir)
  {
    if (tryDirection(dir))
    {
      terminal.Pop(dir);
    }
    else
    {
      terminal.Pop(Direction.none);
    }
  }

  public abstract bool canTarget(Tile tile);

  public abstract bool tryDirection(Direction direction);
}

/// Asks the user to select a direction for a [DirectionSkill].
class SkillDirectionDialog : DirectionDialog
{
  public System.Action<Direction> _onSelect;

  public override string query => "Which direction?";

  public override string helpText => "Choose direction";

  public SkillDirectionDialog(GameScreen gameScreen, System.Action<Direction> _onSelect)
      : base(gameScreen)
  {
    this._onSelect = _onSelect;
  }

  // TODO: Let skill filter out invalid directions.
  public override bool canTarget(Tile tile) => true;

  public override bool tryDirection(Direction direction)
  {
    _onSelect(direction);
    return true;
  }
}

/// Asks the user to select an adjacent tile to close.
class CloseDialog : DirectionDialog
{
  public override string query => "Close what?";
  public override string helpText => "Choose direction";

  public CloseDialog(GameScreen gameScreen) : base(gameScreen)
  {

  }

  public override bool canTarget(Tile tile) => tile.type.canClose;

  public override bool tryDirection(Direction direction)
  {
    var pos = game.hero.pos + direction;
    var tile = game.stage[pos].type;
    if (tile.canClose)
    {
      game.hero.setNextAction(tile.onClose!(pos));
      return true;
    }
    else
    {
      game.log.error("There is nothing to close there.");
      return false;
    }
  }
}

/// Asks the user to select an adjacent tile to open.
class OpenDialog : DirectionDialog
{
  public override string query => "Open what?";
  public override string helpText => "Choose direction";

  public OpenDialog(GameScreen gameScreen) : base(gameScreen)
  {
  }

  public override bool canTarget(Tile tile) => tile.type.canOpen;

  public override bool tryDirection(Direction direction)
  {
    var pos = game.hero.pos + direction;
    var tile = game.stage[pos].type;
    if (tile.canOpen)
    {
      game.hero.setNextAction(tile.onOpen!(pos));
      return true;
    }
    else
    {
      game.log.error("There is nothing to open there.");
      return false;
    }
  }
}
