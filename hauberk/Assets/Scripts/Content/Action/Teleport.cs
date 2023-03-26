using System.Collections;
using System.Collections.Generic;

/// Teleports to a random tile with a given range.
class TeleportAction : Action
{
  public int distance;

  public TeleportAction(int distance)
  {
    this.distance = distance;
  }

  public override ActionResult onPerform()
  {
    var targets = new List<Vec> { };

    var bounds = Rect.intersect(
        Rect.leftTopRightBottom(actor!.x - distance, actor!.y - distance,
            actor!.x + distance, actor!.y + distance),
        game.stage.bounds);

    foreach (var pos in bounds)
    {
      if (!actor!.willEnter(pos)) continue;
      if (pos - actor!.pos > distance) continue;
      targets.Add(pos);
    }

    if (targets.Count == 0)
    {
      return fail("{1} couldn't escape.", actor);
    }

    // Try to teleport as far as possible.
    var best = Rng.rng.item(targets);

    for (var tries = 0; tries < 10; tries++)
    {
      var pos = Rng.rng.item(targets);
      if (pos - actor!.pos > best - actor!.pos) best = pos;
    }

    var from = actor!.pos;
    actor!.pos = best;
    addEvent(EventType.teleport, actor: actor, pos: from);
    return succeed("{1} teleport[s]!", actor);
  }
}