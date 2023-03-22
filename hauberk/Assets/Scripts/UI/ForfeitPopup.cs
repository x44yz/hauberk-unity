using System;
using System.Collections.Generic;
using System.Linq;
using Color = UnityEngine.Color;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

/// Modal dialog for letting the user confirm forfeiting the level.
class ForfeitPopup : Popup {
  public bool _isTown;

  public ForfeitPopup(bool isTown)
  {
    _isTown = isTown;
  }

  List<string> message {
    get {
      if (_isTown) return new List<string>(){"Return to main menu?"};

      return new List<string>(){
        "Are you sure you want to forfeit the level?",
        "You will lose all items and experience gained in the dungeon."
      };
    }
  }

  Dictionary<string, string> helpKeys => new Dictionary<string, string> {
      {"Y", "Yes"}, 
      {"N", "No"},
      {"Esc", "No"}
  };

  public override void HandleInput() {
    if (Input.GetKeyDown(InputX.cancel)) {
      terminal.Pop(false);
    }

    bool shift = Input.GetKey(KeyCode.LeftShift);
    bool alt = Input.GetKey(KeyCode.LeftAlt);
    if (shift || alt)
      return;

    if (Input.GetKeyDown(KeyCode.N))
    {
      terminal.Pop(false);
    }
    else if (Input.GetKeyDown(KeyCode.Y))
    {
      terminal.Pop(true);
    }
  }
}
