using System;
using System.Collections.Generic;
using System.Linq;

void _addStyle(string name,
    {int start = 1,
    int end = 100,
    double? startFrequency,
    double? endFrequency,
    required string decor,
    double? decorDensity,
    string? monsters,
    double? monsterDensity,
    double? itemDensity,
    required Architecture Function() create,
    bool? canFill}) {
  monsters ??= "monster";

  var style = ArchitecturalStyle(name, decor, decorDensity, monsters.split(" "),
      monsterDensity, itemDensity, create,
      canFill: canFill);
  // TODO: Ramp frequencies?
  ArchitecturalStyle.styles.addRanged(style,
      start: start,
      end: end,
      startFrequency: startFrequency,
      endFrequency: endFrequency);
}

void dungeon() {
  _addStyle("dungeon",
      startFrequency: 10.0,
      decor: "dungeon",
      decorDensity: 0.09,
      create: () => Dungeon());
}

void catacomb(string monsters,
    {required double startFrequency, required double endFrequency}) {
  _addStyle("catacomb",
      startFrequency: startFrequency,
      endFrequency: endFrequency,
      decor: "catacomb",
      decorDensity: 0.02,
      monsters: monsters,
      create: () => Catacomb());
}

void cavern(string monsters,
    {required double startFrequency, required double endFrequency}) {
  _addStyle("cavern",
      startFrequency: startFrequency,
      endFrequency: endFrequency,
      decor: "glowing-moss",
      decorDensity: 0.1,
      monsters: monsters,
      create: () => Cavern());
}

void lake(string monsters, {required int start, required int end}) {
  _addStyle("lake",
      start: start,
      end: end,
      decor: "water",
      decorDensity: 0.01,
      monsters: monsters,
      canFill: false,
      monsterDensity: 0.0,
      create: () => Lake());
}

void river(string monsters, {required int start, required int end}) {
  _addStyle("river",
      start: start,
      end: end,
      decor: "water",
      decorDensity: 0.01,
      monsters: monsters,
      monsterDensity: 0.0,
      canFill: false,
      create: () => River());
}

void keep(string monsters, {required int start, required int end}) {
  _addStyle("$monsters keep",
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
      create: () => Keep(5));
}

void pit(string monsterGroup, {required int start, required int end}) {
  _addStyle("$monsterGroup pit",
      start: start,
      end: end,
      startFrequency: 0.2,
      // TODO: Different decor?
      decor: "glowing-moss",
      decorDensity: 0.05,
      canFill: false,
      create: () => Pit(monsterGroup));
}