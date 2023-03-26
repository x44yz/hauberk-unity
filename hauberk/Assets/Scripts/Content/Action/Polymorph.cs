using System.Collections;
using System.Collections.Generic;

// TODO: Use this for more things.
// - Monsters that have a "public form" when killed.
// - Werewolves and other shapeshifters.
/// Turns the monster into [Breed].
class PolymorphAction : Action
{
  public Breed _breed;

  public PolymorphAction(Breed _breed)
  {
    this._breed = _breed;
  }

  public override ActionResult onPerform()
  {
    monster.breed = _breed;
    addEvent(EventType.polymorph, actor: actor);

    // TODO: Message?
    return ActionResult.success;
  }
}

/// "Amputates" part of the monster by spawning a new one for the part and
/// polymorphing the original into a different breed that lacks the part.
class AmputateAction : Action
{
  public Breed _bodyBreed;
  public Breed _partBreed;
  public string _message;

  public AmputateAction(Breed _bodyBreed, Breed _partBreed, string _message)
  {
    this._bodyBreed = _bodyBreed;
    this._partBreed = _partBreed;
    this._message = _message;
  }

  public override ActionResult onPerform()
  {
    // Hack off the part.
    addAction(new PolymorphAction(_bodyBreed));

    log(_message, actor);

    // Create the part.
    var part = _partBreed.spawn(game, Vec.zero, monster);
    part.awaken();

    // Pick an open adjacent tile.
    var positions = new List<Vec> { };
    foreach (var dir in Direction.all)
    {
      var pos = actor!.pos + dir;
      if (part.canEnter(pos)) positions.Add(pos);
    }

    // If there's no room for the part, it disappears.
    if (positions.Count > 0)
    {
      part.pos = Rng.rng.item(positions);
      game.stage.addActor(part);

      // TODO: Different event?
      addEvent(EventType.spawn, actor: part);
    }

    return ActionResult.success;
  }
}
