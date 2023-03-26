using System;
using System.Collections.Generic;
using num = System.Double;

class HowlMove : Move
{
  public int _range;
  public string _verb;

  public override num experience => _range * 0.5;

  public HowlMove(num rate, int _range, string _verb) : base(rate)
  {
    this._range = _range;
    this._verb = _verb;
  }

  public override bool shouldUse(Monster monster)
  {
    // Don't wake up others unless the hero is around.
    // TODO: Should take sight into account.
    if (!monster.isVisibleToHero) return false;

    // See if there are any sleeping monsters nearby.
    foreach (var actor in monster.game.stage.actors)
    {
      if (actor == monster) continue;

      // If we found someone asleep, howl.
      if (actor is Monster &&
          (actor as Monster).isAsleep &&
          (actor.pos - monster.pos) <= _range)
      {
        return true;
      }
    }

    return false;
  }

  public override Action onGetAction(Monster monster) => new HowlAction(_range, _verb);

  public override string ToString() => $"Howl {_range}";
}
