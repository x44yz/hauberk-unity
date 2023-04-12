using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

class LogPanel : Panel
{
  public Log _log;

  public LogPanel(Log _log)
  {
    this._log = _log;
  }

  public override void renderPanel(Terminal terminal)
  {
    Draw.frame(terminal, 0, 0, terminal.width, terminal.height);
    terminal.WriteAt(2, 0, " Messages ", UIHue.text);

    var y = terminal.height - 2;
    for (var i = _log.messages.Count - 1; i >= 0 && y > 0; i--)
    {
      var message = _log.messages[i];

      Color color = Color.white;

      switch (message.type)
      {
        case LogType.message:
          color = Hues.ash;
          break;
        case LogType.error:
          color = Hues.red;
          break;
        case LogType.quest:
          color = Hues.purple;
          break;
        case LogType.gain:
          color = Hues.gold;
          break;
        case LogType.help:
          color = Hues.peaGreen;
          break;
        case LogType.cheat:
          color = Hues.aqua;
          break;
      }

      if (i != _log.messages.Count - 1)
      {
        color = color.Blend(Color.black, 0.5f);
      }

      terminal.WriteAt(1, y, message.text, color);

      if (message.count > 1)
      {
        terminal.WriteAt(
            message.text.Length + 1, y, $" (x{message.count})", Hues.darkCoolGray);
      }

      y--;
    }
  }
}
