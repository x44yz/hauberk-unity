using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;

/// A thing that can be picked up.
public class Item : Noun, System.IComparable<Item>
{
  public ItemType type;

  public Affix prefix;
  public Affix suffix;

  public Item(ItemType type, int _count, Affix prefix = null, Affix suffix = null)
      : base("")
  {
    this.type = type;
    this._count = _count;
    this.prefix = prefix;
    this.suffix = suffix;
  }

  public object appearance => type.appearance;

  public bool canEquip => equipSlot != null;
  public string equipSlot => type.equipSlot;

  /// Whether the item can be used or not.
  public bool canUse => type.use != null;

  /// Create the action to perform when this item is used, and reduce its count.
  public Action use()
  {
    // TODO: Some kinds of usable items shouldn't be consumed, like rods in
    // Angband.
    _count--;
    return type.use?.createAction();
  }

  /// Whether the item can be thrown or not.
  public bool canToss => type.toss != null;

  /// The base attack for the item, ignoring its own affixes.
  public Attack attack => type.attack;

  public Toss toss => type.toss;

  public Element element
  {
    get
    {
      var result = Element.none;
      if (attack != null) result = attack!.element;
      if (prefix != null && prefix!.brand != Element.none) result = prefix!.brand;
      if (suffix != null && suffix!.brand != Element.none) result = suffix!.brand;
      return result;
    }
  }

  public int strikeBonus
  {
    get
    {
      var result = 0;
      _applyAffixes((affix) => result += affix.strikeBonus);
      return result;
    }
  }

  public double damageScale
  {
    get
    {
      var result = 1.0;
      _applyAffixes((affix) => result *= affix.damageScale);
      return result;
    }
  }

  public int damageBonus
  {
    get
    {
      var result = 0;
      _applyAffixes((affix) => result += affix.damageBonus);
      return result;
    }
  }

  // TODO: Affix defenses?
  public Defense defense => type.defense;

  /// The amount of protection provided by the item when equipped.
  public int armor => baseArmor + armorModifier;

  /// The base amount of protection provided by the item when equipped,
  /// ignoring any affix modifiers.
  public int baseArmor => type.armor;

  /// The amount of protection added by the affixes.
  public int armorModifier
  {
    get
    {
      var result = 0;
      _applyAffixes((affix) => result += affix.armor);
      return result;
    }
  }

  public override string nounText
  {
    get
    {
      var name = type.quantifiableName;

      if (prefix != null) name = $"{prefix!.displayName} {name}";
      if (suffix != null) name = $"{name} {suffix!.displayName}";

      return Log.quantify(name, count);
    }
  }

  public override Pronoun pronoun => Pronoun.it;

  /// How much the one unit of the item can be bought and sold for.
  public int price
  {
    get
    {
      var price = (float)type.price;

      // If an item has both a prefix and a suffix, it's even more expensive
      // since that makes the item more powerful.
      var affixScale = 1.0f;
      if (prefix != null && suffix != null) affixScale = 1.5f;

      _applyAffixes((affix) => price *= (float)affix.priceScale * affixScale);
      _applyAffixes((affix) => price += affix.priceBonus * affixScale);

      return Mathf.CeilToInt(price);
    }
  }

  public bool isTreasure => type.isTreasure;

  /// The penalty to the hero's strength when wearing this.
  public int weight
  {
    get
    {
      var result = type.weight;
      _applyAffixes((affix) => result += affix.weightBonus);
      return Mathf.Max(0, result);
    }
  }

  /// The amount of strength required to wield the item effectively.
  public int heft
  {
    get
    {
      var result = (double)type.heft;
      _applyAffixes((affix) => result *= affix.heftScale);
      return Mathf.RoundToInt((float)result);
    }
  }

  // TODO: Affixes that modify.
  public int emanationLevel => type.emanationLevel;

  /// The number of items in this stack.
  public int count => _count;
  int _count = 1;

  /// Apply any affix modifications to hit.
  public void modifyHit(Hit hit)
  {
    hit.addStrike(strikeBonus);
    hit.scaleDamage(damageScale);
    hit.addDamage(damageBonus);
    hit.brand(element);
    // TODO: Range modifier.
  }

  /// Gets the resistance this item confers to [element] when equipped.
  public int resistance(Element element)
  {
    var resistance = 0;
    _applyAffixes((affix) => resistance += affix.resistance(element));
    return resistance;
  }

  // IComparable
  public int CompareTo(Item other) => compareTo(other);

  public int compareTo(Item other)
  {
    if (type.sortIndex != other.type.sortIndex)
    {
      return type.sortIndex.CompareTo(other.type.sortIndex);
    }

    // TODO: Take into account affixes.

    // Order by descending count.
    if (count != other.count) return other.count.CompareTo(count);

    return 0;
  }

  /// Creates a new [Item] with the same type and affixes as this one.
  ///
  /// If [count] is given, the clone has that count. Otherwise, it has the
  /// same count as this item.
  public Item clone(int? count = null) => new Item(type, count ?? _count, prefix, suffix);

  public bool canStack(Item item)
  {
    if (type != item.type) return false;

    // Items with affixes don't stack.
    // TODO: Should they?
    if (prefix != null || item.prefix != null) return false;
    if (suffix != null || item.suffix != null) return false;

    return true;
  }

  /// Try to combine [item] with this item into a single stack.
  ///
  /// Updates the counts of the two items. If completely successful, [item]
  /// will end up with a count of zero. If the items cannot be stacked, [item]'s
  /// count is unchanged.
  public void stack(Item item)
  {
    if (!canStack(item)) return;

    // If we here, we are trying to stack. We don't support stacking
    // items with affixes, and we should avoid that by not having any affixes
    // defined for stackable items. Validate that invariant here.
    Debugger.assert(prefix == null &&
        suffix == null &&
        item.prefix == null &&
        item.suffix == null);

    var total = count + item.count;
    if (total <= type.maxStack)
    {
      // Completely merge the stack.
      _count = total;
      item._count = 0;
    }
    else
    {
      // There is some left.
      _count = type.maxStack;
      item._count = total - type.maxStack;
    }
  }

  /// Splits this item into two stacks. Returns a new item with [count], and
  /// reduces this stack by that amount.
  public Item splitStack(int count)
  {
    Debugger.assert(count < _count);

    _count -= count;
    return clone(count);
  }

  public override string ToString() => nounText;

  void _applyAffixes(System.Action<Affix> callback)
  {
    if (prefix != null) callback(prefix!);
    if (suffix != null) callback(suffix!);
  }
}