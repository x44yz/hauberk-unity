using System.Collections;
using System.Collections.Generic;

/// Alert nearby sleeping monsters.
class HowlAction : Action
{
  public int _range;
  public string _verb;

  public HowlAction(int _range, string verb = null)
  {
    this._range = _range;
    this._verb = verb ?? "howls";
  }

  public override ActionResult onPerform()
  {
    log($"{1} {_verb!}", actor);
    addEvent(EventType.howl, actor: actor);

    foreach (var other in monster.game.stage.actors)
    {
      if (other != actor &&
          other is Monster &&
          (other.pos - monster.pos) <= _range)
      {
        // TODO: Take range into account when attenuating volume?
        (other as Monster).hear(game.stage.volumeBetween(actor!.pos, other.pos));
      }
    }

    return ActionResult.success;
  }
}