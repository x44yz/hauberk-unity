using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

// TODO: Fix this and its subscreens to work with the resizable UI.
abstract class HeroInfoDialog : Screen
{
  static public List<HeroInfoDialog> _screens = new List<HeroInfoDialog>();

  public Content content;
  public HeroSave hero;

  public static HeroInfoDialog create(Content content, HeroSave hero)
  {
    if (_screens.isEmpty())
    {
      _screens.Add(new HeroEquipmentDialog(content, hero));
      _screens.Add(new HeroResistancesDialog(content, hero));
      _screens.Add(new HeroMonsterLoreDialog(content, hero));
      _screens.Add(new HeroItemLoreDialog(content, hero));
      // TODO: Affixes.
    }

    return _screens.First();
  }

  public HeroInfoDialog(Content content, HeroSave hero)
  {
    this.content = content;
    this.hero = hero;
  }

  public virtual string name => "";

  public virtual string extraHelp => null;

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == InputX.cancel)
    {
      terminal.Pop();
      return true;
    }

    if (alt) return false;

    if (keyCode == KeyCode.Tab)
    {
      var index = _screens.IndexOf(this);

      if (shift)
      {
        index += _screens.Count - 1;
      }
      else
      {
        index++;
      }

      var screen = _screens[index % _screens.Count];
      terminal.GoTo(screen);
      return true;
    }

    return false;
  }

  public override void Render(Terminal terminal)
  {
    terminal.Clear();

    var nextScreen = _screens[(_screens.IndexOf(this) + 1) % _screens.Count];
    var helpText = $"[Esc] Exit, [Tab] View {nextScreen.name}";
    if (extraHelp != null)
    {
      helpText += $", {extraHelp}";
    }

    terminal.WriteAt(0, terminal.height - 1, helpText, Hues.coolGray);
  }

  public void drawEquipmentTable(
      Terminal terminal, System.Action<Item, int> callback)
  {
    terminal.WriteAt(2, 1, "Equipment", Hues.gold);

    var y = 3;
    for (var i = 0; i < hero.equipment.slots.Count; i++)
    {
      var item = hero.equipment.slots[i];
      callback(item, y);

      if (item == null)
      {
        terminal.WriteAt(
            2, y, $"({hero.equipment.slotTypes[i]})", Hues.darkCoolGray);
        y += 2;
        continue;
      }

      terminal.WriteAt(0, y, item.appearance as Glyph);
      terminal.WriteAt(2, y, item.nounText, Hues.ash);

      y += 2;
    }
  }
}
