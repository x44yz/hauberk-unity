using System;
using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;

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
  public virtual T _modify(T _base) => _base;

  /// Stores the new base [value]. If [value] is different from the current
  /// base value, calls [onChange], passing in the previous value. Does not take
  /// any modification into account.
  public void update(T value, Action<T> onChange)
  {
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

public abstract class StatBase : Property<int>
{
  public HeroSave _hero;

  public string name => _stat.name;
  public virtual Stat _stat => null;
  public virtual string _gainAdjective => "";
  public virtual string _loseAdjective => "";

  public override int _modify(int _base) =>
      Mathf.Clamp(_base + _statOffset + _hero.statBonus(_stat), 1, Stat.max);

  public virtual int _statOffset => 0;

  public void bindHero(HeroSave hero)
  {
    _hero = hero;
    _value = Mathf.Clamp(_hero.race.valueAtLevel(_stat, _hero.level), 1, Stat.max);
  }

  public void refresh(Game game)
  {
    var newValue = Mathf.Clamp(
        _hero.race.valueAtLevel(_stat, _hero.level), 1, Stat.max);
    update(newValue, (previous) =>
    {
      var gain = newValue - previous;
      if (gain > 0)
      {
        game.log
            .gain($"You feel {_gainAdjective!} Your {name} increased by {gain}.");
      }
      else
      {
        game.log.error(
            $"You feel {_loseAdjective!} Your {name} decreased by {-gain}.");
      }
    });
  }
}

public class Strength : StatBase
{
  public override Stat _stat => Stat.strength;
  public override string _gainAdjective => "mighty";
  public override string _loseAdjective => "weak";
  public override int _statOffset => -_hero.weight;

  public int maxFury
  {
    get
    {
      if (value <= 10) return MathUtils.lerpInt(value, 1, 10, 40, 100);
      return MathUtils.lerpInt(value, 10, 60, 100, 200);
    }
  }

  public double tossRangeScale
  {
    get
    {
      if (value <= 20) return MathUtils.lerpDouble(value, 1, 20, 0.1, 1.0);
      if (value <= 30) return MathUtils.lerpDouble(value, 20, 30, 1.0, 1.5);
      if (value <= 40) return MathUtils.lerpDouble(value, 30, 40, 1.5, 1.8);
      if (value <= 50) return MathUtils.lerpDouble(value, 40, 50, 1.8, 2.0);
      return MathUtils.lerpDouble(value, 50, 60, 2.0, 2.1);
    }
  }

  /// Calculates the melee damage scaling factor based on the hero's strength
  /// relative to the weapon's [heft].
  public double heftScale(int heft)
  {
    var relative = Mathf.Clamp(value - heft, -20, 50);

    if (relative < -10)
    {
      return MathUtils.lerpDouble(relative, -20, -10, 0.05, 0.3);
    }
    else if (relative < 0)
    {
      // Note that there is an immediate step down to 0.8 at -1.
      return MathUtils.lerpDouble(relative, -10, -1, 0.3, 0.8);
    }
    else if (relative < 30)
    {
      return MathUtils.lerpDouble(relative, 0, 30, 1.0, 2.0);
    }
    else
    {
      return MathUtils.lerpDouble(relative, 30, 50, 2.0, 3.0);
    }
  }
}

public class Agility : StatBase
{
  public override Stat _stat => Stat.agility;
  public override string _gainAdjective => "dextrous";
  public override string _loseAdjective => "clumsy";

  // TODO: Subtract encumbrance.

  public int dodgeBonus
  {
    get
    {
      if (value <= 10) return MathUtils.lerpInt(value, 1, 10, -50, 0);
      if (value <= 30) return MathUtils.lerpInt(value, 10, 30, 0, 20);
      return MathUtils.lerpInt(value, 30, 60, 20, 60);
    }
  }

  public int strikeBonus
  {
    get
    {
      if (value <= 10) return MathUtils.lerpInt(value, 1, 10, -30, 0);
      if (value <= 30) return MathUtils.lerpInt(value, 10, 30, 0, 20);
      return MathUtils.lerpInt(value, 30, 60, 20, 50);
    }
  }
}

// TODO: "Vitality"?
public class Fortitude : StatBase
{
  public override Stat _stat => Stat.fortitude;
  public override string _gainAdjective => "tough";
  public override string _loseAdjective => "sickly";

  public int maxHealth => (int)(Mathf.Pow(value, 1.4f) + 1.23 * value + 18);
}

public class Intellect : StatBase
{
  public override Stat _stat => Stat.intellect;
  public override string _gainAdjective => "smart";
  public override string _loseAdjective => "stupid";

  public int maxFocus
  {
    get
    {
      if (value <= 10) return MathUtils.lerpInt(value, 1, 10, 40, 100);
      return MathUtils.lerpInt(value, 10, 60, 100, 200);
    }
  }

  public double spellFocusScale(int complexity)
  {
    var relative = value - Mathf.Clamp(complexity, 0, 50);
    return MathUtils.lerpDouble(relative, 0, 50, 1.0, 0.2);
  }
}

public class Will : StatBase
{
  public override Stat _stat => Stat.will;
  public override string _gainAdjective => "driven";
  public override string _loseAdjective => "foolish";

  /// Scales how much focus is lost when taking damage.
  public double damageFocusScale
  {
    get
    {
      if (value <= 10) return MathUtils.lerpDouble(value, 1, 10, 800, 400);
      return MathUtils.lerpDouble(value, 10, 60, 400, 80);
    }
  }

  /// Scales how much fury is lost when regenerating focus.
  public double restFuryScale
  {
    get
    {
      if (value <= 10) return MathUtils.lerpDouble(value, 1, 10, 4.0, 1.0);
      return MathUtils.lerpDouble(value, 10, 60, 1.0, 0.2);
    }
  }
}

