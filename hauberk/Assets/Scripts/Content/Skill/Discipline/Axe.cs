using System.Collections.Generic;

/// A slashing melee attack that hits a number of adjacent monsters.
class AxeMastery : MasteryDiscipline
{
  // TODO: Tune.
  static double _sweepScale(int level) => MathUtils.lerpDouble(level, 1, 10, 1.0, 3.0);

  // TODO: Better name.
  public override string name => "Axe Mastery";
  public override string useName => "Axe Sweep";

  public override string description =>
      "Axes are not just for woodcutting. In the hands of a skilled user, " +
      "they can cut down a swath of nearby foes as well.";
  public override string weaponType => "axe";

  public override string levelDescription(int level)
  {
    var damage = (int)(_sweepScale(level) * 100);
    return base.levelDescription(level) +
        " Sweep attacks inflict $damage% of the damage of a regular attack.";
  }

  int furyCost(HeroSave hero, int level) => 20;

  Action onGetDirectionAction(Game game, int level, Direction dir)
  {
    return new SweepAction(dir, AxeMastery._sweepScale(level));
  }
}

/// A sweeping melee attack that hits three adjacent tiles.
class SweepAction : MasteryAction
{
  public Direction _dir;
  public GeneratorActionMixin _generatorMixin;

  public override bool isImmediate => false;

  public override string weaponType => "axe";

  public SweepAction(Direction _dir, double damageScale) : base(damageScale)
  {
    this._dir = _dir;
    _generatorMixin = new GeneratorActionMixin();
  }

  public IEnumerator<ActionResult> _iterator => onGenerate().GetEnumerator();
  public override ActionResult onPerform()
  {
    // If it reaches the end, it succeeds.
    if (!_iterator.MoveNext()) return ActionResult.success;

    return _iterator.Current;
  }

  IEnumerable<ActionResult> onGenerate()
  {
    // Make sure there is room to swing it.
    var rt = new List<ActionResult>();
    foreach (var dir in new Direction[] { _dir.rotateLeft45, _dir, _dir.rotateRight45 })
    {
      var pos = actor!.pos + dir;

      var tile = game.stage[pos];
      if (!tile.isExplored)
      {
        rt.Add(fail("You can't see where you're swinging."));
        return rt;
      }

      if (!tile.canEnter(Motility.fly))
      {
        rt.Add(fail("There isn't enough room to swing your weapon."));
        return rt;
      }
    }

    foreach (var dir in new Direction[] { _dir.rotateLeft45, _dir, _dir.rotateRight45 })
    {
      // Show the effect and perform the attack on alternate frames. This
      // ensures the effect gets a chance to be shown before the hit effect
      // covers hit.
      addEvent(EventType.slash, pos: actor!.pos + dir, dir: dir);
      rt.AddRange(_generatorMixin.wait(2));

      attack(actor!.pos + dir);
      rt.AddRange(_generatorMixin.wait(3));
    }
    return rt;
  }

  public override string ToString() => $"{actor} slashes {_dir}";
}
