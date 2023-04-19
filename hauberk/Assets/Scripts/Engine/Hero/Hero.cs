using System;
using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;
using System.Linq;

/// The main player-controlled [Actor]. The player's avatar in the game world.
public class Hero : Actor
{
  /// The highest level the hero can reach.
  public const int maxLevel = 50;

  public HeroSave save;

  /// Monsters the hero has already seen. Makes sure we don't double count them.
  public HashSet<Monster> _seenMonsters = new HashSet<Monster>();

  Behavior _behavior;

  /// Damage scales for each weapon being wielded, based on the weapon, other
  /// equipment, and stats.
  ///
  /// This list parallels the sequence returned by `equipment.weapons`.
  public List<Property<double>> _heftScales =
    new List<Property<double>>() { new Property<double>(), new Property<double>() };

  /// How full the hero is.
  ///
  /// The hero raises this by eating food. It reduces constantly. The hero can
  /// only rest while its non-zero.
  ///
  /// It starts half-full, presumably the hero had a nice meal before heading
  /// off to adventure.
  public int stomach
  {
    get { return _stomach; }
    set
    {
      _stomach = Mathf.Clamp(value, 0, Option.heroMaxStomach);
    }
  }
  int _stomach = Option.heroMaxStomach / 2;

  /// How calm and centered the hero is. Mental skills like spells spend focus.
  public int focus => _focus;
  int _focus = 0;

  /// How enraged the hero is. Physical skills like active disciplines spend
  /// fury.
  public int fury => _fury;
  int _fury = 0;

  /// The number of hero turns since they last took a hit that caused them to
  /// lose focus.
  int _turnsSinceLostFocus = 0;

  /// How much noise the Hero's last action made.
  public double lastNoise => _lastNoise;
  double _lastNoise = 0.0;

  public override string nounText => "you";

  public override Pronoun pronoun => Pronoun.you;

  public Inventory inventory => save.inventory;

  public Equipment equipment => save.equipment;

  public int experience
  {
    get { return save.experience; }
    set { save.experience = value; }
  }

  public SkillSet skills => save.skills;

  public int gold
  {
    get { return save.gold; }
    set { save.gold = value; }
  }

  public Lore lore => save.lore;

  public override int maxHealth => fortitude.maxHealth;

  public Strength strength => save.strength;

  public Agility agility => save.agility;

  public Fortitude fortitude => save.fortitude;

  public Intellect intellect => save.intellect;

  public Will will => save.will;

  // TODO: Equipment and items that let the hero swim, fly, etc.
  public override Motility motility => Motility.doorAndWalk;

  public override int emanationLevel => save.emanationLevel;

  public Hero(Game game, Vec pos, HeroSave save)
    : base(game, pos.x, pos.y)
  {
    this.save = save;
    // Give the hero energy so they can act before all of the monsters.
    energy.energy = Energy.actionCost;

    refreshProperties();

    // Set the meters now that we know the stats.
    health = maxHealth;
    _focus = intellect.maxFocus;

    // Acquire any skills from the starting items.
    // TODO: Doing this here is hacky. It only really comes into play for
    // starting items.
    foreach (var item in inventory)
    {
      _gainItemSkills(item);
    }
  }

  // TODO: Hackish.
  public override object appearance => "hero";

  public override bool needsInput
  {
    get
    {
      if (_behavior != null && !_behavior!.canPerform(this))
      {
        waitForInput();
      }

      return _behavior == null;
    }
  }

  /// The hero's experience level.
  public int level => _level.value;
  public Property<int> _level = new Property<int>();

  public override int armor => save.armor;

  /// The total weight of all equipment.
  int weight => save.weight;

  // TODO: Not currently used since skills are not explicitly learned in the
  // UI. Re-enable when we add rogue skills?
  /*
  /// Updates the hero's skill levels to [skills] and apply any other changes
  /// caused by that.
  void updateSkills(SkillSet skills) {
    // Update anything affected.
    this.skills.update(skills);
  }
  */

