using System.Collections;
using System.Collections.Generic;
using UnityTerminal;

public partial class Items
{
  public static void litter()
  {
    var b = _CategoryBuilder.category(CharCode.latinCapitalLetterCWithCedilla, stack: 10);
    b.tag("item");
    b.toss(damage: 3, range: 7, element: Elements.earth, breakage: 10);
    _ItemBuilder.item("Rock", Hues.tan, frequency: 0.1).depth(1);

    b = _CategoryBuilder.category(CharCode.latinSmallLetterUWithDiaeresis, stack: 4);
    b.tag("item");
    b.toss(damage: 2, range: 5, breakage: 30);
    _ItemBuilder.item("Skull", Hues.lightCoolGray, frequency: 0.1).depth(1);
  }

  public static void treasure()
  {
    // Coins.
    var b = _CategoryBuilder.category(CharCode.centSign);
    b.tag("treasure/coin");
    b.treasure();
    _ItemBuilder.item("Copper Coins", Hues.persimmon, price: 4).depth(1, to: 11);
    _ItemBuilder.item("Bronze Coins", Hues.tan, price: 8).depth(7, to: 20);
    _ItemBuilder.item("Silver Coins", Hues.lightAqua, price: 20).depth(11, to: 30);
    _ItemBuilder.item("Electrum Coins", Hues.buttermilk, price: 50).depth(20, to: 40);
    _ItemBuilder.item("Gold Coins", Hues.gold, price: 100).depth(30, to: 50);
    _ItemBuilder.item("Platinum Coins", Hues.lightCoolGray, price: 300).depth(40, to: 70);

    // Bars.
    b = _CategoryBuilder.category(CharCode.dollarSign);
    b.tag("treasure/bar");
    b.treasure();
    _ItemBuilder.item("Copper Bar", Hues.persimmon, price: 150).depth(35, to: 60);
    _ItemBuilder.item("Bronze Bar", Hues.tan, price: 500).depth(50, to: 70);
    _ItemBuilder.item("Silver Bar", Hues.lightAqua, price: 800).depth(60, to: 80);
    _ItemBuilder.item("Electrum Bar", Hues.buttermilk, price: 1200).depth(70, to: 90);
    _ItemBuilder.item("Gold Bar", Hues.gold, price: 2000).depth(80);
    _ItemBuilder.item("Platinum Bar", Hues.lightCoolGray, price: 3000).depth(90);

    /*
      // TODO: Could add more treasure using other currency symbols.

      // TODO: Instead of treasure, make these recipe components.
      // Gems
      category(r"$", "treasure/gem");
      tossable(damage: 2, range: 7, breakage: 5);
      treasure("Amethyst",      3,  lightPurple,   100);
      treasure("Sapphire",      12, blue,          200);
      treasure("Emerald",       20, green,         300);
      treasure("Ruby",          35, red,           500);
      treasure("Diamond",       60, white,        1000);
      treasure("Blue Diamond",  80, lightBlue,    2000);

      // Rocks
      category(r"$", "treasure/rock");
      tossable(damage: 2, range: 7, breakage: 5);
      treasure("Turquoise Stone", 15, aqua,         60);
      treasure("Onyx Stone",      20, darkGray,    160);
      treasure("Malachite Stone", 25, lightGreen,  400);
      treasure("Jade Stone",      30, darkGreen,   400);
      treasure("Pearl",           35, lightYellow, 600);
      treasure("Opal",            40, lightPurple, 800);
      treasure("Fire Opal",       50, lightOrange, 900);
    */
  }

  public static void pelts()
  {
    // TODO: Should these appear on the floor?
    // TODO: Better pictogram than a pelt?
    // TODO: These currently have no use. Either remove them, or add crafting
    // back in.
    /*
    category(CharCode.latinSmallLetterEWithAcute, stack: 20)
      ..destroy(Elements.fire, chance: 40, fuel: 1);
    item("Flower", cornflower, 1, frequency: 1.0); // TODO: Use in recipe.
    item("Insect Wing", violet, 1, frequency: 1.0);
    item("Red Feather", brickRed, 2, frequency: 1.0); // TODO: Use in recipe.
    item("Black Feather", steelGray, 2, frequency: 1.0);

    category(CharCode.latinSmallLetterEWithAcute, stack: 4)
      ..destroy(Elements.fire, chance: 20, fuel: 3);
    item("Fur Pelt", persimmon, 1, frequency: 1.0);
    */
  }

  public static void food()
  {
    var b = _CategoryBuilder.category(CharCode.invertedExclamationMark);
    b.tag("item/food");
    b.destroy(Elements.fire, chance: 20, fuel: 3);
    var t = _ItemBuilder.item("Stale Biscuit", Hues.sandal);
    t.depth(1, to: 10);
    t.stack(6);
    t.food(100);
    t = _ItemBuilder.item("Loa[f|ves] of Bread", Hues.tan, price: 4);
    t.depth(3, to: 40);
    t.stack(6);
    t.food(200);

    b = _CategoryBuilder.category(CharCode.vulgarFractionOneQuarter);
    b.tag("item/food");
    b.destroy(Elements.fire, chance: 15, fuel: 2);
    t = _ItemBuilder.item("Chunk[s] of Meat", Hues.brown, price: 10);
    t.depth(8, to: 60);
    t.stack(4);
    t.food(400);
    // TODO: Chance of poisoning.
    // TODO: Make some monsters drop this.
    t = _ItemBuilder.item("Piece[s] of Jerky", Hues.tan, price: 20);
    t.depth(15);
    t.stack(12);
    t.food(600);
    // TODO: More foods. Some should also cure minor conditions or cause them.
  }

  public static void lightSources()
  {
    var b = _CategoryBuilder.category(CharCode.notSign, verb: "hit[s]");
    b.tag("equipment/light");
    b.toss(breakage: 70);

    // TODO: Ball of fire when hits toss target.
    var t = _ItemBuilder.item("Tallow Candle", Hues.sandal, price: 6);
    t.depth(1, to: 12);
    t.stack(10);
    t.toss(damage: 2, range: 8, element: Elements.fire);
    t.lightSource(level: 2, range: 5);
    t.destroy(Elements.fire, chance: 40, fuel: 20);

    // TODO: Ball of fire when hits toss target.
    t = _ItemBuilder.item("Wax Candle", Hues.ash, price: 8);
    t.depth(4, to: 20);
    t.stack(10);
    t.toss(damage: 3, range: 8, element: Elements.fire);
    t.lightSource(level: 3, range: 7);
    t.destroy(Elements.fire, chance: 40, fuel: 25);

    // TODO: Larger ball of fire when hits toss target.
    t = _ItemBuilder.item("Oil Lamp", Hues.brown, price: 18);
    t.depth(8, to: 30);
    t.stack(4);
    t.toss(damage: 10, range: 8, element: Elements.fire);
    t.lightSource(level: 4, range: 10);
    t.destroy(Elements.fire, chance: 50, fuel: 40);

    // TODO: Ball of fire when hits toss target.
    t = _ItemBuilder.item("Torch[es]", Hues.tan, price: 16);
    t.depth(11, to: 45);
    t.stack(4);
    t.toss(damage: 6, range: 10, element: Elements.fire);
    t.lightSource(level: 5, range: 14);
    t.destroy(Elements.fire, chance: 60, fuel: 60);

    // TODO: Maybe allow this to be equipped and increase its radius when held?
    t = _ItemBuilder.item("Lantern", Hues.gold, frequency: 0.3, price: 78);
    t.depth(18);
    t.toss(damage: 5, range: 5, element: Elements.fire);
    t.lightSource(level: 6, range: 18);
  }
}

