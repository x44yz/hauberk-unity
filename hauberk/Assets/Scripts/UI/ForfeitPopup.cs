using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

/// Modal dialog for letting the user confirm forfeiting the level.
class ForfeitPopup : Popup
{
  public bool _isTown;

  public ForfeitPopup(bool isTown)
  {
    _isTown = isTown;
  }

  public override List<string> message
  {
    get
    {
      if (_isTown) return new List<string>() { "Return to main menu?" };

      return new List<string>(){
        "Are you sure you want to forfeit the level?",
        "You will lose all items and experience gained in the dungeon."
      };
    }
  }

  public override Dictionary<string, string> helpKeys => new Dictionary<string, string> {
      {"Y", "Yes"},
      {"N", "No"},
      {"Esc", "No"}
  };

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == InputX.cancel)
    {
      terminal.Pop(false);
      return true;
    }

    if (shift || alt)
      return false;

    if (keyCode == KeyCode.N)
    {
      terminal.Pop(false);
      return true;
    }
    else if (keyCode == KeyCode.Y)
    {
      terminal.Pop(true);
      return true;
    }

    return false;
  }
}
