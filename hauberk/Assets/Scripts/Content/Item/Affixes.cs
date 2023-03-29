using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;

class Affixes
{
  public static ResourceSet<Affix> prefixes = new ResourceSet<Affix>();
  public static ResourceSet<Affix> suffixes = new ResourceSet<Affix>();

  /// Creates a new [Item] of [itemType] and chooses affixes for it.
  public static Item createItem(ItemType itemType, int droppedDepth,
      int? affixChance)
  {
    affixChance ??= 0;

    // Only equipped items have affixes.
    if (itemType.equipSlot == null)
      return new Item(itemType, 1);

    // Calculate the effective depth of the item for generating affixes. This
    // affects both the chances of having an affix at all, and which affixes it
    // gets.
    //
    // The basic idea is that an item's overall value should reflect the depth
    // where it's generated. So if an item for a shallower depth appears deeper
    // in the dungeon it is more likely to have an affix to compensate.
    // Likewise, finding a depth 20 item at depth 10 is already a good find, so
    // it's less likely to also have an affix on it.
    var affixDepth = droppedDepth;
    var outOfDepth = itemType.depth - droppedDepth;

    if (outOfDepth > 0)
    {
      // Generating a stronger item than expected, so it will have weaker
      // affixes.
      affixDepth -= outOfDepth;
    }
    else
    {
      // Generating a weaker item than expected, so boost its affix. Reduce the
      // boost as the hero gets deeper in the dungeon. Otherwise, near 100, the
      // boost ends up pushing almost everything past 100 since most equipment
      // has a lower starting depth.
      var weight = MathUtils.lerpDouble(droppedDepth, 1, 100, 0.5, 0.0);
      affixDepth -= Rng.rng.round(outOfDepth * weight);
    }

    affixDepth = Mathf.Clamp(affixDepth, 1, 100);

    // This generates a curve that starts around 1% and slowly ramps upwards.
    var chance = 0.008 * affixDepth * affixDepth + 0.05 * affixDepth + 0.1;

    // See how many affixes the item has. The affixChance only boosts one roll
    // because it increases the odds of *an* affix, but not the odds of
    // multiple.
    var affixes = 0;
    if (Rng.rng.rfloat(100.0) < chance + affixChance) affixes++;

    // Make dual-affix items rarer since they are more powerful (they only take
    // a single equipment slot) and also look kind of funny.
    if (Rng.rng.rfloat(100.0) < chance && Rng.rng.oneIn(5)) affixes++;

    if (affixes == 0) return new Item(itemType, 1);

    var prefix = _chooseAffix(prefixes, itemType, affixDepth);
    var suffix = _chooseAffix(suffixes, itemType, affixDepth);

    if (affixes == 1 && prefix != null && suffix != null)
    {
      if (Rng.rng.oneIn(2))
      {
        prefix = null;
      }
      else
      {
        suffix = null;
      }
    }

    return new Item(itemType, 1, prefix, suffix);
  }

  public static Affix find(string name)
  {
    var type = prefixes.tryFind(name);
    if (type != null) return type;

    return suffixes.find(name);
  }

  static Affix _chooseAffix(
      ResourceSet<Affix> affixes, ItemType itemType, int depth)
  {
    return affixes.tryChooseMatching(depth, Items.types.getTags(itemType.name));
  }

  public static void initialize()
  {
    _elven();
    _dwarven();
    _resists();
    _extraDamage();
    _brands();
    // TODO: "of Accuracy" increases range of bows.
    // TODO: "Heavy" and "adamant" increase weight and armor.
    // TODO: More stat bonus affixes.

    _AffixBuilder.affixCategory("helm");
    for (var i = 0; i < 2; i++)
    {
      var a = _AffixBuilder.affix("_ of Acumen", 1.0);
      a.depth(35, to: 55);
      a.price(300, 2.0);
      a.intellect(1 + i);

      a = _AffixBuilder.affix("_ of Wisdom", 1.0);
      a.depth(45, to: 75);
      a.price(500, 3.0);
      a.intellect(3 + i);

      a = _AffixBuilder.affix("_ of Sagacity", 1.0);
      a.depth(75);
      a.price(700, 4.0);
      a.intellect(5 + i);

      a = _AffixBuilder.affix("_ of Genius", 1.0);
      a.depth(65);
      a.price(1000, 5.0);
      a.intellect(7 + i);
    }

    _AffixBuilder.finishAffix();
  }

