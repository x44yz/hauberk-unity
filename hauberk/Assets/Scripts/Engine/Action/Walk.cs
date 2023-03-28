using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;

public class WalkAction : Action
{
  public Direction dir;
  public WalkAction(Direction dir)
  {
    this.dir = dir;
  }

  public override ActionResult onPerform()
  {
    // Rest if we aren't moving anywhere.
    if (dir == Direction.none)
    {
      return alternate(new RestAction());
    }

    var pos = actor!.pos + dir;

    // See if there is an actor there.
    var target = game.stage.actorAt(pos);
    if (target != null && target != actor)
    {
      return alternate(new AttackAction(target));
    }

    // See if it can be opened.
    var tile = game.stage[pos].type;
    if (tile.canOpen)
    {
      return alternate(tile.onOpen!(pos));
    }

    // See if we can walk there.
    if (!actor!.canOccupy(pos))
    {
      // If the hero runs into something in the dark, they can figure out what
      // it is.
      if (actor is Hero)
      {
        game.stage.explore(pos, force: true);
      }

      return fail($"{1} hit[s] the {tile.name}.", actor);
    }

    actor!.pos = pos;

    // See if the hero stepped on anything interesting.
    if (actor is Hero)
    {
      // FIX:@dongl1n
      // fix error caused by remove item
      var items = game.stage.itemsAt(pos)._items;
      for (int i = items.Count - 1; i >= 0; --i)
      {
        var item = items[i];
        hero.disturb();

        // Treasure is immediately, freely acquired.
        if (item.isTreasure)
        {
          // Pick a random value near the price.
          var min = Mathf.CeilToInt(item.price * 0.5f);
          var max = Mathf.CeilToInt(item.price * 1.5f);
          var value = Rng.rng.range(min, max);
          hero.gold += value;
          log($"{1} pick[s] up {2} worth {value} gold.", hero, item);
          game.stage.removeItem(item, pos);

          addEvent(EventType.gold, actor: actor, pos: actor!.pos, other: item);
        }
        else
        {
          log("{1} [are|is] standing on {2}.", actor, item);
        }
      }

      hero.regenerateFocus(4);
    }

    return succeed();
  }

  public override string ToString() => $"{actor} walks {dir}";
}

class OpenDoorAction : Action
{
  public Vec pos;
  public TileType openDoor;

  public OpenDoorAction(Vec pos, TileType openDoor)
  {
    this.pos = pos;
    this.openDoor = openDoor;
  }

  public override ActionResult onPerform()
  {
    game.stage[pos].type = openDoor;
    game.stage.tileOpacityChanged();

    if (actor is Hero) hero.regenerateFocus(4);
    return succeed("{1} open[s] the door.", actor);
  }
}

class CloseDoorAction : Action
{
  public Vec doorPos;
  public TileType closedDoor;

  public CloseDoorAction(Vec doorPos, TileType closedDoor)
  {
    this.doorPos = doorPos;
    this.closedDoor = closedDoor;
  }

  public override ActionResult onPerform()
  {
    var blockingActor = game.stage.actorAt(doorPos);
    if (blockingActor != null)
    {
      return fail("{1} [are|is] in the way!", blockingActor);
    }

    // TODO: What should happen if items are on the tile?
    game.stage[doorPos].type = closedDoor;
    game.stage.tileOpacityChanged();

    if (actor is Hero) hero.regenerateFocus(4);
    return succeed("{1} close[s] the door.", actor);
  }
}

/// Action for doing nothing for a turn.
class RestAction : Action
{
  public override ActionResult onPerform()
  {
    if (actor is Hero)
    {
      if (hero.stomach > 0 && !hero.poison.isActive)
      {
        // TODO: Does this scale well when the hero has very high max health?
        // Might need to go up by more than one point then.
        hero.health++;
      }

      // TODO: Have this amount increase over successive resting turns?
      hero.regenerateFocus(10);
    }
    else if (!actor!.isVisibleToHero)
    {
      // Monsters can rest if out of sight.
      actor!.health++;
    }

    return succeed();
  }

  public override double noise => Sound.restNoise;
}
