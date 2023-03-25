using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Color = UnityEngine.Color;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

/// Dialog shown while a new level is being generated.
class LoadingDialog : Screen {
  public Game _game;
  IEnumerator<string> m_steps = null;
  public IEnumerator<string> _steps {
    get {
        if (m_steps == null)
          m_steps = _game.generate().GetEnumerator();
        return m_steps;
    }
  }
  int _frame = 0;

  public override bool isTransparent => true;

  public LoadingDialog(HeroSave save, Content content, int depth)
  {
    _game = new Game(content, save.clone(), depth);
  }

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt) {
    if (keyCode == InputX.cancel) {
      terminal.Pop(false);
      return true;
    }

    if (shift || alt) return false;

    switch (keyCode) {
      case KeyCode.N:
        terminal.Pop(false);
        break;

      case KeyCode.Y:
        terminal.Pop(true);
        break;
    }

    return true;
  }

  public override void Tick(float dt) {
    var stopwatch = new Stopwatch();
    stopwatch.Start();

    while (stopwatch.ElapsedMilliseconds < 16) {
      if (_steps.MoveNext()) {
        Dirty();
      } else {
        terminal.Pop(_game);
        return;
      }
    }

    _frame = (_frame + 1) % 10;
  }

  public override void Render(Terminal terminal) {
    var width = 30;
    var height = 7;

    terminal = terminal.Rect((terminal.width - width) / 2,
        (terminal.height - height) / 2, width, height);

    Draw.doubleBox(terminal, 0, 0, terminal.width, terminal.height, Hues.gold);

    terminal.WriteAt(2, 2, "Entering dungeon...", UIHue.text);

    var offset = _frame / 2;
    var bar = DartUtils.strConcat("/    ", 6).Substring(offset, offset + 26);
    terminal.WriteAt(2, 4, bar, UIHue.primary);
  }
}
