using System.Collections;
using System.Collections.Generic;

using System.Linq;

/// The history of interesting events the hero has experienced.
/// The number of monsters of each breed the hero has detected.
public class Lore
{
  ///
  /// (Or, more specifically, that have died.)
  public Dictionary<Breed, int> _seenBreeds;

  /// The number of monsters of each breed the hero has killed.
  ///
  /// (Or, more specifically, that have died.)
  public Dictionary<Breed, int> _slainBreeds;

  /// The number of items of each type that the hero has picked up.
  public Dictionary<ItemType, int> _foundItems;

  /// The number of items with each affix that the hero has picked up or used.
  public Dictionary<Affix, int> _foundAffixes;

  /// The number of consumable items of each type that the hero has used.
  public Dictionary<ItemType, int> _usedItems;

  /// The breeds the hero has killed at least one of.
  List<Breed> slainBreeds => _slainBreeds.Keys.ToList();

  /// The total number of monsters slain.
  public int allSlain
  {
    get
    {
      int rt = 0;
      foreach (var kv in _slainBreeds)
        rt += kv.Value;
      return rt;
    }
  }

  public Lore()
  {
    this._seenBreeds = new Dictionary<Breed, int>();
    this._slainBreeds = new Dictionary<Breed, int>();
    this._foundItems = new Dictionary<ItemType, int>();
    this._foundAffixes = new Dictionary<Affix, int>();
    this._usedItems = new Dictionary<ItemType, int>();
  }

  public Lore(Dictionary<Breed, int> _seenBreeds,
      Dictionary<Breed, int> _slainBreeds,
      Dictionary<ItemType, int> _foundItems,
      Dictionary<Affix, int> _foundAffixes,
      Dictionary<ItemType, int> _usedItems)
  {
    this._seenBreeds = _seenBreeds;
    this._slainBreeds = _slainBreeds;
    this._foundItems = _foundItems;
    this._foundAffixes = _foundAffixes;
    this._usedItems = _usedItems;
  }

  public void seeBreed(Breed breed)
  {
    if (!_seenBreeds.ContainsKey(breed)) _seenBreeds.Add(breed, 0);
    _seenBreeds[breed] = _seenBreeds[breed] + 1;
  }

  public void slay(Breed breed)
  {
    if (!_slainBreeds.ContainsKey(breed)) _slainBreeds.Add(breed, 0);
    _slainBreeds[breed] = _slainBreeds[breed] + 1;
  }

  public void findItem(Item item)
  {
    if (!_foundItems.ContainsKey(item.type)) _foundItems.Add(item.type, 0);
    _foundItems[item.type] = _foundItems[item.type]! + 1;

    void findAffix(Affix affix)
    {
      if (affix == null) return;

      if (!_foundAffixes.ContainsKey(affix)) _foundAffixes.Add(affix, 0);
      _foundAffixes[affix] = _foundAffixes[affix]! + 1;
    }

    findAffix(item.prefix);
    findAffix(item.suffix);
  }

  public void useItem(Item item)
  {
    if (!_usedItems.ContainsKey(item.type)) _usedItems.Add(item.type, 0);
    _usedItems[item.type] = _usedItems[item.type]! + 1;
  }

  /// The number of monsters of [breed] that the hero has detected.
  public int seenBreed(Breed breed)
  {
    int val = 0;
    _seenBreeds.TryGetValue(breed, out val);
    return val;
  }

  /// The number of monsters of [breed] that the hero has killed.
  public int slain(Breed breed)
  {
    int val = 0;
    _slainBreeds.TryGetValue(breed, out val);
    return val;
  }

  /// The number of items of [type] the hero has picked up.
  public int foundItems(ItemType type)
  {
    int val = 1;
    _foundItems.TryGetValue(type, out val);
    return val;
  }

  /// The number of items with [affix] the hero has picked up.
  int foundAffixes(Affix affix)
  {
    int val = 0;
    _foundAffixes.TryGetValue(affix, out val);
    return val;
  }

  /// The number of items of [type] the hero has used.
  public int usedItems(ItemType type)
  {
    int val = 0;
    _usedItems.TryGetValue(type, out val);
    return val;
  }

  public Lore clone()
  {
    return new Lore(new Dictionary<Breed, int>(_seenBreeds),
      new Dictionary<Breed, int>(_slainBreeds),
      new Dictionary<ItemType, int>(_foundItems),
      new Dictionary<Affix, int>(_foundAffixes),
      new Dictionary<ItemType, int>(_usedItems));
  }
}