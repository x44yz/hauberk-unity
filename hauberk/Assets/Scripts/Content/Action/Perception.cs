using System.Collections;
using System.Collections.Generic;

/// An [Action] that gives the hero temporary monster perception.
class PerceiveAction : Action
{
  public int _duration;
  public int _distance;

  public PerceiveAction(int _duration, int _distance)
  {
    this._duration = _duration;
    this._distance = _distance;
  }

  // TODO: Options for range and monster tag.
  public override bool isImmediate => false;

  public override ActionResult onPerform()
  {
    var alreadyPerceived = new List<Actor> { };
    foreach (var actor in game.stage.actors)
    {
      if (actor == hero) continue;

      if (hero.canPerceive(actor)) alreadyPerceived.Add(actor);
    }

    hero.perception.activate(_duration, _distance);

    var perceived = false;
    foreach (var actor in game.stage.actors)
    {
      if (actor == hero) continue;

      if (hero.canPerceive(actor) && !alreadyPerceived.Contains(actor))
      {
        addEvent(EventType.perceive, actor: actor);
        perceived = true;
      }
    }

    if (perceived)
    {
      return succeed("{1} perceive[s] monsters beyond your sight!", actor);
    }
    else
    {
      return succeed("{1} do[es]n't perceive anything.", actor);
    }
  }
}
