using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;


// TODO: Split this into multiple panels and/or give it a better name.
// TODO: There's room at the bottom of the panel for something else. Maybe a
// mini-map?
class SidebarPanel : Panel
{
  public static Dictionary<Element, string> _resistLetters = new Dictionary<Element, string>(){
    {Elements.air, "A"},
    {Elements.earth, "E"},
    {Elements.fire, "F"},
    {Elements.water, "W"},
    {Elements.acid, "A"},
    {Elements.cold, "C"},
    {Elements.lightning, "L"},
    {Elements.poison, "P"},
    {Elements.dark, "D"},
    {Elements.light, "L"},
    {Elements.spirit, "S"}
  };

  public GameScreen _gameScreen;

  public SidebarPanel(GameScreen _gameScreen)
  {
    this._gameScreen = _gameScreen;
  }

  public override void renderPanel(Terminal terminal)
  {
    Draw.frame(terminal, 0, 0, terminal.width, terminal.height);

    var game = _gameScreen.game;
    var hero = game.hero;
    terminal.WriteAt(2, 0, $" {hero.save.name} ", UIHue.text);
    terminal.WriteAt(1, 2, $"{hero.save.race.name} {hero.save.heroClass.name}",
        UIHue.primary);

    _drawStats(hero, terminal, 4);

    // TODO: Decide on a consistent set of colors for attributes and use them
    // consistently through the UI.
    _drawHealth(hero, terminal, 7);
    _drawLevel(hero, terminal, 8);
    _drawGold(hero, terminal, 9);

    _drawArmor(hero, terminal, 10);
    _drawDefense(hero, terminal, 11);
    _drawWeapons(hero, terminal, 12);

    _drawFood(hero, terminal, 15);
    _drawFocus(hero, terminal, 16);
    _drawFury(hero, terminal, 17);

    // Draw the nearby monsters.
    terminal.WriteAt(1, 19, "@", _gameScreen.heroColor);
    terminal.WriteAt(3, 19, hero.save.name, UIHue.text);
    _drawHealthBar(terminal, 20, hero);

    var visibleMonsters = _gameScreen.stagePanel.visibleMonsters;
    visibleMonsters.Sort((a, b) =>
    {
      var aDistance = (a.pos - hero.pos).lengthSquared;
      var bDistance = (b.pos - hero.pos).lengthSquared;
      return aDistance.CompareTo(bDistance);
    });

    for (var i = 0; i < 10 && i < visibleMonsters.Count; i++)
    {
      var y = 21 + i * 2;
      if (y >= terminal.height - 2) break;

      var monster = visibleMonsters[i];

      var glyph = monster.appearance as Glyph;
      if (_gameScreen.currentTargetActor == monster)
      {
        glyph = new Glyph(glyph.ch, glyph.back, glyph.fore);
      }

      var name = monster.breed.name;
      if (name.Length > terminal.width - 4)
      {
        name = name.Substring(0, terminal.width - 4);
      }

      terminal.WriteAt(1, y, glyph);
      terminal.WriteAt(
          3,
          y,
          name,
          (_gameScreen.currentTargetActor == monster)
              ? UIHue.selection
              : UIHue.text);

      _drawHealthBar(terminal, y + 1, monster);
    }
  }

  void _drawStats(Hero hero, Terminal terminal, int y)
  {
    var x = 1;
    void drawStat(StatBase stat)
    {
      terminal.WriteAt(x, y, stat.name.Substring(0, 3), UIHue.helpText);
      terminal.WriteAt(
          x, y + 1, stat.value.ToString().PadLeft(3), UIHue.primary);
      x += (terminal.width - 4) / 4;
    }

    drawStat(hero.strength);
    drawStat(hero.agility);
    drawStat(hero.fortitude);
    drawStat(hero.intellect);
    drawStat(hero.will);
  }

  void _drawHealth(Hero hero, Terminal terminal, int y)
  {
    _drawStat(terminal, y, "Health", hero.health, Hues.red, hero.maxHealth, Hues.maroon);
  }

  void _drawLevel(Hero hero, Terminal terminal, int y)
  {
    terminal.WriteAt(1, y, "Level", UIHue.helpText);

    var levelString = hero.level.ToString();
    terminal.WriteAt(
        terminal.width - levelString.Length - 1, y, levelString, Hues.lightAqua);

    if (hero.level < Hero.maxLevel)
    {
      var levelPercent = 100 *
          (hero.experience - Hero.experienceLevelCost(hero.level)) /
          (Hero.experienceLevelCost(hero.level + 1) -
              Hero.experienceLevelCost(hero.level));
      Draw.thinMeter(terminal, 10, y, terminal.width - 14, levelPercent, 100,
          Hues.lightAqua, Hues.aqua);
    }
  }

  void _drawGold(Hero hero, Terminal terminal, int y)
  {
    terminal.WriteAt(1, y, "Gold", UIHue.helpText);
    var heroGold = DartUtils.formatMoney(hero.gold);
    terminal.WriteAt(terminal.width - 1 - heroGold.Length, y, heroGold, Hues.gold);
  }

