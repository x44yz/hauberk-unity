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
  public static KeyCode forfeit = KeyCode.F; // Input("forfeit"); + shift

  /// Exit the successfully completed level.
  public static KeyCode quit = KeyCode.Q; // Input("quit");

  /// Open nearby doors, chests, etc.
  public static KeyCode open = KeyCode.C; // Input("open"); + shift

  /// Close nearby doors.
  public static KeyCode close = KeyCode.C; // Input("close");

  public static KeyCode drop = KeyCode.D; //Input("drop");
  public static KeyCode use = KeyCode.U; // Input("use");
  public static KeyCode pickUp = KeyCode.G; // Input("pickUp");
  public static KeyCode toss = KeyCode.T; // Input("toss");
  public static KeyCode swap = KeyCode.X; // Input("swap");
  public static KeyCode equip = KeyCode.E; // Input("equip");

  public static KeyCode heroInfo = KeyCode.A; // Input("heroInfo");
  public static KeyCode selectSkill = KeyCode.S; // Input("selectSkill");
  public static KeyCode editSkills = KeyCode.S; // Input("editSkills"); + shift

  /// Directional inputs.
  ///
  /// These are used both for navigating in the level and menu screens.
  public static KeyCode n = KeyCode.O; // Input("n");
  public static KeyCode ne = KeyCode.P; // Input("ne");
  public static KeyCode e = KeyCode.Semicolon; // Input("e");
  public static KeyCode se = KeyCode.Slash; // Input("se");
  public static KeyCode s = KeyCode.Period; // Input("s");
  public static KeyCode sw = KeyCode.Comma; // Input("sw");
  public static KeyCode w = KeyCode.K; // Input("w");
  public static KeyCode nw = KeyCode.I; // Input("nw");

  /// Rest repeatedly.
  public static KeyCode rest = KeyCode.L; // Input("rest"); + shift

  public static KeyCode runN = KeyCode.O; // Input("runN"); + shift
  public static KeyCode runNE = KeyCode.P; // Input("runNE"); + shift
  public static KeyCode runE = KeyCode.Semicolon; // Input("runE"); + shift
  public static KeyCode runSE = KeyCode.Slash; // Input("runSE"); + shift
  public static KeyCode runS = KeyCode.Period; // Input("runS"); + shift
  public static KeyCode runSW = KeyCode.Comma; // Input("runSW"); + shift
  public static KeyCode runW = KeyCode.K; // Input("runW"); + shift
  public static KeyCode runNW = KeyCode.I; // Input("runNW"); + shift

  /// Fire the last selected skill.
  public static KeyCode fire = KeyCode.L; // Input("fire"); + alt

  public static KeyCode fireN = KeyCode.O; // Input("fireN"); + alt
  public static KeyCode fireNE = KeyCode.P; // Input("fireNE"); + alt
  public static KeyCode fireE = KeyCode.Semicolon; // Input("fireE"); + alt
  public static KeyCode fireSE = KeyCode.Slash; // Input("fireSE"); + alt
  public static KeyCode fireS = KeyCode.Period; // Input("fireS"); + alt
  public static KeyCode fireSW = KeyCode.Comma; // Input("fireSW"); + alt
  public static KeyCode fireW = KeyCode.K; // Input("fireW"); + alt
  public static KeyCode fireNW = KeyCode.I; // Input("fireNW"); + alt

  /// Open the wizard cheat menu.
  public static KeyCode wizard = KeyCode.W; // Input("wizard"); + shift + alt

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
