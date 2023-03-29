using System.Linq;
using Mathf = UnityEngine.Mathf;

class Archery : Discipline, UsableSkill, TargetSkill
{
  // TODO: Tune.
  public override int maxLevel => 20;

  static double _strikeScale(int level) => MathUtils.lerpDouble(level, 1, 20, 0.7, 2.0);

  public override string name => "Archery";

  public override string description =>
      "Kill your foe without risking harm to yourself by unleashing a volley " +
      "of arrows from far away.";

  public override string levelDescription(int level) =>
      $"Scales strike by {(int)(_strikeScale(level) * 100)}%.";

  public string unusableReason(Game game)
  {
    if (_hasBow(game.hero)) return null;

    return "No bow equipped.";
  }

  bool _hasBow(Hero hero) =>
      hero.equipment.weapons.Any((item) => item.type.weaponType == "bow");

  // TODO: Tune.
  public override int baseTrainingNeeded(int level)
  {
    // Reach level 1 immediately so that the hero can begin using the bow.
    level--;

    return 100 * level * level * level;
  }

  /// Focus cost goes down with level.
  public int focusCost(HeroSave hero, int level) => 21 - level;
  public int furyCost(HeroSave hero, int level) => 0;

  public bool canTargetSelf() => false;
  public int getRange(Game game)
  {
    var hit = game.hero.createRangedHit();
    var level = game.hero.skills.level(this);
    hit.scaleStrike(_strikeScale(level));
    return hit.range;
  }

  public Action onGetTargetAction(Game game, int level, Vec target)
  {
    var hit = game.hero.createRangedHit();
    return new ArrowAction(this, target, hit);
  }
}

/// Fires a bolt, a straight line of an elemental attack that stops at the
/// first [Actor] is hits or opaque tile.
class ArrowAction : BoltAction
{
  public Archery _skill;

  public ArrowAction(Archery _skill, Vec target, Hit hit)
      : base(target, hit, canMiss: true)
  {
    this._skill = _skill;
  }

  public override bool onHitActor(Vec pos, Actor target)
  {
    base.onHitActor(pos, target);

    var monster = target as Monster;
    hero.skills.earnPoints(_skill, Mathf.CeilToInt(monster.experience * 1f / 1000f));
    hero.refreshSkill(_skill);
    return true;
  }
}