  // TODO: The set of skills discovered from items should probably be stored in
  // lore. Then the skill levels can be stored using Property and refreshed
  // like other properties.
  /// Discover or acquire any skills associated with [item].
  void _gainItemSkills(Item item)
  {
    foreach (var skill in item.type.skills)
    {
      if (save.heroClass.proficiency(skill) != 0.0 && skills.discover(skill))
      {
        // See if the hero can immediately use it.
        var level = skill.calculateLevel(save);
        if (skills.gain(skill, level))
        {
          game.log.gain(skill.gainMessage(level), this);
        }
        else
        {
          game.log.gain(skill.discoverMessage, this);
        }
      }
    }
  }

  public override int baseSpeed => Energy.normalSpeed;

  public override int baseDodge => 20 + agility.dodgeBonus;

  public override List<Defense> onGetDefenses()
  {
    var rt = new List<Defense>();
    foreach (var item in equipment)
    {
      var defense = item.defense;
      if (defense != null) rt.Add(defense);
    }

    foreach (var skill in skills.acquired)
    {
      var defense = skill.getDefense(this, skills.level(skill));
      if (defense != null) rt.Add(defense);
    }

    // TODO: Temporary bonuses, etc.
    return rt;
  }

  public override Action onGetAction() => _behavior!.getAction(this);

  public override List<Hit> onCreateMeleeHits(Actor defender)
  {
    var hits = new List<Hit>();

    // See if any melee weapons are equipped.
    var weapons = equipment.weapons;
    for (var i = 0; i < weapons.Count; i++)
    {
      var weapon = weapons[i];
      if (weapon.attack!.isRanged) continue;

      var hit = weapon.attack!.createHit();

      weapon.modifyHit(hit);

      // Take heft and strength into account.
      hit.scaleDamage(_heftScales[i].value);
      hits.Add(hit);
    }

    // If not, punch it.
    if (hits.Count == 0)
    {
      hits.Add(new Attack(this, "punch[es]", Option.heroPunchDamage).createHit());
    }

    foreach (var hit in hits)
    {
      hit.addStrike(agility.strikeBonus);

      foreach (var skill in skills.acquired)
      {
        skill.modifyAttack(
            this, defender as Monster, hit, skills.level(skill));
      }
    }

    return hits;
  }

  public Hit createRangedHit()
  {
    var weapons = equipment.weapons;
    var i = weapons.FindIndex((weapon) => weapon.attack!.isRanged);
    Debugger.assert(i != -1, "Should have ranged weapon equipped.");

    var hit = weapons[i].attack!.createHit();

    // Take heft and strength into account.
    hit.scaleDamage(_heftScales[i].value);

    modifyHit(hit, HitType.ranged);
    return hit;
  }

  /// Applies the hero-specific modifications to [hit].
  public override void onModifyHit(Hit hit, HitType type)
  {
    // TODO: Use agility to affect strike.

    switch (type)
    {
      case HitType.melee:
        break;

      case HitType.ranged:
        // TODO: Use strength to affect range.
        // TODO: Take heft into account.
        break;

      case HitType.toss:
        hit.scaleRange(strength.tossRangeScale);
        break;
    }

    // Let armor modify it. We don't worry about weapons here since the weapon
    // modified it when the hit was created. This ensures that when
    // dual-wielding that one weapon's modifiers don't affect the other.
    foreach (var item in equipment)
    {
      if (item.type.weaponType == null) item.modifyHit(hit);
    }

    // TODO: Apply skills.
  }

  // TODO: If class or race can affect this, add it in.
  public override int onGetResistance(Element element) => save.equipmentResistance(element);

  public override void onGiveDamage(Action action, Actor defender, int damage)
  {
    // Hitting increases fury.
    _gainFury(damage * 1f / defender.maxHealth * maxHealth / 100f);
  }

  public override void onTakeDamage(Action action, Actor attacker, int damage)
  {
    // Getting hit loses focus and gains fury.
    // TODO: Lose less focus for ranged attacks?
    var focus = Mathf.CeilToInt((float)(damage * 1f / maxHealth * will.damageFocusScale));
    _focus = Mathf.Clamp(_focus - focus, 0, intellect.maxFocus);

    _gainFury(damage * 1f / maxHealth * 50);
    _turnsSinceLostFocus = 0;

    // TODO: Would be better to do skills.discovered, but right now this also
    // discovers BattleHardening.
    foreach (var skill in game.content.skills)
    {
      skill.takeDamage(this, damage);
    }
  }

