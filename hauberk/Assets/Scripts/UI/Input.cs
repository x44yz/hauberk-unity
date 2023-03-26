using System.Collections.Generic;
using KeyCode = UnityEngine.KeyCode;

/// Enum class defining the high-level inputs from the user.
///
/// Physical keys on the keyboard are mapped to these, which the user interface
/// then interprets.
class InputX
{
  /// Rests in the level, selects a menu item.
  public const KeyCode ok = KeyCode.Return; // Input("ok");

  // TODO: Unify cancel, forfeit, and quit?

  public const KeyCode cancel = KeyCode.Escape; // Input("cancel");
  public const KeyCode forfeit = KeyCode.F; // Input("forfeit"); + shift

  /// Exit the successfully completed level.
  public const KeyCode quit = KeyCode.Q; // Input("quit");

  /// Open nearby doors, chests, etc.
  public const KeyCode open = KeyCode.C; // Input("open"); + shift

  /// Close nearby doors.
  public const KeyCode close = KeyCode.C; // Input("close");

  public const KeyCode drop = KeyCode.D; //Input("drop");
  public const KeyCode use = KeyCode.U; // Input("use");
  public const KeyCode pickUp = KeyCode.G; // Input("pickUp");
  public const KeyCode toss = KeyCode.T; // Input("toss");
  public const KeyCode swap = KeyCode.X; // Input("swap");
  public const KeyCode equip = KeyCode.E; // Input("equip");

  public const KeyCode heroInfo = KeyCode.A; // Input("heroInfo");
  public const KeyCode selectSkill = KeyCode.S; // Input("selectSkill");
  public const KeyCode editSkills = KeyCode.S; // Input("editSkills"); + shift

  /// Directional inputs.
  ///
  /// These are used both for navigating in the level and menu screens.
  public const KeyCode n = KeyCode.O; // Input("n");
  public const KeyCode ne = KeyCode.P; // Input("ne");
  public const KeyCode e = KeyCode.Semicolon; // Input("e");
  public const KeyCode se = KeyCode.Slash; // Input("se");
  public const KeyCode s = KeyCode.Period; // Input("s");
  public const KeyCode sw = KeyCode.Comma; // Input("sw");
  public const KeyCode w = KeyCode.K; // Input("w");
  public const KeyCode nw = KeyCode.I; // Input("nw");

  /// Rest repeatedly.
  public const KeyCode rest = KeyCode.L; // Input("rest"); + shift

  public const KeyCode runN = KeyCode.O; // Input("runN"); + shift
  public const KeyCode runNE = KeyCode.P; // Input("runNE"); + shift
  public const KeyCode runE = KeyCode.Semicolon; // Input("runE"); + shift
  public const KeyCode runSE = KeyCode.Slash; // Input("runSE"); + shift
  public const KeyCode runS = KeyCode.Period; // Input("runS"); + shift
  public const KeyCode runSW = KeyCode.Comma; // Input("runSW"); + shift
  public const KeyCode runW = KeyCode.K; // Input("runW"); + shift
  public const KeyCode runNW = KeyCode.I; // Input("runNW"); + shift

  /// Fire the last selected skill.
  public const KeyCode fire = KeyCode.L; // Input("fire"); + alt

  public const KeyCode fireN = KeyCode.O; // Input("fireN"); + alt
  public const KeyCode fireNE = KeyCode.P; // Input("fireNE"); + alt
  public const KeyCode fireE = KeyCode.Semicolon; // Input("fireE"); + alt
  public const KeyCode fireSE = KeyCode.Slash; // Input("fireSE"); + alt
  public const KeyCode fireS = KeyCode.Period; // Input("fireS"); + alt
  public const KeyCode fireSW = KeyCode.Comma; // Input("fireSW"); + alt
  public const KeyCode fireW = KeyCode.K; // Input("fireW"); + alt
  public const KeyCode fireNW = KeyCode.I; // Input("fireNW"); + alt

  /// Open the wizard cheat menu.
  public const KeyCode wizard = KeyCode.W; // Input("wizard"); + shift + alt

  // public const bool GetKeyDown(KeyCode key)
  // {
  //   return UnityEngine.Input.GetKeyDown(key);
  // }

  // public const bool GetKey(KeyCode key)
  // {
  //   return UnityEngine.Input.GetKey(key);
  // }

  // public const bool GetKeyUp(KeyCode key)
  // {
  //   return UnityEngine.Input.GetKeyUp(key);
  // }

  // public const bool anyKeyDown
  // {
  //   get { return UnityEngine.Input.anyKeyDown; }
  // }
}
