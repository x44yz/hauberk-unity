using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using UnityTerminal;


/// Renders a list of items in some UI context, including the surrounding frame.
abstract class ItemView
{
  /// The ideal maximum width of an item list, including the frame.
  public const int preferredWidth = 46;

  public virtual HeroSave save => null;

  public virtual ItemCollection items => null;

  public virtual bool showLetters => true;

  public virtual bool canSelectAny => true;

  public virtual bool capitalize => false;

  public virtual bool showPrices => false;

  public virtual Item inspectedItem => null;

  public virtual bool inspectorOnRight => false;

  public virtual bool canSelect(Item item) => false;

  public virtual int? getPrice(Item item) => item.price;

  public virtual void render(
      Terminal terminal, int left, int top, int width, int itemSlotCount)
  {
    Draw.frame(terminal, left, top, width, itemSlotCount + 2,
        canSelectAny ? UIHue.selection : UIHue.disabled);
    terminal.WriteAt(left + 2, top, $" {items.name} ",
        canSelectAny ? UIHue.selection : UIHue.text);

    var letters = capitalize
        ? "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
        : "abcdefghijklmnopqrstuvwxyz";

    // Shift the stats over to make room for prices, if needed.
    var statRight = left + width - 1;

    if (showPrices)
    {
      foreach (var item in items)
      {
        var price = getPrice(item);
        if (price != null)
        {
          statRight =
              Math.Min(statRight, left + width - DartUtils.formatMoney(price.Value).Length - 3);
        }
      }
    }

    var slot = 0;
    var letter = 0;
    foreach (var item in items.slots)
    {
      var x = left + (showLetters ? 3 : 1);
      var y = top + slot + 1;

      // If there's no item in this equipment slot, show the slot name.
      if (item == null)
      {
        // If this is the second hand slot and the previous one has a
        // two-handed item in it, mark this one.
        if (slot > 0 &&
            items.slotTypes[slot] == "hand" &&
            items.slotTypes[slot - 1] == "hand" &&
            // TODO: Use "?.".
            items.slots.elementAt(slot - 1) != null &&
            items.slots.elementAt(slot - 1)!.type.isTwoHanded)
        {
          terminal.WriteAt(x + 2, y, "↑ two-handed", UIHue.disabled);
        }
        else
        {
          terminal.WriteAt(
              x + 2, y, $"({items.slotTypes[slot]})", UIHue.disabled);
        }
        letter++;
        slot++;
        continue;
      }

      var borderColor = Hues.darkCoolGray;
      var letterColor = UIHue.secondary;
      var textColor = UIHue.primary;
      var enabled = true;

      if (canSelectAny)
      {
        if (canSelect(item))
        {
          borderColor = UIHue.secondary;
          letterColor = UIHue.selection;
          textColor = UIHue.primary;
        }
        else
        {
          borderColor = Color.black;
          letterColor = Color.black;
          textColor = UIHue.disabled;
          enabled = false;
        }
      }

      if (item == inspectedItem)
      {
        textColor = UIHue.selection;
      }

      if (showLetters)
      {
        terminal.WriteAt(left + 1, y, " )", borderColor);
        terminal.WriteAt(left + 1, y, letters[letter], letterColor);
      }

      letter++;

      if (enabled)
      {
        terminal.WriteAt(x, y, (item.appearance as Glyph));
      }

      var nameRight = left + width - 1;
      if (showPrices && getPrice(item) != null)
      {
        var price = DartUtils.formatMoney(getPrice(item).Value);
        var priceLeft = left + width - 1 - price.Length - 1;
        terminal.WriteAt(priceLeft, y, "$", enabled ? Hues.tan : UIHue.disabled);
        terminal.WriteAt(
            priceLeft + 1, y, price, enabled ? Hues.gold : UIHue.disabled);

        nameRight = priceLeft;
      }

      void drawStat(int symbol, object stat, Color light, Color dark)
      {
        var str = stat.ToString();
        var statLeft = statRight - str.Length - 1;
        terminal.WriteAt(statLeft, y, symbol, enabled ? dark : UIHue.disabled);
        terminal.WriteAt(
            statLeft + 1, y, str, enabled ? light : UIHue.disabled);

        nameRight = statLeft;
      }

      // TODO: Eventually need to handle equipment that gives both an armor and
      // attack bonus.
      if (item.attack != null)
      {
        var hit = item.attack!.createHit();
        drawStat(
            CharCode.feminineOrdinalIndicator, hit.damageString, Hues.carrot, Hues.brown);
      }
      else if (item.armor != 0)
      {
        drawStat(CharCode.latinSmallLetterAe, item.armor, Hues.peaGreen, Hues.sherwood);
      }

      var name = item.nounText;
      var nameWidth = nameRight - (x + 2);
      if (name.Length > nameWidth) name = name.Substring(0, nameWidth);
      terminal.WriteAt(x + 2, y, name, textColor);

      // Draw the inspector for this item.
      if (item == inspectedItem)
      {
        var inspector = new Inspector(save, item);
        if (inspectorOnRight)
        {
          if (left + width + Inspector.width > terminal.width)
          {
            // No room on the right so draw it below.
            terminal.WriteAt(left + width - 1, y, "▼", UIHue.selection);
            inspector.draw(left + (width - Inspector.width) / 2,
                top + itemSlotCount + 3, terminal);
          }
          else
          {
            terminal.WriteAt(left + width - 1, y, "►", UIHue.selection);
            inspector.draw(left + width, y, terminal);
          }
        }
        else
        {
          terminal.WriteAt(left, y, "◄", UIHue.selection);
          inspector.draw(left - Inspector.width, y, terminal);
        }
      }

      slot++;
    }
  }
}

