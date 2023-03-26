using System;
using System.Collections.Generic;
using num = System.Double;

class ConeMove : Move
{
  public Attack attack;
  public override int range => attack.range;

  public override num experience =>
      attack.damage * 3.0 * attack.element.experience * (1.0 + range / 10.0);

  public ConeMove(num rate, Attack attack) : base(rate)
  {
    this.attack = attack;
  }

  public override bool shouldUse(Monster monster)
  {
    if (monster.isBlinded && Rng.rng.rfloat(1.0) < monster.sightReliance)
    {
      var chance =
          (int)MathUtils.lerpDouble((float)monster.sightReliance, 0.0f, 1.0f, 0.0, 70.0);
      if (Rng.rng.percent(chance)) return false;
    }

    var target = monster.game.hero.pos;

    // Don't fire if out of range.
    var toTarget = target - monster.pos;
    if (toTarget > range)
    {
      Debugger.monsterLog(monster, "cone move too far");
      return false;
    }

    // TODO: Should minimize friendly fire.
    if (!monster.canView(target))
    {
      Debugger.monsterLog(monster, "cone move can't target");
      return false;
    }

    Debugger.monsterLog(monster, "cone move OK");
    return true;
  }

  public override Action onGetAction(Monster monster) =>
      RayAction.cone(monster.pos, monster.game.hero.pos, attack.createHit());

  public override string ToString() => $"Cone {attack} rate: {rate}";
}
