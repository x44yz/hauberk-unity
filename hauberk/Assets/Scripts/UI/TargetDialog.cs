using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

/// Modal dialog for letting the user select a target to perform a
/// [UsableSkill] on.
class TargetDialog : Screen
{
  public const int _numFrames = 5;
  public const int _ticksPerFrame = 5;

  public GameScreen _gameScreen;
  public int _range;
  public System.Action<Vec> _onSelect;
  public List<Monster> _monsters = new List<Monster>();

  bool _targetingFloor = false;
  int _animateOffset = 0;

  public override bool isTransparent => true;

  // TODO: Prevent targeting self if skill doesn't allow it?

  public TargetDialog(GameScreen _gameScreen, int _range, System.Action<Vec> _onSelect)
  {
    this._gameScreen = _gameScreen;
    this._range = _range;
    this._onSelect = _onSelect;

    // Find the targetable monsters.
    var hero = _gameScreen.game.hero;
    foreach (var actor in _gameScreen.game.stage.actors)
    {
      if ((actor is Monster) == false) continue;
      if (!hero.canPerceive(actor)) continue;

      // Must be within range.
      var toMonster = actor.pos - hero.pos;
      if (toMonster > _range) continue;

      _monsters.Add(actor as Monster);
    }

    if (_monsters.isEmpty())
    {
      // No visible monsters, so switch to floor targeting.
      _targetingFloor = true;
      _gameScreen.targetFloor(_gameScreen.game.hero.pos);
    }
    else
    {
      // Default to targeting the nearest monster to the hero.
      _targetNearest(_gameScreen.game.hero.pos);
    }
  }

