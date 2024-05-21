using System.Collections.Generic;
using UnityTerminal;

public partial class Items
{
  public static void potions()
  {
    // TODO: Max depths for more of these.
    // TODO: Some potions should perform an effect when thrown.

    // Healing.
    var b = _CategoryBuilder.category(CharCode.latinSmallLetterCWithCedilla, stack: 10);
    b.tag("magic/potion/healing");
    b.toss(damage: 1, range: 6, breakage: 100);
    b.destroy(Elements.cold, chance: 20);
    var t = _ItemBuilder.item("Soothing Balm", Hues.pink, price: 10);
    t.depth(2, to: 30);
    t.heal(36);
    t = _ItemBuilder.item("Mending Salve", Hues.red, price: 30);
    t.depth(20, to: 40);
    t.heal(64);
    t = _ItemBuilder.item("Healing Poultice", Hues.maroon, price: 80);
    t.depth(30);
    t.heal(120, curePoison: true);
    t = _ItemBuilder.item("Potion[s] of Amelioration", Hues.violet, price: 220);
    t.depth(60);
    t.heal(200, curePoison: true);
    t = _ItemBuilder.item("Potion[s] of Rejuvenation", Hues.purple, price: 1000);
    t.depth(80);
    t.heal(1000, curePoison: true);
    t = _ItemBuilder.item("Antidote", Hues.peaGreen, price: 20);
    t.depth(2);
    t.heal(0, curePoison: true);

    b = _CategoryBuilder.category(CharCode.latinSmallLetterEWithCircumflex, stack: 10);
    b.tag("magic/potion/resistance");
    b.toss(damage: 1, range: 6, breakage: 100);
    b.destroy(Elements.cold, chance: 20);
    // TODO: Don't need to strictly have every single element here.
    // TODO: Have stronger versions that appear at higher depths.
    t = _ItemBuilder.item("Salve[s] of Heat Resistance", Hues.carrot, frequency: 0.5, price: 50);
    t.depth(5);
    t.resistSalve(Elements.fire);
    // TODO: Make not freezable?
    t = _ItemBuilder.item("Salve[s] of Cold Resistance", Hues.lightBlue, frequency: 0.5, price: 55);
    t.depth(6);
    t.resistSalve(Elements.cold);
    t = _ItemBuilder.item("Salve[s] of Light Resistance", Hues.buttermilk, frequency: 0.5, price: 60);
    t.depth(7);
    t.resistSalve(Elements.light);
    t = _ItemBuilder.item("Salve[s] of Wind Resistance", Hues.lightAqua, frequency: 0.5, price: 65);
    t.depth(8);
    t.resistSalve(Elements.air);
    t = _ItemBuilder.item("Salve[s] of Lightning Resistance", Hues.lilac, frequency: 0.5, price: 70);
    t.depth(9);
    t.resistSalve(Elements.lightning);
    t = _ItemBuilder.item("Salve[s] of Darkness Resistance", Hues.coolGray, frequency: 0.5, price: 75);
    t.depth(10);
    t.resistSalve(Elements.dark);
    t = _ItemBuilder.item("Salve[s] of Earth Resistance", Hues.tan, frequency: 0.5, price: 80);
    t.depth(13);
    t.resistSalve(Elements.earth);
    t = _ItemBuilder.item("Salve[s] of Water Resistance", Hues.darkBlue, frequency: 0.5, price: 85);
    t.depth(16);
    t.resistSalve(Elements.water);
    t = _ItemBuilder.item("Salve[s] of Acid Resistance", Hues.sandal, frequency: 0.5, price: 90);
    t.depth(19);
    t.resistSalve(Elements.acid); // TODO: Better color.
    t = _ItemBuilder.item("Salve[s] of Poison Resistance", Hues.lima, frequency: 0.5, price: 95);
    t.depth(23);
    t.resistSalve(Elements.poison);
    t = _ItemBuilder.item("Salve[s] of Death Resistance", Hues.purple, frequency: 0.5, price: 100);
    t.depth(30);
    t.resistSalve(Elements.spirit);

    // TODO: "Insulation", "the Elements" and other multi-element resistances.

    // Speed.
    b = _CategoryBuilder.category(CharCode.latinSmallLetterEWithDiaeresis, stack: 10);
    b.tag("magic/potion/speed");
    b.toss(damage: 1, range: 6, breakage: 100);
    b.destroy(Elements.cold, chance: 20);
    t = _ItemBuilder.item("Potion[s] of Quickness", Hues.lima, frequency: 0.3, price: 25);
    t.depth(3, to: 30);
    t.haste(1, 40);
    t = _ItemBuilder.item("Potion[s] of Alacrity", Hues.peaGreen, frequency: 0.3, price: 60);
    t.depth(18, to: 50);
    t.haste(2, 60);
    t = _ItemBuilder.item("Potion[s] of Speed", Hues.sherwood, frequency: 0.25, price: 150);
    t.depth(34);
    t.haste(3, 100);

    // dram, draught, elixir, philter

    // TODO: Don't need to strictly have every single element here.
    b = _CategoryBuilder.category(CharCode.latinSmallLetterEWithGrave, stack: 10);
    b.tag("magic/potion/bottled");
    b.toss(damage: 1, range: 8, breakage: 100);
    b.destroy(Elements.cold, chance: 15);
    t = _ItemBuilder.item("Bottled Wind", Hues.lightBlue, frequency: 0.5, price: 100);
    t.depth(4);
    t.flow(Elements.air, "the wind", "blasts", 20, fly: true);
    // TODO: Make not freezable?
    t = _ItemBuilder.item("Bottled Ice", Hues.blue, frequency: 0.5, price: 120);
    t.depth(7);
    t.ball(Elements.cold, "the cold", "freezes", 30);
    t = _ItemBuilder.item("Bottled Fire", Hues.red, frequency: 0.5, price: 140);
    t.depth(11);
    t.flow(Elements.fire, "the fire", "burns", 44, fly: true);
    t = _ItemBuilder.item("Bottled Ocean", Hues.darkBlue, frequency: 0.5, price: 160);
    t.depth(12);
    t.flow(Elements.water, "the water", "drowns", 52);
    t = _ItemBuilder.item("Bottled Earth", Hues.tan, frequency: 0.5, price: 180);
    t.depth(13);
    t.ball(Elements.earth, "the dirt", "crushes", 58);
    t = _ItemBuilder.item("Bottled Lightning", Hues.lilac, frequency: 0.5, price: 200);
    t.depth(16);
    t.ball(Elements.lightning, "the lightning", "shocks", 68);
    t = _ItemBuilder.item("Bottled Acid", Hues.lima, frequency: 0.5, price: 220);
    t.depth(18);
    t.flow(Elements.acid, "the acid", "corrodes", 72);
    t = _ItemBuilder.item("Bottled Poison", Hues.sherwood, frequency: 0.5, price: 240);
    t.depth(22);
    t.flow(Elements.poison, "the poison", "infects", 90, fly: true);
    t = _ItemBuilder.item("Bottled Shadow", Hues.darkCoolGray, frequency: 0.5, price: 260);
    t.depth(28);
    t.ball(Elements.dark, "the darkness", "torments", 120);
    t = _ItemBuilder.item("Bottled Radiance", Hues.buttermilk, frequency: 0.5, price: 280);
    t.depth(34);
    t.ball(Elements.light, "light", "sears", 140);
    t = _ItemBuilder.item("Bottled Spirit", Hues.coolGray, frequency: 0.5, price: 300);
    t.depth(40);
    t.flow(Elements.spirit, "the spirit", "haunts", 160, fly: true);
  }

