using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;
using System.Linq;

// TODO: More disciplines:
// - Dodging attacks, which increases dodge.
// - Fury. Increases damage when health is low. Trained by killing monsters
//   when near death.

abstract class MasteryDiscipline : Discipline
{
  // TODO: Tune.
  public override int maxLevel => 20;

  public virtual string weaponType => "";

  double _damageScale(int level) => MathUtils.lerpDouble(level, 1, maxLevel, 1.0, 2.0);

  public override void modifyAttack(Hero hero, Monster monster, Hit hit, int level)
  {
    if (!_hasWeapon(hero)) return;

    // TODO: Tune.
    hit.scaleDamage(_damageScale(level));
  }

  public override string levelDescription(int level)
  {
    var damage = (int)((_damageScale(level) - 1.0) * 100);
    return $"Melee attacks inflict {damage}% more damage when using a {weaponType}.";
  }

  string unusableReason(Game game)
  {
    if (_hasWeapon(game.hero)) return null;

    return $"No {weaponType} equipped.";
  }

  bool _hasWeapon(Hero hero) =>
      hero.equipment.weapons.Any((item) => item.type.weaponType == weaponType);

  public override void killMonster(Hero hero, Action action, Monster monster)
  {
    // Have to have killed the monster by hitting it.
    if ((action is AttackAction) == false) return;

    if (!_hasWeapon(hero)) return;

    hero.skills.earnPoints(this, Mathf.CeilToInt(monster.experience / 100f));
    hero.refreshSkill(this);
  }

  public override int baseTrainingNeeded(int level)
  {
    if (level == 0) return 0;

    // Get the mastery and unlock the action as soon as it's first used.
    if (level == 1) return 1;

    // TODO: Tune.
    level--;
    return 100 * level * level * level;
  }
}

abstract class MasteryAction : Action
{
  public double damageScale;

  public MasteryAction(double damageScale)
  {
    this.damageScale = damageScale;
  }

  public virtual string weaponType => "";

  /// Attempts to hit the [Actor] at [pos], if any.
  public int? attack(Vec pos)
  {
    var defender = game.stage.actorAt(pos);
    if (defender == null) return null;

    // If dual-wielding two weapons of the mastered type, both are used.
    var weapons = hero.equipment.weapons.ToList();
    var hits = hero.createMeleeHits(defender);
    Debugger.assert(weapons.Count == hits.Count);

    var damage = 0;
    for (var i = 0; i < weapons.Count; i++)
    {
      if (weapons[i].type.weaponType != weaponType) continue;

      var hit = hits[i];
      hit.scaleDamage(damageScale);
      damage += hit.perform(this, actor, defender);

      if (!defender.isAlive) break;
    }

    return damage;
  }

  public override double noise => Sound.attackNoise;
}
