using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;

/// Base class for an [Action] that works with an existing [Item] in the game.
public abstract class ItemAction : Action
{
  /// The location of the item in the game.
  public ItemLocation location;

  /// The item.
  public Item item;

  public ItemAction(ItemLocation location, Item item)
  {
    this.location = location;
    this.item = item;
  }

  // TODO: There's a lot of duplication in the code that calls this to handle
  // splitting the stack in some cases or removing in others. Unify that here
  // or maybe in Item or ItemCollection.
  /// Removes the item from its current location so it can be placed elsewhere.
  public void removeItem()
  {
    if (location == ItemLocation.onGround)
    {
      game.stage.removeItem(item, actor!.pos);
    }
    else if (location == ItemLocation.inventory)
    {
      hero.inventory.remove(item);
    }
    else if (location == ItemLocation.equipment)
    {
      hero.equipment.remove(item);

      if (item.emanationLevel > 0)
      {
        game.stage.actorEmanationChanged();
      }
    }
    else
      throw new System.Exception("Invalid location.");
  }

  /// Called when the action has changed the count of [item].
  public void countChanged()
  {
    if (location == ItemLocation.onGround)
    {
      // TODO: Need to optimize stacks on the ground too.
      // If the hero picks up part of a floor stack, it should be reshuffled.
      //        game.stage.itemsAt(actor.pos).countChanged();
    }
    else if (location == ItemLocation.inventory)
    {
      hero.inventory.countChanged();
    }
    else if (location == ItemLocation.equipment)
    {
      hero.equipment.countChanged();
    }
    else
      throw new System.Exception("Invalid location.");
  }
}

/// [Action] for picking up an [Item] off the ground.
class PickUpAction : Action
{
  public Item item;

  public PickUpAction(Item item)
  {
    this.item = item;
  }

  public override ActionResult onPerform()
  {
    var result = hero.inventory.tryAdd(item, false);
    if (result.added == 0)
    {
      return fail("{1} [don't|doesn't] have room for {2}.", actor, item);
    }

    log("{1} pick[s] up {2}.", actor, item.clone(result.added));

    if (result.remaining == 0)
    {
      game.stage.removeItem(item, actor!.pos);
    }
    else
    {
      log("{1} [don't|doesn't] have room for {2}.", actor,
          item.clone(result.remaining));
    }

    hero.pickUp(item);
    return ActionResult.success;
  }
}

/// [Action] for dropping an [Item] from the [Hero]'s [Inventory] or [Equipment]
/// onto the ground.
class DropAction : ItemAction
{
  /// The number of items in the stack to drop.
  public int _count;

  public DropAction(ItemLocation location, Item item, int _count)
      : base(location, item)
  {
    this._count = _count;
  }

  public override ActionResult onPerform()
  {
    Item dropped;
    if (_count == item.count)
    {
      // Dropping the entire stack.
      dropped = item;
      removeItem();
    }
    else
    {
      dropped = item.splitStack(_count);
      countChanged();
    }

    if (location == ItemLocation.equipment)
    {
      log("{1} take[s] off and drop[s] {2}.", actor, dropped);
      hero.refreshProperties();
    }
    else
    {
      log("{1} drop[s] {2}.", actor, dropped);
    }

    game.stage.addItem(dropped, actor!.pos);
    return ActionResult.success;
  }
}

/// [Action] for moving an [Item] from the [Hero]'s [Inventory] to his
/// [Equipment]. May cause a currently equipped Item to become unequipped. If
/// there is no room in the Inventory for that Item, it will drop to the ground.
class EquipAction : ItemAction
{
  public EquipAction(ItemLocation location, Item item) : base(location, item)
  {

  }

  public override ActionResult onPerform()
  {
    // If it's already equipped, unequip it.
    if (location == ItemLocation.equipment)
    {
      return alternate(new UnequipAction(location, item));
    }

    if (!hero.equipment.canEquip(item))
    {
      return fail("{1} cannot equip {2}.", actor, item);
    }

    var equipped = item;
    if (item.count == 1)
    {
      removeItem();
    }
    else
    {
      equipped = item.splitStack(1);
      countChanged();
    }
    var unequipped = hero.equipment.equip(equipped);

    // Add the previously equipped items to inventory.
    foreach (var unequippedItem in unequipped)
    {
      // Make a copy with the original count for the message.
      var copy = unequippedItem.clone();
      var result = hero.inventory.tryAdd(unequippedItem, wasUnequipped: true);
      if (result.remaining == 0)
      {
        log("{1} unequip[s] {2}.", actor, copy);
      }
      else
      {
        // No room in inventory, so drop it.
        game.stage.addItem(unequippedItem, actor!.pos);
        log(
            "{1} [don't|doesn't] have room for {2} and {2 he} drops to the ground.",
            actor,
            unequippedItem);
      }
    }

    log("{1} equip[s] {2}.", actor, equipped);

    if (item.emanationLevel > 0)
    {
      game.stage.actorEmanationChanged();
    }

    hero.refreshProperties();
    return ActionResult.success;
  }
}

