using System.Collections;
using System.Collections.Generic;


/// The hero's class.
public class HeroClass
{
  public string name;

  public string description;

  public Dictionary<Skill, double> _proficiency;

  /// Generates items a hero of this class should start with.
  public Drop startingItems;

  public HeroClass(string name, string description, Dictionary<Skill, double> _proficiency, Drop startingItems)
  {
    this.name = name;
    this.description = description;
    this._proficiency = _proficiency;
    this.startingItems = startingItems;
  }

  /// How adept heroes of this class are at a given skill.
  ///
  /// A proficiency of 1.0 is normal. Zero means "can't acquire at all". Numbers
  /// larger than 1.0 make the skill easier to acquire or more powerful.
  public double proficiency(Skill skill)
  {
    double val = 1.0;
    _proficiency.TryGetValue(skill, out val);
    return val;
  }
}
