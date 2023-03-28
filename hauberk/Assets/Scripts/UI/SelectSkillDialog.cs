using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

class SelectSkillDialog : Screen
{
  public GameScreen _gameScreen;
  public List<UsableSkill> _skills = new List<UsableSkill>();

  public override bool isTransparent => true;

  public SelectSkillDialog(GameScreen _gameScreen)
  {
    this._gameScreen = _gameScreen;

    foreach (var skill in _gameScreen.game.hero.skills.acquired)
    {
      if (skill is UsableSkill) _skills.Add((UsableSkill)skill);
    }
  }

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == InputX.cancel)
    {
      terminal.Pop();
      return true;
    }

    if (shift || alt) return false;

    if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
    {
      selectCommand(keyCode - KeyCode.A);
      return true;
    }

    // TODO: Quick keys!
    return false;
  }

  void selectCommand(int index)
  {
    if (index >= _skills.Count) return;
    if (_skills[index].unusableReason(_gameScreen.game) != null) return;

    terminal.Pop(_skills[index]);
  }

  public override void Render(Terminal terminal)
  {
    Draw.helpKeys(terminal, new Dictionary<string, string>{
      {"A-Z", "Select skill"},
      // "1-9": "Bind quick key",
      {"Esc", "Exit"}
    });

    // If the item panel is visible, put it there. Otherwise, put it in the
    // stage area.
    if (_gameScreen.itemPanel.isVisible)
    {
      terminal = terminal.Rect(
          _gameScreen.itemPanel.bounds.left,
          _gameScreen.itemPanel.bounds.top,
          _gameScreen.itemPanel.bounds.width,
          _gameScreen.itemPanel.bounds.height);
    }
    else
    {
      terminal = terminal.Rect(
          _gameScreen.stagePanel.bounds.left,
          _gameScreen.stagePanel.bounds.top,
          _gameScreen.stagePanel.bounds.width,
          _gameScreen.stagePanel.bounds.height);
    }

    // Draw a box for the contents.
    var height = Math.Max(_skills.Count + 2, 3);

    Draw.frame(terminal, 0, 0, terminal.width, height, UIHue.selection);
    terminal.WriteAt(2, 0, " Use which skill? ", UIHue.selection);

    terminal = terminal.Rect(1, 1, terminal.width - 2, terminal.height - 2);

    if (_skills.isEmpty())
    {
      terminal.WriteAt(0, 0, "(You don't have any skills yet)", UIHue.disabled);
      return;
    }

    // TODO: Handle this being taller than the screen.
    for (var y = 0; y < _skills.Count; y++)
    {
      var skill = _skills[y];

      var borderColor = UIHue.secondary;
      var letterColor = Hues.darkerCoolGray;
      var textColor = UIHue.disabled;

      var reason = skill.unusableReason(_gameScreen.game);
      if (reason == null)
      {
        borderColor = UIHue.primary;
        letterColor = UIHue.selection;
        textColor = UIHue.selection;
      }

      if (reason != null)
      {
        terminal.WriteAt(
            terminal.width - reason.Length - 2, y, $"({reason})", textColor);
      }

      terminal.WriteAt(0, y, "( )   ", borderColor);
      terminal.WriteAt(1, y, "abcdefghijklmnopqrstuvwxyz"[y], letterColor);
      terminal.WriteAt(4, y, (skill as Skill).useName, textColor);
    }
  }
}
