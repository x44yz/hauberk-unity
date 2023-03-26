using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

class HeroResistancesDialog : HeroInfoDialog
{
  public HeroResistancesDialog(Content content, HeroSave hero)
      : base(content, hero)
  {
  }

  public override string name => "Resistances";

  public override void Render(Terminal terminal)
  {
    base.Render(terminal);

    void writeLine(int y, Color color)
    {
      terminal.WriteAt(
          2,
          y,
          "───────────────────────────────────────────── " +
          "── ── ── ── ── ── ── ── ── ── ──",
          color);
    }

    terminal.WriteAt(48, 0, "══════════ Resistances ═════════", Hues.darkCoolGray);
    drawEquipmentTable(terminal, (item, y) =>
    {
      writeLine(y - 1, Hues.darkerCoolGray);

      if (item == null) return;

      var i = 0;
      foreach (var element in content.elements)
      {
        if (element == Element.none) continue;

        var x = 48 + i * 3;
        var resistance = item.resistance(element);
        var str = resistance.ToString().PadLeft(2);
        if (resistance > 0)
        {
          terminal.WriteAt(x, y, str, Hues.peaGreen);
        }
        else if (resistance < 0)
        {
          terminal.WriteAt(x, y, str, Hues.red);
        }

        i++;
      }
    });

    var totalY = 21;
    terminal.WriteAt(41, totalY, "Totals", Hues.coolGray);

    writeLine(2, Hues.darkCoolGray);
    writeLine(totalY - 1, Hues.darkCoolGray);

    var i = 0;
    foreach (var element in content.elements)
    {
      if (element == Element.none) continue;

      var x = 48 + i * 3;
      terminal.WriteAt(x, 1, element.abbreviation, Hues.elementColor(element));

      // Show the total resistance.
      var resistance = hero.equipmentResistance(element);
      var color = Hues.darkCoolGray;
      if (resistance > 0)
      {
        color = Hues.peaGreen;
      }
      else if (resistance < 0)
      {
        color = Hues.red;
      }

      terminal.WriteAt(x, totalY, resistance.ToString().PadLeft(2), color);
      i++;
    }
  }
}
