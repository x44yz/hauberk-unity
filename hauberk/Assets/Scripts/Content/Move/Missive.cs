using System;
using System.Collections.Generic;
using num = System.Double;

class MissiveMove : Move
{
  public Missive _missive;

  public MissiveMove(Missive _missive, num rate) : base(rate)
  {
    this._missive = _missive;
  }

  public override num experience => 0.0;

  public override bool shouldUse(Monster monster)
  {
    var target = monster.game.hero.pos;
    var distance = (target - monster.pos).kingLength;

    // Don't insult when in melee distance.
    if (distance <= 1) return false;

    // Don't insult someone it can't see.
    return monster.canView(target);
  }

  public override Action onGetAction(Monster monster) =>
      new MissiveAction(monster.game.hero, _missive);

  public override string ToString() => $"{_missive} rate: {rate}";
}