  static void _elven()
  {
    _AffixBuilder.affixCategory("body");
    var a = _AffixBuilder.affix("Elven _", 1.0);
    a.depth(40, to: 80);
    a.price(400, 2.0);
    a.weight(-2);
    a.armor(2);
    a.resist(Elements.light);

    a = _AffixBuilder.affix("Elven _", 0.3);
    a.depth(60);
    a.price(600, 3.0);
    a.weight(-3);
    a.armor(4);
    a.resist(Elements.light);

    _AffixBuilder.affixCategory("cloak");
    a = _AffixBuilder.affix("Elven _", 1.0);
    a.depth(40, to: 80);
    a.price(300, 2.0);
    a.weight(-1);
    a.armor(3);
    a.resist(Elements.light);

    a = _AffixBuilder.affix("Elven _", 0.3);
    a.depth(60);
    a.price(500, 3.0);
    a.weight(-2);
    a.armor(5);
    a.resist(Elements.light);

    _AffixBuilder.affixCategory("boots");
    a = _AffixBuilder.affix("Elven _", 1.0);
    a.depth(50);
    a.price(400, 2.5);
    a.weight(-2);
    a.armor(2);
    // TODO: Increase dodge.

    _AffixBuilder.affixCategory("helm");
    a = _AffixBuilder.affix("Elven _", 1.0);
    a.depth(40, to: 80);
    a.price(400, 2.0);
    a.weight(-1);
    a.armor(1);
    a.intellect(1);
    a.resist(Elements.light);
    // TODO: Emanate.
    a = _AffixBuilder.affix("Elven _", 0.3);
    a.depth(60);
    a.price(600, 3.0);
    a.weight(-1);
    a.armor(2);
    a.intellect(2);
    a.resist(Elements.light);
    // TODO: Emanate.

    _AffixBuilder.affixCategory("shield");
    a = _AffixBuilder.affix("Elven _", 1.0);
    a.depth(40, to: 80);
    a.price(300, 1.6);
    a.heft(0.8);
    a.damage(scale: 1.3);
    a.resist(Elements.light);

    a = _AffixBuilder.affix("Elven _", 0.5);
    a.depth(50);
    a.price(500, 2.2);
    a.heft(0.6);
    a.damage(scale: 1.5);
    a.will(1);
    a.resist(Elements.light);
  }

