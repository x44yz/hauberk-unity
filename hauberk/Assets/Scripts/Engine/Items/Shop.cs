using System.Collections;
using System.Collections.Generic;


public class Shop
{
  /// The maximum number of items a shop can contain.
  public static int capacity = 26;

  public Drop _drop;

  public string name;

  public Shop(string name, Drop _drop)
  {
    this.name = name;
    this._drop = _drop;
  }

  public Inventory create()
  {
    var inventory = new Inventory(ItemLocation.shop(name), capacity);
    update(inventory);
    return inventory;
  }

  public Inventory load(List<Item> items)
  {
    return new Inventory(ItemLocation.shop(name), capacity, items);
  }

  public void update(Inventory inventory)
  {
    // Remove some.
    var remainCount = (int)Rng.rng.rfloat(capacity * 0.2, capacity * 0.4);

    while (inventory.length > remainCount)
    {
      inventory.removeAt(Rng.rng.range(inventory.length));
    }

    // Add some.
    var tries = 0;
    var count = (int)Rng.rng.rfloat(capacity * 0.3, capacity * 0.7);

    while (inventory.length < count && tries++ < 100)
    {
      // Try to add an item.
      _drop.dropItem(1, inventory._tryAdd);

      // Remove duplicates.
      for (var i = 1; i < inventory.length; i++)
      {
        var previous = inventory[i - 1];
        var item = inventory[i];

        if (previous.type == item.type &&
            previous.prefix == item.prefix &&
            previous.suffix == item.suffix)
        {
          inventory.removeAt(i);
          i--;
        }
      }
    }
  }
}
