using System.Collections;
using System.Collections.Generic;
using UnityTerminal;

public partial class Monsters
{
  // TODO: Describe other monsters.
  public static void arachnids()
  {
    // TODO: Should all spiders hide in passages?
    var f = _MBaseBuilder.family("a", flags: "fearless");
    f.groups("spider");
    f.sense(see: 4, hear: 2);
    f.stain(Tiles.spiderweb);
    var b = _MBaseBuilder.breed("brown spider", 5, Hues.tan, 6, dodge: 30, meander: 40);
    b.attack("bite[s]", 5, Elements.poison);
    _MBaseBuilder.describe("Like a large dog, if the dog had eight articulated legs, eight glittering eyes, and wanted nothing more than to kill you.");

    b = _MBaseBuilder.breed("gray spider", 7, Hues.coolGray, 12, dodge: 30, meander: 30);
    b.attack("bite[s]", 5, Elements.poison);

    b = _MBaseBuilder.breed("spiderling", 9, Hues.ash, 14, dodge: 35, meander: 50);
    b.count(2, 7);
    b.attack("bite[s]", 10, Elements.poison);

    b = _MBaseBuilder.breed("giant spider", 12, Hues.darkBlue, 40, meander: 30);
    b.attack("bite[s]", 7, Elements.poison);
  }

