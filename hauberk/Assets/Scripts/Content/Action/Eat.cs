using System.Collections;
using System.Collections.Generic;

class EatAction : Action
{
  public int _amount;

  public EatAction(int _amount)
  {
    this._amount = _amount;
  }

  public override ActionResult onPerform()
  {
    if (hero.stomach == Option.heroMaxStomach)
    {
      log("{1} [is|are] already full!", actor);
    }
    else if (hero.stomach + _amount > Option.heroMaxStomach)
    {
      log("{1} [is|are] stuffed!", actor);
    }
    else
    {
      log("{1} feel[s] satiated.", actor);
    }

    hero.stomach += _amount;

    return ActionResult.success;
  }
}
