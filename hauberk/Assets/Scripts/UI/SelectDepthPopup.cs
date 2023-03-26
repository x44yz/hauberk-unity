using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

class SelectDepthPopup : Popup
{
  public Content content;
  public HeroSave save;

  /// The selected depth.
  int _depth = 1;

  public SelectDepthPopup(Content content, HeroSave save)
  {
    this.content = content;
    this.save = save;
    _depth = Math.Min(Option.maxDepth, save.maxDepth + 1);
  }

  public override int? width => 42;

  public override int? height => 26;

  public override List<string> message => new List<string>(){
        "Stairs descend into darkness.",
        "How far down shall you venture?"
  };

  public override Dictionary<string, string> helpKeys => new Dictionary<string, string>() {
      {"OK", "Enter dungeon"},
      {"↕↔", "Change depth"},
      {"Esc", "Cancel"}
  };

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == InputX.w)
      _changeDepth(_depth - 1);
    else if (keyCode == InputX.e)
      _changeDepth(_depth + 1);
    else if (keyCode == InputX.n)
      _changeDepth(_depth - 10);
    else if (keyCode == InputX.s)
      _changeDepth(_depth + 10);
    else if (keyCode == InputX.ok)
      terminal.Pop(_depth);
    else if (keyCode == InputX.cancel)
      terminal.Pop();

    return true;
  }

  public override void renderPopup(Terminal terminal)
  {
    for (var depth = 1; depth <= Option.maxDepth; depth++)
    {
      var x = (depth - 1) % 10;
      var y = ((depth - 1) / 10) * 2;

      var color = UIHue.primary;
      if (!Debugger.enabled && depth > save.maxDepth + 1)
      {
        color = UIHue.disabled;
      }
      else if (depth == _depth)
      {
        color = UIHue.selection;
        terminal.WriteAt(
            x * 4, y + 5, CharCode.blackRightPointingPointer, color);
        terminal.WriteAt(
            x * 4 + 4, y + 5, CharCode.blackLeftPointingPointer, color);
      }

      terminal.WriteAt(x * 4 + 1, y + 5, depth.ToString().PadLeft(3), color);
    }
  }

  void _changeDepth(int depth)
  {
    if (depth < 1) return;
    if (depth > Option.maxDepth) return;
    if (!Debugger.enabled && depth > save.maxDepth + 1) return;

    _depth = depth;
    Dirty();
  }
}
