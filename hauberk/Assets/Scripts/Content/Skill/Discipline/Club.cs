using Mathf = UnityEngine.Mathf;

class ClubMastery : MasteryDiscipline, UsableSkill, DirectionSkill
{
  // TODO: Tune.
  static double _bashScale(int level) => MathUtils.lerpDouble(level, 1, 10, 1.0, 2.0);

  // TODO: Better name.
  public override string name => "Club Mastery";

  public override string useName => "Club Bash";

  public override string description =>
      "Bludgeons may not be the most sophisticated of weapons, but what they " +
      "lack in refinement, they make up for in brute force.";

  public override string weaponType => "club";

  public override string levelDescription(int level)
  {
    // TODO: Describe scale.
    return base.levelDescription(level) + " Bashes the enemy away.";
  }

  public int focusCost(HeroSave hero, int level) => 0;
  public int furyCost(HeroSave hero, int level) => 20;
  public string unusableReason(Game game) => null;

  public Action onGetDirectionAction(Game game, int level, Direction dir)
  {
    return new BashAction(dir, ClubMastery._bashScale(level));
  }
}

/// A melee attack that attempts to push back the defender.
class BashAction : MasteryAction
{
  public Direction _dir;
  int _step = 0;
  int? _damage = 0;

  public BashAction(Direction _dir, double scale) : base(scale)
  {
    this._dir = _dir;
  }

  public override bool isImmediate => false;
  public override string weaponType => "club";

  public override ActionResult onPerform()
  {
    if (_step == 0)
    {
      _damage = attack(actor!.pos + _dir);

      // If the hit missed, no pushback.
      if (_damage == null) return ActionResult.success;
    }
    else if (_step == 1)
    {
      // Push the defender.
      var defender = game.stage.actorAt(actor!.pos + _dir);

      // Make sure the defender is still there. Could have died.
      if (defender == null) return ActionResult.success;

      var dest = actor!.pos + _dir + _dir;

      // TODO: Strength bonus?
      var chance = 300 * _damage.Value / defender.maxHealth;
      chance = Mathf.Clamp(chance, 5, 100);

      if (defender.canEnter(dest) && Rng.rng.percent(chance))
      {
        defender.pos = dest;
        defender.energy.energy = 0;
        log("{1} is knocked back!", defender);
        addEvent(EventType.knockBack, pos: actor!.pos + _dir, dir: _dir);
      }
    }
    else
    {
      addEvent(EventType.pause);
    }

    _step++;
    return doneIf(_step > 10);
  }

  public override string ToString() => $"{actor} bashes {_dir}";
}

