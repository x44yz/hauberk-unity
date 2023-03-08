using System.Collections;
using System.Collections.Generic;
using UnityEngine;

Drop parseDrop(String name, {int? depth, int? affixChance}) {
  // See if we're parsing a drop for a single item type.
  var itemType = Items.types.tryFind(name);
  if (itemType != null) return _ItemDrop(itemType, depth, affixChance);

  // Otherwise, it's a tag name.
  return _TagDrop(name, depth, affixChance);
}

/// Creates a [Drop] that has a [chance]% chance of dropping [drop].
Drop percentDrop(int chance, String drop, [int? depth, int? affixChance]) {
  return _PercentDrop(
      chance, parseDrop(drop, depth: depth, affixChance: affixChance));
}

/// Creates a [Drop] that drops all of [drops].
Drop dropAllOf(List<Drop> drops) => _AllOfDrop(drops);

/// Creates a [Drop] that drops one of [drops] based on their frequency.
Drop dropOneOf(Map<Drop, double> drops) => _OneOfDrop(drops);

Drop repeatDrop(int count, Object drop, [int? depth]) {
  if (drop is String) drop = parseDrop(drop, depth: depth);
  return _RepeatDrop(count, drop as Drop);
}

/// Drops an item of a given type.
class _ItemDrop : Drop {
  public ItemType _type;

  /// The depth to use for selecting affixes.
  ///
  /// If `null`, uses the current depth when the drop is generated.
  public int? _depth;

  /// Modifier to the apply to the percent chance of adding an affix.
  public int? _affixChance;

  _ItemDrop(ItemType _type, int? _depth, int? _affixChance)
  {
    this._type = _type;
    this._depth = _depth;
    this._affixChance = _affixChance;
  }

  void dropItem(int depth, AddItem addItem) {
    addItem(Affixes.createItem(_type, _depth ?? depth, _affixChance));
  }
}

/// Drops a randomly selected item near a level with a given tag.
class _TagDrop : Drop {
  /// The tag to choose from.
  public string _tag;

  /// The average depth of the drop.
  ///
  /// If `null`, uses the current depth when the drop is generated.
  public int? _depth;

  /// Modifier to the apply to the percent chance of adding an affix.
  public int? _affixChance;

  _TagDrop(string _tag, int? _depth, int? _affixChance)
  {
    this._tag = _tag;
    this._depth = _depth;
    this._affixChance = _affixChance;
  }

  public override void dropItem(int depth, AddItem addItem) {
    var itemType = Items.types.tryChoose(_depth ?? depth, tag: _tag);
    if (itemType == null) return;

    addItem(Affixes.createItem(itemType, _depth ?? depth, _affixChance));
  }
}

/// A [Drop] that will create an inner drop some random percentage of the time.
class _PercentDrop : Drop {
  public int _chance;
  public Drop _drop;

  _PercentDrop(int _chance, Drop _drop)
  {
    this._chance = _chance; 
    this._drop = _drop;
  }

  void dropItem(int depth, AddItem addItem) {
    if (Rng.rng.range(100) >= _chance) return;
    _drop.dropItem(depth, addItem);
  }
}

/// A [Drop] that drops all of a list of child drops.
class _AllOfDrop : Drop {
  public List<Drop> _drops;

  _AllOfDrop(List<Drop> _drops)
  {
    this._drops = _drops;
  }

  void dropItem(int depth, AddItem addItem) {
    foreach (var drop in _drops) {
      drop.dropItem(depth, addItem);
    }
  }
}

/// A [Drop] that drops one of a set of child drops.
class _OneOfDrop : Drop {
  public ResourceSet<Drop> _drop = new ResourceSet<Drop>();

  _OneOfDrop(Dictionary<Drop, double> drops) {
    foreach (var kv in drops)
    {
        // TODO: Allow passing in depth?
        _drop.add(kv.Key, frequency: kv.Value);
    }
  }

  public override void dropItem(int depth, AddItem addItem) {
    // TODO: Allow passing in depth?
    var drop = _drop.tryChoose(1);
    if (drop == null) return;

    drop.dropItem(depth, addItem);
  }
}

/// A [Drop] that drops a child drop more than once.
class _RepeatDrop : Drop {
  public int _count;
  public Drop _drop;

  _RepeatDrop(int _count, Drop _drop)
  {
    this._count = _count;
    this._drop = _drop;
  }

  public override void dropItem(int depth, AddItem addItem) {
    var taper = 5;
    if (_count > 3) taper = 4;
    if (_count > 6) taper = 3;

    var count = Rng.rng.triangleInt(_count, _count / 2) + Rng.rng.taper(0, taper);
    for (var i = 0; i < count; i++) {
      _drop.dropItem(depth, addItem);
    }
  }
}
