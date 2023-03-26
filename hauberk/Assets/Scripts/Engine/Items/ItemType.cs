using System.Collections;
using System.Collections.Generic;

using TossItemUse = System.Func<Vec, Action>;
using AddItem = System.Action<Item>;

public abstract class Drop
{
  public Drop()
  {
  }

  public abstract void dropItem(int depth, AddItem addItem);
}

public class ItemUse
{
  public string description;
  public System.Func<Action> createAction;

  public ItemUse(string description, System.Func<Action> createAction)
  {
    this.description = description;
    this.createAction = createAction;
  }
}

/// Tracks information about a tossable [ItemType].
public class Toss
{
  /// The percent chance of the item breaking when thrown. `null` if the item
  /// can't be thrown.
  public int breakage;

  /// The item's attack when thrown or `null` if the item can't be thrown.
  public Attack attack;

  /// The action created when the item is tossed and hits something, or `null`
  /// if it just falls to the ground.
  public TossItemUse use;

  public Toss(int breakage, Attack attack, TossItemUse use)
  {
    this.breakage = breakage;
    this.attack = attack;
    this.use = use;
  }
}

/// A kind of [Item]. Each item will have a type that describes the item.
public class ItemType
{
  /// The pattern string used to generate quantified names for items of this
  /// type: `Scroll[s] of Disappearing`, etc.
  public string quantifiableName;

  /// The singular name of the item type: "Sword", "Scroll of Disappearing".
  ///
  /// This is used to identify the item type in drops, affixes, etc.
  public string name => Log.singular(quantifiableName);

  public object appearance;

  /// The item types's depth.
  ///
  /// Higher depth objects are found later in the game.
  public int depth;

  public int sortIndex;

  // TODO: These two fields are sort of redundant with tags, but ItemTypes
  // don't own their tags. Should they?

  /// The name of the equipment slot that [Item]s can be placed in. If `null`
  /// then this Item cannot be equipped.
  public string equipSlot;

  /// If true, the item takes up both hand slots.
  public bool isTwoHanded;

  /// If this item is a weapon, returns which kind of weapon it is -- "spear",
  /// "sword", etc. Otherwise returns `null`.
  public string weaponType;

  public ItemUse use;

  /// The item's [Attack] or `null` if the item is not an equippable weapon.
  public Attack attack;

  /// The items toss information, or `null` if it can't be tossed.
  public Toss toss;

  public Defense defense;

  public int armor;

  /// How much gold this item is worth.
  public int price;

  /// The penalty to the hero's strength when wearing this.
  public int weight;

  /// The amount of strength required to wield the item effectively.
  public int heft;

  /// The amount of light this items gives off when equipped.
  ///
  /// This isn't a raw emanation value, but a level to be passed to
  /// [Lighting.emanationFromLevel()].
  public int emanationLevel;

  /// True if this item is "treasure".
  ///
  /// That means it just has a gold value. As soon as the hero steps on it, it
  /// increases the hero's gold and disappears.
  public bool isTreasure;

  /// The maximum number of items of this type that a single stack may contain.
  public int maxStack;

  /// Percent chance of this item being destroyed when hit with a given element.
  public Dictionary<Element, int> destroyChance = new Dictionary<Element, int>();

  /// If the item burns when on the ground, how much fuel it adds to the
  /// burning tile.
  public int fuel;

  /// The [Skill]s discovered when picking up an item of this type.
  public List<Skill> skills = new List<Skill>();

  public ItemType(
      string quantifiableName,
      object appearance,
      int depth,
      int sortIndex,
      string equipSlot,
      string weaponType,
      ItemUse use,
      Attack attack,
      Toss toss,
      Defense defense,
      int armor,
      int price,
      int maxStack,
      int weight = 0,
      int heft = 1,
      int? emanation = null,
      int? fuel = null,
      bool? treasure = null,
      bool? twoHanded = null)
  {
    this.quantifiableName = quantifiableName;
    this.appearance = appearance;
    this.depth = depth;
    this.sortIndex = sortIndex;
    this.equipSlot = equipSlot;
    this.weaponType = weaponType;
    this.use = use;
    this.attack = attack;
    this.toss = toss;
    this.defense = defense;
    this.armor = armor;
    this.price = price;
    this.maxStack = maxStack;

    this.emanationLevel = emanation ?? 0;
    this.fuel = fuel ?? 0;
    this.isTreasure = treasure ?? false;
    this.isTwoHanded = twoHanded ?? false;
  }

  public override string ToString() => name;
}