/// [Action] for moving an [Item] from the [Hero]'s [Equipment] to his
/// [Inventory]. If there is no room in the inventory, it will drop to the
/// ground.
class UnequipAction : ItemAction
{
  public UnequipAction(ItemLocation location, Item item) : base(location, item)
  {

  }

  public override ActionResult onPerform()
  {
    // Make a copy with the original count for the message.
    var copy = item.clone();
    removeItem();
    var result = hero.inventory.tryAdd(item, wasUnequipped: true);
    if (result.remaining == 0)
    {
      log("{1} unequip[s] {2}.", actor, copy);
    }
    else
    {
      // No room in inventory, so drop it.
      game.stage.addItem(item, actor!.pos);
      log(
          "{1} [don't|doesn't] have room for {2} and {2 he} drops to the ground.",
          actor,
          item);
    }

    hero.refreshProperties();
    return ActionResult.success;
  }
}

/// [Action] for using an [Item].
class UseAction : ItemAction
{
  public UseAction(ItemLocation location, Item item) : base(location, item)
  {
  }

  public override ActionResult onPerform()
  {
    if (!item.canUse) return fail("{1} can't be used.", item);

    // TODO: Some items should not be usable when certain conditions are active.
    // For example, you cannot read scrolls when dazzled or blinded.

    var useAction = item.use();

    if (item.count == 0)
    {
      // The stack is used up, delete it.
      removeItem();
    }
    else
    {
      countChanged();
    }

    // Using it from the ground counts as picking it up.
    if (location == ItemLocation.onGround) hero.pickUp(item);

    hero.lore.useItem(item);
    // TODO: If using an item can change hero properties, refresh them.

    return alternate(useAction);
  }
}

public class ActionMixin
{
  public Action action;

  public ActionMixin(Action action)
  {
    this.action = action;
  }

  public void log(string message, Noun noun1 = null, Noun noun2 = null, Noun noun3 = null)
  {
    action.log(message, noun1, noun2, noun3);
  }

  public Game game => action.game;
  public Actor actor => action.actor;
  public Hero hero => action.hero;
  public void addAction(Action action, Actor actor = null) => action.addAction(action, actor);

  public void addEvent(EventType type,
      Actor actor = null,
      Element element = null,
      object other = null,
      Vec pos = default(Vec),
      Direction dir = default(Direction)) => action.addEvent(type, actor, element, other, pos, dir);

}

/// Mixin for actions that permanently destroy items.
public class DestroyActionMixin : ActionMixin
{
  // TODO: Take damage into account when choosing the odds?

  public DestroyActionMixin(Action action) : base(action)
  {
  }

  /// Tries to destroy [items] with [element].
  ///
  /// Handles splitting stacks and logging errors. Returns the total fuel
  /// produced by all destroyed items.
  int _destroy(Element element, IEnumerable<Item> items, bool isHeld,
      System.Action<Item> removeItem)
  {
    var fuel = 0;

    // Copy items to avoid concurrent modification.
    foreach (var item in items)
    {
      // TODO: Having to handle missing keys here is lame.
      var chance = item.type.destroyChance.ContainsKey(element) ? item.type.destroyChance[element] : 0;

      // Holding an item makes it less likely to be destroyed.
      if (isHeld) chance = Mathf.Min(30, chance / 2);

      if (chance == 0) continue;

      // See how much of the stack is destroyed.
      var destroyedCount = 0;
      for (var i = 0; i < item.count; i++)
      {
        if (Rng.rng.percent(chance)) destroyedCount++;
      }

      if (destroyedCount == item.count)
      {
        // TODO: Effect.
        log($"{1} {element.destroyMessage}!", item);
        removeItem(item);
      }
      else if (destroyedCount > 0)
      {
        var destroyedPart = item.splitStack(destroyedCount);
        // TODO: Effect.
        log($"{1} {element.destroyMessage}!", destroyedPart);
      }

      fuel += item.type.fuel * destroyedCount;
    }

    return fuel;
  }

  /// Attempts to destroy items on the floor that can be destroyed by [element].
  public int destroyFloorItems(Vec pos, Element element)
  {
    var fuel = _destroy(element, game.stage.itemsAt(pos), false, (item) =>
    {
      game.stage.removeItem(item, pos);
      // TODO: If the item takes effect when destroyed, do that here.
    });

    return fuel;
  }

  /// Attempts to destroy items the actor is holding that can be destroyed by
  /// [element].
  public int destroyHeldItems(Element element)
  {
    // TODO: If monsters have inventories, need to handle that here.
    if ((actor is Hero) == false) return 0;

    // Any resistance prevents destruction.
    if (actor!.resistance(element) > 0) return 0;

    var fuel = _destroy(element, hero.inventory, true, (item) =>
    {
      hero.inventory.remove(item);
      // TODO: If the item takes effect when destroyed, do that here.
    });

    var anyEquipmentDestroyed = false;
    fuel += _destroy(element, hero.equipment, true, (item) =>
    {
      hero.equipment.remove(item);
      // TODO: If the item takes effect when destroyed, do that here.
      anyEquipmentDestroyed = true;
    });

    if (anyEquipmentDestroyed) hero.refreshProperties();
    return fuel;
  }
}
