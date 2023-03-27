using System;
using System.Collections.Generic;
using num = System.Double;

// TODO: BoltMove does not pass its range to BoltAction. Probably OK since
// monster won't choose move if out of range, but it means the action can go
// farther than intended range.
class BoltMove : RangedMove
{
  public override num experience =>
      attack.damage * attack.element.experience * (1.0 + range / 20.0);

  public BoltMove(num rate, Attack attack) : base(rate, attack)
  {

  }

  public override bool shouldUse(Monster monster)
  {
    if (monster.isBlinded && Rng.rng.rfloat(1.0) < monster.sightReliance)
    {
      var chance =
          (int)MathUtils.lerpDouble((float)monster.sightReliance, 0.0f, 1.0f, 0.0, 90.0);
      if (Rng.rng.percent(chance)) return false;
    }

    var target = monster.game.hero.pos;

    // Don't fire if out of range.
    var toTarget = target - monster.pos;
    if (toTarget > range)
    {
      Debugger.monsterLog(monster, "bolt move too far");
      return false;
    }
    if (toTarget < 1.5f)
    {
      Debugger.monsterLog(monster, "bolt move too close");
      return false;
    }

    // Don't fire a bolt if it's obstructed.
    if (!monster.canTarget(target))
    {
      Debugger.monsterLog(monster, "bolt move can't target");
      return false;
    }

    Debugger.monsterLog(monster, "bolt move OK");
    return true;
  }

  public override Action onGetAction(Monster monster) =>
      new BoltAction(monster.game.hero.pos, attack.createHit());

  public override string ToString() => $"Bolt {attack} rate: {rate}";
}
