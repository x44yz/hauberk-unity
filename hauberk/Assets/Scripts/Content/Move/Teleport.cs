using System;
using System.Collections.Generic;
using num = System.Double;


/// Teleports the [Monster] randomly from its current position.
class TeleportMove : Move
{
  public int _range;

  public override num experience => _range * 0.7;

  public TeleportMove(num rate, int _range) : base(rate)
  {
    this._range = _range;
  }

  public override bool shouldUse(Monster monster)
  {
    if (monster.isAfraid) return true;

    var target = monster.game.hero.pos;
    var distance = (target - monster.pos).kingLength;

    // If we're next to the hero and want to start there, don't teleport away.
    if (monster.wantsToMelee && distance <= 1) return false;

    return true;
  }

  public override Action onGetAction(Monster monster) => new TeleportAction(_range);

  public override string ToString() => "Teleport $_range";
}
