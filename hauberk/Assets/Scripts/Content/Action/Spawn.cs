using System.Collections;
using System.Collections.Generic;

/// Spawns a new [Monster] of a given [Breed].
class SpawnAction : Action
{
  public Vec m_pos;
  public Breed _breed;

  public SpawnAction(Vec _pos, Breed _breed)
  {
    this.m_pos = _pos;
    this._breed = _breed;
  }

  public override ActionResult onPerform()
  {
    // There's a chance the move will do nothing (except burn charge) based on
    // the monster's generation. This is to keep breeders from filling the
    // dungeon.
    if (!Rng.rng.oneIn(monster.generation)) return ActionResult.success;

    // Increase the generation on the spawner too so that its rate decreases
    // over time.
    monster.generation++;

    var spawned = _breed.spawn(game, m_pos, monster);
    game.stage.addActor(spawned);

    addEvent(EventType.spawn, actor: spawned);

    // TODO: Message?
    return ActionResult.success;
  }
}