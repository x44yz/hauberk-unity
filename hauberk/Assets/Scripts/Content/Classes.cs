using System;
using System.Collections.Generic;

class Classes
{
  // TODO: Tune battle-hardening.
  // TODO: Better starting items?
  public static HeroClass adventurer = _class("Adventurer", "TODO", DropUtils.parseDrop("item"),
      masteries: 0.5, spells: 0.2);
  public static HeroClass warrior = _class("Warrior", "TODO", DropUtils.parseDrop("weapon"),
      masteries: 1.0, spells: 0.0);
  // TODO: Different book for generalist mage versus sorceror?
  public static HeroClass mage = _class(
      "Mage", "TODO", DropUtils.parseDrop("Spellbook \"Elemental Primer\""),
      masteries: 0.2, spells: 1.0);

  // TODO: Add these once their skill types are working.
  //  static final rogue = new HeroClass("Rogue", "TODO");
  //  static final priest = new HeroClass("Priest", "TODO");

  // TODO: Specialist subclasses.

  /// All of the known classes.
  public static List<HeroClass> all = new List<HeroClass> { adventurer, warrior, mage };

  public static HeroClass _class(String name, String description, Drop startingItems,
    double masteries, double spells)
  {
    var proficiencies = new Dictionary<Skill, double> { };

    foreach (var skill in Skills.all)
    {
      var proficiency = 1.0;
      if (skill is MasteryDiscipline) proficiency *= masteries;
      if (skill is Spell) proficiency *= spells;

      proficiencies[skill] = proficiency;
    }

    return new HeroClass(name, description, proficiencies, startingItems);
  }
}

