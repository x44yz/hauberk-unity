using num = System.Double;

class HealMove : Move
{
  /// How much health to restore.
  public int _amount;

  public override num experience => _amount;

  public HealMove(num rate, int _amount) : base(rate)
  {
    this._amount = _amount;
  }

  public override bool shouldUse(Monster monster)
  {
    // Heal if it could heal the full amount, or it's getting close to death.
    return (monster.health * 1f / monster.maxHealth < 0.25) ||
        (monster.maxHealth - monster.health >= _amount);
  }

  public override Action onGetAction(Monster monster)
  {
    return new HealAction(_amount);
  }

  public override string ToString() => $"Heal {_amount} rate: {rate}";
}