  static void _dwarven()
  {
    // TODO: These prices need tuning.
    _AffixBuilder.affixCategory("body");
    var a = _AffixBuilder.affix("Dwarven _", 1.0);
    a.depth(30, to: 70);
    a.price(400, 2.0);
    a.weight(2);
    a.armor(4);
    a.resist(Elements.earth);
    a.resist(Elements.dark);

    a = _AffixBuilder.affix("Dwarven _", 0.5);
    a.depth(40);
    a.price(600, 3.0);
    a.weight(2);
    a.armor(6);
    a.resist(Elements.earth);
    a.resist(Elements.dark);

    _AffixBuilder.affixCategory("helm");
    a = _AffixBuilder.affix("Dwarven _", 1.0);
    a.depth(50, to: 80);
    a.price(300, 2.0);
    a.weight(1);
    a.armor(3);
    a.resist(Elements.dark);

    a = _AffixBuilder.affix("Dwarven _", 0.5);
    a.depth(60);
    a.price(500, 3.0);
    a.weight(1);
    a.armor(4);
    a.strength(1);
    a.fortitude(1);
    a.resist(Elements.dark);

    _AffixBuilder.affixCategory("gloves");
    a = _AffixBuilder.affix("Dwarven _", 1.0);
    a.depth(50);
    a.price(300, 2.0);
    a.weight(1);
    // TODO: Encumbrance.
    a.armor(3);
    a.strength(1);
    a.resist(Elements.earth);

    _AffixBuilder.affixCategory("boots");
    a = _AffixBuilder.affix("Dwarven _", 1.0);
    a.depth(50, to: 70);
    a.price(300, 2.0);
    a.weight(1);
    a.armor(3);
    a.resist(Elements.earth);

    a = _AffixBuilder.affix("Dwarven _", 0.3);
    a.depth(60);
    a.price(500, 3.0);
    a.weight(2);
    a.armor(5);
    a.fortitude(1);
    a.resist(Elements.dark);
    a.resist(Elements.earth);

    _AffixBuilder.affixCategory("shield");
    a = _AffixBuilder.affix("Dwarven _", 1.0);
    a.depth(40, to: 80);
    a.price(200, 2.2);
    a.heft(1.2);
    a.damage(scale: 1.5, bonus: 4);
    a.resist(Elements.earth);
    a.resist(Elements.dark);

    a = _AffixBuilder.affix("Dwarven _", 1.0);
    a.depth(60);
    a.price(400, 2.4);
    a.heft(1.3);
    a.damage(scale: 1.7, bonus: 5);
    a.fortitude(1);
    a.resist(Elements.earth);
    a.resist(Elements.dark);
  }

