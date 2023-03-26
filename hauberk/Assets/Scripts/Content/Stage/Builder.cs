using System;
using System.Collections.Generic;
using System.Linq;

public partial class ArchitecturalStyle
{
  public static void _addStyle(string name,
      int start = 1,
      int end = 100,
      double? startFrequency = null,
      double? endFrequency = null,
      string decor = "",
      double? decorDensity = null,
      string monsters = null,
      double? monsterDensity = null,
      double? itemDensity = null,
      System.Func<Architecture> create = null,
      bool? canFill = null)
  {
    monsters ??= "monster";

    var style = new ArchitecturalStyle(name, decor, decorDensity, monsters.Split(' ').ToList(),
        monsterDensity, itemDensity, create,
        canFill: canFill);
    // TODO: Ramp frequencies?
    ArchitecturalStyle.styles.addRanged(style,
        start: start,
        end: end,
        startFrequency: startFrequency,
        endFrequency: endFrequency);
  }

  public static void dungeon()
  {
    _addStyle("dungeon",
        startFrequency: 10.0,
        decor: "dungeon",
        decorDensity: 0.09,
        create: () => new Dungeon());
  }

  public static void catacomb(string monsters,
       double startFrequency, double endFrequency)
  {
    _addStyle("catacomb",
        startFrequency: startFrequency,
        endFrequency: endFrequency,
        decor: "catacomb",
        decorDensity: 0.02,
        monsters: monsters,
        create: () => new Catacomb());
  }

  public static void cavern(string monsters,
       double startFrequency, double endFrequency)
  {
    _addStyle("cavern",
        startFrequency: startFrequency,
        endFrequency: endFrequency,
        decor: "glowing-moss",
        decorDensity: 0.1,
        monsters: monsters,
        create: () => new Cavern());
  }

  public static void lake(string monsters, int start, int end)
  {
    _addStyle("lake",
        start: start,
        end: end,
        decor: "water",
        decorDensity: 0.01,
        monsters: monsters,
        canFill: false,
        monsterDensity: 0.0,
        create: () => new Lake());
  }

  public static void river(string monsters, int start, int end)
  {
    _addStyle("river",
        start: start,
        end: end,
        decor: "water",
        decorDensity: 0.01,
        monsters: monsters,
        monsterDensity: 0.0,
        canFill: false,
        create: () => new River());
  }

  public static void keep(string monsters, int start, int end)
  {
    _addStyle($"{monsters} keep",
        start: start,
        end: end,
        startFrequency: 2.0,
        decor: "keep",
        decorDensity: 0.07,
        monsters: monsters,
        // Keep spawns monsters itself.
        monsterDensity: 0.0,
        itemDensity: 1.5,
        canFill: false,
        create: () => Keep.create(5));
  }

  public static void pit(string monsterGroup, int start, int end)
  {
    _addStyle($"monsterGroup pit",
        start: start,
        end: end,
        startFrequency: 0.2,
        // TODO: Different decor?
        decor: "glowing-moss",
        decorDensity: 0.05,
        canFill: false,
        create: () => new Pit(monsterGroup));
  }
}
