using System;
using System.Collections.Generic;
using System.Linq;
using Color = UnityEngine.Color;
using Input = UnityEngine.Input;
using UnityTerminal;

class ExitPopup : Popup {
  public HeroSave _save;
  public Game _game;
  public List<_AnimatedValue> _values = new List<_AnimatedValue>();

  public int width => 38;
  public int height => 19;

  Dictionary<string, string> helpKeys => new Dictionary<string, string>()
  {
    {"OK", "Return to town"}
  };

  public ExitPopup(HeroSave _save, Game _game) 
  {
    this._save = _save;
    this._game = _game;

    var hero = _game.hero;

    _values.Add(new _AnimatedValue(5, "Gold", hero.gold - _save.gold, Hues.gold));
    _values.Add(new _AnimatedValue(
        6, "Experience", hero.experience - _save.experience, Hues.peaGreen));
    _values
        .Add(new _AnimatedValue(7, "Levels", hero.level - _save.level, Hues.lightAqua));

    _values.Add(new _AnimatedValue(
        9, "Strength", hero.strength.value - _save.strength.value, Hues.blue));
    _values.Add(new _AnimatedValue(
        10, "Agility", hero.agility.value - _save.agility.value, Hues.blue));
    _values.Add(new _AnimatedValue(
        11, "Fortitude", hero.fortitude.value - _save.fortitude.value, Hues.blue));
    _values.Add(new _AnimatedValue(
        12, "Intellect", hero.intellect.value - _save.intellect.value, Hues.blue));
    _values.Add(
        new _AnimatedValue(13, "Will", hero.will.value - _save.will.value, Hues.blue));

    var slain = hero.lore.allSlain - _save.lore.allSlain;
    var remainingMonsters =
        _game.stage.actors.Where((actor) => (actor is Hero) == false).ToList().Count;
    _values.Add(new _AnimatedValue(17, "Monsters", slain, Hues.red,
        total: slain + remainingMonsters));
  }

  public override bool isTransparent => true;

  public override void HandleInput()
  {
    if (Input.GetKeyDown(InputX.ok)) return;

    // Remember that this depth was reached.
    _game.hero.save.maxDepth = Math.Max(_save.maxDepth, _game.depth);

    terminal.Pop();
  }

  public override void Tick(float dt) {
    foreach (var value in _values) {
      if (value.update()) Dirty();
    }
  }

  void renderPopup(Terminal terminal) {
    terminal.WriteAt(1, 1, "You survived depth ${_game.depth}!", UIHue.text);

    terminal.WriteAt(1, 3, "You gained:", UIHue.text);
    terminal.WriteAt(1, 15, "You slayed:", UIHue.text);

    foreach (var value in _values) {
      terminal.WriteAt(
          5, value.y, "................................", UIHue.disabled);
      terminal.WriteAt(5, value.y, "${value.name}:",
          value.value == 0 ? UIHue.disabled : UIHue.primary);

      var number = value.current.tostring();
      if (value.total != null) {
        var total = value.total.tostring();
        terminal.writeAt(
            terminal.width - 1 - total.length, value.y, total, value.color);
        terminal.writeAt(
            terminal.width - 1 - total.length - 3, value.y, " / ", value.color);
        terminal.writeAt(terminal.width - 4 - total.length - number.length,
            value.y, number, value.color);
      } else {
        terminal.writeAt(terminal.width - 1 - number.length, value.y, number,
            value.value == 0 ? UIHue.disabled : value.color);
      }
    }

    // TODO: Skills.
    // TODO: Items?
    // TODO: Show how much of stage was explored.
    // TODO: Slain uniques.
    // TODO: Achievements?
  }
}

class _AnimatedValue {
  public int y;
  public string name;
  public int value;
  public Color color;
  public int? total;

  int current;

  public _AnimatedValue(int y, string name, int value, Color color, int? total = null)
  {
    this.y = y;
    this.name = name;
    this.value = value;
    this.color = color;
    this.total = total;
    current = 0;
  }

  public bool update() {
    if (current >= value) return false;

    if (value > 200) {
      current += Rng.rng.round(value / 200);
      if (current > value) current = value;
    } else {
      current++;
    }

    return true;
  }
}
