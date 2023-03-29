using Mathf = UnityEngine.Mathf;

public class BattleHardening : Discipline
{
  public override int maxLevel => 40;

  public override string description => "Years of taking hits have turned your skin as hard as cured leather.";

  public override string name => "Battle Hardening";

  public override void takeDamage(Hero hero, int damage)
  {
    hero.discoverSkill(this);

    // A point is one tenth of the hero's health.
    var points = Mathf.CeilToInt(10f * damage / hero.maxHealth);

    hero.skills.earnPoints(this, points);
    hero.refreshSkill(this);
  }

  public override int modifyArmor(HeroSave hero, int level, int armor) => armor + level;

  public override string levelDescription(int level) => $"Increases armor by {level}.";

  public override int baseTrainingNeeded(int level) => Mathf.CeilToInt(60 * Mathf.Pow(level, 1.5f));
}