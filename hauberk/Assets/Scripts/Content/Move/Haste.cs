using System;
using System.Collections.Generic;
using num = System.Double;

class HasteMove : Move
{
  public int _duration;
  public int _speed;

  public override num experience => _duration * _speed;

  public HasteMove(num rate, int _duration, int _speed) : base(rate)
  {
    this._duration = _duration;
    this._speed = _speed;
  }

  public override bool shouldUse(Monster monster)
  {
    // Don't use if already hasted.
    return !monster.haste.isActive;
  }

  public override Action onGetAction(Monster monster) => new HasteAction(_duration, _speed);

  public override string ToString() => $"Haste {_speed} for {_duration} turns rate: {rate}";
}
