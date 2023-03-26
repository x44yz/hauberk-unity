using System.Collections.Generic;

class Skills
{
  /// All of the known skills.
  public static List<Skill> m_all;
  public static List<Skill> all
  {
    get
    {
      if (m_all == null)
      {
        m_all = new List<Skill>(){
            // Disciplines.
                new BattleHardening(),
                new DualWield(),

                // Masteries.
                new Archery(),
                new AxeMastery(),
                new ClubMastery(),
                new SpearMastery(),
                new Swordfighting(),
                new WhipMastery(),

                // Slays.
                new SlayDiscipline("Animals", "animal"),
                new SlayDiscipline("Bugs", "bug"),
                new SlayDiscipline("Dragons", "dragon"),
                new SlayDiscipline("Fae Folk", "fae"),
                new SlayDiscipline("Goblins", "goblin"),
                new SlayDiscipline("Humans", "human"),
                new SlayDiscipline("Jellies", "jelly"),
                new SlayDiscipline("Kobolds", "kobold"),
                new SlayDiscipline("Plants", "plant"),
                new SlayDiscipline("Saurians", "saurian"),
                new SlayDiscipline("Undead", "undead"),

                // Spells.
                // Divination.
                new SenseItems(),

                // Conjuring.
                new Flee(),
                new Escape(),
                new Disappear(),

                // Sorcery.
                new Icicle(),
                new BrilliantBeam(),
                new Windstorm(),
                new FireBarrier(),
                new TidalWave(),
            };
      }
      return m_all;
    }
  }

  static Dictionary<string, Skill> m_byName;
  public static Dictionary<string, Skill> _byName
  {
    get
    {
      if (m_byName == null)
      {
        m_byName = new Dictionary<string, Skill>();
        foreach (var skill in all)
          m_byName.Add(skill.name, skill);
      }
      return m_byName;
    }
  }

  public static Skill find(string name)
  {
    var skill = _byName[name];
    if (skill == null) throw new System.Exception($"Unknown skill '{name}'.");
    return skill;
  }
}
