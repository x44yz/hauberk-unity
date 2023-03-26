using System.Collections;
using System.Collections.Generic;
using UnityTerminal;

public partial class Items
{
  public static void weapons()
  {
    // https://en.wikipedia.org/wiki/List_of_mythological_objects#Weapons

    // Bludgeons.
    var b = _CategoryBuilder.category(CharCode.latinSmallLetterAWithAcute, verb: "hit[s]");
    b.tag("equipment/weapon/club");
    b.skill("Club Mastery");
    b.toss(breakage: 25, range: 5);
    var t = _ItemBuilder.item("Stick", Hues.tan, frequency: 0.5);
    t.depth(1, to: 20);
    t.weapon(7, heft: 6);
    t.toss(damage: 3);
    t.destroy(Elements.fire, chance: 10, fuel: 10);
    t = _ItemBuilder.item("Cudgel", Hues.lightCoolGray, frequency: 0.5, price: 20);
    t.depth(6, to: 60);
    t.weapon(9, heft: 8);
    t.toss(damage: 4);
    t.destroy(Elements.fire, chance: 5, fuel: 10);
    t = _ItemBuilder.item("Club", Hues.brown, frequency: 0.5, price: 40);
    t.depth(14);
    t.weapon(12, heft: 11);
    t.toss(damage: 5);
    t.destroy(Elements.fire, chance: 2, fuel: 10);

    // Staves.
    // TODO: Staff skill. Distance attack + pushback?
    b = _CategoryBuilder.category(CharCode.latinSmallLetterIWithAcute, verb: "hit[s]");
    b.tag("equipment/weapon/staff");
    b.twoHanded();
    b.toss(breakage: 35, range: 4);
    t = _ItemBuilder.item("Walking Stick", Hues.tan, frequency: 0.5, price: 10);
    t.depth(2, to: 40);
    t.weapon(10, heft: 9);
    t.toss(damage: 3);
    t.destroy(Elements.fire, chance: 5, fuel: 15);
    t = _ItemBuilder.item("Sta[ff|aves]", Hues.brown, frequency: 0.5, price: 50);
    t.depth(7);
    t.weapon(14, heft: 11);
    t.toss(damage: 5);
    t.destroy(Elements.fire, chance: 2, fuel: 15);
    t = _ItemBuilder.item("Quartersta[ff|aves]", Hues.lightCoolGray, frequency: 0.5, price: 80);
    t.depth(24);
    t.weapon(20, heft: 13);
    t.toss(damage: 8);
    t.destroy(Elements.fire, chance: 2, fuel: 15);

    // Hammers.
    b = _CategoryBuilder.category(CharCode.latinSmallLetterOWithAcute, verb: "bash[es]");
    b.tag("equipment/weapon/hammer");
    b.toss(breakage: 15, range: 5);
    t = _ItemBuilder.item("Hammer", Hues.tan, frequency: 0.5, price: 120);
    t.depth(40);
    t.weapon(32, heft: 22);
    t.toss(damage: 12);
    t = _ItemBuilder.item("Mattock", Hues.brown, frequency: 0.5, price: 240);
    t.depth(46);
    t.weapon(40, heft: 26);
    t.toss(damage: 16);
    t = _ItemBuilder.item("War Hammer", Hues.lightCoolGray, frequency: 0.5, price: 400);
    t.depth(52);
    t.weapon(48, heft: 30);
    t.toss(damage: 20);

    // Maces.
    b = _CategoryBuilder.category(CharCode.latinSmallLetterUWithAcute, verb: "bash[es]");
    b.tag("equipment/weapon/mace");
    b.toss(breakage: 15, range: 4);
    t = _ItemBuilder.item("Morningstar", Hues.lightCoolGray, frequency: 0.5, price: 130);
    t.depth(24);
    t.weapon(26, heft: 17);
    t.toss(damage: 11);
    t = _ItemBuilder.item("Mace", Hues.coolGray, frequency: 0.5, price: 310);
    t.depth(33);
    t.weapon(36, heft: 23);
    t.toss(damage: 16);

    // Whips.
    b = _CategoryBuilder.category(CharCode.latinSmallLetterNWithTilde, verb: "whip[s]");
    b.tag("equipment/weapon/whip");
    b.toss(breakage: 25, range: 4);
    b.skill("Whip Mastery");
    t = _ItemBuilder.item("Whip", Hues.tan, frequency: 0.5, price: 40);
    t.depth(4);
    t.weapon(10, heft: 7);
    t.toss(damage: 1);
    t.destroy(Elements.fire, chance: 10, fuel: 5);
    t = _ItemBuilder.item("Chain Whip", Hues.lightCoolGray, frequency: 0.5, price: 230);
    t.depth(15);
    t.weapon(18, heft: 15);
    t.toss(damage: 2);
    t = _ItemBuilder.item("Flail", Hues.coolGray, frequency: 0.5, price: 350);
    t.depth(27);
    t.weapon(28, heft: 24);
    t.toss(damage: 4);

    // Knives.
    // TODO: Dagger skill.
    b = _CategoryBuilder.category(CharCode.latinCapitalLetterNWithTilde, verb: "stab[s]");
    b.tag("equipment/weapon/dagger");
    b.toss(breakage: 2, range: 8);
    t = _ItemBuilder.item("Kni[fe|ves]", Hues.darkCoolGray, frequency: 0.5, price: 20);
    t.depth(3, to: 20);
    t.weapon(8, heft: 5);
    t.toss(damage: 8);
    t = _ItemBuilder.item("Dirk", Hues.lightCoolGray, frequency: 0.5, price: 30);
    t.depth(4, to: 40);
    t.weapon(10, heft: 6);
    t.toss(damage: 10);
    t = _ItemBuilder.item("Dagger", Hues.lightBlue, frequency: 0.5, price: 50);
    t.depth(6, to: 70);
    t.weapon(12, heft: 7);
    t.toss(damage: 12);
    t = _ItemBuilder.item("Stiletto[es]", Hues.coolGray, frequency: 0.5, price: 80);
    t.depth(10);
    t.weapon(14, heft: 6);
    t.toss(damage: 14);
    t = _ItemBuilder.item("Rondel", Hues.lightAqua, frequency: 0.5, price: 130);
    t.depth(20);
    t.weapon(16, heft: 9);
    t.toss(damage: 16);
    t = _ItemBuilder.item("Baselard", Hues.gold, frequency: 0.5, price: 200);
    t.depth(30);
    t.weapon(18, heft: 11);
    t.toss(damage: 18);
    // Main-guache
    // Unique dagger: "Mercygiver" (see Misericorde at Wikipedia)

    b = _CategoryBuilder.category(CharCode.feminineOrdinalIndicator, verb: "slash[es]");
    b.tag("equipment/weapon/sword");
    b.toss(breakage: 20, range: 5);
    b.skill("Swordfighting");
    t = _ItemBuilder.item("Rapier", Hues.darkCoolGray, frequency: 0.5, price: 140);
    t.depth(13);
    t.weapon(13, heft: 12);
    t.toss(damage: 4);
    t = _ItemBuilder.item("Shortsword", Hues.coolGray, frequency: 0.5, price: 230);
    t.depth(17);
    t.weapon(15, heft: 13);
    t.toss(damage: 6);
    t = _ItemBuilder.item("Scimitar", Hues.lightCoolGray, frequency: 0.5, price: 370);
    t.depth(18);
    t.weapon(24, heft: 16);
    t.toss(damage: 9);
    t = _ItemBuilder.item("Cutlass[es]", Hues.buttermilk, frequency: 0.5, price: 520);
    t.depth(20);
    t.weapon(26, heft: 17);
    t.toss(damage: 11);
    t = _ItemBuilder.item("Falchion", Hues.lightAqua, frequency: 0.5, price: 750);
    t.depth(34);
    t.weapon(28, heft: 18);
    t.toss(damage: 15);

    /*

    // Two-handed swords.
    Bastard Sword[s]
    Longsword[s]
    Broadsword[s]
    Claymore[s]
    Flamberge[s]

    */

    // Spears.
    b = _CategoryBuilder.category(CharCode.masculineOrdinalIndicator, verb: "stab[s]");
    b.tag("equipment/weapon/spear");
    b.toss(range: 9);
    b.skill("Spear Mastery");
    t = _ItemBuilder.item("Pointed Stick", Hues.brown, frequency: 0.5, price: 10);
    t.depth(2, to: 30);
    t.weapon(10, heft: 9);
    t.toss(damage: 9);
    t.destroy(Elements.fire, chance: 7, fuel: 12);
    t = _ItemBuilder.item("Spear", Hues.tan, frequency: 0.5, price: 160);
    t.depth(13, to: 60);
    t.weapon(16, heft: 13);
    t.toss(damage: 15);
    t = _ItemBuilder.item("Angon", Hues.lightCoolGray, frequency: 0.5, price: 340);
    t.depth(21);
    t.weapon(20, heft: 19);
    t.toss(damage: 20);

    b = _CategoryBuilder.category(CharCode.masculineOrdinalIndicator, verb: "stab[s]");
    b.tag("equipment/weapon/polearm");
    b.twoHanded();
    b.toss(range: 4);
    b.skill("Spear Mastery");
    t = _ItemBuilder.item("Lance", Hues.lightBlue, frequency: 0.5, price: 550);
    t.depth(28);
    t.weapon(22, heft: 23);
    t.toss(damage: 20);
    t = _ItemBuilder.item("Partisan", Hues.coolGray, frequency: 0.5, price: 850);
    t.depth(35);
    t.weapon(26, heft: 25);
    t.toss(damage: 26);

    // glaive, voulge, halberd, pole-axe, lucerne hammer,

    b = _CategoryBuilder.category(CharCode.invertedQuestionMark, verb: "chop[s]");
    b.tag("equipment/weapon/axe");
    b.skill("Axe Mastery");
    t = _ItemBuilder.item("Hatchet", Hues.coolGray, frequency: 0.5, price: 90);
    t.depth(6, to: 50);
    t.weapon(12, heft: 10);
    t.toss(damage: 20, range: 8);
    t = _ItemBuilder.item("Axe", Hues.tan, frequency: 0.5, price: 210);
    t.depth(12, to: 70);
    t.weapon(15, heft: 14);
    t.toss(damage: 24, range: 7);
    t = _ItemBuilder.item("Valaska", Hues.lightCoolGray, frequency: 0.5, price: 330);
    t.depth(24);
    t.weapon(19, heft: 19);
    t.toss(damage: 26, range: 5);
    // TODO: Two-handed?
    t = _ItemBuilder.item("Battleaxe", Hues.darkCoolGray, frequency: 0.5, price: 550);
    t.depth(40);
    t.weapon(25, heft: 30);
    t.toss(damage: 28, range: 4);

    // Bows.
    b = _CategoryBuilder.category(CharCode.reversedNotSign, verb: "hit[s]");
    b.tag("equipment/weapon/bow");
    b.twoHanded();
    b.toss(breakage: 50, range: 5);
    b.skill("Archery");
    t = _ItemBuilder.item("Short Bow", Hues.tan, frequency: 0.3, price: 120);
    t.depth(6, to: 60);
    t.ranged("the arrow", heft: 12, damage: 5, range: 8);
    t.toss(damage: 2);
    t.destroy(Elements.fire, chance: 15, fuel: 10);
    t = _ItemBuilder.item("Longbow", Hues.brown, frequency: 0.3, price: 250);
    t.depth(13);
    t.ranged("the arrow", heft: 18, damage: 9, range: 12);
    t.toss(damage: 3);
    t.destroy(Elements.fire, chance: 7, fuel: 13);
    // TODO: Warbow.
    t = _ItemBuilder.item("Crossbow", Hues.lightCoolGray, frequency: 0.3, price: 600);
    t.depth(28);
    t.ranged("the bolt", heft: 24, damage: 14, range: 16);
    t.toss(damage: 4);
    t.destroy(Elements.fire, chance: 4, fuel: 14);
  }
}
