using System.Collections;
using System.Collections.Generic;
using UnityTerminal;

public partial class Items
{
  public static void helms()
  {
    var b = _CategoryBuilder.category(CharCode.latinCapitalLetterEWithAcute);
    b.tag("equipment/armor/helm");
    b.toss(damage: 3, range: 5, breakage: 10);
    var t = _ItemBuilder.item("Leather Cap", Hues.tan, frequency: 0.5, price: 50);
    t.depth(4, to: 40);
    t.armor(2, weight: 2);
    t.destroy(Elements.fire, chance: 12, fuel: 2);
    t = _ItemBuilder.item("Chainmail Coif", Hues.darkCoolGray, frequency: 0.5, price: 160);
    t.depth(10, to: 60);
    t.armor(3, weight: 3);
    t = _ItemBuilder.item("Steel Cap", Hues.coolGray, frequency: 0.5, price: 200);
    t.depth(25, to: 80);
    t.armor(4, weight: 3);
    t = _ItemBuilder.item("Visored Helm", Hues.lightCoolGray, frequency: 0.5, price: 350);
    t.depth(40);
    t.armor(5, weight: 6);
    t = _ItemBuilder.item("Great Helm", Hues.ash, frequency: 0.5, price: 550);
    t.depth(50);
    t.armor(6, weight: 8);
  }

  public static void bodyArmor()
  {
    // Robes.
    var b = _CategoryBuilder.category(CharCode.latinSmallLetterOWithCircumflex);
    b.tag("equipment/armor/body/robe");
    var t = _ItemBuilder.item("Robe", Hues.blue, frequency: 0.5, price: 20);
    t.depth(2, to: 40);
    t.armor(4);
    t.destroy(Elements.fire, chance: 15, fuel: 8);
    t = _ItemBuilder.item("Fur-lined Robe", Hues.sherwood, frequency: 0.25, price: 110);
    t.depth(6);
    t.armor(6);
    t.destroy(Elements.fire, chance: 12, fuel: 8);
    // TODO: Better robes that don't add weight and appear later.

    // Soft armor.
    b = _CategoryBuilder.category(CharCode.latinSmallLetterOWithDiaeresis);
    b.tag("equipment/armor/body");
    t = _ItemBuilder.item("Cloth Shirt", Hues.ash, frequency: 0.5, price: 40);
    t.depth(2, to: 30);
    t.armor(3);
    t.destroy(Elements.fire, chance: 15, fuel: 4);
    t = _ItemBuilder.item("Leather Shirt", Hues.tan, frequency: 0.5, price: 90);
    t.depth(5, to: 50);
    t.armor(6, weight: 1);
    t.destroy(Elements.fire, chance: 12, fuel: 4);
    t = _ItemBuilder.item("Jerkin", Hues.lightCoolGray, frequency: 0.5, price: 130);
    t.depth(8, to: 70);
    t.armor(8, weight: 1);
    t = _ItemBuilder.item("Leather Armor", Hues.brown, frequency: 0.5, price: 240);
    t.depth(12, to: 90);
    t.armor(11, weight: 2);
    t.destroy(Elements.fire, chance: 10, fuel: 4);
    t = _ItemBuilder.item("Padded Armor", Hues.darkCoolGray, frequency: 0.5, price: 320);
    t.depth(16);
    t.armor(15, weight: 3);
    t.destroy(Elements.fire, chance: 8, fuel: 4);
    t = _ItemBuilder.item("Studded Armor", Hues.coolGray, frequency: 0.5, price: 400);
    t.depth(20);
    t.armor(22, weight: 4);
    t.destroy(Elements.fire, chance: 6, fuel: 4);

    // Mail armor.
    b = _CategoryBuilder.category(CharCode.latinSmallLetterOWithGrave);
    b.tag("equipment/armor/body");
    t = _ItemBuilder.item("Mail Hauberk", Hues.darkCoolGray, frequency: 0.5, price: 500);
    t.depth(25);
    t.armor(28, weight: 5);
    t = _ItemBuilder.item("Scale Mail", Hues.lightCoolGray, frequency: 0.5, price: 700);
    t.depth(35);
    t.armor(36, weight: 7);

    //  CharCode.latinSmallLetterUWithCircumflex // armor

    /*
    Metal Lamellar Armor[s]
    Chain Mail Armor[s]
    Metal Scale Mail[s]
    Plated Mail[s]
    Brigandine[s]
    Steel Breastplate[s]
    Partial Plate Armor[s]
    Full Plate Armor[s]
    */
  }

