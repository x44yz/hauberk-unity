using System.Collections;
using System.Collections.Generic;


/// When the player is playing the game inside a dungeon, he is using a [Hero].
/// When outside of the dungeon on the menu screens, though, only a subset of
/// the hero's data persists (for example, there is no position when not in a
/// dungeon). This class stores that state.
// TODO: This is no longer true with the town. Now that the game plays more like
// a classic roguelike, it's weird that some hero state (hunger, log,
// conditions) evaporates when the hero leaves and enters the dungeon. Need to
// figure out what gets saved and what doesn't now.
public class HeroSave
{
  public string name;
  public RaceStats race;
  public HeroClass heroClass;

  public int level => Hero.experienceLevel(experience);

  Inventory _inventory = new Inventory(ItemLocation.inventory, Option.inventoryCapacity);

  public Inventory inventory => _inventory;

  Equipment _equipment = new Equipment();

  public Equipment equipment => _equipment;

  /// Items in the hero's home.
  public Inventory home => _home;
  Inventory _home = new Inventory(ItemLocation.home, Option.homeCapacity);

  /// Items in the hero's crucible.
  public Inventory crucible => _crucible;
  Inventory _crucible = new Inventory(ItemLocation.crucible, Option.crucibleCapacity);

  /// The current inventories of all the shops.
  public Dictionary<Shop, Inventory> shops;

  public int experience = 0;

  public SkillSet skills;

  /// How much gold the hero has.
  public int gold = Option.heroGoldStart;

  /// The lowest depth that the hero has successfully explored and exited.
  public int maxDepth = 0;

  public Lore lore => _lore;
  Lore _lore;

  public Strength strength = new Strength();
  public Agility agility = new Agility();
  public Fortitude fortitude = new Fortitude();
  public Intellect intellect = new Intellect();
  public Will will = new Will();

  public int emanationLevel
  {
    get
    {
      var level = 0;
      // Add the emanation of all equipment.
      foreach (var item in equipment)
      {
        level += item.emanationLevel;
      }
      return level;
    }
  }

  public int armor
  {
    get
    {
      var total = 0;
      foreach (var item in equipment)
      {
        total += item.armor;
      }

      foreach (var skill in skills.acquired)
      {
        total = skill.modifyArmor(this, skills.level(skill), total);
      }

      return total;
    }
  }

  /// The total weight of all equipment.
  public int weight
  {
    get
    {
      var total = 0;
      foreach (var item in equipment)
      {
        total += item.weight;
      }

      return total;
    }
  }

  public HeroSave(string name, Race race, HeroClass heroClass)
  {
    this.name = name;
    this.heroClass = heroClass;
    this.race = race.rollStats();
    shops = new Dictionary<Shop, Inventory>();
    skills = new SkillSet();
    _lore = new Lore();

    _bindStats();
  }

  // TODO: Rename.
  public HeroSave(
      string name,
      RaceStats race,
      HeroClass heroClass,
      Inventory _inventory,
      Equipment _equipment,
      Inventory _home,
      Inventory _crucible,
      Dictionary<Shop, Inventory> shops,
      int experience,
      SkillSet skills,
      Lore _lore,
      int gold,
      int maxDepth)
  {
    this.name = name;
    this.race = race;
    this.heroClass = heroClass;
    this._inventory = _inventory;
    this._equipment = _equipment;
    this._home = _home;
    this._crucible = _crucible;
    this.shops = shops;
    this.experience = experience;
    this.skills = skills;
    this._lore = _lore;
    this.gold = gold;
    this.maxDepth = maxDepth;

    _bindStats();
  }

  /// Move data from [hero] into this object. This should be called when the
  /// [Hero] has successfully completed a stage and his changes need to be
  /// "saved".
  public void takeFrom(Hero hero)
  {
    _inventory = hero.inventory;
    _equipment = hero.equipment;
    experience = hero.experience;
    gold = hero.gold;
    skills = hero.skills;
    _lore = hero.lore;
    maxDepth = hero.save.maxDepth;
  }

  public HeroSave clone() => new HeroSave(
      name,
      race,
      heroClass,
      inventory.clone(),
      equipment.clone(),
      // TODO: Assumes home doesn't change in game.
      home,
      // TODO: Assumes home doesn't change in game.
      crucible,
      // TODO: Assumes shops don't change in game.
      shops,
      experience,
      skills.clone(),
      _lore.clone(),
      gold,
      maxDepth);

  /// Gets the total permament resistance provided by all equipment.
  public int equipmentResistance(Element element)
  {
    // TODO: If class or race can affect this, add it in.
    var resistance = 0;

    foreach (var item in equipment)
    {
      resistance += item.resistance(element);
    }

    // TODO: Unify this with onDefend().

    return resistance;
  }

  /// Gets the total modifiers to [stat] provided by all equipment.
  public int statBonus(Stat stat)
  {
    var bonus = 0;

    // Let equipment modify it.
    foreach (var item in equipment)
    {
      if (item.prefix != null) bonus += item.prefix!.statBonus(stat);
      if (item.suffix != null) bonus += item.suffix!.statBonus(stat);
    }

    return bonus;
  }

  void _bindStats()
  {
    strength.bindHero(this);
    agility.bindHero(this);
    fortitude.bindHero(this);
    intellect.bindHero(this);
    will.bindHero(this);
  }
}

