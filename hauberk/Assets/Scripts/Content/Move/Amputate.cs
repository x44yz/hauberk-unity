using System;
using System.Collections.Generic;
using num = System.Double;

/// Splits a part of a monster off into a new monster.
///
/// For example, a skeleton can amputate into a decapitated skeleton and a
/// skull.
class AmputateMove : Move
{
  public override num experience => _body.breed.maxHealth * 0.5;

  /// The breed the remaining body turns into.
  public BreedRef _body;

  /// The spawned monster representing the body part.
  public BreedRef _part;

  public string _message;

  public AmputateMove(BreedRef _body, BreedRef _part, string _message) : base(1.0)
  {
    this._body = _body;
    this._part = _part;
    this._message = _message;
  }

  /// Doesn't spontaneously amputate.
  public override bool shouldUse(Monster monster) => false;

  public override bool shouldUseOnDamage(Monster monster, int damage)
  {
    // Doing more damage increases the odds.
    var odds = damage * 1f / monster.maxHealth;
    if (Rng.rng.rfloat(2.0) <= odds) return true;

    // Getting closer to death increases the odds.
    odds = monster.health * 1f / monster.maxHealth;
    if (Rng.rng.rfloat(2.0) <= odds) return true;

    return false;
  }

  public override Action onGetAction(Monster monster) =>
      new AmputateAction(_body.breed, _part.breed, _message);

  public override string ToString() => $"Amputate {_body.breed.name} + {_part.breed.name}";
}
