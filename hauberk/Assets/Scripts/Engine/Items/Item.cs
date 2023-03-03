using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// A thing that can be picked up.
class Item implements Comparable<Item>, Noun 
{
    public ItemType type;

    public Affix? prefix;
    public Affix? suffix;

    Item(this.type, this._count, [this.prefix, this.suffix]);

    Object appearance => type.appearance;

    bool canEquip => equipSlot != null;
    string? equipSlot => type.equipSlot;

    /// Whether the item can be used or not.
    bool canUse => type.use != null;

    /// Create the action to perform when this item is used, and reduce its count.
    Action use() {
    // TODO: Some kinds of usable items shouldn't be consumed, like rods in
    // Angband.
    _count--;
    return type.use!.createAction();
    }

    /// Whether the item can be thrown or not.
    bool canToss => type.toss != null;

    /// The base attack for the item, ignoring its own affixes.
    Attack? attack => type.attack;

    Toss? toss => type.toss;

    Element element {
    var result = Element.none;
    if (attack != null) result = attack!.element;
    if (prefix != null && prefix!.brand != Element.none) result = prefix!.brand;
    if (suffix != null && suffix!.brand != Element.none) result = suffix!.brand;
    return result;
    }

    int strikeBonus {
    var result = 0;
    _applyAffixes((affix) => result += affix.strikeBonus);
    return result;
    }

    double damageScale {
    var result = 1.0;
    _applyAffixes((affix) => result *= affix.damageScale);
    return result;
    }

    int damageBonus {
    var result = 0;
    _applyAffixes((affix) => result += affix.damageBonus);
    return result;
    }

    // TODO: Affix defenses?
    Defense? defense => type.defense;

    /// The amount of protection provided by the item when equipped.
    int armor => baseArmor + armorModifier;

    /// The base amount of protection provided by the item when equipped,
    /// ignoring any affix modifiers.
    int baseArmor => type.armor;

    /// The amount of protection added by the affixes.
    int armorModifier {
    var result = 0;
    _applyAffixes((affix) => result += affix.armor);
    return result;
    }

    string nounText {
    var name = type.quantifiableName;

    if (prefix != null) name = "${prefix!.displayName} $name";
    if (suffix != null) name = "$name ${suffix!.displayName}";

    return Log.quantify(name, count);
    }

    Pronoun pronoun => Pronoun.it;

    /// How much the one unit of the item can be bought and sold for.
    int price {
    var price = type.price.toDouble();

    // If an item has both a prefix and a suffix, it's even more expensive
    // since that makes the item more powerful.
    var affixScale = 1.0;
    if (prefix != null && suffix != null) affixScale = 1.5;

    _applyAffixes((affix) => price *= affix.priceScale * affixScale);
    _applyAffixes((affix) => price += affix.priceBonus * affixScale);

    return price.ceil();
    }

    bool isTreasure => type.isTreasure;

    /// The penalty to the hero's strength when wearing this.
    int weight {
        get {
            var result = type.weight;
            _applyAffixes((affix) => result += affix.weightBonus);
            return math.max(0, result);
        }
    }

    /// The amount of strength required to wield the item effectively.
    int heft {
        get {
            var result = type.heft.toDouble();
            _applyAffixes((affix) => result *= affix.heftScale);
            return result.round();
        }
    }

    // TODO: Affixes that modify.
    int emanationLevel => type.emanationLevel;

    /// The number of items in this stack.
    int count => _count;
    int _count = 1;

    /// Apply any affix modifications to hit.
    void modifyHit(Hit hit) {
    hit.addStrike(strikeBonus);
    hit.scaleDamage(damageScale);
    hit.addDamage(damageBonus);
    hit.brand(element);
    // TODO: Range modifier.
    }

    /// Gets the resistance this item confers to [element] when equipped.
    int resistance(Element element) {
    var resistance = 0;
    _applyAffixes((affix) => resistance += affix.resistance(element));
    return resistance;
    }

    int compareTo(Item other) {
    if (type.sortIndex != other.type.sortIndex) {
        return type.sortIndex.compareTo(other.type.sortIndex);
    }

    // TODO: Take into account affixes.

    // Order by descending count.
    if (count != other.count) return other.count.compareTo(count);

    return 0;
    }

    /// Creates a new [Item] with the same type and affixes as this one.
    ///
    /// If [count] is given, the clone has that count. Otherwise, it has the
    /// same count as this item.
    Item clone([int? count]) => Item(type, count ?? _count, prefix, suffix);

    bool canStack(Item item) {
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
    void stack(Item item) {
    if (!canStack(item)) return;

    // If we here, we are trying to stack. We don't support stacking
    // items with affixes, and we should avoid that by not having any affixes
    // defined for stackable items. Validate that invariant here.
    assert(prefix == null &&
        suffix == null &&
        item.prefix == null &&
        item.suffix == null);

    var total = count + item.count;
    if (total <= type.maxStack) {
        // Completely merge the stack.
        _count = total;
        item._count = 0;
    } else {
        // There is some left.
        _count = type.maxStack;
        item._count = total - type.maxStack;
    }
    }

    /// Splits this item into two stacks. Returns a new item with [count], and
    /// reduces this stack by that amount.
    Item splitStack(int count) {
    assert(count < _count);

    _count -= count;
    return clone(count);
    }

    string toString() => nounText;

    void _applyAffixes(Function(Affix) callback) {
    if (prefix != null) callback(prefix!);
    if (suffix != null) callback(suffix!);
    }
}