using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// A derived property of the hero that needs to log a message when it changes.
///
/// If some property of the hero cannot be recalcuted based on other state,
/// then it is stored directly in the hero: experience points, equipment, etc.
///
/// If a property is calculated based on other state but doesn't notify the
/// user when it changes, then it can just be a getter: weight, stomach, etc.
///
/// The remaining properties use this. It stores the previously-calculated
/// value so that we can tell when a recalculation has actually changed it.
public class Property<T> where T : IComparable
{
  public T _value;

  /// The current modified value.
  public T value => _modify(_value!);

  /// A subclass can override this to modify the observed value. The updating
  /// and notifications are based on the raw base value.
  public T _modify(T _base) => _base;

  /// Stores the new base [value]. If [value] is different from the current
  /// base value, calls [onChange], passing in the previous value. Does not take
  /// any modification into account.
  protected void update(T value, Action<T> onChange) {
    if (_value.Equals(value)) return;

    var previous = _value;
    _value = value;

    // Don't notify when first initialized.
    if (previous != null) onChange(previous);
  }
}

public class Stat 
{
  public static int max = 60;

  public static Stat strength = new Stat("Strength");
  public static Stat agility = new Stat("Agility");
  public static Stat fortitude = new Stat("Fortitude");
  public static Stat intellect = new Stat("Intellect");
  public static Stat will = new Stat("Will");

  public static Stat[] all = new Stat[]{
    strength,
    agility,
    fortitude,
    intellect,
    will,
  };

  public string name;

  public Stat(string name)
  {
    this.name = name;
  }
}

abstract class StatBase : Property<int> 
{
  public HeroSave _hero;

  string name => _stat.name;

  Stat _stat;

  string _gainAdjective;

  string _loseAdjective;

  int _modify(int _base) =>
      (_base + _statOffset + _hero.statBonus(_stat)).clamp(1, Stat.max);

  int _statOffset => 0;

  void bindHero(HeroSave hero) {
    _hero = hero;
    _value = _hero.race.valueAtLevel(_stat, _hero.level).clamp(1, Stat.max);
  }

  void refresh(Game game) {
    var newValue =
        _hero.race.valueAtLevel(_stat, _hero.level).clamp(1, Stat.max);
    update(newValue, (previous) {
      var gain = newValue - previous;
      if (gain > 0) {
        game.log
            .gain("You feel $_gainAdjective! Your $name increased by $gain.");
      } else {
        game.log.error(
            "You feel $_loseAdjective! Your $name decreased by ${-gain}.");
      }
    });
  }
}

class Strength : StatBase 
{
  Stat _stat => Stat.strength;

  string _gainAdjective => "mighty";

  string _loseAdjective => "weak";

  int _statOffset => -_hero.weight;

  int maxFury {
    get {
        if (value <= 10) return lerpInt(value, 1, 10, 40, 100);
        return lerpInt(value, 10, 60, 100, 200);
    }
  }

  double tossRangeScale => {
    if (value <= 20) return lerpDouble(value, 1, 20, 0.1, 1.0);
    if (value <= 30) return lerpDouble(value, 20, 30, 1.0, 1.5);
    if (value <= 40) return lerpDouble(value, 30, 40, 1.5, 1.8);
    if (value <= 50) return lerpDouble(value, 40, 50, 1.8, 2.0);
    return lerpDouble(value, 50, 60, 2.0, 2.1);
  }

  /// Calculates the melee damage scaling factor based on the hero's strength
  /// relative to the weapon's [heft].
  double heftScale(int heft) => {
    var relative = (value - heft).clamp(-20, 50);

    if (relative < -10) {
      return lerpDouble(relative, -20, -10, 0.05, 0.3);
    } else if (relative < 0) {
      // Note that there is an immediate step down to 0.8 at -1.
      return lerpDouble(relative, -10, -1, 0.3, 0.8);
    } else if (relative < 30) {
      return lerpDouble(relative, 0, 30, 1.0, 2.0);
    } else {
      return lerpDouble(relative, 30, 50, 2.0, 3.0);
    }
  }
}

class Agility : StatBase {
  Stat _stat => Stat.agility;

  string _gainAdjective => "dextrous";

  string _loseAdjective => "clumsy";

  // TODO: Subtract encumbrance.

  int dodgeBonus => {
    if (value <= 10) return lerpInt(value, 1, 10, -50, 0);
    if (value <= 30) return lerpInt(value, 10, 30, 0, 20);
    return lerpInt(value, 30, 60, 20, 60);
  }

  int strikeBonus => {
    if (value <= 10) return lerpInt(value, 1, 10, -30, 0);
    if (value <= 30) return lerpInt(value, 10, 30, 0, 20);
    return lerpInt(value, 30, 60, 20, 50);
  }
}

// TODO: "Vitality"?
class Fortitude : StatBase {
  Stat _stat => Stat.fortitude;

  string _gainAdjective => "tough";

  string _loseAdjective => "sickly";

  int maxHealth => (math.pow(value, 1.4) + 1.23 * value + 18).toInt();
}

class Intellect : StatBase {
  Stat _stat => Stat.intellect;

  string _gainAdjective => "smart";

  string _loseAdjective => "stupid";

  int maxFocus {
    if (value <= 10) return lerpInt(value, 1, 10, 40, 100);
    return lerpInt(value, 10, 60, 100, 200);
  }

  double spellFocusScale(int complexity) {
    var relative = value - complexity.clamp(0, 50);
    return lerpDouble(relative, 0, 50, 1.0, 0.2);
  }
}

class Will : StatBase {
  Stat _stat => Stat.will;

  string _gainAdjective => "driven";

  string _loseAdjective => "foolish";

  /// Scales how much focus is lost when taking damage.
  double damageFocusScale {
    if (value <= 10) return lerpDouble(value, 1, 10, 800, 400);
    return lerpDouble(value, 10, 60, 400, 80);
  }

  /// Scales how much fury is lost when regenerating focus.
  double restFuryScale {
    if (value <= 10) return lerpDouble(value, 1, 10, 4.0, 1.0);
    return lerpDouble(value, 10, 60, 1.0, 0.2);
  }
}

