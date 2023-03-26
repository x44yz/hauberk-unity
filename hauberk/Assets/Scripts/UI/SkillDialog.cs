using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using Mathf = UnityEngine.Mathf;
using UnityTerminal;

// TODO: Get working with resizable UI.
abstract class SkillDialog : Screen
{
  // TODO: Make this a getter instead of a field.
  public SkillDialog _nextScreen;

  public static SkillDialog create(HeroSave hero)
  {
    var screens = new List<SkillDialog>(){
      new DisciplineDialog(hero),
      new SpellDialog(hero),
    };

    for (var i = 0; i < screens.Count; i++)
    {
      screens[i]._nextScreen = screens[(i + 1) % screens.Count];
    }

    return screens.First();
  }

  public SkillDialog()
  {
  }

  public virtual string _name => "";
}

abstract class SkillTypeDialog<T> : SkillDialog where T : Skill
{
  public HeroSave _hero;
  public List<T> _skills = new List<T>();

  int _selectedSkill = 0;

  public SkillTypeDialog(HeroSave _hero)
  {
    this._hero = _hero;

    foreach (var skill in _hero.skills.discovered)
    {
      if (skill is T) _skills.Add((T)skill);
    }
  }

  public virtual string _extraHelp => null;

  // TODO: Eventually should clone skill set so we can cancel changes on dialog.
  public virtual SkillSet _skillSet => _hero.skills;

  public virtual string _rowSeparator => "";

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == InputX.n)
    {
      _changeSelection(-1);
      return true;
    }
    else if (keyCode == InputX.s)
    {
      _changeSelection(1);
      return true;
    }
    // TODO: Get this working with spells, tricks, etc. that need to be
    // explicitly raised.
    //      case Input.e:
    //        if (!_canRaiseSkill) return false;
    //        _raiseSkill();
    //        return true;

    // TODO: Use OK to confirm changes and cancel to discard them?
    else if (keyCode == InputX.cancel)
    {
      // TODO: Pass back updated skills for skills that are learned on this
      // screen.
      terminal.Pop();
      return true;
    }

    if (shift || alt) return false;

    if (keyCode == KeyCode.Tab)
    {
      terminal.GoTo(_nextScreen);
      return true;
    }

    return false;
  }

  public override void Render(Terminal terminal)
  {
    terminal.Clear();

    _renderSkillList(terminal);
    _renderSkill(terminal);

    var helpText = $"[Esc] Exit, [Tab] View {_nextScreen._name}";
    if (_extraHelp != null)
    {
      helpText += $", {_extraHelp}";
    }

    terminal.WriteAt(0, terminal.height - 1, helpText, UIHue.helpText);
  }

  void _renderSkillList(Terminal terminal)
  {
    terminal = terminal.Rect(0, 0, 40, terminal.height - 1);

    Draw.frame(terminal, 0, 0, terminal.width, terminal.height);
    terminal.WriteAt(1, 0, _name, UIHue.text);

    _renderSkillListHeader(terminal);
    terminal.WriteAt(2, 2, _rowSeparator, Hues.darkCoolGray);

    if (_skills.isEmpty())
    {
      terminal.WriteAt(2, 3, "(None known.)", Hues.darkCoolGray);
      return;
    }

    var i = 0;
    foreach (var skill in _skills)
    {
      var y = i * 2 + 3;
      terminal.WriteAt(2, y + 1, _rowSeparator, Hues.darkerCoolGray);

      var nameColor = UIHue.primary;
      var detailColor = UIHue.text;
      if (i == _selectedSkill)
      {
        nameColor = UIHue.selection;
      }
      else if (!_skillSet.isAcquired(skill))
      {
        nameColor = UIHue.disabled;
        detailColor = UIHue.disabled;
      }

      terminal.WriteAt(2, y, skill.name, nameColor);

      _renderSkillInList(terminal, y, detailColor, skill);

      i++;
    }

    terminal.WriteAt(1, _selectedSkill * 2 + 3,
        CharCode.blackRightPointingPointer, UIHue.selection);
  }

  void _renderSkill(Terminal terminal)
  {
    terminal = terminal.Rect(40, 0, terminal.width - 40, terminal.height - 1);
    Draw.frame(terminal, 0, 0, terminal.width, terminal.height);

    if (_skills.isEmpty()) return;

    var skill = _skills[_selectedSkill];
    terminal.WriteAt(1, 0, skill.name, UIHue.selection);

    _writeText(terminal, 1, 2, skill.description);

    _renderSkillDetails(terminal, skill);
  }

  protected void _writeText(Terminal terminal, int x, int y, string text)
  {
    foreach (var line in Log.wordWrap(terminal.width - 1 - x, text))
    {
      terminal.WriteAt(x, y++, line, UIHue.text);
    }
  }

  public abstract void _renderSkillListHeader(Terminal terminal);

  public abstract void _renderSkillInList(Terminal terminal, int y, Color color, T skill);

  public abstract void _renderSkillDetails(Terminal terminal, T skill);

  void _changeSelection(int offset)
  {
    if (_skills.isEmpty()) return;

    _selectedSkill = Mathf.Clamp(_selectedSkill + offset, 0, _skills.Count - 1);
    Dirty();
  }
}

