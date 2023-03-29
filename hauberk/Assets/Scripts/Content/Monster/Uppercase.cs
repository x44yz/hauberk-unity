
public partial class Monsters
{
  // TODO: Describe other monsters.
  public static void ancients() { }

  public static void birds()
  {
    var f = _MBaseBuilder.family("B");
    f.groups("bird");
    f.sense(see: 8, hear: 6);
    f.defense(10, "{1} flaps out of the way.");
    f.fly();
    f.count(3, 6);
    var b = _MBaseBuilder.breed("crow", 4, Hues.darkCoolGray, 7, speed: 2, meander: 30);
    b.attack("bite[s]", 5);
    b.drop("treasure", percent: 10);
    _MBaseBuilder.describe("\"What harm can a stupid little crow do?\" you think as it and its murderous friends dive towards your eyes, claws extended.");

    b = _MBaseBuilder.breed("raven", 6, Hues.coolGray, 16, meander: 15);
    b.attack("bite[s]", 5);
    b.attack("claw[s]", 4);
    b.drop("treasure", percent: 10);
    b.flags("protective");
    _MBaseBuilder.describe("It's black eyes gleam with a malevolent intelligence.");
  }

  public static void canids() { }

  public static void greaterDragons() { }

  public static void elementals() { }

  public static void faeFolk()
  {
    // Sprites, pixies, fairies, elves, etc.

    var f = _MBaseBuilder.family("F", speed: 2, meander: 30, flags: "cowardly");
    f.groups("fae");
    f.sense(see: 10, hear: 8);
    f.defense(10, "{1} flits out of the way.");
    f.fly();
    f.preferOpen();
    var b = _MBaseBuilder.breed("forest sprite", 2, Hues.mint, 6);
    b.attack("scratch[es]", 3);
    b.missive(Missive.insult, rate: 4);
    b.sparkBolt(rate: 12, damage: 4);
    b.drop("treasure", percent: 10);
    b.drop("magic", percent: 30);

    b = _MBaseBuilder.breed("house sprite", 5, Hues.lightBlue, 10);
    b.attack("poke[s]", 5);
    b.missive(Missive.insult, rate: 4);
    b.stoneBolt(rate: 10, damage: 4);
    b.teleport(rate: 8, range: 4);
    b.drop("treasure", percent: 10);
    b.drop("magic", percent: 30);

    b = _MBaseBuilder.breed("mischievous sprite", 7, Hues.pink, 24);
    b.attack("poke[s]", 6);
    b.missive(Missive.insult, rate: 4);
    b.windBolt(rate: 10, damage: 8);
    b.teleport(range: 5);
    b.drop("treasure", percent: 10);
    b.drop("magic", percent: 30);

    b = _MBaseBuilder.breed("Tink", 8, Hues.peaGreen, 40, meander: 10);
    b.she();
    b.attack("poke[s]", 8);
    b.missive(Missive.insult, rate: 4);
    b.sparkBolt(rate: 8, damage: 4);
    b.windBolt(rate: 10, damage: 7);
    b.teleport(range: 5);
    b.drop("treasure", count: 2);
    b.drop("magic", count: 3, depthOffset: 3);
    b.flags("unique");

    // TODO: https://en.wikipedia.org/wiki/Puck_(folklore)
  }

  public static void golems()
  {
    // TODO: Animated dolls, poppets, and marionettes.
  }

  public static void hybrids()
  {
    var f = _MBaseBuilder.family("H");
    f.groups("hybrid");
    f.sense(see: 10, hear: 12);

    // TODO: Cause disease when scratched?
    var b = _MBaseBuilder.breed("harpy", 25, Hues.lilac, 50, speed: 2);
    b.fly();
    b.count(2, 5);
    b.attack("bite[s]", 10);
    b.attack("scratch[es]", 15);
    b.howl(verb: "screeches");
    b.missive(Missive.screech);

    b = _MBaseBuilder.breed("griffin", 35, Hues.gold, 200);
    b.attack("bite[s]", 20);
    b.attack("scratch[es]", 15);

    // TODO: https://en.wikipedia.org/wiki/List_of_hybrid_creatures_in_folklore
  }

  public static void insubstantials() { }

  public static void krakens() { }

  public static void lichs() { }

  public static void hydras() { }

  public static void demons() { }

  public static void ogres() { }

  public static void giants() { }

  public static void quest()
  {
    // TODO: Better group?
    var f = _MBaseBuilder.family("Q");
    f.groups("magical");
    var b = _MBaseBuilder.breed("Nameless Unmaker", 100, Hues.purple, 1000, speed: 2);
    b.sense(see: 16, hear: 16);
    b.attack("crushe[s]", 250, Elements.earth);
    b.attack("blast[s]", 200, Elements.lightning);
    b.darkCone(rate: 10, damage: 500);
    b.flags("fearless unique");
    b.openDoors();
    b.drop("item", count: 20, affixChance: 50);
    // TODO: Minions. Moves.
  }

  public static void reptiles()
  {
    var f = _MBaseBuilder.family("R");
    f.groups("herp");
    var b = _MBaseBuilder.breed("frog", 1, Hues.lima, 4, dodge: 30, meander: 30);
    b.sense(see: 6, hear: 4);
    b.swim();
    b.attack("hop[s] on", 2);

    f = _MBaseBuilder.family("R", dodge: 30, meander: 20);
    f.groups("salamander");
    f.sense(see: 6, hear: 5);
    f.preferOpen();
    f.emanate(3);
    b = _MBaseBuilder.breed("juvenile salamander", 7, Hues.pink, 20);
    b.attack("bite[s]", 14, Elements.fire);
    b.fireCone(rate: 16, damage: 20, range: 4);

    b = _MBaseBuilder.breed("salamander", 13, Hues.red, 30);
    b.attack("bite[s]", 18, Elements.fire);
    b.fireCone(rate: 16, damage: 30, range: 5);

    b = _MBaseBuilder.breed("three-headed salamander", 23, Hues.maroon, 90);
    b.attack("bite[s]", 24, Elements.fire);
    b.fireCone(rate: 10, damage: 20, range: 5);
  }

  public static void snakes()
  {
    var f = _MBaseBuilder.family("S", dodge: 30, meander: 30);
    f.groups("snake");
    f.sense(see: 4, hear: 7);
    var b = _MBaseBuilder.breed("water snake", 1, Hues.lima, 9);
    b.attack("bite[s]", 3);

    b = _MBaseBuilder.breed("brown snake", 3, Hues.tan, 25);
    b.attack("bite[s]", 4);

    b = _MBaseBuilder.breed("cave snake", 8, Hues.lightCoolGray, 40);
    b.attack("bite[s]", 10);
  }

  public static void trolls() { }

  public static void majorUndead() { }

  public static void vampires() { }

  public static void wraiths() { }

  public static void xorns() { }

  public static void serpents() { }
}