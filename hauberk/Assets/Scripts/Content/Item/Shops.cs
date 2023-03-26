using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// Static class containing all of the [Shop]s.
class Shops
{
  public static Dictionary<string, Shop> all = new Dictionary<string, Shop>();

  public static void shop(string name, Dictionary<string, double> itemTypes)
  {
    var drops = new Dictionary<Drop, double> { };
    foreach (var kv in itemTypes)
    {
      var _name = kv.Key;
      var frequency = kv.Value;
      drops[DropUtils.parseDrop(_name, depth: 1)] = frequency;
    };

    Shops.all[name] = new Shop(name, DropUtils.dropOneOf(drops));
  }

  public static void initialize()
  {
    shop("The General's General Store", new Dictionary<string, double>{
      {"Loaf of Bread", 2.0},
      {"Chunk of Meat", 0.6},
      {"Tallow Candle", 1.0},
      {"Wax Candle", 0.7},
      {"Oil Lamp", 0.5},
      {"Torch", 0.3},
      {"Lantern", 0.1},
      {"Soothing Balm", 0.6},
      {"Mending Salve", 0.4},
      {"Healing Poultice", 0.2},
      {"Club", 0.1},
      {"Staff", 0.1},
      {"Quarterstaff", 0.05},
      {"Whip", 0.1},
      {"Dagger", 0.1},
    });

    // TODO: Update based on new items. Update frequencies.
    shop("Dirk's Death Emporium", new Dictionary<string, double>{
      {"Hammer", 0.5},
      {"Mattock", 0.2},
      {"War Hammer", 0.1},
      {"Morningstar", 0.6},
      {"Mace", 0.3},
      {"Chain Whip", 0.2},
      {"Flail", 0.1},
      {"Falchion", 0.7},
      {"Rapier", 1.0},
      {"Shortsword", 0.6},
      {"Scimitar", 0.4},
      {"Cutlass", 0.2},
      {"Spear", 1.0},
      {"Angon", 0.4},
      {"Lance", 0.2},
      {"Partisan", 0.1},
      {"Hatchet", 1.0},
      {"Axe", 0.5},
      {"Valaska", 0.25},
      {"Battleaxe", 0.2},
      {"Short Bow", 1.0},
      {"Longbow", 0.3},
      {"Crossbow", 0.05}
    });

    shop("Skullduggery and Bamboozelry", new Dictionary<string, double>{
      {"Dirk", 1.0},
      {"Dagger", 0.3},
      {"Stiletto", 0.1},
      {"Rondel", 0.05},
      {"Baselard", 0.02}
    });

    shop("Garthag's Armoury", new Dictionary<string, double>{
      {"Cloak", 1.0},
      {"Fur Cloak", 1.0},
      {"Cloth Shirt", 1.0},
      {"Leather Shirt", 1.0},
      {"Jerkin", 1.0},
      {"Leather Armor", 1.0},
      {"Padded Armor", 1.0},
      {"Studded Armor", 1.0},
      {"Mail Hauberk", 1.0},
      {"Scale Mail", 1.0},
      {"Robe", 1.0},
      {"Fur-lined Robe", 1.0},
      {"Pair of Sandals", 1.0},
      {"Pair of Shoes", 1.0},
      {"Pair of Boots", 1.0},
      {"Pair of Plated Boots", 1.0},
      {"Pair of Greaves", 1.0}
    });

    shop("Unguence the Alchemist", new Dictionary<string, double>{
      {"Soothing Balm", 1.0},
      {"Mending Salve", 1.0},
      {"Healing Poultice", 1.0},
      {"Antidote", 1.0},
      {"Potion of Quickness", 1.0},
      {"Potion of Alacrity", 1.0},
      {"Bottled Wind", 1.0},
      {"Bottled Ice", 1.0},
      {"Bottled Fire", 1.0},
      {"Bottled Ocean", 1.0},
      {"Bottled Earth", 1.0},
    });

    shop("The Droll Magery", new Dictionary<string, double>{
      {"Spellbook \"Elemental Primer\"", 1.0},
      {"Scroll of Sidestepping", 1.0},
      {"Scroll of Phasing", 1.0},
      {"Scroll of Item Detection", 1.0},
    });
  }

  /*
    Glur's Rare Artifacts
    */
}
