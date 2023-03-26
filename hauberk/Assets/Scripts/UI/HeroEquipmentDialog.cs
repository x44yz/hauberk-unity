using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

// TODO: Unify with HeroItemLoreDialog so that we can select and show inspector
// for equipment.
class HeroEquipmentDialog : HeroInfoDialog
{
  public HeroEquipmentDialog(Content content, HeroSave hero)
      : base(content, hero)
  {
  }

  public override string name => "Equipment";

  public override void Render(Terminal terminal)
  {
    base.Render(terminal);

    void writeLine(int y, Color color)
    {
      terminal.WriteAt(
          2,
          y,
          "───────────────────────────────────────────── " +
          "── ─────────── ──── ───── ──────",
          color);
    }

    void writeScale(int x, int y, double scale)
    {
      var str = scale.ToString("F1");

      if (scale > 1.0)
      {
        terminal.WriteAt(x, y, "x", Hues.sherwood);
        terminal.WriteAt(x + 1, y, str, Hues.peaGreen);
      }
      else if (scale < 1.0)
      {
        terminal.WriteAt(x, y, "x", Hues.maroon);
        terminal.WriteAt(x + 1, y, str, Hues.red);
      }
    }

    void writeBonus(int x, int y, int bonus)
    {
      var str = Math.Abs(bonus).ToString();

      if (bonus > 0)
      {
        terminal.WriteAt(x + 2 - str.Length, y, "+", Hues.sherwood);
        terminal.WriteAt(x + 3 - str.Length, y, str, Hues.peaGreen);
      }
      else if (bonus < 0)
      {
        terminal.WriteAt(x + 2 - str.Length, y, "-", Hues.maroon);
        terminal.WriteAt(x + 3 - str.Length, y, str, Hues.red);
      }
    }

    terminal.WriteAt(48, 0, "══════ Attack ═════ ══ Defend ══", Hues.darkCoolGray);
    terminal.WriteAt(48, 1, "El Damage      Hit  Dodge Armor", Hues.coolGray);

    drawEquipmentTable(terminal, (item, y) =>
    {
      writeLine(y - 1, Hues.darkerCoolGray);

      if (item == null) return;

      var attack = item.attack;
      if (attack != null)
      {
        terminal.WriteAt(
            48, y, attack.element.abbreviation, Hues.elementColor(attack.element));

        terminal.WriteAt(51, y, attack.damage.ToString().PadLeft(2), Hues.ash);
      }

      writeScale(54, y, item.damageScale);
      writeBonus(59, y, item.damageBonus);
      writeBonus(64, y, item.strikeBonus);

      // TODO: Dodge bonuses.

      if (item.baseArmor != 0)
      {
        terminal.WriteAt(74, y, item.baseArmor.ToString().PadLeft(2), Hues.ash);
      }

      writeBonus(77, y, item.armorModifier);
    });

    var element = Element.none;
    var baseDamage = Option.heroPunchDamage;
    var totalDamageScale = 1.0;
    var totalDamageBonus = 0;
    var totalStrikeBonus = 0;
    var totalArmor = 0;
    var totalArmorBonus = 0;
    foreach (var item in hero.equipment.slots)
    {
      if (item == null) continue;

      if (item.attack != null)
      {
        element = item.attack!.element;
        baseDamage = item.attack!.damage;
      }

      totalDamageScale *= item.damageScale;
      totalDamageBonus += item.damageBonus;
      totalStrikeBonus += item.strikeBonus;
      totalArmor += item.baseArmor;
      totalArmorBonus += item.armorModifier;
    }

    var totalY = 21;
    terminal.WriteAt(41, totalY, "Totals", Hues.coolGray);

    writeLine(2, Hues.darkCoolGray);
    writeLine(totalY - 1, Hues.darkCoolGray);

    terminal.WriteAt(48, totalY, element.abbreviation, Hues.elementColor(element));
    terminal.WriteAt(51, totalY, baseDamage.ToString().PadLeft(2));
    writeScale(54, totalY, totalDamageScale);
    writeBonus(59, totalY, totalDamageBonus);
    writeBonus(64, totalY, totalStrikeBonus);

    // TODO: Might need three digits for armor.
    terminal.WriteAt(74, totalY, totalArmor.ToString().PadLeft(2), Hues.ash);
    writeBonus(77, totalY, totalArmorBonus);

    // TODO: Show resulting average damage. Include stat bonuses and stuff too.
    // TODO: Show heft, weight, encumbrance, etc.
  }
}