  static void _resists()
  {
    // TODO: Don't apply to all armor types?
    _AffixBuilder.affixCategory("armor");
    var a = _AffixBuilder.affix("_ of Resist Air", 0.5);
    a.depth(10, to: 50);
    a.price(200, 1.2);
    a.resist(Elements.air);

    a = _AffixBuilder.affix("_ of Resist Earth", 0.5);
    a.depth(11, to: 51);
    a.price(230, 1.2);
    a.resist(Elements.earth);

    a = _AffixBuilder.affix("_ of Resist Fire", 0.5);
    a.depth(12, to: 52);
    a.price(260, 1.3);
    a.resist(Elements.fire);

    a = _AffixBuilder.affix("_ of Resist Water", 0.5);
    a.depth(13, to: 53);
    a.price(310, 1.2);
    a.resist(Elements.water);

    a = _AffixBuilder.affix("_ of Resist Acid", 0.3);
    a.depth(14, to: 54);
    a.price(340, 1.3);
    a.resist(Elements.acid);

    a = _AffixBuilder.affix("_ of Resist Cold", 0.5);
    a.depth(15, to: 55);
    a.price(400, 1.2);
    a.resist(Elements.cold);

    a = _AffixBuilder.affix("_ of Resist Lightning", 0.3);
    a.depth(16, to: 56);
    a.price(430, 1.2);
    a.resist(Elements.lightning);

    a = _AffixBuilder.affix("_ of Resist Poison", 0.25);
    a.depth(17, to: 57);
    a.price(460, 1.5);
    a.resist(Elements.poison);

    a = _AffixBuilder.affix("_ of Resist Dark", 0.25);
    a.depth(18, to: 58);
    a.price(490, 1.3);
    a.resist(Elements.dark);

    a = _AffixBuilder.affix("_ of Resist Light", 0.25);
    a.depth(19, to: 59);
    a.price(490, 1.3);
    a.resist(Elements.light);

    a = _AffixBuilder.affix("_ of Resist Spirit", 0.4);
    a.depth(10, to: 60);
    a.price(520, 1.4);
    a.resist(Elements.spirit);

    a = _AffixBuilder.affix("_ of Resist Nature", 0.3);
    a.depth(40);
    a.price(3000, 4.0);
    a.resist(Elements.air);
    a.resist(Elements.earth);
    a.resist(Elements.fire);
    a.resist(Elements.water);
    a.resist(Elements.cold);
    a.resist(Elements.lightning);

    a = _AffixBuilder.affix("_ of Resist Destruction", 0.3);
    a.depth(40);
    a.price(1300, 2.6);
    a.resist(Elements.acid);
    a.resist(Elements.fire);
    a.resist(Elements.lightning);
    a.resist(Elements.poison);

    a = _AffixBuilder.affix("_ of Resist Evil", 0.3);
    a.depth(60);
    a.price(1500, 3.0);
    a.resist(Elements.acid);
    a.resist(Elements.poison);
    a.resist(Elements.dark);
    a.resist(Elements.spirit);

    a = _AffixBuilder.affix("_ of Resistance", 0.3);
    a.depth(70);
    a.price(5000, 6.0);
    a.resist(Elements.air);
    a.resist(Elements.earth);
    a.resist(Elements.fire);
    a.resist(Elements.water);
    a.resist(Elements.acid);
    a.resist(Elements.cold);
    a.resist(Elements.lightning);
    a.resist(Elements.poison);
    a.resist(Elements.dark);
    a.resist(Elements.light);
    a.resist(Elements.spirit);

    a = _AffixBuilder.affix("_ of Protection from Air", 0.25);
    a.depth(36);
    a.price(500, 1.4);
    a.resist(Elements.air, 2);

    a = _AffixBuilder.affix("_ of Protection from Earth", 0.25);
    a.depth(37);
    a.price(500, 1.4);
    a.resist(Elements.earth, 2);

    a = _AffixBuilder.affix("_ of Protection from Fire", 0.25);
    a.depth(38);
    a.price(500, 1.5);
    a.resist(Elements.fire, 2);

    a = _AffixBuilder.affix("_ of Protection from Water", 0.25);
    a.depth(39);
    a.price(500, 1.4);
    a.resist(Elements.water, 2);

    a = _AffixBuilder.affix("_ of Protection from Acid", 0.2);
    a.depth(40);
    a.price(500, 1.5);
    a.resist(Elements.acid, 2);

    a = _AffixBuilder.affix("_ of Protection from Cold", 0.25);
    a.depth(41);
    a.price(500, 1.4);
    a.resist(Elements.cold, 2);

    a = _AffixBuilder.affix("_ of Protection from Lightning", 0.16);
    a.depth(42);
    a.price(500, 1.4);
    a.resist(Elements.lightning, 2);

    a = _AffixBuilder.affix("_ of Protection from Poison", 0.14);
    a.depth(43);
    a.price(1000, 1.6);
    a.resist(Elements.poison, 2);

    a = _AffixBuilder.affix("_ of Protection from Dark", 0.14);
    a.depth(44);
    a.price(500, 1.5);
    a.resist(Elements.dark, 2);

    a = _AffixBuilder.affix("_ of Protection from Light", 0.14);
    a.depth(45);
    a.price(500, 1.5);
    a.resist(Elements.light, 2);

    a = _AffixBuilder.affix("_ of Protection from Spirit", 0.13);
    a.depth(46);
    a.price(800, 1.6);
    a.resist(Elements.spirit, 2);
  }

  static void _extraDamage()
  {
    // TODO: Exclude bows?
    _AffixBuilder.affixCategory("weapon");
    var a = _AffixBuilder.affix("_ of Harming", 1.0);
    a.depth(1, to: 30);
    a.price(100, 1.2);
    a.heft(1.05);
    a.damage(bonus: 1);

    a = _AffixBuilder.affix("_ of Wounding", 1.0);
    a.depth(10, to: 50);
    a.price(140, 1.3);
    a.heft(1.07);
    a.damage(bonus: 3);

    a = _AffixBuilder.affix("_ of Maiming", 1.0);
    a.depth(25, to: 75);
    a.price(180, 1.5);
    a.heft(1.09);
    a.damage(scale: 1.2, bonus: 3);

    a = _AffixBuilder.affix("_ of Slaying", 1.0);
    a.depth(45);
    a.price(200, 2.0);
    a.heft(1.11);
    a.damage(scale: 1.4, bonus: 5);

    _AffixBuilder.affixCategory("bow");
    a = _AffixBuilder.affix("Ash _", 1.0);
    a.depth(10, to: 70);
    a.price(300, 1.3);
    a.heft(0.8);
    a.damage(bonus: 3);

    a = _AffixBuilder.affix("Yew _", 1.0);
    a.depth(20);
    a.price(500, 1.4);
    a.heft(0.8);
    a.damage(bonus: 5);
  }

