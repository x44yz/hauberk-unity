using System.Collections;
using System.Collections.Generic;

using System.Linq;

class Swordfighting : MasteryDiscipline
{
  static int _parryDefense(int level) => MathUtils.lerpInt(level, 1, 20, 1, 20);

  public override string name => "Swordfighting";

  public override string description =>
      "The most elegant tool for the most refined of martial arts.";

  public override string weaponType => "sword";

  public override string levelDescription(int level) =>
      base.levelDescription(level) +
      $" Parrying increases dodge by {_parryDefense(level)}.";

  public override Defense getDefense(Hero hero, int level)
  {
    var swords = hero.equipment.weapons
        .Where((weapon) => weapon.type.weaponType == "sword")
        .Count();

    // No parrying if not using a sword.
    if (swords == 0) return null;

    // Dual-wielding swords doubles the parry.
    return new Defense(_parryDefense(level) * swords, "{1} parr[y|ies] {2}.");
  }
}