  bool _targetNearest(Vec pos)
  {
    if (_monsters.isEmpty()) return false;

    Actor nearest = null;
    foreach (var monster in _monsters)
    {
      if (nearest == null || pos - monster.pos < pos - nearest.pos)
      {
        nearest = monster;
      }
    }

    _gameScreen.targetActor(nearest);
    return true;
  }

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == InputX.ok)
      if (_gameScreen.currentTarget != null)
      {
        terminal.Pop();
        _onSelect(_gameScreen.currentTarget!);
      }
      else if (keyCode == InputX.cancel)
        terminal.Pop();
      else if (keyCode == InputX.nw)
        _changeTarget(Direction.nw);
      else if (keyCode == InputX.n)
        _changeTarget(Direction.n);
      else if (keyCode == InputX.ne)
        _changeTarget(Direction.ne);
      else if (keyCode == InputX.w)
        _changeTarget(Direction.w);
      else if (keyCode == InputX.e)
        _changeTarget(Direction.e);
      else if (keyCode == InputX.sw)
        _changeTarget(Direction.sw);
      else if (keyCode == InputX.s)
        _changeTarget(Direction.s);
      else if (keyCode == InputX.se)
        _changeTarget(Direction.se);
      else if (keyCode == KeyCode.Tab && _monsters.isNotEmpty())
      {
        _targetingFloor = !_targetingFloor;
        if (!_targetingFloor)
        {
          // Target the nearest monster to the floor tile we were previously
          // targeting.
          _targetNearest(_gameScreen.currentTarget ?? _gameScreen.game.hero.pos);
        }
        else
        {
          _gameScreen.targetFloor(_gameScreen.currentTarget);
        }
        return true;
      }

    return false;
  }

  public override void Tick(float dt)
  {
    _animateOffset = (_animateOffset + 1) % (_numFrames * _ticksPerFrame);
    if (_animateOffset % _ticksPerFrame == 0) Dirty();
  }

  public override void Render(Terminal terminal)
  {
    var stage = _gameScreen.game.stage;
    var hero = _gameScreen.game.hero;

    // Show the range field.
    foreach (var pos in _gameScreen.cameraBounds)
    {
      var tile1 = stage[pos];
      var actor = stage.actorAt(pos);

      // Don't leak information to the player about unknown tiles. Instead,
      // treat them as potentially targetable.
      if (tile1.isExplored)
      {
        // If the tile can't be reached, don't show it as targetable.
        if (tile1.isOccluded) continue;

        if (!tile1.isWalkable && tile1.blocksView) continue;

        // Don't obscure monsters and items.
        if (actor != null) continue;
        if (stage.isItemAt(pos)) continue;
      }
      else if (_isKnownOccluded(pos))
      {
        // The player knows it can't be targeted.
        continue;
      }
      else if (actor != null && hero.canPerceive(actor))
      {
        // Show the actor.
        continue;
      }

      // Must be in range.
      var toPos = pos - hero.pos;
      if (toPos > _range) continue;

      int charCode;
      if (tile1.isExplored)
      {
        var appearance = tile1.type.appearance;
        if (appearance is Glyph)
        {
          charCode = (appearance as Glyph).ch;
        }
        else
        {
          charCode = (appearance as List<Glyph>)[0].ch;
        }
      }
      else
      {
        // Since the hero doesn't know what's on the tile, optimistically guess
        // that it's some kind of floor.
        charCode = CharCode.middleDot;
      }

      _gameScreen.drawStageGlyph(
          terminal, pos.x, pos.y, new Glyph(charCode, Hues.gold));
    }

    var target = _gameScreen.currentTarget;
    if (target == null) return;

    // Don't target a tile the player knows can't be hit.
    var reachedTarget = false;
    var tile = _gameScreen.game.stage[target];
    var frame = 0;
    if (!tile.isExplored || (!tile.blocksView && !tile.isOccluded))
    {
      // Show the path that the bolt will trace, stopping when it hits an
      // obstacle.
      frame = _animateOffset / _ticksPerFrame;
      foreach (var pos in new Line(_gameScreen.game.hero.pos, target))
      {
        // Note if we made it to the target.
        if (pos == target)
        {
          reachedTarget = true;
          break;
        }

        var tile2 = stage[pos];

        // Don't leak information about unexplored tiles.
        if (tile2.isExplored)
        {
          if (stage.actorAt(pos) != null) break;
          if (!tile2.isFlyable) break;
        }

        _gameScreen.drawStageGlyph(
            terminal,
            pos.x,
            pos.y,
            new Glyph(
                CharCode.bullet, (frame == 0) ? Hues.gold : Hues.darkCoolGray));
        frame = (frame + _numFrames - 1) % _numFrames;
      }
    }

    // Highlight the reticle if the bolt will reach the target.
    var reticleColor = Hues.darkCoolGray;
    if (reachedTarget) reticleColor = (frame == 0) ? Hues.gold : Hues.darkCoolGray;
    _gameScreen.drawStageGlyph(
        terminal, target.x - 1, target.y, new Glyph('-', reticleColor));
    _gameScreen.drawStageGlyph(
        terminal, target.x + 1, target.y, new Glyph('-', reticleColor));
    _gameScreen.drawStageGlyph(
        terminal, target.x, target.y - 1, new Glyph('|', reticleColor));
    _gameScreen.drawStageGlyph(
        terminal, target.x, target.y + 1, new Glyph('|', reticleColor));
    if (!reachedTarget)
    {
      _gameScreen.drawStageGlyph(
          terminal, target.x, target.y, new Glyph('X', reticleColor));
    }

    var helpKeys = new Dictionary<string, string> { };
    if (_monsters.isEmpty())
    {
      helpKeys["↕↔"] = "Choose tile";
    }
    else if (_targetingFloor)
    {
      helpKeys["↕↔"] = "Choose tile";
      helpKeys["Tab"] = "Target monsters";
    }
    else
    {
      helpKeys["↕↔"] = "Choose monster";
      helpKeys["Tab"] = "Target floor";
    }
    helpKeys["Esc"] = "Cancel";
    Draw.helpKeys(terminal, helpKeys, "Choose a target");
  }

  void _changeTarget(Direction dir)
  {
    if (_targetingFloor)
    {
      _changeFloorTarget(dir);
    }
    else
    {
      _changeMonsterTarget(dir);
    }
  }

  void _changeFloorTarget(Direction dir)
  {
    var pos = _gameScreen.currentTarget! + dir;

    // Don't target out of range.
    var toPos = pos - _gameScreen.game.hero.pos;
    if (toPos > _range) return;

    _gameScreen.targetFloor(pos);
  }

  /// Target the nearest monster in [dir] from the current target. Precisely,
  /// draws a line perpendicular to [dir] and divides the monsters into two
  /// half-planes. If the half-plane towards [dir] contains any monsters, then
  /// this targets the nearest one. Otherwise, it wraps around and targets the
  /// *farthest* monster in the other half-place.
  void _changeMonsterTarget(Direction dir)
  {
    var ahead = new List<Monster>();
    var behind = new List<Monster>();

    var target = _gameScreen.currentTarget!;
    var perp = dir.rotateLeft90;
    foreach (var monster in _monsters)
    {
      var relative = monster.pos - target;
      var dotProduct = perp.x * relative.y - perp.y * relative.x;
      if (dotProduct > 0)
      {
        ahead.Add(monster);
      }
      else
      {
        behind.Add(monster);
      }
    }

    var nearest = DartUtils._findLowest<Monster>(
        ahead, (monster) => (monster.pos - target).lengthSquared);
    if (nearest != null)
    {
      _gameScreen.targetActor(nearest);
      return;
    }

    var farthest = DartUtils._findHighest<Monster>(
        behind, (monster) => (monster.pos - target).lengthSquared);
    if (farthest != null)
    {
      _gameScreen.targetActor(farthest);
    }
  }

  /// Returns `true` if there is at least one *explored* tile that block LOS to
  /// [target].
  ///
  /// We need to ensure the targeting dialog doesn't leak information about
  /// unexplored tiles. At the same time, we do want to let the player try to
  /// target unexplored tiles because they may turn out to be reachable. (In
  /// particular, it's useful to let them lob light sources into the dark.)
  ///
  /// This is used to determine which unexplored tiles should be treated as
  /// targetable. We don't want to allow all unexplored tiles to be targeted
  /// because that would include tiles behind known walls, so this filters out
  /// any tile that is blocked by a known tile.
  bool _isKnownOccluded(Vec target)
  {
    var stage = _gameScreen.game.stage;

    foreach (var pos in new Line(_gameScreen.game.hero.pos, target))
    {
      // Note if we made it to the target.
      if (pos == target) return false;

      if (!stage.bounds.contains(pos)) return true;

      var tile = stage[pos];
      if (tile.isExplored && tile.blocksView) return true;
    }

    throw new System.Exception("Unreachable.");
  }
}