  public static void scrolls()
  {
    // TODO: Consider adding "complexity" to items. Like heft but for intellect,
    // it's a required intellect level needed to use the item successfully. An
    // item too complex for the user is likely to fail.

    // Teleportation.
    var b = _CategoryBuilder.category(CharCode.latinSmallLetterAWithCircumflex, stack: 20);
    b.tag("magic/scroll/teleportation");
    b.toss(damage: 1, range: 3, breakage: 75);
    b.destroy(Elements.fire, chance: 20, fuel: 5);
    var t = _ItemBuilder.item("Scroll[s] of Sidestepping", Hues.lilac, frequency: 0.5, price: 16);
    t.depth(2);
    t.teleport(8);
    t = _ItemBuilder.item("Scroll[s] of Phasing", Hues.purple, frequency: 0.3, price: 28);
    t.depth(6);
    t.teleport(14);
    t = _ItemBuilder.item("Scroll[s] of Teleportation", Hues.violet, frequency: 0.3, price: 52);
    t.depth(15);
    t.teleport(28);
    t = _ItemBuilder.item("Scroll[s] of Disappearing", Hues.darkBlue, frequency: 0.3, price: 74);
    t.depth(26);
    t.teleport(54);

    // Detection.
    b = _CategoryBuilder.category(CharCode.latinSmallLetterAWithDiaeresis, stack: 20);
    b.tag("magic/scroll/detection");
    b.toss(damage: 1, range: 3, breakage: 75);
    b.destroy(Elements.fire, chance: 20, fuel: 5);
    t = _ItemBuilder.item("Scroll[s] of Find Nearby Escape", Hues.buttermilk, frequency: 0.5, price: 12);
    t.depth(1, to: 10);
    t.detection(new List<DetectType> { DetectType.exit }, range: 20);
    t = _ItemBuilder.item("Scroll[s] of Find Nearby Items", Hues.gold, frequency: 0.5, price: 24);
    t.depth(2, to: 16);
    t.detection(new List<DetectType> { DetectType.item }, range: 20);
    t = _ItemBuilder.item("Scroll[s] of Detect Nearby", Hues.lima, frequency: 0.25, price: 36);
    t.depth(3, to: 24);
    t.detection(new List<DetectType> { DetectType.exit, DetectType.item }, range: 20);

    t = _ItemBuilder.item("Scroll[s] of Locate Escape", Hues.sandal, price: 28);
    t.depth(6);
    t.detection(new List<DetectType> { DetectType.exit });
    t = _ItemBuilder.item("Scroll[s] of Item Detection", Hues.carrot, frequency: 0.5, price: 64);
    t.depth(12);
    t.detection(new List<DetectType> { DetectType.item });
    t = _ItemBuilder.item("Scroll[s] of Detection", Hues.persimmon, frequency: 0.25, price: 124);
    t.depth(18);
    t.detection(new List<DetectType> { DetectType.exit, DetectType.item });

    // Perception.
    // TODO: Different scrolls for different kinds of monsters? (Evil, natural,
    // with brain, invisible, etc.)
    t = _ItemBuilder.item("Scroll[s] of Sense Nearby Monsters", Hues.lightBlue, price: 50);
    t.depth(6, to: 19);
    t.perception(distance: 15);

    t = _ItemBuilder.item("Scroll[s] of Sense Monsters", Hues.aqua, price: 70);
    t.depth(20, to: 39);
    t.perception(distance: 20);

    t = _ItemBuilder.item("Scroll[s] of Perceive Monsters", Hues.blue, price: 100);
    t.depth(40, to: 69);
    t.perception(duration: 50, distance: 30);

    t = _ItemBuilder.item("Scroll[s] of Telepathy", Hues.darkBlue, price: 150);
    t.depth(70, to: 100);
    t.perception(distance: 200);

    // Mapping.
    b = _CategoryBuilder.category(CharCode.latinSmallLetterAWithGrave, stack: 20);
    b.tag("magic/scroll/mapping");
    b.toss(damage: 1, range: 3, breakage: 75);
    b.destroy(Elements.fire, chance: 15, fuel: 5);
    t = _ItemBuilder.item("Adventurer's Map", Hues.sherwood, frequency: 0.25, price: 70);
    t.depth(10, to: 50);
    t.mapping(16);
    t = _ItemBuilder.item("Explorer's Map", Hues.peaGreen, frequency: 0.25, price: 160);
    t.depth(30, to: 70);
    t.mapping(32);
    t = _ItemBuilder.item("Cartographer's Map", Hues.mint, frequency: 0.25, price: 240);
    t.depth(50, to: 90);
    t.mapping(64);
    t = _ItemBuilder.item("Wizard's Map", Hues.aqua, frequency: 0.25, price: 360);
    t.depth(70);
    t.mapping(200, illuminate: true);

    //  CharCode.latinSmallLetterAWithRingAbove // scroll
  }

  public static void spellBooks()
  {
    var b = _CategoryBuilder.category(CharCode.vulgarFractionOneHalf, stack: 3);
    b.tag("magic/book/sorcery");
    b.toss(damage: 1, range: 3, breakage: 25);
    b.destroy(Elements.fire, chance: 5, fuel: 10);
    var t = _ItemBuilder.item("Spellbook \"Elemental Primer\"", Hues.maroon, frequency: 0.05, price: 100);
    t.depth(1);
    t.skills(new List<string>{
                "Sense Items",
                "Flee",
                "Escape",
                "Disappear",
                "Icicle",
                "Brilliant Beam",
                "Windstorm",
                "Fire Barrier",
                "Tidal Wave"
            });

    // TODO: More spell books and reorganize spells.
  }
}
