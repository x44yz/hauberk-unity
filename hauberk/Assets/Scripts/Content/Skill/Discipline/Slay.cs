using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;

class SlayDiscipline : Discipline
{
  // TODO: Tune.
  public override int maxLevel => 20;

  public string _displayName;
  public string _breedGroup;

  // TODO: Implement description.
  public override string description => "TODO: Implement description.";

  public override string discoverMessage =>
      $"{1} are eager to learn to slay {_displayName.ToLower()}.";

  public override string name => $"Slay {_displayName}";

  public SlayDiscipline(string _displayName, string _breedGroup)
  {
    this._displayName = _displayName;
    this._breedGroup = _breedGroup;
  }

  double _damageScale(int level) => MathUtils.lerpDouble(level, 1, maxLevel, 1.05, 2.0);

  public override void seeBreed(Hero hero, Breed breed)
  {
    if (!Monsters.breeds.hasTag(breed.name, _breedGroup)) return;
    hero.discoverSkill(this);
  }

  public override void killMonster(Hero hero, Action action, Monster monster)
  {
    if (!Monsters.breeds.hasTag(monster.breed.name, _breedGroup)) return;

    hero.skills.earnPoints(this, Mathf.CeilToInt(monster.experience / 1000f));
    // TODO: Having to call this manually every place we call earnPoints()
    // is lame. Fix?
    hero.refreshSkill(this);
  }

  public override void modifyAttack(Hero hero, Monster monster, Hit hit, int level)
  {
    if (monster == null) return;
    if (!Monsters.breeds.hasTag(monster.breed.name, _breedGroup)) return;

    // TODO: Tune.
    hit.scaleDamage(_damageScale(level));
  }

  public override string levelDescription(int level)
  {
    var damage = (int)((_damageScale(level) - 1.0) * 100);
    return $"Melee attacks inflict {damage}% more damage against {_displayName.ToLower()}.";
  }

  // TODO: Tune.
  public override int baseTrainingNeeded(int level) => 100 * level * level * level;
}
