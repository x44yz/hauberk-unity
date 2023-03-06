using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// The collection of wielded [Item]s held by the hero. Unlike [Inventory], the
/// [Equipment] holds each item in a categorized slot.
public class Equipment : IEnumerable<Item> {
  IEnumerator IEnumerable.GetEnumerator()
  {
    return iterator;
  }
  public IEnumerator<Item> GetEnumerator()
  {
    return iterator;
  }

  ItemLocation location => ItemLocation.equipment;

  public List<string> slotTypes;
  public List<Item> slots;

  Equipment()
  {
    slotTypes = new List<string>(){
      "hand",
      "hand",
      "ring",
      "necklace",
      "body",
      "cloak",
      "helm",
      "gloves",
      "boots"
    };
    slots = new List<Item>(9);
    for (int i = 0; i < slots.Capacity; ++i)
      slots.Add(null);
  }

  /// Gets the currently-equipped weapons, if any.
  IEnumerable<Item> weapons
  {
    get {
      return slots.Where<Item>((item) => item.type.weaponType != null);
    }
  }

  /// Gets the number of equipped items. Ignores empty slots.
  int length {
    get {
      int count = 0;
      slots.ForEach((item) => count += (item == null) ? 0 : 1);
      return count;
    }
  }

  /// Gets the equipped item at the given index. Ignores empty slots.
  Item this[int index] {
    get {
      // Find the slot, skipping over empty ones.
      for (var i = 0; i < slotTypes.Count; i++) {
        if (slots[i] != null) {
          if (index == 0) return slots[i]!;
          index--;
        }
      }

      throw new System.Exception("Unreachable.");
    }
  }

  /// Creates a new copy of this [Equipment]. This is done when the hero enters
  /// the dungeon so that any inventory changes that happen there are discarded
  /// if the hero dies.
  public Equipment clone() {
    var equipment = new Equipment();
    for (var i = 0; i < slotTypes.Count; i++) {
      if (slots[i] != null) {
        equipment.slots[i] = slots[i]!.clone();
      }
    }

    return equipment;
  }

  /// Gets whether or not there is a slot to equip [item].
  bool canEquip(Item item) {
    return slotTypes.Any((slot) => item.equipSlot == slot);
  }

  bool canAdd(Item item) {
    // Look for an empty slot of the right type.
    for (var i = 0; i < slots.Count; i++) {
      if (slotTypes[i] == item.equipSlot && slots[i] == null) return true;
    }

    return false;
  }

  /// Tries to add the item. This will only succeed if there is an empty slot
  /// that allows the item. Unlike [equip], this will not swap items. It is
  /// used by the ItemScreen.
  AddItemResult tryAdd(Item item) {
    // Should not be able to equip stackable items. If we want to make, say,
    // knives stackable, we'll have to add support for splitting stacks here.
    DartUtils.assert(item.count == 1);

    for (var i = 0; i < slotTypes.Count; i++) 
    {
      if (slotTypes[i] == item.equipSlot && slots[i] == null) {
        slots[i] = item;
        return new AddItemResult(item.count, 0);
      }
    }

    return new AddItemResult(0, item.count);
  }

  void countChanged() {
    // Do nothing. Equipment doesn't stack.
  }

  /// Equips [item].
  ///
  /// Returns any items that had to be unequipped to make room for it. This is
  /// usually nothing or a single item, but can be two held items if equipping
  /// a two-handed item.
  List<Item> equip(Item item) {
    DartUtils.assert(item.count == 1, "Must split the stack before equipping.");

    // Handle hands and two-handed items specially. We need to preserve the
    // invariant that you can never get into a state where you are holding a
    // two-handed item and something else.
    if (item.equipSlot == "hand") {
      var handSlots = new List<int>();
      var heldSlots = new List<int>();
      for (var i = 0; i < slotTypes.Count; i++) {
        if (slotTypes[i] == "hand") {
          handSlots.Add(i);
          if (slots[i] != null) heldSlots.Add(i);
        }
      }

      // Nothing held, so hold it.
      if (heldSlots.Count == 0) {
        slots[handSlots[0]] = item;
        return new List<Item>();
      }

      // Holding a two-handed item, so unequip it.
      if (heldSlots.Count == 1 && slots[heldSlots[0]]!.type.isTwoHanded) {
        var unequipped0 = slots[heldSlots[0]]!;
        slots[handSlots[0]] = item;
        return new List<Item>(){unequipped0};
      }

      // Equipping a two-handed, so unequip anything held.
      if (item.type.isTwoHanded) {
        var unequipped1 = new List<Item>();
        foreach (var slot in heldSlots) {
          unequipped1.Add(slots[slot]!);
          slots[slot] = null;
        }

        slots[handSlots[0]] = item;
        return unequipped1;
      }

      // Both hands full, so empty one.
      if (heldSlots.Count == 2) {
        var unequipped2 = slots[heldSlots[0]]!;
        slots[heldSlots[0]] = item;
        return new List<Item>(){unequipped2};
      }

      // One empty hand, so use it.
      if (heldSlots[0] == handSlots[0]) {
        slots[handSlots[1]] = item;
      } else {
        slots[handSlots[0]] = item;
      }
      return new List<Item>();
    }

    var usedSlot = -1;
    for (var i = 0; i < slotTypes.Count; i++) {
      if (slotTypes[i] == item.equipSlot) {
        if (slots[i] == null) {
          // Found an empty slot, so put it there.
          slots[i] = item;
          return new List<Item>();
        } else {
          // Found the slot, but it's occupied.
          usedSlot = i;
        }
      }
    }

    // If we get here, all matching slots were already full. Swap out an item.
    DartUtils.assert(usedSlot != -1, "Should have at least one of every slot.");
    var unequipped = new List<Item>(){slots[usedSlot]!};
    slots[usedSlot] = item;
    return unequipped;
  }

  void remove(Item item) {
    for (var i = 0; i < slots.Count; i++) {
      if (slots[i] == item) {
        slots[i] = null;
        break;
      }
    }
  }

  /// Unequips and returns the [Item] at [index].
  Item removeAt(int index) {
    // Find the slot, skipping over empty ones.
    for (var i = 0; i < slotTypes.Count; i++) {
      if (slots[i] == null) continue;
      if (index == 0) {
        var item = slots[i];
        slots[i] = null;
        return item!;
      }

      index--;
    }

    throw new System.Exception("Unreachable.");
  }

  /// Gets the non-empty item slots.
  IEnumerator<Item> iterator => slots.Where(x => x != null).GetEnumerator();
}
