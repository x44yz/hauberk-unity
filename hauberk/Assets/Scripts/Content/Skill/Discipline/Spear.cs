using System.Collections;
using System.Collections.Generic;

using System.Linq;

class SpearMastery : MasteryDiscipline {
  // TODO: Tune.
  static double _spearScale(int level) => MathUtils.lerpDouble(level, 1, 10, 1.0, 3.0);

  // TODO: Better name.
  public override string name => "Spear Mastery";

  public override string useName => "Spear Attack";

  public override string description =>
      "Your diligent study of spears and polearms lets you attack at a " +
      "distance when wielding one.";

  public override string weaponType => "spear";

  public override string levelDescription(int level) {
    var damage = (int)(_spearScale(level) * 100);
    return base.levelDescription(level) +
        $" Distance spear attacks inflict {damage}% of the damage of a regular attack.";
  }

  int furyCost(HeroSave hero, int level) => 20;

  Action onGetDirectionAction(Game game, int level, Direction dir) =>
      new SpearAction(dir, SpearMastery._spearScale(level));
}

/// A melee attack that penetrates a row of actors.
class SpearAction : MasteryAction {
  public Direction _dir;

  public GeneratorActionMixin _generatorMixin;

  public SpearAction(Direction _dir, double damageScale) : base(damageScale)
  {
    this._dir = _dir;
    _generatorMixin = new GeneratorActionMixin();
  }

  public override bool isImmediate => false;
  public override string weaponType => "spear";

  public IEnumerator<ActionResult> _iterator => onGenerate().GetEnumerator();
  public override ActionResult onPerform() {
    // If it reaches the end, it succeeds.
    if (!_iterator.MoveNext()) return ActionResult.success;

    return _iterator.Current;
  }

  IEnumerable<ActionResult> onGenerate() {
    var rt = new List<ActionResult>();
    // Can only do a spear attack if the entire range is clear.
    for (var step = 1; step <= 2; step++) {
      var pos = actor!.pos + _dir * step;

      var tile = game.stage[pos];
      if (!tile.isExplored) {
        rt.Add(fail("You can't see far enough to aim."));
        return rt;
      }

      if (!tile.canEnter(Motility.fly)) {
        rt.Add(fail("There isn't enough room to use your weapon."));
        return rt;
      }
    }

    for (var step = 1; step <= 2; step++) {
      var pos = actor!.pos + _dir * step;

      // Show the effect and perform the attack on alternate frames. This
      // ensures the effect gets a chance to be shown before the hit effect
      //  covers hit.
      var weapon = hero.equipment.weapons.First().appearance;
      addEvent(EventType.stab, pos: pos, dir: _dir, other: weapon);
      rt.Add(_generatorMixin.waitOne());

      attack(pos);
      rt.Add(_generatorMixin.waitOne());
    }
    return rt;
  }

  public override string ToString() => $"{actor} spears {_dir}";
}
