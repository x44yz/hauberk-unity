using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using Mathf = UnityEngine.Mathf;
using UnityTerminal;

/// Shows a detailed info box for an item.
class Inspector
{
  public const int width = 34;

  public Item _item;

  public List<_Section> _sections = new List<_Section>();

  public Inspector(HeroSave hero, Item _item)
  {
    this._item = _item;

    // TODO: Handle armor that gives attack bonuses even though the item
    // itself has no attack.
    if (_item.attack != null) _sections.Add(new _AttackSection(hero, _item));

    if (_item.armor != 0 || _item.defense != null)
    {
      _sections.Add(new _DefenseSection(_item));
    }

    // TODO: Show spells for spellbooks.

    if (_item.canEquip) _sections.Add(new _ResistancesSection(_item));

    if (_item.canUse) _use();
    _description();

    // TODO: Max stack size?
  }

  public void draw(int x, int itemY, Terminal terminal)
  {
    // Frame.
    var height = 2;

    // Two more for the item box.
    height += 2;
    foreach (var section in _sections)
    {
      // +1 for the header.
      height += section.height + 1;
    }

    // A line of space between each section.
    height += _sections.Count - 1;

    // Try to align the box next to the item, but shift it as needed to keep it
    // in bounds and not overlapping the help box on the bottom.
    var top = Mathf.Clamp(itemY - 1, 0, terminal.height - 4 - height);
    terminal = terminal.Rect(x, top, 34, height);

    // Draw the frame.
    Draw.frame(
        terminal, 0, 1, terminal.width, terminal.height - 1, UIHue.helpText);

    Draw.box(terminal, 1, 0, 3, 3, UIHue.helpText);
    terminal.WriteAt(1, 1, "╡", UIHue.helpText);
    terminal.WriteAt(3, 1, "╞", UIHue.helpText);

    terminal.WriteAt(2, 1, _item.appearance as Glyph);
    terminal.WriteAt(4, 1, _item.nounText, UIHue.primary);

    // Draw the sections.
    var y = 3;
    foreach (var section in _sections)
    {
      terminal.WriteAt(1, y, $"{section.header}:", UIHue.selection);
      y++;

      section.draw(terminal, y);
      y += section.height + 1;
    }
  }

  void _use()
  {
    _sections.Add(new _TextSection("Use", _wordWrap(_item.type.use!.description)));
  }

  void _description()
  {
    // TODO: Support color codes in strings to make important information stand
    // out more.

    var sentences = new List<string>();

    // TODO: General description.
    // TODO: Equip slot.

    foreach (var stat in Stat.all)
    {
      var bonus = 0;
      if (_item.prefix != null) bonus += _item.prefix!.statBonus(stat);
      if (_item.suffix != null) bonus += _item.suffix!.statBonus(stat);

      if (bonus < 0)
      {
        sentences.Add($"It lowers your {stat.name} by {-bonus}.");
      }
      else if (bonus > 0)
      {
        sentences.Add($"It raises your {stat.name} by {bonus}.");
      }
    }

    var toss = _item.toss;
    if (toss != null)
    {
      var element = "";
      if (toss.attack.element != Element.none)
      {
        element = $" {toss.attack.element.name}";
      }

      sentences.Add($"It can be thrown for {toss.attack.damage}{element}" +
          $" damage up to range {toss.attack.range}.");

      if (toss.breakage != 0)
      {
        sentences
            .Add($"It has a {toss.breakage}% chance of breaking when thrown.");
      }

      // TODO: Describe toss use.
    }

    if (_item.emanationLevel > 0)
    {
      sentences.Add($"It emanates {_item.emanationLevel} light.");
    }

    foreach (var element in _item.type.destroyChance.Keys)
    {
      sentences.Add($"It can be destroyed by {element.name.ToLower()}.");
    }

    _sections.Add(new _TextSection("Description", _wordWrap(string.Join(" ", sentences))));
  }

  List<string> _wordWrap(string text) => Log.wordWrap(width - 2, text);
}

abstract class _Section
{
  public virtual string header => "";
  public virtual int height => 0;
  public abstract void draw(Terminal terminal, int y);

