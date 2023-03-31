using System;
using System.Collections;
using System.Collections.Generic;
using num = System.Double;
using System.Text;
using System.Linq;
using Mathf = UnityEngine.Mathf;

/// A class for storing debugging information.
///
/// Unlike the rest of the engine, this is static state to make it easier for
/// the engine to punch debug info all the way to where the UI can get it. It
/// should not be used outside of a debugging scenario.
class Debugger
{
  public static bool enabled = true;

  /// If true, all monsters are rendered, regardless of in-game visibility.
  public static bool showAllMonsters = false;

  /// If true, monster alertness is rendered.
  public static bool showMonsterAlertness = false;

  public static bool showHeroVolume = false;

  // for debug
  public static bool debugHideFog = false;
  public static bool debugSelectDepth = true;

  /// The current density map being used.
  ///
  /// Typed as Object? so that this library isn't coupled to the UI.
  public static object densityMap;

  static public Dictionary<Monster, _MonsterLog> _monsters = new Dictionary<Monster, _MonsterLog>();

  /// The current game screen.
  ///
  /// Typed as Object so that this library isn't coupled to the UI.
  static object _gameScreen;
  static object gameScreen => _gameScreen;

  public static void bindGameScreen(object screen)
  {
    _gameScreen = screen;
    _monsters.Clear();
  }

  /// Appends [message] to the debug log for [monster].
  public static void monsterLog(Monster monster, string message)
  {
    if (!enabled) return;

    _MonsterLog monsterLog = null;
    if (_monsters.TryGetValue(monster, out monsterLog) == false)
    {
      monsterLog = new _MonsterLog(monster);
      _monsters[monster] = monsterLog;
    }
    monsterLog.add(message);
  }

  /// Appends a new [value] for [stat] for [monster].
  ///
  /// The value should range from 0.0 to 1.0. If there is a descriptive [reason]
  /// for the value, that can be provided too.
  public static void monsterStat(Monster monster, string stat, num value,
      string reason = null)
  {
    if (!enabled) return;

    _MonsterLog monsterLog = null;
    if (_monsters.TryGetValue(monster, out monsterLog) == false)
    {
      monsterLog = new _MonsterLog(monster);
      _monsters[monster] = monsterLog;
    }

    List<num> stats = null;
    if (monsterLog.stats.TryGetValue(stat, out stats) == false)
    {
      stats = new List<num>();
      monsterLog.stats[stat] = stats;
    }
    stats.Add(value);
    if (stats.Count > 20) stats.RemoveAt(0);

    monsterReason(monster, stat, reason);
  }

  /// Updates [stat]'s [reason] text without appending a new value.
  public static void monsterReason(Monster monster, string stat, string reason)
  {
    if (!enabled) return;

    _MonsterLog monsterLog = null;
    if (_monsters.TryGetValue(monster, out monsterLog) == false)
    {
      monsterLog = new _MonsterLog(monster);
      _monsters[monster] = monsterLog;
    }
    monsterLog.statReason[stat] = reason;
  }

  /// Gets the debug info for [monster].
  static string monsterInfo(Monster monster)
  {
    if (!enabled || _gameScreen == null) return null;

    var log = _monsters[monster];
    if (log == null) return null;
    return log.ToString();
  }

  public static void log(object message)
  {
    UnityEngine.Debug.Log(message);
  }

  public static void logWarning(object message)
  {
    UnityEngine.Debug.LogWarning(message);
  }

  public static void logError(object message)
  {
    UnityEngine.Debug.LogError(message);
  }

  public static void assert(bool v, string msg = null)
  {
    if (msg != null)
      UnityEngine.Debug.Assert(v, msg);
    else
      UnityEngine.Debug.Assert(v);
  }
}

class _MonsterLog
{
  public Monster monster;
  public Queue<string> log = new Queue<string>();

  public Dictionary<string, List<num>> stats = new Dictionary<string, List<num>>();
  public Dictionary<string, string> statReason = new Dictionary<string, string>();

  public _MonsterLog(Monster monster)
  {
    this.monster = monster;
  }

  public void add(string logItem)
  {
    log.Enqueue(logItem);
    if (log.Count > 10) log.Dequeue();
  }

  public override string ToString()
  {
    var buffer = new StringBuilder();

    buffer.Append(monster.breed.name);

    var state = "asleep";
    if (monster.isAfraid)
    {
      state = "afraid";
    }
    else if (monster.isAwake)
    {
      state = "awake";
    }
    buffer.Append($" ({state})");

    var statNames = stats.Keys.ToList();
    statNames.Sort();
    var length =
      statNames.fold<string>(0, (length, name) => Math.Max(length, name.Length));

    var barChars = " ▁▂▃▄▅▆▇█";
    foreach (var name in statNames)
    {
      var barBuffer = new StringBuilder($"{name.PadRight(length)} ");
      var showBar = false;

      var values = stats[name]!;
      foreach (var value in values)
      {
        var i = Mathf.Clamp(Mathf.CeilToInt((float)(value * barChars.Length)), 0, barChars.Length - 1);
        barBuffer.Append(barChars[i]);
        if (i > 0) showBar = true;
      }

      if (values.Count > 0)
      {
        barBuffer.Append($" {values[values.Count - 1].ToString("F4").PadLeft(6)}");
      }

      if (statReason[name] != null)
      {
        barBuffer.Append($" {statReason[name]}");
        showBar = true;
      }

      if (showBar) buffer.Append(barBuffer.ToString());
    }

    buffer.Append(log + "\n");
    return buffer.ToString();
  }
}
