using System.Collections.Generic;
using UnityEngine;

/// Enum class defining the high-level inputs from the user.
///
/// Physical keys on the keyboard are mapped to these, which the user interface
/// then interprets.
class InputX {
  /// Rests in the level, selects a menu item.
  public static KeyCode ok = KeyCode.Return; // Input("ok");

  // TODO: Unify cancel, forfeit, and quit?

  public static KeyCode cancel = KeyCode.Escape; // Input("cancel");
  // public static KeyCode forfeit = Input("forfeit");

  /// Exit the successfully completed level.
  // public static KeyCode quit = Input("quit");

  /// Open nearby doors, chests, etc.
  // public static KeyCode open = Input("open");

  /// Close nearby doors.
  // public static KeyCode close = Input("close");

  // public static KeyCode drop = Input("drop");
  // public static KeyCode use = Input("use");
  // public static KeyCode pickUp = Input("pickUp");
  // public static KeyCode toss = Input("toss");
  // public static KeyCode swap = Input("swap");
  // public static KeyCode equip = Input("equip");

  // public static KeyCode heroInfo = Input("heroInfo");
  // public static KeyCode selectSkill = Input("selectSkill");
  // public static KeyCode editSkills = Input("editSkills");

  /// Directional inputs.
  ///
  /// These are used both for navigating in the level and menu screens.
  public static KeyCode n = KeyCode.UpArrow; // Input("n");
  // public static KeyCode ne = Input("ne");
  public static KeyCode e = KeyCode.RightArrow; // Input("e");
  // public static KeyCode se = Input("se");
  public static KeyCode s = KeyCode.DownArrow; // Input("s");
  // public static KeyCode sw = Input("sw");
  public static KeyCode w = KeyCode.LeftArrow; // Input("w");
  // public static KeyCode nw = Input("nw");

  /// Rest repeatedly.
  // public static KeyCode rest = Input("rest");

  // public static KeyCode runN = Input("runN");
  // public static KeyCode runNE = Input("runNE");
  // public static KeyCode runE = Input("runE");
  // public static KeyCode runSE = Input("runSE");
  // public static KeyCode runS = Input("runS");
  // public static KeyCode runSW = Input("runSW");
  // public static KeyCode runW = Input("runW");
  // public static KeyCode runNW = Input("runNW");

  /// Fire the last selected skill.
  // public static KeyCode fire = Input("fire");

  // public static KeyCode fireN = Input("fireN");
  // public static KeyCode fireNE = Input("fireNE");
  // public static KeyCode fireE = Input("fireE");
  // public static KeyCode fireSE = Input("fireSE");
  // public static KeyCode fireS = Input("fireS");
  // public static KeyCode fireSW = Input("fireSW");
  // public static KeyCode fireW = Input("fireW");
  // public static KeyCode fireNW = Input("fireNW");

  /// Open the wizard cheat menu.
  // public static KeyCode wizard = Input("wizard");

  // public static bool GetKeyDown(KeyCode key)
  // {
  //   return UnityEngine.Input.GetKeyDown(key);
  // }

  // public static bool GetKey(KeyCode key)
  // {
  //   return UnityEngine.Input.GetKey(key);
  // }

  // public static bool GetKeyUp(KeyCode key)
  // {
  //   return UnityEngine.Input.GetKeyUp(key);
  // }

  // public static bool anyKeyDown
  // {
  //   get { return UnityEngine.Input.anyKeyDown; }
  // }
}