  // TODO: Mostly copied from hero_equipment_dialog. Unify.
  public void _writeBonus(Terminal terminal, int x, int y, int bonus)
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
    else
    {
      terminal.WriteAt(x + 2 - str.Length, y, "+", UIHue.disabled);
      terminal.WriteAt(x + 3 - str.Length, y, str, UIHue.disabled);
    }
  }

  public void _writeLabel(Terminal terminal, int y, string label)
  {
    terminal.WriteAt(1, y, $"{label}:", UIHue.text);
  }

  public void _writeStat(Terminal terminal, int y, string label, int value)
  {
    _writeLabel(terminal, y, label);
    terminal.WriteAt(12, y, value.ToString(), UIHue.primary);
  }

  // TODO: Mostly copied from hero_equipment_dialog. Unify.
  public void _writeScale(Terminal terminal, int x, int y, double scale)
  {
    var str = scale.ToString("F1");

    var xColor = UIHue.disabled;
    var numberColor = UIHue.disabled;
    if (scale > 1.0)
    {
      xColor = Hues.sherwood;
      numberColor = Hues.peaGreen;
    }
    else if (scale < 1.0)
    {
      xColor = Hues.maroon;
      numberColor = Hues.red;
    }

    terminal.WriteAt(x, y, "x", xColor);
    terminal.WriteAt(x + 1, y, str, numberColor);
  }
}

class _AttackSection : _Section
{
  public HeroSave _hero;
  public Item _item;

  public override string header => "Attack";

  public override int height
  {
    get
    {
      // Damage and heft.
      var height = 2;

      if (_item.strikeBonus != 0) height++;
      if (_item.attack!.isRanged) height++;

      return height;
    }
  }

  public _AttackSection(HeroSave _hero, Item _item)
  {
    this._hero = _hero;
    this._item = _item;
  }

  public override void draw(Terminal terminal, int y)
  {
    _writeLabel(terminal, y, "Damage");
    if (_item.element != Element.none)
    {
      terminal.WriteAt(
          9, y, _item.element.abbreviation, Hues.elementColor(_item.element));
    }

    terminal.WriteAt(12, y, _item.attack!.damage.ToString(), UIHue.text);
    _writeScale(terminal, 16, y, _item.damageScale);
    _writeBonus(terminal, 20, y, _item.damageBonus);
    terminal.WriteAt(25, y, "=", UIHue.secondary);

    var damage = _item.attack!.damage * _item.damageScale + _item.damageBonus;
    terminal.WriteAt(27, y, damage.ToString("F2").PadLeft(6), Hues.carrot);
    y++;

    if (_item.strikeBonus != 0)
    {
      _writeLabel(terminal, y, "Strike");
      _writeBonus(terminal, 12, y, _item.strikeBonus);
      y++;
    }

    if (_item.attack!.isRanged)
    {
      _writeStat(terminal, y, "Range", _item.attack!.range);
    }

    _writeLabel(terminal, y, "Heft");
    var strongEnough = _hero.strength.value >= _item.heft;
    var color = strongEnough ? UIHue.primary : Hues.red;
    terminal.WriteAt(12, y, _item.heft.ToString(), color);
    _writeScale(terminal, 16, y, _hero.strength.heftScale(_item.heft));
    // TODO: Show heft when dual-wielding somehow?
    y++;
  }
}

class _DefenseSection : _Section
{
  public Item _item;

  public override string header => "Defense";
  public override int height => _item.defense != null ? 3 : 2;

  public _DefenseSection(Item _item)
  {
    this._item = _item;
  }

  public override void draw(Terminal terminal, int y)
  {
    if (_item.defense != null)
    {
      _writeStat(terminal, y, "Dodge", _item.defense!.amount);
    }

    if (_item.armor != 0)
    {
      _writeLabel(terminal, y, "Armor");
      terminal.WriteAt(12, y, _item.baseArmor.ToString(), UIHue.text);
      _writeBonus(terminal, 16, y, _item.armorModifier);
      terminal.WriteAt(25, y, "=", UIHue.secondary);

      var armor = _item.armor.ToString().PadLeft(6);
      terminal.WriteAt(27, y, armor, Hues.peaGreen);
      y++;
    }

    _writeStat(terminal, y, "Weight", _item.weight);
    // TODO: Encumbrance.
  }
}

class _ResistancesSection : _Section
{
  public Item _item;

  public override string header => "Resistances";
  public override int height => 2;

  public _ResistancesSection(Item _item)
  {
    this._item = _item;
  }

  public override void draw(Terminal terminal, int y)
  {
    var x = 1;
    foreach (var element in Elements.all)
    {
      if (element == Element.none) continue;
      var resistance = _item.resistance(element);
      _writeBonus(terminal, x - 1, y, resistance);
      terminal.WriteAt(x, y + 1, element.abbreviation,
          resistance == 0 ? UIHue.disabled : Hues.elementColor(element));
      x += 3;
    }
  }
}

class _TextSection : _Section
{
  public List<string> _lines;

  string _header;
  public override string header => _header;
  public override int height => _lines.Count;

  public _TextSection(string header, List<string> _lines)
  {
    this._header = header;
    this._lines = _lines;
  }

  public override void draw(Terminal terminal, int y)
  {
    foreach (var line in _lines)
    {
      terminal.WriteAt(1, y, line, UIHue.text);
      y++;
    }
  }
}