  public static void bats()
  {
    var f = _MBaseBuilder.family("b", speed: 1);
    f.groups("bat");
    f.sense(see: 2, hear: 8);
    f.fly();
    f.preferOpen();
    var b = _MBaseBuilder.breed("brown bat", 1, Hues.tan, 3, frequency: 0.5, meander: 50);
    b.defense(20, "{1} flits out of the way.");
    b.count(2, 4);
    b.attack("bite[s]", 3);
    // 
    b = _MBaseBuilder.breed("giant bat", 4, Hues.brown, 24, meander: 30);
    b.attack("bite[s]", 6);
    // 
    b = _MBaseBuilder.breed("cave bat", 6, Hues.lightCoolGray, 30, meander: 40);
    b.defense(20, "{1} flits out of the way.");
    b.count(2, 5);
    b.attack("bite[s]", 6);
  }
  // 
  public static void canines()
  {
    var f = _MBaseBuilder.family("c", dodge: 25, tracking: 20, meander: 25);
    f.groups("canine");
    f.sense(see: 5, hear: 10);
    // 
    var b = _MBaseBuilder.breed("mangy cur", 2, Hues.buttermilk, 11);
    b.count(4);
    b.attack("bite[s]", 4);
    b.howl(range: 6);
    // 
    b = _MBaseBuilder.breed("wild dog", 4, Hues.lightCoolGray, 20);
    b.count(4);
    b.attack("bite[s]", 6);
    b.howl(range: 8);
    // 
    b = _MBaseBuilder.breed("mongrel", 7, Hues.carrot, 28);
    b.count(2, 5);
    b.attack("bite[s]", 8);
    b.howl();
    // 
    b = _MBaseBuilder.breed("wolf", 26, Hues.ash, 60);
    b.count(3, 6);
    b.attack("bite[s]", 12);
    b.howl();
    // 
    b = _MBaseBuilder.breed("varg", 30, Hues.coolGray, 80);
    b.count(2, 6);
    b.attack("bite[s]", 16);
    b.howl();
    // 
    // TODO: Drops.
    b = _MBaseBuilder.breed("Skoll", 36, Hues.gold, 200);
    b.flags("unique");
    b.minion("canine", 5, 9);
    b.attack("bite[s]", 20);
    b.howl();
    // 
    b = _MBaseBuilder.breed("Hati", 40, Hues.blue, 250);
    b.flags("unique");
    b.minion("canine", 5, 9);
    b.attack("bite[s]", 23);
    b.howl();
    // 
    b = _MBaseBuilder.breed("Fenrir", 44, Hues.darkCoolGray, 300);
    b.flags("unique");
    b.minion("canine", 3, 5);
    b.minion("Skoll");
    b.minion("Hati");
    b.attack("bite[s]", 26);
    b.howl();
  }
  // 
  public static void dragons()
  {
    // TODO: Tune. Give more attacks. Tune drops.
    // TODO: Juvenile and elder dragons.
    // TODO: Minions?
    var dragons = new Dictionary<string, List<object>> {
            {"forest", new List<object>{Element.none, Hues.peaGreen, Hues.sherwood}},
            {"brown", new List<object>{Elements.earth, Hues.sandal, Hues.tan}},
            {"blue", new List<object>{Elements.water, Hues.lightBlue, Hues.blue}},
            {"white", new List<object>{Elements.cold, Hues.lightCoolGray, Hues.ash}},
            {"purple", new List<object>{Elements.poison, Hues.lilac, Hues.purple}},
            {"green", new List<object>{Elements.acid, Hues.lima, Hues.olive}},
            {"silver", new List<object>{Elements.lightning, Hues.lightAqua, Hues.lightBlue}},
            {"red", new List<object>{Elements.fire, Hues.pink, Hues.red}},
            {"gold", new List<object>{Elements.light, Hues.buttermilk, Hues.gold}},
            {"black", new List<object>{Elements.dark, Hues.coolGray, Hues.darkCoolGray}},
            {"ethereal", new List<object>{Elements.spirit, Hues.aqua, Hues.darkBlue}}
        };
    // 
    var i = 0;
    foreach (var kv in dragons)
    {
      var name = kv.Key;
      var data = kv.Value;

      var element = data[0] as Element;
      var youngColor = (Color)data[1];
      var adultColor = (Color)data[1];
      // 
      var f = _MBaseBuilder.family("d");
      f.groups("dragon");
      f.sense(see: 12, hear: 8);
      f.defense(10, "{2} [is|are] deflected by its scales.");
      f.preferOpen();
      // 
      var dragon = _MBaseBuilder.breed($"juvenile {name} dragon", 46 + i * 2, youngColor, 150 + i * 20);
      dragon.attack("bite[s]", 20 + i * 2);
      dragon.attack("claw[s]", 15 + i);
      dragon.drop("treasure", count: 2 + i / 2);
      dragon.drop("magic");
      dragon.drop("equipment");
      // 
      if (element != Element.none)
      {
        dragon.cone(element, rate: 11, damage: 40 + i * 6, range: 5);
      }
      // 
      f = _MBaseBuilder.family("d");
      f.groups("dragon");
      f.sense(see: 16, hear: 10);
      f.defense(20, "{2} [is|are] deflected by its scales.");
      f.preferOpen();
      // 
      dragon = _MBaseBuilder.breed($"{name} dragon", 50 + i * 2, adultColor, 350 + i * 50);
      dragon.attack("bite[s]", 30 + i * 2);
      dragon.attack("claw[s]", 25 + i);
      dragon.drop("treasure", count: 5 + i / 2);
      dragon.drop("magic", count: 3 + i / 3);
      dragon.drop("equipment", count: 2 + i / 3);
      // 
      if (element != Element.none)
      {
        dragon.cone(element, rate: 8, damage: 70 + i * 8);
      }
      // 
      i++;
    };
  }
  // 
  public static void eyes()
  {
    var f = _MBaseBuilder.family("e", flags: "immobile");
    f.groups("eye");
    f.sense(see: 16, hear: 1);
    f.defense(10, "{1} blinks out of the way.");
    f.fly();
    f.preferOpen();
    // 
    var b = _MBaseBuilder.breed("lazy eye", 5, Hues.lightBlue, 20);
    b.attack("stare[s] at", 8);
    b.sparkBolt(rate: 5, damage: 12, range: 8);
    // 
    b = _MBaseBuilder.breed("mad eye", 9, Hues.pink, 40);
    b.attack("stare[s] at", 8);
    b.windBolt(rate: 6, damage: 15);
    // 
    b = _MBaseBuilder.breed("floating eye", 15, Hues.buttermilk, 60);
    b.attack("stare[s] at", 10);
    b.sparkBolt(rate: 4, damage: 24);
    b.teleport(range: 7);
    // ;
    b = _MBaseBuilder.breed("baleful eye", 20, Hues.carrot, 80);
    b.attack("gaze[s] into", 12);
    b.fireBolt(rate: 4, damage: 20);
    b.waterBolt(rate: 4, damage: 20);
    b.teleport(range: 9);
    // 
    b = _MBaseBuilder.breed("malevolent eye", 30, Hues.red, 120);
    b.attack("gaze[s] into", 20);
    b.lightBolt(rate: 4, damage: 20);
    b.darkBolt(rate: 4, damage: 20);
    b.fireCone(rate: 7, damage: 30);
    b.teleport(range: 9);
    // 
    b = _MBaseBuilder.breed("murderous eye", 40, Hues.maroon, 180);
    b.attack("gaze[s] into", 30);
    b.acidBolt(rate: 7, damage: 40);
    b.stoneBolt(rate: 7, damage: 40);
    b.iceCone(rate: 7, damage: 30);
    b.teleport(range: 9);
    // 
    b = _MBaseBuilder.breed("watcher", 60, Hues.lightCoolGray, 300);
    b.attack("see[s]", 50);
    b.lightBolt(rate: 7, damage: 40);
    b.lightCone(rate: 7, damage: 30);
    b.darkBolt(rate: 7, damage: 50);
    b.darkCone(rate: 7, damage: 40);
    // 
    // beholder, undead beholder, rotting beholder
  }
  // 
  public static void felines()
  {
    var f = _MBaseBuilder.family("f");
    f.sense(see: 10, hear: 8);
    f.groups("feline");
    var b = _MBaseBuilder.breed("stray cat", 1, Hues.gold, 11, speed: 1, meander: 30);
    b.attack("bite[s]", 5);
    b.attack("scratch[es]", 4);
  }
  // 
  public static void goblins()
  {
    var f = _MBaseBuilder.family("g", meander: 10);
    f.sense(see: 8, hear: 4);
    f.groups("goblin");
    f.openDoors();
    var b = _MBaseBuilder.breed("goblin peon", 4, Hues.sandal, 30, meander: 20);
    b.count(4);
    b.attack("stab[s]", 8);
    b.missive(Missive.insult, rate: 8);
    b.drop("treasure", percent: 20);
    b.drop("spear", percent: 5);
    b.drop("healing", percent: 10);
    // 
    b = _MBaseBuilder.breed("goblin archer", 6, Hues.peaGreen, 36);
    b.count(2);
    b.minion("goblin", 0, 3);
    b.attack("stab[s]", 4);
    b.arrow(rate: 3, damage: 8);
    b.drop("treasure", percent: 30);
    b.drop("bow", percent: 10);
    b.drop("dagger", percent: 5);
    b.drop("healing", percent: 10);
    // 
    b = _MBaseBuilder.breed("goblin fighter", 6, Hues.tan, 58);
    b.count(2);
    b.minion("goblin", 1, 4);
    b.attack("stab[s]", 12);
    b.drop("treasure", percent: 20);
    b.drop("spear", percent: 10);
    b.drop("armor", percent: 10);
    b.drop("resistance", percent: 5);
    b.drop("healing", percent: 10);
    // 
    b = _MBaseBuilder.breed("goblin warrior", 8, Hues.lightCoolGray, 68);
    b.count(2);
    b.minion("goblin", 1, 5);
    b.attack("stab[s]", 16);
    b.drop("treasure", percent: 25);
    b.drop("axe", percent: 10);
    b.drop("armor", percent: 10);
    b.drop("resistance", percent: 5);
    b.drop("healing", percent: 10);
    b.flags("protective");
    // 
    b = _MBaseBuilder.breed("goblin mage", 9, Hues.darkBlue, 50);
    b.minion("goblin", 1, 4);
    b.attack("whip[s]", 7);
    b.fireBolt(rate: 12, damage: 12);
    b.sparkBolt(rate: 12, damage: 16);
    b.drop("treasure", percent: 20);
    b.drop("robe", percent: 10);
    b.drop("magic", percent: 30);
    // 
    b = _MBaseBuilder.breed("goblin ranger", 12, Hues.sherwood, 60);
    b.minion("goblin", 0, 5);
    b.attack("stab[s]", 10);
    b.arrow(rate: 3, damage: 12);
    b.drop("treasure", percent: 20);
    b.drop("bow", percent: 15);
    b.drop("armor", percent: 10);
    b.drop("magic", percent: 20);
    // 
    b = _MBaseBuilder.breed("Erlkonig, the Goblin Prince", 14, Hues.darkCoolGray, 120);
    b.he();
    b.minion("goblin", 4, 8);
    b.attack("hit[s]", 10);
    b.attack("slash[es]", 14);
    b.darkBolt(rate: 20, damage: 20);
    b.drop("treasure", count: 3);
    b.drop("equipment", count: 2, depthOffset: 8, affixChance: 30);
    b.drop("magic", count: 3, depthOffset: 4);
    b.flags("protective unique");
    // 
    // TODO: Hobgoblins, bugbears, bogill.
    // TODO: https://en.wikipedia.org/wiki/Moss_people
  }
  // 
  public static void humanoids() { }
  // 
  public static void insects()
  {
    var f = _MBaseBuilder.family("i", tracking: 3, meander: 40, flags: "fearless");
    f.groups("bug");
    f.sense(see: 5, hear: 2);
    // TODO: Spawn as eggs which can hatch into cockroaches?
    var b = _MBaseBuilder.breed("giant cockroach[es]", 1, Hues.brown, 1, frequency: 0.4);
    b.count(2, 5);
    b.preferCorner();
    b.attack("crawl[s] on", 2);
    b.spawn(rate: 6);
    _MBaseBuilder.describe("It's not quite as easy to squash one of these when it's as long as your arm.");
    // 
    b = _MBaseBuilder.breed("giant centipede", 3, Hues.red, 14, speed: 2, meander: 20);
    b.attack("crawl[s] on", 4);
    b.attack("bite[s]", 8);
    // 
    f = _MBaseBuilder.family("i", tracking: 3, meander: 40, flags: "fearless");
    f.groups("fly");
    f.sense(see: 5, hear: 2);
    b = _MBaseBuilder.breed("firefly", 8, Hues.carrot, 6, speed: 1, meander: 70);
    b.count(3, 8);
    b.attack("bite[s]", 12, Elements.fire);
  }
  // 
  public static void jellies()
  {
    var f = _MBaseBuilder.family("j", frequency: 0.7, speed: -1, meander: 30, flags: "fearless");
    f.groups("jelly");
    f.sense(see: 3, hear: 1);
    f.preferWall();
    f.count(4);
    var b = _MBaseBuilder.breed("green jelly", 1, Hues.lima, 5);
    b.stain(Tiles.greenJellyStain);
    b.attack("crawl[s] on", 3);
    // TODO: More elements.
    // 
    f = _MBaseBuilder.family("j", frequency: 0.6, flags: "fearless immobile");
    f.groups("jelly");
    f.sense(see: 2, hear: 1);
    f.preferCorner();
    f.count(4);
    b = _MBaseBuilder.breed("green slime", 2, Hues.peaGreen, 10);
    b.stain(Tiles.greenJellyStain);
    b.attack("crawl[s] on", 4);
    b.spawn(rate: 4);
    // 
    b = _MBaseBuilder.breed("frosty slime", 4, Hues.ash, 14);
    b.stain(Tiles.whiteJellyStain);
    b.attack("crawl[s] on", 5, Elements.cold);
    b.spawn(rate: 4);
    // 
    b = _MBaseBuilder.breed("mud slime", 6, Hues.tan, 20);
    b.stain(Tiles.brownJellyStain);
    b.attack("crawl[s] on", 8, Elements.earth);
    b.spawn(rate: 4);
    // 
    b = _MBaseBuilder.breed("smoking slime", 15, Hues.red, 30);
    b.emanate(4);
    b.stain(Tiles.redJellyStain);
    b.attack("crawl[s] on", 10, Elements.fire);
    b.spawn(rate: 4);
    // 
    b = _MBaseBuilder.breed("sparkling slime", 20, Hues.purple, 40);
    b.emanate(3);
    b.stain(Tiles.violetJellyStain);
    b.attack("crawl[s] on", 12, Elements.lightning);
    b.spawn(rate: 4);
    // 
    // TODO: Erode nearby walls?
    b = _MBaseBuilder.breed("caustic slime", 25, Hues.mint, 50);
    b.stain(Tiles.greenJellyStain);
    b.attack("crawl[s] on", 13, Elements.acid);
    b.spawn(rate: 4);
    // 
    b = _MBaseBuilder.breed("virulent slime", 35, Hues.sherwood, 60);
    b.stain(Tiles.greenJellyStain);
    b.attack("crawl[s] on", 14, Elements.poison);
    b.spawn(rate: 4);
    // 
    // TODO: Fly?
    b = _MBaseBuilder.breed("ectoplasm", 45, Hues.darkCoolGray, 40);
    b.stain(Tiles.grayJellyStain);
    b.attack("crawl[s] on", 15, Elements.spirit);
    b.spawn(rate: 4);
  }
  // 
  public static void kobolds()
  {
    var f = _MBaseBuilder.family("k", meander: 15, flags: "cowardly");
    f.groups("kobold");
    f.sense(see: 10, hear: 4);
    var b = _MBaseBuilder.breed("scurrilous imp", 1, Hues.pink, 12, meander: 20);
    b.count(2);
    b.attack("club[s]", 4);
    b.missive(Missive.insult);
    b.haste();
    b.drop("treasure", percent: 20);
    b.drop("club", percent: 10);
    b.drop("speed", percent: 20);
    // 
    b = _MBaseBuilder.breed("vexing imp", 2, Hues.purple, 16);
    b.count(2);
    b.minion("kobold", 0, 1);
    b.attack("scratch[es]", 4);
    b.missive(Missive.insult);
    b.sparkBolt(rate: 5, damage: 6);
    b.drop("treasure", percent: 25);
    b.drop("teleportation", percent: 20);
    // 
    _MBaseBuilder.family("k", meander: 20).groups("kobold");
    b = _MBaseBuilder.breed("kobold", 3, Hues.red, 20);
    b.count(3);
    b.minion("canine", 0, 3);
    b.attack("poke[s]", 4);
    b.teleport(range: 6);
    b.drop("treasure", percent: 25);
    b.drop("equipment", percent: 10);
    b.drop("magic", percent: 20);
    // 
    b = _MBaseBuilder.breed("kobold shaman", 4, Hues.darkBlue, 20);
    b.count(2);
    b.minion("canine", 0, 3);
    b.attack("hit[s]", 4);
    b.waterBolt(rate: 10, damage: 8);
    b.drop("treasure", percent: 25);
    b.drop("robe", percent: 10);
    b.drop("magic", percent: 20);
    // 
    b = _MBaseBuilder.breed("kobold trickster", 5, Hues.gold, 24);
    b.attack("hit[s]", 5);
    b.missive(Missive.insult);
    b.sparkBolt(rate: 5, damage: 8);
    b.teleport(rate: 7, range: 6);
    b.haste(rate: 7);
    b.drop("treasure", percent: 35);
    b.drop("magic", percent: 20);
    // 
    b = _MBaseBuilder.breed("kobold priest", 6, Hues.blue, 30);
    b.count(2);
    b.minion("kobold", 1, 3);
    b.attack("club[s]", 6);
    b.heal(rate: 15, amount: 10);
    b.haste(rate: 7);
    b.drop("treasure", percent: 20);
    b.drop("club", percent: 10);
    b.drop("robe", percent: 10);
    b.drop("magic", percent: 30);
    // 
    b = _MBaseBuilder.breed("imp incanter", 7, Hues.lilac, 33);
    b.count(2);
    b.minion("kobold", 1, 3);
    b.minion("canine", 0, 3);
    b.attack("scratch[es]", 4);
    b.missive(Missive.insult, rate: 6);
    b.sparkBolt(rate: 5, damage: 10);
    b.drop("treasure", percent: 30);
    b.drop("robe", percent: 10);
    b.drop("magic", percent: 35);
    b.flags("cowardly");
    // 
    b = _MBaseBuilder.breed("imp warlock", 8, Hues.violet, 46);
    b.minion("kobold", 2, 5);
    b.minion("canine", 0, 3);
    b.attack("stab[s]", 5);
    b.iceBolt(rate: 8, damage: 12);
    b.sparkBolt(rate: 8, damage: 12);
    b.drop("treasure", percent: 30);
    b.drop("staff", percent: 20);
    b.drop("robe", percent: 10);
    b.drop("magic", percent: 30);
    // 
    b = _MBaseBuilder.breed("Feng", 10, Hues.carrot, 80, speed: 1, meander: 10);
    b.he();
    b.minion("kobold", 4, 10);
    b.minion("canine", 1, 3);
    b.attack("stab[s]", 5);
    b.missive(Missive.insult, rate: 7);
    b.teleport(rate: 5, range: 6);
    b.teleport(rate: 50, range: 30);
    b.lightningCone(rate: 8, damage: 12);
    b.drop("treasure", count: 3, depthOffset: 5);
    b.drop("spear", percent: 20, depthOffset: 5, affixChance: 20);
    b.drop("armor", percent: 30, depthOffset: 5, affixChance: 10);
    b.drop("magic", count: 2, depthOffset: 5);
    b.flags("unique");
    // 
    // homonculous
  }
  // 
  public static void lizardMen()
  {
    // troglodyte
    // 
    var f = _MBaseBuilder.family("l", meander: 10, flags: "fearless");
    f.groups("saurian");
    f.sense(see: 10, hear: 5);
    f.defense(5, "{2} [is|are] deflected by its scales.");
    var b = _MBaseBuilder.breed("lizard guard", 11, Hues.gold, 26);
    b.attack("claw[s]", 8);
    b.attack("bite[s]", 10);
    b.drop("treasure", percent: 30);
    b.drop("armor", percent: 10);
    b.drop("spear", percent: 10);
    // 
    b = _MBaseBuilder.breed("lizard protector", 15, Hues.lima, 30);
    b.minion("saurian", 0, 2);
    b.attack("claw[s]", 10);
    b.attack("bite[s]", 14);
    b.drop("treasure", percent: 30);
    b.drop("armor", percent: 10);
    b.drop("spear", percent: 10);
    // 
    b = _MBaseBuilder.breed("armored lizard", 17, Hues.lightCoolGray, 38);
    b.minion("saurian", 0, 2);
    b.attack("claw[s]", 10);
    b.attack("bite[s]", 15);
    b.drop("treasure", percent: 30);
    b.drop("armor", percent: 20);
    b.drop("spear", percent: 10);
    // 
    b = _MBaseBuilder.breed("scaled guardian", 19, Hues.darkCoolGray, 50);
    b.minion("saurian", 0, 3);
    b.minion("salamander", 0, 2);
    b.attack("claw[s]", 10);
    b.attack("bite[s]", 15);
    b.drop("treasure", percent: 40);
    b.drop("equipment", percent: 10);
    // 
    b = _MBaseBuilder.breed("saurian", 21, Hues.carrot, 64);
    b.minion("saurian", 1, 4);
    b.minion("salamander", 0, 2);
    b.attack("claw[s]", 12);
    b.attack("bite[s]", 17);
    b.drop("treasure", percent: 50);
    b.drop("equipment", percent: 10);
  }
  // 
  public static void mushrooms() { }
  // 
  public static void nagas()
  {
    // TODO: https://en.wikipedia.org/wiki/Nagaraja
  }
  // 
  public static void orcs()
  {
    var f = _MBaseBuilder.family("o", meander: 10);
    f.sense(see: 7, hear: 6);
    f.groups("orc");
    f.openDoors();
    f.flags("protective");
    var b = _MBaseBuilder.breed("orc", 28, Hues.carrot, 100);
    b.count(3, 6);
    b.attack("stab[s]", 12);
    b.drop("treasure", percent: 20);
    b.drop("equipment", percent: 5);
    b.drop("spear", percent: 5);
    // 
    b = _MBaseBuilder.breed("orc brute", 29, Hues.mint, 120);
    b.count(1);
    b.minion("orc", 2, 5);
    b.attack("bash[es]", 16);
    b.drop("treasure", percent: 20);
    b.drop("club", percent: 10);
    b.drop("armor", percent: 10);
    // 
    b = _MBaseBuilder.breed("orc soldier", 30, Hues.lightCoolGray, 140);
    b.count(4, 6);
    b.minion("orcus", 1, 5);
    b.attack("stab[s]", 20);
    b.drop("treasure", percent: 25);
    b.drop("axe", percent: 10);
    b.drop("armor", percent: 10);
    // 
    b = _MBaseBuilder.breed("orc chieftain", 31, Hues.red, 180);
    b.minion("orcus", 2, 10);
    b.attack("stab[s]", 10);
    b.drop("treasure", count: 2, percent: 40);
    b.drop("equipment", percent: 20);
    b.drop("item", percent: 20);
    // 
    // TODO: Uniques. Some kind of magic-user.
  }
  // 
  public static void people()
  {
    var f = _MBaseBuilder.family("p", tracking: 14, meander: 10);
    f.groups("human");
    f.sense(see: 10, hear: 5);
    f.openDoors();
    f.emanate(2);
    var b = _MBaseBuilder.breed("Harold the Misfortunate", 1, Hues.lilac, 30);
    b.he();
    b.attack("hit[s]", 3);
    b.missive(Missive.clumsy);
    b.drop("treasure", percent: 80);
    b.drop("weapon", percent: 20, depthOffset: 4);
    b.drop("armor", percent: 30, depthOffset: 4);
    b.drop("magic", percent: 40, depthOffset: 4);
    b.flags("unique");
    // 
    b = _MBaseBuilder.breed("hapless adventurer", 1, Hues.buttermilk, 14, dodge: 15, meander: 30);
    b.attack("hit[s]", 3);
    b.missive(Missive.clumsy, rate: 12);
    b.drop("treasure", percent: 15);
    b.drop("weapon", percent: 10);
    b.drop("armor", percent: 15);
    b.drop("magic", percent: 20);
    b.flags("cowardly");
    // 
    b = _MBaseBuilder.breed("simpering knave", 2, Hues.carrot, 17);
    b.attack("hit[s]", 2);
    b.attack("stab[s]", 4);
    b.drop("treasure", percent: 20);
    b.drop("whip", percent: 10);
    b.drop("armor", percent: 15);
    b.drop("magic", percent: 20);
    b.flags("cowardly");
    // 
    b = _MBaseBuilder.breed("decrepit mage", 3, Hues.purple, 20, meander: 30);
    b.attack("hit[s]", 2);
    b.sparkBolt(rate: 10, damage: 8);
    b.drop("treasure", percent: 15);
    b.drop("magic", percent: 30);
    b.drop("dagger", percent: 5);
    b.drop("staff", percent: 5);
    b.drop("robe", percent: 10);
    b.drop("boots", percent: 5);
    // 
    b = _MBaseBuilder.breed("unlucky ranger", 5, Hues.peaGreen, 30, dodge: 25, meander: 20);
    b.attack("slash[es]", 2);
    b.arrow(rate: 4, damage: 2);
    b.missive(Missive.clumsy, rate: 10);
    b.drop("treasure", percent: 20);
    b.drop("potion", percent: 20);
    b.drop("bow", percent: 10);
    b.drop("body", percent: 20);
    // 
    b = _MBaseBuilder.breed("drunken priest", 5, Hues.blue, 34, meander: 40);
    b.attack("hit[s]", 8);
    b.heal(rate: 15, amount: 8);
    b.missive(Missive.clumsy);
    b.drop("treasure", percent: 35);
    b.drop("scroll", percent: 20);
    b.drop("club", percent: 10);
    b.drop("robe", percent: 10);
    b.flags("fearless");
  }
  // 
  public static void quadrupeds() { }
  // 
  public static void rodents()
  {
    var f = _MBaseBuilder.family("r", dodge: 30, meander: 30);
    f.groups("rodent");
    f.sense(see: 4, hear: 6);
    f.preferWall();
    var b = _MBaseBuilder.breed("[mouse|mice]", 1, Hues.sandal, 2, frequency: 0.7);
    b.count(2, 5);
    b.attack("bite[s]", 3);
    b.attack("scratch[es]", 2);
    // 
    b = _MBaseBuilder.breed("sewer rat", 2, Hues.coolGray, 8, meander: 20);
    b.count(1, 4);
    b.attack("bite[s]", 4);
    b.attack("scratch[es]", 3);
    // 
    b = _MBaseBuilder.breed("sickly rat", 3, Hues.peaGreen, 10);
    b.attack("bite[s]", 8, Elements.poison);
    b.attack("scratch[es]", 4);
    // 
    b = _MBaseBuilder.breed("plague rat", 6, Hues.lima, 20);
    b.count(1, 4);
    b.attack("bite[s]", 15, Elements.poison);
    b.attack("scratch[es]", 8);
    // 
    b = _MBaseBuilder.breed("giant rat", 8, Hues.carrot, 40);
    b.attack("bite[s]", 12);
    b.attack("scratch[es]", 8);
    // 
    b = _MBaseBuilder.breed("The Rat King", 8, Hues.maroon, 120);
    b.he();
    b.attack("bite[s]", 16);
    b.attack("scratch[es]", 10);
    b.minion("rodent", 8, 16);
    b.drop("treasure", count: 3);
    b.drop("item", percent: 50, depthOffset: 10, affixChance: 10);
    b.flags("unique");
  }
  // 
  public static void slugs()
  {
    var f = _MBaseBuilder.family("s", tracking: 2, flags: "fearless", speed: -3, dodge: 5, meander: 30);
    f.groups("slug");
    f.sense(see: 3, hear: 1);
    _MBaseBuilder.breed("giant slug", 3, Hues.olive, 20).attack("crawl[s] on", 8);
    // 
    var b = _MBaseBuilder.breed("suppurating slug", 6, Hues.lima, 50);
    b.attack("crawl[s] on", 12, Elements.poison);
    // 
    // TODO: Leave a trail.
    _MBaseBuilder.breed("acidic slug", 9, Hues.olive, 70).attack("crawl[s] on", 16, Elements.acid);
  }
  // 
  public static void troglodytes() { }
  // 
  public static void minorUndead() { }
  // 
  public static void vines()
  {
    var f = _MBaseBuilder.family("v", flags: "fearless immobile");
    f.groups("vine");
    f.sense(see: 10, hear: 10);
    _MBaseBuilder.breed("choker", 16, Hues.peaGreen, 40).attack("strangle", 12);
    // TODO: Touch to confuse?
    var b = _MBaseBuilder.breed("nightshade", 19, Hues.lilac, 50);
    b.whip(rate: 3, damage: 10);
    b.attack("touch[es]", 12, Elements.poison);
    b = _MBaseBuilder.breed("creeper", 22, Hues.lima, 60);
    b.spawn(preferStraight: true);
    b.whip(rate: 3, damage: 10);
    b.attack("strangle", 8);
    _MBaseBuilder.breed("strangler", 26, Hues.sherwood, 80).attack("strangle", 14);
  }
  // 
  public static void worms()
  {
    var f = _MBaseBuilder.family("w", dodge: 15, meander: 40, flags: "fearless");
    f.groups("worm");
    f.sense(see: 2, hear: 3);
    var b = _MBaseBuilder.breed("blood worm", 1, Hues.maroon, 4, frequency: 0.5);
    b.count(2, 5);
    b.attack("crawl[s] on", 5);
    // 
    b = _MBaseBuilder.breed("fire worm", 10, Hues.carrot, 6);
    b.count(2, 6);
    b.preferWall();
    b.attack("crawl[s] on", 5, Elements.fire);
    // 
    _MBaseBuilder.family("w", dodge: 10, meander: 30, flags: "fearless").groups("worm");
    _MBaseBuilder.breed("giant earthworm", 3, Hues.pink, 20, speed: -2).attack("crawl[s] on", 5);
    // 
    b = _MBaseBuilder.breed("giant cave worm", 7, Hues.sandal, 80, speed: -2);
    b.attack("crawl[s] on", 12, Elements.acid);
  }
  // 
  public static void skeletons()
  {
    var f = _MBaseBuilder.family("x", meander: 30);
    f.groups("skeleton");
    f.sense(see: 4, hear: 4);
    // TODO: Special room/trap where these get spawned and come up from the
    // ground?
    var b = _MBaseBuilder.breed("bony hand", 3, Hues.coolGray, 16, frequency: 3.0, meander: 40, speed: -1);
    b.attack("claw[s]", 6);
    // 
    b = _MBaseBuilder.breed("bony arm", 4, Hues.lightCoolGray, 22, frequency: 4.0, meander: 40);
    b.attack("claw[s]", 8);
    // 
    b = _MBaseBuilder.breed("severed skull", 7, Hues.sandal, 28, frequency: 3.0, meander: 40, speed: -2);
    b.attack("bite[s]", 10);
    // 
    b = _MBaseBuilder.breed("decapitated skeleton", 10, Hues.buttermilk, 38, frequency: 4.0, meander: 60);
    b.sense(see: 0, hear: 0);
    b.openDoors();
    b.attack("claw[s]", 7);
    b.drop("treasure", percent: 30);
    b.drop("weapon", percent: 10);
    b.drop("armor", percent: 10);
    // 
    b = _MBaseBuilder.breed("armless skeleton", 12, Hues.mint, 40, frequency: 4.0);
    b.attack("bite[s]", 9);
    b.attack("kick[s]", 7);
    b.drop("treasure", percent: 30);
    b.drop("armor", percent: 10);
    // 
    b = _MBaseBuilder.breed("one-armed skeleton", 13, Hues.lima, 46, frequency: 5.0);
    b.openDoors();
    b.attack("claw[s]", 7);
    b.amputate("armless skeleton", "bony arm", "{1}'s arm falls off!");
    b.amputate("armless skeleton", "bony hand", "{1}'s hand falls off!");
    b.drop("treasure", percent: 30);
    b.drop("weapon", percent: 5);
    b.drop("armor", percent: 10);
    // 
    b = _MBaseBuilder.breed("skeleton", 15, Hues.ash, 50, frequency: 6.0);
    b.openDoors();
    b.attack("claw[s]", 7);
    b.attack("bite[s]", 9);
    b.amputate("decapitated skeleton", "severed skull", "{1}'s head pops off!");
    b.amputate("one-armed skeleton", "bony arm", "{1}'s arm falls off!");
    b.amputate("one-armed skeleton", "bony hand", "{1}'s hand falls off!");
    b.drop("treasure", percent: 40);
    b.drop("weapon", percent: 10);
    b.drop("armor", percent: 10);
    // 
    b = _MBaseBuilder.breed("skeleton warrior", 17, Hues.pink, 70, frequency: 6.0);
    b.openDoors();
    b.attack("slash[es]", 13);
    b.attack("stab[s]", 10);
    b.amputate("decapitated skeleton", "severed skull", "{1}'s head pops off!");
    b.amputate("one-armed skeleton", "bony arm", "{1}'s arm falls off!");
    b.amputate("one-armed skeleton", "bony hand", "{1}'s hand falls off!");
    b.drop("treasure", percent: 50);
    b.drop("weapon", percent: 20);
    b.drop("armor", percent: 15);
    // 
    b = _MBaseBuilder.breed("robed skeleton", 19, Hues.lilac, 70, frequency: 4.0);
    b.openDoors();
    b.attack("slash[es]", 13);
    b.attack("stab[s]", 10);
    b.lightningBolt(rate: 8, damage: 15);
    b.amputate("decapitated skeleton", "severed skull", "{1}'s head pops off!");
    b.amputate("one-armed skeleton", "bony arm", "{1}'s arm falls off!");
    b.amputate("one-armed skeleton", "bony hand", "{1}'s hand falls off!");
    b.drop("treasure", percent: 50);
    b.drop("magic", percent: 20);
    b.drop("armor", percent: 10);
    // 
    // TODO: Stronger skeletons.
  }
  // 
  public static void zombies() { }
}
