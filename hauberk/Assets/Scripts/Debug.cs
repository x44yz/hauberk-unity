using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using num = System.Double;

/// A class for storing debugging information.
///
/// Unlike the rest of the engine, this is static state to make it easier for
/// the engine to punch debug info all the way to where the UI can get it. It
/// should not be used outside of a debugging scenario.
class Debug {
  public static bool enabled = true;

  /// If true, all monsters are rendered, regardless of in-game visibility.
  static bool showAllMonsters = false;

  /// If true, monster alertness is rendered.
  static bool showMonsterAlertness = false;

  static bool showHeroVolume = false;

  /// The current density map being used.
  ///
  /// Typed as Object? so that this library isn't coupled to the UI.
  static Object? densityMap;

  static public Map<Monster, _MonsterLog> _monsters = {};

  /// The current game screen.
  ///
  /// Typed as Object so that this library isn't coupled to the UI.
  static object? _gameScreen;
  static object? gameScreen => _gameScreen;

  static void bindGameScreen(Object? screen) {
    _gameScreen = screen;
    _monsters.clear();
  }

  /// Appends [message] to the debug log for [monster].
  public static void monsterLog(Monster monster, string message) {
    if (!enabled) return;

    var monsterLog = _monsters.putIfAbsent(monster, () => _MonsterLog(monster));
    monsterLog.add(message);
  }

  /// Appends a new [value] for [stat] for [monster].
  ///
  /// The value should range from 0.0 to 1.0. If there is a descriptive [reason]
  /// for the value, that can be provided too.
  public static void monsterStat(Monster monster, string stat, num value,
      string reason = null) {
    if (!enabled) return;

    var monsterLog = _monsters.putIfAbsent(monster, () => _MonsterLog(monster));
    var stats = monsterLog.stats.putIfAbsent(stat, () => Queue());
    stats.add(value);
    if (stats.length > 20) stats.removeFirst();

    monsterReason(monster, stat, reason);
  }

  /// Updates [stat]'s [reason] text without appending a new value.
  static void monsterReason(Monster monster, string stat, string? reason) {
    if (!enabled) return;

    var monsterLog = _monsters.putIfAbsent(monster, () => _MonsterLog(monster));
    monsterLog.statReason[stat] = reason;
  }

  /// Gets the debug info for [monster].
  static string? monsterInfo(Monster monster) {
    if (!enabled || _gameScreen == null) return null;

    var log = _monsters[monster];
    if (log == null) return null;
    return log.toString();
  }
}

class _MonsterLog {
  public Monster monster;
  public Queue<string> log = Queue<string>();

  public Map<string, Queue<num>> stats = {};
  public Map<string, string?> statReason = {};

  _MonsterLog(this.monster);

  void add(string logItem) {
    log.add(logItem);
    if (log.length > 10) log.removeFirst();
  }

  string toString() {
    var buffer = StringBuffer();

    buffer.write(monster.breed.name);

    var state = "asleep";
    if (monster.isAfraid) {
      state = "afraid";
    } else if (monster.isAwake) {
      state = "awake";
    }
    buffer.writeln(" ($state)");

    var statNames = stats.keys.toList();
    statNames.sort();
    var length =
        statNames.fold<int>(0, (length, name) => math.max(length, name.length));

    var barChars = " ▁▂▃▄▅▆▇█";
    for (var name in statNames) {
      var barBuffer = StringBuffer("${name.padRight(length)} ");
      var showBar = false;

      var values = stats[name]!;
      for (var value in values) {
        var i = (value * barChars.length).ceil().clamp(0, barChars.length - 1);
        barBuffer.write(barChars[i]);
        if (i > 0) showBar = true;
      }

      if (values.isNotEmpty) {
        barBuffer.write(" ${values.last.toStringAsFixed(4).padLeft(6)}");
      }

      if (statReason[name] != null) {
        barBuffer.write(" ${statReason[name]}");
        showBar = true;
      }

      if (showBar) buffer.writeln(barBuffer.toString());
    }

    buffer.writeAll(log, "\n");
    return buffer.toString();
  }
}
