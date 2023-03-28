using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;

public class DualWield : Discipline
{
  // TODO: Tune.
  public override int maxLevel => 10;

  static double _heftModifier(int level) => MathUtils.lerpDouble(level, 0, 10, 1.0, 0.5);

  public override string name => "Dual-wield";

  public override string description =>
      "Attack with a weapon in each hand as effectively as other lesser " +
      "warriors do with only a single weapon in their puny arms.";

  public override string levelDescription(int level) => "Reduces heft when dual-wielding by " +
      $"{(int)((1.0 - _heftModifier(level)) * 100)}%.";

  public override int baseTrainingNeeded(int level) => 100 * level * level * level;

  public override void dualWield(Hero hero)
  {
    hero.discoverSkill(this);
  }

  public override double modifyHeft(Hero hero, int level, double heftModifier)
  {
    // Have to be dual-wielding.
    if (hero.equipment.weapons.Count != 2) return heftModifier;

    return heftModifier * _heftModifier(level);
  }

  public override void killMonster(Hero hero, Action action, Monster monster)
  {
    // Have to have killed the monster by hitting it.
    if ((action is AttackAction) == false) return;

    // Have to be dual-wielding.
    if (hero.equipment.weapons.Count != 2) return;

    hero.skills.earnPoints(this, Mathf.CeilToInt(monster.experience / 100f));
    hero.refreshSkill(this);
  }
}

