using System.Collections;
using System.Collections.Generic;

using System.Linq;

/// An [Item] in the game can be either on the ground in the level, or held by
/// the [Hero] in their [Inventory] or [Equipment]. This enum describes which
/// of those is the case.
public class ItemLocation
{
  public static ItemLocation onGround =
      new ItemLocation("On Ground", "There is nothing on the ground.");
  public static ItemLocation inventory =
      new ItemLocation("Inventory", "Your backpack is empty.");
  public static ItemLocation equipment = new ItemLocation("Equipment", "<not used>");
  public static ItemLocation home = new ItemLocation("Home", "There is nothing in your home.");
  public static ItemLocation crucible =
      new ItemLocation("Crucible", "The crucible is waiting.");

  public string name;
  public string emptyDescription;

  ItemLocation(string name, string emptyDescription)
  {
    this.name = name;
    this.emptyDescription = emptyDescription;
  }

  public static ItemLocation shop(string name) => new ItemLocation(name, "All sold out!");

  public static bool operator ==(ItemLocation a, ItemLocation b)
  {
    return a.name.Equals(b.name);
  }

  public static bool operator !=(ItemLocation a, ItemLocation b)
  {
    return a.name.Equals(b.name) == false;
  }

  public override bool Equals(object obj)
  {
    if (obj is ItemLocation)
    {
      var k = obj as ItemLocation;
      return this == k;
    }
    return false;
  }
  public override int GetHashCode()
  {
    return name.GetHashCode() ^ emptyDescription.GetHashCode();
  }
}

// TODO: Move tryAdd() out of ItemCollection and Equipment? I think it's only
// needed for the home and crucible?
public abstract class ItemCollection : IEnumerable<Item>
{
  IEnumerator IEnumerable.GetEnumerator()
  {
    return (IEnumerator)GetEnumerator();
  }
  public virtual IEnumerator<Item> GetEnumerator()
  {
    return null;
  }

  public virtual ItemLocation location { get; }

  public string name => location.name;

  public virtual int length { get; }

  // Item operator [](int index);

  /// If the item collection has named slots, returns their names.
  ///
  /// Otherwise returns `null`. It's only valid to access this if [slots]
  /// returns `null` for some index.
  public virtual List<string> slotTypes => new List<string>();

  /// If the item collection may have empty slots in it (equipment) this returns
  /// the sequence of items and slots.
  public virtual List<Item> slots => this.ToList();

  public abstract void remove(Item item);

  public abstract Item removeAt(int index);

  /// Returns `true` if the entire stack of [item] will fit in this collection.
  public abstract bool canAdd(Item item);

  public abstract AddItemResult tryAdd(Item item);

  /// Called when the count of an item in the collection has changed.
  public abstract void countChanged();
}

/// The collection of [Item]s held by an [Actor].
public class Inventory : ItemCollection
{
  public override IEnumerator<Item> GetEnumerator()
  {
    return iterator;
  }

  ItemLocation _location = null;
  public override ItemLocation location => _location;

  public List<Item> _items = new List<Item>();
  public int _capacity;

  public bool isEmpty => _items == null || _items.Count == 0;

  /// If the [Hero] had to unequip an item in order to equip another one, this
  /// will refer to the item that was unequipped.
  ///
  /// If the hero isn't holding an unequipped item, returns `null`.
  public Item lastUnequipped => _lastUnequipped;
  Item _lastUnequipped;

  public override int length => _items.Count;

  public Item this[int index] => _items[index];

  public Inventory(ItemLocation location, int _capacity = 0, List<Item> items = null)
  {
    this._location = location;
    this._capacity = _capacity;
    if (items != null)
      this._items.AddRange(items);
  }

  /// Creates a new copy of this Inventory. This is done when the [Hero] enters
  /// a stage so that any inventory changes that happen in the stage are
  /// discarded if the hero dies.
  public Inventory clone()
  {
    var items = new List<Item>();
    foreach (var k in _items)
      items.Add(k.clone());
    return new Inventory(location, _capacity, items);
  }

  /// Removes all items from the inventory.
  void clear()
  {
    _items.Clear();
    _lastUnequipped = null;
  }

  public override void remove(Item item)
  {
    _items.Remove(item);
  }

  public override Item removeAt(int index)
  {
    var item = _items[index];
    _items.RemoveAt(index);
    if (_lastUnequipped == item) _lastUnequipped = null;
    return item;
  }

  public override bool canAdd(Item item)
  {
    // If there's an empty slot, can always add it.
    if (_capacity > 0 || _items.Count < _capacity!) return true;

    // See if we can merge it with other stacks.
    var remaining = item.count;
    foreach (var existing in _items)
    {
      if (existing.canStack(item))
      {
        remaining -= existing.type.maxStack - existing.count;
        if (remaining <= 0) return true;
      }
    }

    // If we get here, there are no open slots and not enough stacks that can
    // take the item.
    return false;
  }

  public void _tryAdd(Item item)
  {
    tryAdd(item, false);
  }

  public override AddItemResult tryAdd(Item item)
  {
    return tryAdd(item, false);
  }

  public AddItemResult tryAdd(Item item, bool wasUnequipped = false)
  {
    var adding = item.count;

    // Try to add it to existing stacks.
    foreach (var existing in _items)
    {
      existing.stack(item);

      // If we completely stacked it, we're done.
      if (item.count == 0)
      {
        return new AddItemResult(adding, 0);
      }
    }

    // See if there is room to start a new stack with the rest.
    if (_capacity > 0 && _items.Count >= _capacity!)
    {
      // There isn't room to pick up everything.
      return new AddItemResult(adding - item.count, item.count);
    }

    // Add a new stack.
    _items.Add(item);
    _items.Sort();

    if (wasUnequipped) _lastUnequipped = item;
    return new AddItemResult(adding, 0);
  }

  /// Re-sorts multiple stacks of the same item to pack them into the minimum
  /// number of stacks.
  ///
  /// This should be called any time the count of an item stack in the hero's
  /// inventory is changed.
  public override void countChanged()
  {
    // Hacky. Just re-add everything from scratch.
    var items = _items.ToList();
    _items.Clear();

    foreach (var item in items)
    {
      var result = tryAdd(item);
      Debugger.assert(result.remaining == 0);
    }
  }

  IEnumerator<Item> iterator => _items.GetEnumerator();

  public bool isNotEmpty
  {
    get
    {
      return _items != null && _items.Count > 0;
    }
  }
}

/// Describes the result of attempting to add an item stack to the inventory.
public class AddItemResult
{
  /// The count of items in the stack that were successfully added.
  public int added;

  /// The count of items that could not be fit into the inventory.
  public int remaining;

  public AddItemResult(int added, int remaining)
  {
    this.added = added;
    this.remaining = remaining;
  }
}