  public static void cloaks()
  {
    _CategoryBuilder.category(CharCode.latinCapitalLetterAe).tag("equipment/armor/cloak");
    var t = _ItemBuilder.item("Cloak", Hues.darkBlue, frequency: 0.5, price: 70);
    t.depth(10, to: 40);
    t.armor(2, weight: 1);
    t.destroy(Elements.fire, chance: 20, fuel: 5);
    t = _ItemBuilder.item("Fur Cloak", Hues.brown, frequency: 0.2, price: 140);
    t.depth(20, to: 60);
    t.armor(4, weight: 2);
    t.destroy(Elements.fire, chance: 16, fuel: 5);
    t = _ItemBuilder.item("Spidersilk Cloak", Hues.darkCoolGray, frequency: 0.2, price: 460);
    t.depth(40);
    t.armor(6);
    t.destroy(Elements.fire, chance: 25, fuel: 3);
    // TODO: Better cloaks that don't add weight and appear later.
  }

  public static void gloves()
  {
    var b = _CategoryBuilder.category(CharCode.latinCapitalLetterAWithRingAbove);
    b.tag("equipment/armor/gloves");
    b.toss(damage: 5, range: 4, breakage: 20);
    // TODO: Encumbrance.
    // Here's an idea to get mages wearing light armor and no gloves: Give weapons
    // and usable items the equivalent of heft for agility. (Need a name.) If
    // their agility is too low, the weapon gets a negative strike bonus. But
    // also *affix power is reduced*. Thus, if a mage is too encumbered by gloves,
    // their wand no longer makes spells more powerful.
    //
    // Could probably do the same for heft. A battleaxe of fire doesn't do a lot
    // of fire damage if you can't lift it.
    var t = _ItemBuilder.item("Pair[s] of Gloves", Hues.sandal, frequency: 0.5, price: 170);
    t.depth(8);
    t.armor(1);
    t.destroy(Elements.fire, chance: 7, fuel: 2);
    t = _ItemBuilder.item("Set[s] of Bracers", Hues.brown, frequency: 0.5, price: 480);
    t.depth(17);
    t.armor(2, weight: 1);
    t = _ItemBuilder.item("Pair[s] of Gauntlets", Hues.darkCoolGray, frequency: 0.5, price: 800);
    t.depth(34);
    t.armor(4, weight: 2);
  }

  public static void shields()
  {
    var b = _CategoryBuilder.category(CharCode.latinSmallLetterAe);
    b.tag("equipment/armor/shield");
    b.toss(damage: 5, range: 8, breakage: 10);
    // TODO: Encumbrance.
    var t = _ItemBuilder.item("Small Leather Shield", Hues.brown, frequency: 0.5, price: 170);
    t.depth(12, to: 50);
    t.armor(0, weight: 2);
    t.defense(4, "The shield blocks {2}.");
    t.destroy(Elements.fire, chance: 7, fuel: 14);
    t = _ItemBuilder.item("Wooden Targe", Hues.sandal, frequency: 0.5, price: 250);
    t.depth(25);
    t.armor(0, weight: 4);
    t.defense(6, "The targe blocks {2}.");
    t.destroy(Elements.fire, chance: 14, fuel: 20);
    t = _ItemBuilder.item("Large Leather Shield", Hues.tan, frequency: 0.5, price: 320);
    t.depth(35);
    t.armor(0, weight: 5);
    t.defense(8, "The shield blocks {2}.");
    t.destroy(Elements.fire, chance: 7, fuel: 17);
    t = _ItemBuilder.item("Steel Buckler", Hues.darkCoolGray, frequency: 0.5, price: 450);
    t.depth(50);
    t.armor(0, weight: 4);
    t.defense(10, "The buckler blocks {2}.");
    t = _ItemBuilder.item("Kite Shield", Hues.lightCoolGray, frequency: 0.5, price: 650);
    t.depth(65);
    t.armor(0, weight: 7);
    t.defense(12, "The shield blocks {2}.");
  }

  public static void boots()
  {
    var b = _CategoryBuilder.category(CharCode.latinSmallLetterIWithGrave);
    b.tag("equipment/armor/boots");
    var t = _ItemBuilder.item("Pair[s] of Sandals", Hues.tan, frequency: 0.24, price: 10);
    t.depth(2, to: 20);
    t.armor(1);
    t.destroy(Elements.fire, chance: 20, fuel: 3);
    t = _ItemBuilder.item("Pair[s] of Shoes", Hues.brown, frequency: 0.3, price: 30);
    t.depth(8, to: 40);
    t.armor(2);
    t.destroy(Elements.fire, chance: 14, fuel: 3);

    b = _CategoryBuilder.category(CharCode.latinCapitalLetterAWithDiaeresis);
    b.tag("equipment/armor/boots");
    t = _ItemBuilder.item("Pair[s] of Boots", Hues.tan, frequency: 0.3, price: 70);
    t.depth(14);
    t.armor(6, weight: 1);
    t = _ItemBuilder.item("Pair[s] of Plated Boots", Hues.coolGray, frequency: 0.3, price: 250);
    t.depth(22);
    t.armor(8, weight: 2);
    t = _ItemBuilder.item("Pair[s] of Greaves", Hues.lightCoolGray, frequency: 0.25, price: 350);
    t.depth(47);
    t.armor(12, weight: 3);
  }

}