class DisciplineDialog : SkillTypeDialog<Discipline>
{
  public DisciplineDialog(HeroSave hero) : base(hero)
  {
  }

  public override string _name => "Disciplines";

  public override string _rowSeparator => "──────────────────────────── ─── ────";

  public override void _renderSkillListHeader(Terminal terminal)
  {
    terminal.WriteAt(31, 1, "Lev Next", UIHue.helpText);
  }

  public override void _renderSkillInList(
      Terminal terminal, int y, Color color, Discipline skill)
  {
    var level = _skillSet.level(skill).ToString().PadLeft(3);
    terminal.WriteAt(31, y, level, color);

    var percent = skill.percentUntilNext(_hero);
    terminal.WriteAt(
        35, y, percent == null ? "  --" : $"{percent}%".PadLeft(4), color);
  }

  public override void _renderSkillDetails(Terminal terminal, Discipline skill)
  {
    var level = _skillSet.level(skill);

    terminal.WriteAt(1, 8, $"At current level {level}:", UIHue.primary);
    if (level > 0)
    {
      _writeText(terminal, 3, 10, skill.levelDescription(level));
    }
    else
    {
      terminal.WriteAt(
          3, 10, "(You haven't trained this yet.)", UIHue.disabled);
    }

    // TODO: Show fury cost.

    if (level < skill.maxLevel)
    {
      terminal.WriteAt(1, 16, $"At next level {level + 1}:", UIHue.primary);
      _writeText(terminal, 3, 18, skill.levelDescription(level + 1));
    }

    terminal.WriteAt(1, 30, "Level:", UIHue.secondary);
    terminal.WriteAt(9, 30, level.ToString().PadLeft(4), UIHue.text);
    Draw.meter(terminal, 14, 30, 25, level, skill.maxLevel, Hues.red, Hues.maroon);

    terminal.WriteAt(1, 32, "Next:", UIHue.secondary);
    var percent = skill.percentUntilNext(_hero);
    if (percent != null)
    {
      var points = _hero.skills.points(skill);
      var current = skill.trainingNeeded(_hero.heroClass, level) ?? 0;
      var next = skill.trainingNeeded(_hero.heroClass, level + 1) ?? 0;
      terminal.WriteAt(9, 32, $"{percent}%".PadLeft(4), UIHue.text);
      Draw.meter(
          terminal, 14, 32, 25, points - current, next - current, Hues.red, Hues.maroon);
    }
    else
    {
      terminal.WriteAt(14, 32, "(At max level.)", UIHue.disabled);
    }
  }
}

class SpellDialog : SkillTypeDialog<Spell>
{
  public override string _name => "Spells";

  public override string _rowSeparator => "──────────────────────────────── ────";

  public SpellDialog(HeroSave hero) : base(hero)
  {
  }

  public override void _renderSkillListHeader(Terminal terminal)
  {
    terminal.WriteAt(35, 1, "Comp", UIHue.helpText);
  }

  public override void _renderSkillInList(Terminal terminal, int y, Color color, Spell skill)
  {
    terminal.WriteAt(
        35, y, skill.complexity(_hero.heroClass).ToString().PadLeft(4), color);
  }

  public override void _renderSkillDetails(Terminal terminal, Spell skill)
  {
    terminal.WriteAt(1, 30, "Complexity:", UIHue.secondary);
    if (_hero.skills.isAcquired(skill))
    {
      terminal.WriteAt(13, 30,
          skill.complexity(_hero.heroClass).ToString().PadLeft(3), UIHue.text);
    }
    else
    {
      terminal.WriteAt(
          13, 30, skill.complexity(_hero.heroClass).ToString().PadLeft(3), Hues.red);

      var need = skill.complexity(_hero.heroClass) - _hero.intellect.value;
      terminal.WriteAt(17, 30, $"Need {need} more intellect", UIHue.secondary);
    }

    var level = _skillSet.level(skill);
    terminal.WriteAt(1, 32, "Focus cost:", UIHue.secondary);
    terminal.WriteAt(13, 32,
        skill.focusCost(_hero, level).ToString().PadLeft(3), UIHue.text);

    if (skill.damage != 0)
    {
      terminal.WriteAt(1, 34, "Damage:", UIHue.secondary);
      terminal.WriteAt(13, 34, skill.damage.ToString().PadLeft(3), UIHue.text);
    }

    if (skill.range != 0)
    {
      terminal.WriteAt(1, 36, "Range:", UIHue.secondary);
      terminal.WriteAt(13, 36, skill.range.ToString().PadLeft(3), UIHue.text);
    }
  }
}