  public override void onKilled(Action action, Actor defender)
  {
    var monster = defender as Monster;

    // It only counts if the hero's seen the monster at least once.
    if (!_seenMonsters.Contains(monster)) return;

    // Gain some fury.
    _gainFury(defender.maxHealth * 1f / maxHealth * 50);

    lore.slay(monster.breed);

    foreach (var skill in skills.discovered)
    {
      skill.killMonster(this, action, monster);
    }

    experience += monster.experience;
    refreshProperties();
  }

  public override void onDied(Noun attackNoun)
  {
    game.log.message("you were slain by {1}.", attackNoun);
  }

  public override void onFinishTurn(Action action)
  {
    // Make some noise.
    _lastNoise = action.noise;

    // Always digesting.
    if (stomach > 0)
    {
      stomach--;
      if (stomach == 0) game.log.message("You are getting hungry.");
    }

    _turnsSinceLostFocus++;

    // TODO: Passive skills?
  }

  public override void changePosition(Vec from, Vec to)
  {
    base.changePosition(from, to);
    game.stage.heroVisibilityChanged();
  }

  public void waitForInput()
  {
    _behavior = null;
  }

  public void setNextAction(Action action)
  {
    _behavior = new ActionBehavior(action);
  }

  /// Starts resting, if the hero has eaten and is able to regenerate.
  public bool rest()
  {
    if (poison.isActive)
    {
      game.log
          .error("You cannot rest while poison courses through your veins!");
      return false;
    }

    if (health == maxHealth)
    {
      game.log.message("You are fully rested.");
      return false;
    }

    if (stomach == 0)
    {
      game.log.error("You are too hungry to rest.");
      return false;
    }

    _behavior = new RestBehavior();
    return true;
  }

  public void run(Direction direction)
  {
    _behavior = new RunBehavior(direction);
  }

  public void disturb()
  {
    if (!(_behavior is ActionBehavior))
      waitForInput();
  }

  public void seeMonster(Monster monster)
  {
    // TODO: Blindness and dazzle.

    if (_seenMonsters.Add(monster))
    {
      // TODO: If we want to give the hero experience for seeing a monster too,
      // (so that sneak play-style still lets the player gain levels), do that
      // here.
      lore.seeBreed(monster.breed);

      // If this is the first time we've seen this breed, see if that unlocks
      // a slaying skill for it.
      if (lore.seenBreed(monster.breed) == 1)
      {
        foreach (var skill in game.content.skills)
        {
          skill.seeBreed(this, monster.breed);
        }
      }
    }
  }

  /// Spends focus on some useful action.
  ///
  /// Does not reset [_turnsSinceLostFocus].
  public void spendFocus(int focus)
  {
    Debugger.assert(_focus >= focus);

    _focus -= focus;
  }

  /// Spends fury on some useful action.
  ///
  /// Does not reset [_turnsSinceLostFocus].
  public void spendFury(int fury)
  {
    Debugger.assert(_fury >= fury);

    _fury -= fury;
  }

  public void regenerateFocus(int focus)
  {
    // The longer the hero goes without losing focus, the more quickly it
    // regenerates.
    var scale = Mathf.Clamp(_turnsSinceLostFocus + 1, 1, 8) / 4f;
    _focus = (int)Mathf.Clamp(Mathf.Ceil(_focus + focus * scale), 0, intellect.maxFocus);

    _fury = Mathf.Clamp(Mathf.CeilToInt((float)(_fury - focus * scale * will.restFuryScale)), 0, strength.maxFury);
  }

  void _gainFury(double fury)
  {
    _fury = (int)Mathf.Clamp(_fury + Mathf.Ceil((float)fury), 0, strength.maxFury);
  }