  void _drawWeapons(Hero hero, Terminal terminal, int y)
  {
    var hits = hero.createMeleeHits(null).ToList();

    var label = hits.Count == 2 ? "Weapons" : "Weapon";
    terminal.WriteAt(1, y, label, UIHue.helpText);

    for (var i = 0; i < hits.Count; i++)
    {
      var hitString = hits[i].damageString;
      // TODO: Show element and other bonuses.
      terminal.WriteAt(
          terminal.width - hitString.Length - 1, y + i, hitString, Hues.carrot);
    }
  }

  void _drawDefense(Hero hero, Terminal terminal, int y)
  {
    var total = 0;
    foreach (var defense in hero.defenses)
    {
      total += defense.amount;
    }

    _drawStat(terminal, y, "Dodge", $"{total}%", Hues.aqua);
  }

  void _drawArmor(Hero hero, Terminal terminal, int y)
  {
    // Show equipment resistances.
    var x = 10;
    foreach (var element in Elements.all)
    {
      if (hero.resistance(element) > 0)
      {
        terminal.WriteAt(x, y, _resistLetters[element]!, Hues.elementColor(element));
        x++;
      }
    }

    var armor = $" {(int)(100 - Attack.getArmorMultiplier(hero.armor) * 100)}%";
    _drawStat(terminal, y, "Armor", armor, Hues.peaGreen);
  }

  void _drawFood(Hero hero, Terminal terminal, int y)
  {
    terminal.WriteAt(1, y, "Food", UIHue.helpText);
    Draw.thinMeter(terminal, 10, y, terminal.width - 11, hero.stomach,
        Option.heroMaxStomach, Hues.tan, Hues.brown);
  }

  void _drawFocus(Hero hero, Terminal terminal, int y)
  {
    // TODO: Show bar once these are tuned.
    // terminal.WriteAt(1, y, 'Focus', UIHue.helpText);
    // Draw.thinMeter(terminal, 10, y, terminal.width - 11, hero.focus,
    //     hero.intellect.maxFocus, blue, darkBlue);
    _drawStat(terminal, y, "Focus", hero.focus, Hues.blue, hero.intellect.maxFocus,
        Hues.darkBlue);
  }

  void _drawFury(Hero hero, Terminal terminal, int y)
  {
    // TODO: Show bar once these are tuned.
    // terminal.WriteAt(1, y, 'Fury', UIHue.helpText);
    // Draw.thinMeter(terminal, 10, y, terminal.width - 11, hero.fury,
    //     hero.strength.maxFury, red, maroon);
    _drawStat(
        terminal, y, "Fury", hero.fury, Hues.red, hero.strength.maxFury, Hues.maroon);
  }

  /// Draws a labeled numeric stat.
  void _drawStat(
      Terminal terminal, int y, string label, Object value, Color valueColor,
      int? max = null, Color? maxColor = null)
  {
    terminal.WriteAt(1, y, label, UIHue.helpText);

    var x = terminal.width - 1;
    if (max != null)
    {
      var maxString = max.ToString();
      x -= maxString.Length;
      terminal.WriteAt(x, y, maxString, maxColor);

      x -= 3;
      terminal.WriteAt(x, y, " / ", maxColor);
    }

    var valueString = value.ToString();
    x -= valueString.Length;
    terminal.WriteAt(x, y, valueString, valueColor);
  }

  /// Draws a health bar for [actor].
  void _drawHealthBar(Terminal terminal, int y, Actor actor)
  {
    // Show conditions.
    var x = 3;

    void drawCondition(string char_, Color fore, Color? back = null)
    {
      // Don't overlap other stuff.
      if (x > 8) return;

      terminal.WriteAt(x, y, char_, fore, back);
      x++;
    }

    if (actor is Monster && (actor as Monster).isAfraid)
    {
      drawCondition("!", Hues.sandal);
    }

    if (actor.poison.isActive)
    {
      switch (actor.poison.intensity)
      {
        case 1:
          drawCondition("P", Hues.sherwood);
          break;
        case 2:
          drawCondition("P", Hues.peaGreen);
          break;
        default:
          drawCondition("P", Hues.mint);
          break;
      }
    }

    if (actor.cold.isActive) drawCondition("C", Hues.lightBlue);
    switch (actor.haste.intensity)
    {
      case 1:
        drawCondition("S", Hues.tan);
        break;
      case 2:
        drawCondition("S", Hues.gold);
        break;
      case 3:
        drawCondition("S", Hues.buttermilk);
        break;
    }

    if (actor.blindness.isActive) drawCondition("B", Hues.darkCoolGray);
    if (actor.dazzle.isActive) drawCondition("D", Hues.lilac);
    if (actor.perception.isActive) drawCondition("V", Hues.ash);

    foreach (var element in Elements.all)
    {
      if (actor.resistances[element]!.isActive)
      {
        drawCondition(
            _resistLetters[element]!, Color.black, Hues.elementColor(element));
      }
    }

    if (Debugger.showMonsterAlertness && actor is Monster)
    {
      var alertness = ((int)((actor as Monster).alertness * 100)).ToString().PadLeft(3);
      terminal.WriteAt(2, y, alertness, Hues.ash);
    }

    Draw.meter(terminal, 10, y, terminal.width - 11, actor.health,
        actor.maxHealth, Hues.red, Hues.maroon);
  }
}
