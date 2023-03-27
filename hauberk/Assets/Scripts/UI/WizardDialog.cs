using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

/// Cheat menu.
class WizardDialog : Screen
{
  public Dictionary<string, System.Action> _menuItems = new Dictionary<string, System.Action> { };
  public Game _game;

  public override bool isTransparent => true;

  public WizardDialog(Game _game)
  {
    this._game = _game;

    _menuItems["Map Dungeon"] = _mapDungeon;
    _menuItems["Illuminate Dungeon"] = _illuminateDungeon;
    _menuItems["Drop Item"] = () =>
    {
      terminal.Push(new _WizardDropDialog(_game));
    };
    _menuItems["Spawn Monster"] = () =>
    {
      terminal.Push(new _WizardSpawnDialog(_game));
    };
    _menuItems["Gain Level"] = _gainLevel;
    _menuItems["Train Discipline"] = () =>
    {
      terminal.Push(new _WizardTrainDialog(_game));
    };

    _menuItems["Toggle Show All Monsters"] = () =>
    {
      Debugger.showAllMonsters = !Debugger.showAllMonsters;
      _game.log.cheat($"Show all monsters = {Debugger.showAllMonsters}");
      terminal.Pop();
    };
    _menuItems["Toggle Show Monster Alertness"] = () =>
    {
      Debugger.showMonsterAlertness = !Debugger.showMonsterAlertness;
      _game.log.cheat($"Show monster alertness = {Debugger.showMonsterAlertness}");
      terminal.Pop();
    };
    _menuItems["Toggle Show Hero Volume"] = () =>
    {
      Debugger.showHeroVolume = !Debugger.showHeroVolume;
      _game.log.cheat($"Show hero volume = {Debugger.showHeroVolume}");
      terminal.Pop();
    };
  }

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == InputX.cancel)
    {
      terminal.Pop();
      return true;
    }

    if (shift || alt) return false;

    var index = keyCode - KeyCode.A;
    if (index < 0 || index >= _menuItems.Count) return false;

    var menuItem = _menuItems[_menuItems.Keys.elementAt(index)]!;
    menuItem();
    Dirty();

    // TODO: Invoking a wizard command should mark the hero as a cheater.

    return true;
  }

  public override void Render(Terminal terminal)
  {
    // Draw a box for the contents.
    var width = _menuItems.Keys.ToList()
        .fold<string>(0, (width, name) => Math.Max(width, name.Length));
    Draw.frame(terminal, 0, 0, width + 4, _menuItems.Count + 3);
    terminal.WriteAt(1, 0, "Wizard Menu", UIHue.selection);

    var i = 0;
    foreach (var menuItem in _menuItems.Keys)
    {
      terminal.WriteAt(1, i + 2, " )", UIHue.secondary);
      terminal.WriteAt(
          1, i + 2, "abcdefghijklmnopqrstuvwxyz"[i], UIHue.selection);
      terminal.WriteAt(3, i + 2, menuItem, UIHue.primary);

      i++;
    }

    terminal.WriteAt(0, terminal.height - 1, "[Esc] Exit", UIHue.helpText);
  }

  void _mapDungeon()
  {
    var stage = _game.stage;
    foreach (var pos in stage.bounds)
    {
      // If the tile isn't opaque, explore it.
      if (!stage[pos].blocksView)
      {
        stage[pos].updateExplored(force: true);
        continue;
      }

      // If it is opaque, but it's next to a non-opaque tile (i.e. it's an edge
      // wall), explore it.
      foreach (var neighbor in pos.neighbors)
      {
        if (stage.bounds.contains(neighbor) && !stage[neighbor].blocksView)
        {
          stage[pos].updateExplored(force: true);
          break;
        }
      }
    }
  }

  void _illuminateDungeon()
  {
    var stage = _game.stage;
    foreach (var pos in stage.bounds)
    {
      // If the tile isn't opaque, explore it.
      if (!stage[pos].blocksView)
      {
        stage[pos].addEmanation(255);
      }
    }

    stage.floorEmanationChanged();
    stage.refreshView();
  }

  void _gainLevel()
  {
    if (_game.hero.level == Hero.maxLevel)
    {
      _game.log.cheat("Already at max level.");
    }
    else
    {
      _game.hero.experience = Hero.experienceLevelCost(_game.hero.level + 1);
      _game.hero.refreshProperties();
    }

    Dirty();
  }
}

/// Base class for a dialog that searches for things by name.
abstract class _SearchDialog<T> : Screen
{
  public Game _game;
  string _pattern = "";

  public _SearchDialog(Game _game)
  {
    this._game = _game;
  }