  /// Refreshes all hero state whose change should be logged.
  ///
  /// For example, if the hero equips a helm that increases intellect, we want
  /// to log that. Likewise, if they level up and their strength increases. Or
  /// maybe a ghost drains their experience, which lowers their level, which
  /// reduces dexterity.
  ///
  /// To track that, any calculated property whose change should be noted is
  /// wrapped in a [Property] and updated here. Note that order that these are
  /// updated matters. Properties must be updated after the properties they
  /// depend on.
  public void refreshProperties()
  {
    var level = experienceLevel(experience);
    _level.update(level, (previous) =>
    {
      game.log.gain($"You have reached level {level}.");
      // TODO: Different message if level went down.
    });

    strength.refresh(game);
    agility.refresh(game);
    fortitude.refresh(game);
    intellect.refresh(game);
    will.refresh(game);

    // Refresh the heft scales.
    var weapons = equipment.weapons;
    for (var i = 0; i < weapons.Count; i++)
    {
      var weapon = weapons[i];

      // Dual-wielding imposes a heft penalty.
      var heftModifier = 1.0;

      if (weapons.Count == 2)
      {
        heftModifier = 1.3;

        // Discover the dual-wield skill.
        // TODO: This is a really specific method to put on Skill. Is there a
        // cleaner way to handle this?
        foreach (var skill in game.content.skills)
        {
          skill.dualWield(this);
        }
      }

      foreach (var skill in skills.acquired)
      {
        heftModifier =
            skill.modifyHeft(this, skills.level(skill), heftModifier);
      }

      var heft = Mathf.RoundToInt((float)(weapon.heft * heftModifier));
      var heftScale = strength.heftScale(heft);
      _heftScales[i].update(heftScale, (previous) =>
      {
        // TODO: Reword these if there is no weapon equipped?
        if (heftScale < 1.0 && previous >= 1.0)
        {
          game.log.error($"You are too weak to effectively wield {weapon}.");
        }
        else if (heftScale >= 1.0 && previous < 1.0)
        {
          game.log.message($"You feel comfortable wielding {weapon}.");
        }
      });
    }

    // See if any skills changed. (Gaining intellect learns spells.)
    _refreshSkills();

    // Keep other stats in bounds.
    health = Mathf.Clamp(health, 0, maxHealth);
    _focus = Mathf.Clamp(_focus, 0, intellect.maxFocus);
    // TODO: Is this how we want max fury derived?
    _fury = Mathf.Clamp(_fury, 0, strength.maxFury);
  }

  /// Called when the hero holds an item.
  ///
  /// This can be in response to picking it up, or equipping or using it
  /// straight from the ground.
  public void pickUp(Item item)
  {
    // TODO: If the user repeatedly picks up and drops the same item, it gets
    // counted every time. Maybe want to put a (serialized) flag on items for
    // whether they have been picked up or not.
    lore.findItem(item);

    _gainItemSkills(item);
    refreshProperties();
  }

  /// See if any known skills have leveled up.
  void _refreshSkills()
  {
    skills.discovered.ToList().ForEach(x => refreshSkill(x));
  }

  /// Ensures the hero has discovered [skill] and logs if it is the first time
  /// it's been seen.
  public void discoverSkill(Skill skill)
  {
    if (save.heroClass.proficiency(skill) == 0.0) return;

    if (!skills.discover(skill)) return;

    game.log.gain(skill.discoverMessage, this);
  }

  public void refreshSkill(Skill skill)
  {
    var level = skill.calculateLevel(save);
    if (skills.gain(skill, level))
    {
      game.log.gain(skill.gainMessage(level), this);
    }
  }

  /// Whether the hero can currently perceive [actor].
  ///
  /// Takes into account both visibility and [perception].
  public bool canPerceive(Actor actor)
  {
    if (game.stage[actor.pos].isVisible) return true;
    if (perception.isActive && (pos - actor.pos) < perception.intensity)
    {
      return true;
    }
    return false;
  }

  public static int experienceLevel(int experience)
  {
    for (var level = 1; level <= Hero.maxLevel; level++)
    {
      if (experience < Hero.experienceLevelCost(level))
        return level - 1;
    }
    return Hero.maxLevel;
  }

  /// Returns how much experience is needed to reach [level].
  public static int experienceLevelCost(int level)
  {
    if (level > Hero.maxLevel) throw new System.ArgumentOutOfRangeException("level", level.ToString());
    return (int)(Mathf.Pow(level - 1, 3)) * 1000;
  }
}