  static void _brands()
  {
    _AffixBuilder.affixCategory("weapon");

    var a = _AffixBuilder.affix("Glimmering _", 0.3);
    a.depth(20, to: 60);
    a.price(300, 1.3);
    a.damage(scale: 1.2);
    a.brand(Elements.light);

    a = _AffixBuilder.affix("Shining _", 0.25);
    a.depth(32, to: 90);
    a.price(400, 1.6);
    a.damage(scale: 1.4);
    a.brand(Elements.light);

    a = _AffixBuilder.affix("Radiant _", 0.2);
    a.depth(48);
    a.price(500, 2.0);
    a.damage(scale: 1.6);
    a.brand(Elements.light, resist: 2);

    a = _AffixBuilder.affix("Dim _", 0.3);
    a.depth(16, to: 60);
    a.price(300, 1.3);
    a.damage(scale: 1.2);
    a.brand(Elements.dark);

    a = _AffixBuilder.affix("Dark _", 0.25);
    a.depth(32, to: 80);
    a.price(400, 1.6);
    a.damage(scale: 1.4);
    a.brand(Elements.dark);

    a = _AffixBuilder.affix("Black _", 0.2);
    a.depth(56);
    a.price(500, 2.0);
    a.damage(scale: 1.6);
    a.brand(Elements.dark, resist: 2);

    a = _AffixBuilder.affix("Chilling _", 0.3);
    a.depth(20, to: 65);
    a.price(300, 1.5);
    a.damage(scale: 1.4);
    a.brand(Elements.cold);

    a = _AffixBuilder.affix("Freezing _", 0.25);
    a.depth(40);
    a.price(400, 1.7);
    a.damage(scale: 1.6);
    a.brand(Elements.cold, resist: 2);

    a = _AffixBuilder.affix("Burning _", 0.3);
    a.depth(20, to: 60);
    a.price(300, 1.5);
    a.damage(scale: 1.3);
    a.brand(Elements.fire);

    a = _AffixBuilder.affix("Flaming _", 0.25);
    a.depth(40, to: 90);
    a.price(360, 1.8);
    a.damage(scale: 1.6);
    a.brand(Elements.fire);

    a = _AffixBuilder.affix("Searing _", 0.2);
    a.depth(60);
    a.price(500, 2.1);
    a.damage(scale: 1.8);
    a.brand(Elements.fire, resist: 2);

    a = _AffixBuilder.affix("Electric _", 0.2);
    a.depth(50);
    a.price(300, 1.5);
    a.damage(scale: 1.4);
    a.brand(Elements.lightning);

    a = _AffixBuilder.affix("Shocking _", 0.2);
    a.depth(70);
    a.price(400, 2.0);
    a.damage(scale: 1.8);
    a.brand(Elements.lightning, resist: 2);

    a = _AffixBuilder.affix("Poisonous _", 0.2);
    a.depth(35, to: 90);
    a.price(500, 1.5);
    a.damage(scale: 1.1);
    a.brand(Elements.poison);

    a = _AffixBuilder.affix("Venomous _", 0.2);
    a.depth(70);
    a.price(800, 1.8);
    a.damage(scale: 1.3);
    a.brand(Elements.poison, resist: 2);

    a = _AffixBuilder.affix("Ghostly _", 0.2);
    a.depth(45, to: 85);
    a.price(300, 1.6);
    a.heft(0.7);
    a.damage(scale: 1.4);
    a.brand(Elements.spirit);

    a = _AffixBuilder.affix("Spiritual _", 0.15);
    a.depth(80);
    a.price(400, 2.1);
    a.heft(0.7);
    a.damage(scale: 1.7);
    a.brand(Elements.spirit, resist: 2);
  }

  public static void defineItemTag(string tag)
  {
    prefixes.defineTags(tag);
    suffixes.defineTags(tag);
  }
}
