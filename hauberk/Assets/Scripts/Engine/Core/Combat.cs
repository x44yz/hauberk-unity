using System.Collections;
using System.Collections.Generic;
using System;
using num = System.Double;
using System.Linq;

public class Attack
{

  /// Armor reduces damage by an inverse curve such that increasing armor has
  /// less and less effect. Damage is reduced to the following:
  ///
  ///     armor damage
  ///     ------------
  ///     0     100%
  ///     40    50%
  ///     80    33%
  ///     120   25%
  ///     160   20%
  ///     ...   etc.
  public static num getArmorMultiplier(int armor)
  {
    // Damage is never increased.
    return 1.0 / (1.0 + Math.Max(0, armor) / 40.0);
  }

  /// The thing performing the attack. If `null`, then the attacker will be
  /// used.
  public Noun noun;

  /// A verb string describing the attack: "hits", "fries", etc.
  public string verb;

  public int damage;

  // TODO: Some kind of minimum range would be good to prevent players from
  // using bows at close range and to make bows a little less powerful. However,
  // doing that requires figuring out what happens if a monster is within the
  // minimum range and the hero fires *past* it.
  public int range;

  public Element element;

  public Attack(Noun noun, string verb, int damage, int range = 0, Element element = null)
  {
    this.noun = noun;
    this.verb = verb;
    this.damage = damage;
    this.range = range;
    this.element = element ?? Element.none;
  }

  public bool isRanged => range > 0;

  public Hit createHit() => new Hit(this);

  public override string ToString()
  {
    var result = damage.ToString();
    if (element != Element.none) result = $"{element} {result}";
    if (range > 0) result += $"@{range}";
    return result;
  }
}

public enum HitType { melee, ranged, toss }

public class Hit
{
  public Attack _attack;

  int _strikeBonus = 0;
  double _strikeScale = 1.0;

  double _damageScale = 1.0;
  int _damageBonus = 0;

  Element _brand = Element.none;

  public int range
  {
    get
    {
      if (_attack.range == 0) return 0;

      return (int)Math.Round(Math.Max(1, (float)(_attack.range * _rangeScale)));
    }
  }

  double _rangeScale = 1.0;

  public Element element
  {
    get
    {
      if (_brand != Element.none) return _brand;
      return _attack.element;
    }
  }

  public double averageDamage => _attack.damage * _damageScale + _damageBonus;

  // TODO: This is just used for the game screen weapon display. Show the
  // bonuses and stuff more explicitly there and get rid of this.
  /// The average amount of damage this hit causes, with two decimal points of
  /// precision.
  public string damageString
  {
    get
    {
      return ((int)(averageDamage * 100) / 100f).ToString();
    }
  }

  public Hit(Attack _attack)
  {
    this._attack = _attack;
  }

  public void addStrike(int bonus)
  {
    _strikeBonus += bonus;
  }

  public void scaleStrike(double factor)
  {
    _strikeScale *= factor;
  }

  public void addDamage(int offset)
  {
    _damageBonus += offset;
  }

  public void scaleDamage(double factor)
  {
    _damageScale *= factor;
  }

  public void brand(Element element)
  {
    // TODO: What if it's already branded? How do they compose?
    if (element != Element.none) _brand = element;
  }

  public void scaleRange(double factor)
  {
    _rangeScale *= factor;
  }

  /// Performs a melee [Hit] from [attacker] to [defender] in the course of
  /// [action].
  ///
  /// Returns the amount of damage done if the attack connected or `null` if
  /// it missed.
  public int perform(Action action, Actor attacker, Actor defender, bool canMiss = true)
  {
    // If the attack itself doesn't have a noun ("the arrow hits"), use the
    // attacker ("the wolf bites").
    var attackNoun = (_attack.noun ?? attacker)!;

    // Don't sleep through being attacked.
    if (defender is Hero)
    {
      (defender as Hero).disturb();
    }

    // See if any defense blocks the attack.
    // TODO: Instead of a single "canMiss" flag, consider having each defense
    // define the set of elements it can block and then apply them based on
    // that.
    if (canMiss)
    {
      var strike = Rng.rng.inclusive(1, 100) * _strikeScale + _strikeBonus;

      // Shuffle them so the message shown isn't biased by their order (just
      // their relative amounts).
      var defenses = defender.defenses;
      Rng.rng.shuffle<Defense>(defenses);
      foreach (var defense in defenses)
      {
        strike -= defense.amount;
        if (strike < 0)
        {
          action.log(defense.message, defender, attackNoun);
          return 0;
        }
      }
    }

    // Roll for damage.
    var armor = defender.armor;
    var resistance = defender.resistance(element);
    var damage = _rollDamage(armor, resistance);

    if (damage == 0)
    {
      // Armor cancelled out all damage.
      // TODO: Should still affect monster alertness.
      action.log("{1} do[es] no damage to {2}.", attackNoun, defender);
      return 0;
    }

    if (attacker != null)
    {
      attacker.onGiveDamage(action, defender, damage);
    }

    if (defender.takeDamage(action, damage, attackNoun, attacker))
    {
      return damage;
    }

    // Any resistance cancels all side effects.
    if (resistance <= 0)
    {
      var sideEffect = element.attackAction(damage);
      if (sideEffect != null)
      {
        action.addAction(sideEffect, defender);
      }

      // TODO: Should we log a message to let the player know the side effect
      // was resisted?
    }

    action.addEvent(EventType.hit,
        actor: defender, element: element, other: damage);
    action.log(
        $"{1} {_attack.verb} {2} for {damage} damage.", attackNoun, defender);
    return damage;
  }

  int _rollDamage(int armor, int resistance)
  {
    var resistScale = 1.0 / (1.0 + resistance);

    // Calculate in cents so that we don't do as much rounding until after
    // armor is taken into account.
    var damage = (_attack.damage * _damageScale + _damageBonus) * resistScale;
    var damageCents = (int)(damage * 100);

    var rolled = (double)Rng.rng.triangleInt(damageCents, damageCents / 2);
    rolled *= Attack.getArmorMultiplier(armor);
    return (int)Math.Round(rolled / 100f);
  }
}

/// TODO: Flags for which kinds of attacks (melee, ranged, magic) the dodge
/// can apply to?
public class Defense
{
  public int amount;
  public string message;

  public Defense(int amount, string message)
  {
    this.amount = amount;
    this.message = message;
  }
}