  public override bool isTransparent => true;

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == InputX.cancel)
    {
      terminal.Pop();
      return true;
    }

    if (alt) return false;

    switch (keyCode)
    {
      case KeyCode.Return:
        foreach (var item in _matchedItems)
        {
          _selectItem(item);
        }

        terminal.Pop();
        return true;

      case KeyCode.Delete:
        if (_pattern.isNotEmpty())
        {
          _pattern = _pattern.Substring(0, _pattern.Length - 1);
          Dirty();
        }
        return true;

      case KeyCode.Space:
        _pattern += " ";
        Dirty();
        return true;

      default:
        if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
        {
          _pattern += Char.ConvertFromUtf32((int)keyCode).ToLower();
          Dirty();
          return true;
        }
        else if (keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9)
        {
          var n = keyCode - KeyCode.Alpha0;
          if (n < _matchedItems.Count)
          {
            _selectItem(_matchedItems[n]);
            terminal.Pop();
            return true;
          }
        }
        break;
    }

    return false;
  }

  public override void Render(Terminal terminal)
  {
    // Draw a box for the contents.
    Draw.frame(terminal, 25, 0, 43, 39);
    terminal.WriteAt(26, 0, _question, UIHue.selection);

    terminal.WriteAt(28 + _question.Length, 0, _pattern, UIHue.selection);
    terminal.WriteAt(28 + _question.Length + _pattern.Length, 0, " ",
        UIHue.selection, UIHue.selection);

    var n = 0;
    foreach (var item in _matchedItems)
    {
      if (!_itemName(item).ToLower().Contains(_pattern.ToLower()))
      {
        continue;
      }

      if (n < 10)
      {
        terminal.WriteAt(26, n + 2, n.ToString(), UIHue.selection);
        terminal.WriteAt(27, n + 2, ")", UIHue.disabled);
      }

      var appearance = _itemAppearance(item);
      if (appearance is Glyph)
      {
        terminal.WriteAt(28, n + 2, appearance as string);
      }
      else
      {
        terminal.WriteAt(28, n + 2, "-");
      }
      terminal.WriteAt(30, n + 2, _itemName(item), UIHue.primary);

      n++;
      if (n >= 36) break;
    }

    terminal.WriteAt(0, terminal.height - 1,
        "[0-9] Select, [Return] Select All, [Esc] Exit", UIHue.helpText);
  }

  List<T> _matchedItems => _allItems
      .Where((item) =>
          _itemName(item).ToLower().Contains(_pattern.ToLower()))
      .ToList();

  public virtual string _question => null;

  public virtual List<T> _allItems => null;

  public abstract string _itemName(T item);

  public abstract object _itemAppearance(T item);

  public abstract void _selectItem(T item);
}

class _WizardDropDialog : _SearchDialog<ItemType>
{
  public _WizardDropDialog(Game game) : base(game)
  {
  }

  public override string _question => "Drop what?";

  public override List<ItemType> _allItems => _game.content.items;

  public override string _itemName(ItemType item) => item.name;

  public override object _itemAppearance(ItemType item) => item.appearance;

  public override void _selectItem(ItemType itemType)
  {
    var item = new Item(itemType, itemType.maxStack);
    _game.stage.addItem(item, _game.hero.pos);
    _game.log.cheat("Dropped {1}.", item);
  }
}

class _WizardSpawnDialog : _SearchDialog<Breed>
{
  public _WizardSpawnDialog(Game game) : base(game)
  {
  }

  public override string _question => "Spawn what?";

  public override List<Breed> _allItems => _game.content.breeds;

  public override string _itemName(Breed breed) => breed.name;

  public override object _itemAppearance(Breed breed) => breed.appearance;

  public override void _selectItem(Breed breed)
  {
    var flow = new MotilityFlow(_game.stage, _game.hero.pos, Motility.walk);
    var pos = flow.bestWhere((pos) => (pos - _game.hero.pos) > 6);
    if (pos == null) return;

    var monster = breed.spawn(_game, pos);
    _game.stage.addActor(monster);
  }
}

class _WizardTrainDialog : _SearchDialog<Discipline>
{
  public _WizardTrainDialog(Game game) : base(game)
  {
  }

  public override string _question => "Train which discipline?";

  public override List<Discipline> _allItems
  {
    get
    {
      List<Discipline> rt = new List<Discipline>();
      foreach (var k in _game.content.skills)
      {
        if (k is Discipline)
          rt.Add(k as Discipline);
      }
      return rt;
    }
  }

  public override string _itemName(Discipline discipline) => discipline.name;

  public override object _itemAppearance(Discipline discipline) => null;

  public override void _selectItem(Discipline discipline)
  {
    var level = _game.hero.skills.level(discipline);
    if (level + 1 < discipline.maxLevel)
    {
      var training =
          discipline.trainingNeeded(_game.hero.save.heroClass, level + 1) ?? 0;
      _game.hero.skills.earnPoints(
          discipline, training - _game.hero.skills.points(discipline));
      _game.hero.refreshSkill(discipline);
    }
    else
    {
      _game.log.cheat("Already at max level.");
    }
  }
}
