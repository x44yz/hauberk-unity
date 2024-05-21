using System;
using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;
using System.Text;

/// A lazy named reference to a Breed.
///
/// This allows cyclic references while the breeds are still being defined.
class BreedRef
{
  public static List<BreedRef> _unresolved = new List<BreedRef>();

  public static void resolve(Func<string, Breed> resolver)
  {
    foreach (var _ref in _unresolved)
    {
      _ref._breed = resolver(_ref._name);
    }

    _unresolved.Clear();
  }

  public string _name;
  public Breed _breed;
  public Breed breed => _breed;

  public BreedRef(string name)
  {
    this._name = name;
    _unresolved.Add(this);
  }
}

/// A single kind of [Monster] in the game.
public class Breed
{
  public Pronoun pronoun;
  public string name => Log.singular(_name);

  /// Untyped so the engine isn't coupled to how monsters appear.
  public object appearance;

  /// The breeds's depth.
  ///
  /// Higher depth breeds are found later in the game.
  public int depth;

  public List<Attack> attacks;
  public List<Move> moves;

  public int maxHealth;

  /// How well the monster can navigate the stage to reach its target.
  ///
  /// Used to determine maximum pathfinding distance.
  public int tracking;

  /// How many tiles away the monster can see the hero.
  public int vision;

  /// How many tiles away (as sound flows) the monster can hear the hero.
  public int hearing;

  /// Percent chance of choosing a non-optimal direction when walking.
  public int meander;

  /// The breed's speed, relative to normal. Ranges from `-6` (slowest) to `6`
  /// (fastest) where `0` is normal speed.
  public int speed;

  /// The [Item]s this monster may drop when killed.
  public Drop drop;

  public SpawnLocation location;

  public Motility motility;

  public BreedFlags flags;

  /// Base chance for this breed to dodge an attack.
  public int dodge;

  /// How much light the monster emanates.
  public int emanationLevel;

  /// Additional defenses this breed has.
  public List<Defense> defenses = new List<Defense>();

  /// The minimum number of this breed that are spawned when it is placed in
  /// the dungeon.
  public int countMin;

  /// The minimum number of this breed that are spawned when it is placed in
  /// the dungeon.
  public int countMax;

  /// Additional monsters that should be spawned when this one is spawned.
  public Spawn minions;

  /// The name of the breed. If the breed's name has irregular pluralization
  /// like "bunn[y|ies]", this will be the original unparsed string.
  public string _name;

  /// If this breed should stain some of the nearby floor tiles when spawned,
  /// this is the type is should stain them with. Otherwise `null`.
  public TileType stain;

  /// The groups that the breed is a part of.
  ///
  /// Used to determine which kinds of slaying affect which monsters. For
  /// display purposes in the lore screen, the last group in the list should
  /// be noun-like while the others are adjectives, like `["undead", "bug"]`.
  public List<string> groups = new List<string>();

  public string description;

  public Breed(string _name, Pronoun pronoun, object appearance, List<Attack> attacks,
      List<Move> moves,
      Drop drop, SpawnLocation location, Motility motility,
      int depth,
      int maxHealth,
      int tracking,
      int? vision,
      int? hearing,
      int meander,
      int? speed,
      int? dodge,
      int? emanationLevel,
      int? countMin,
      int? countMax,
      Spawn minions,
      TileType stain,
      BreedFlags flags,
      string description)
  {
    this._name = _name;
    this.pronoun = pronoun;
    this.appearance = appearance;
    this.attacks = attacks;
    this.moves = moves;
    this.drop = drop;
    this.location = location;
    this.motility = motility;
    this.depth = depth;
    this.maxHealth = maxHealth;
    this.tracking = tracking;

    this.vision = vision ?? 8;
    this.hearing = hearing ?? 10;
    this.meander = meander;
    this.speed = speed ?? 0;
    this.dodge = dodge ?? 20;
    this.emanationLevel = emanationLevel ?? 0;
    this.countMin = countMin ?? 1;
    this.countMax = countMax ?? 1;
    this.minions = minions;
    this.stain = stain;
    this.flags = flags;
    this.description = description ?? "Indescribable.";
  }

  /// How much experience a level one [Hero] gains for killing a [Monster] of
  /// this breed.
  ///
  /// The basic idea is that experience roughly correlates to how much damage
  /// the monster can dish out to the hero before it dies.
  public int experience
  {
    get
    {
      // The more health it has, the longer it can hurt the hero.
      var exp = (double)maxHealth;

      // The more it can dodge, the longer it lives.
      var totalDodge = dodge;
      foreach (var defense in defenses)
      {
        totalDodge += defense.amount;
      }

      exp *= 1.0 + totalDodge / 100.0;

      // Faster monsters can hit the hero more often.
      exp *= Energy.gains[Energy.normalSpeed + speed];

      // Average the attacks, since they are selected randomly.
      var attackTotal = 0.0;
      foreach (var attack in attacks)
      {
        if (attack == null)
          Debugger.logError("attack is null");
        else if (attack.element == null)
          Debugger.logError("attack element is null");

        // TODO: Take range into account?
        attackTotal += attack.damage * attack.element.experience;
      }

      attackTotal /= attacks.Count;

      // Average the moves.
      var moveTotal = 0.0;
      var moveRateTotal = 0.0;
      foreach (var move in moves)
      {
        // Scale by the move rate. The less frequently a move can be performed,
        // the less it affects experience.
        moveTotal += move.experience / move.rate;
        moveRateTotal += 1 / move.rate;
      }

      // Time spent using moves is not time spent attacking.
      attackTotal *= 1.0 - moveRateTotal;

      // Add in moves and attacks.
      exp *= attackTotal + moveTotal;

      // Take into account flags.
      exp *= flags.experienceScale;

      // TODO: Modify by motility?
      // TODO: Modify by count?
      // TODO: Modify by minions.

      // Meandering monsters are worth less.
      exp *= MathUtils.lerpDouble(meander, 0.0f, 100.0f, 1.0, 0.7);

      // Scale it down arbitrarily to keep the numbers reasonable. This is tuned
      // so that the weakest monsters can still have some variance in experience
      // when rounded to an integer.
      exp /= 40;

      return Mathf.CeilToInt((float)exp);
    }
  }

  public Monster spawn(Game game, Vec pos, Monster parent = null)
  {
    var generation = 1;
    if (parent != null) generation = parent.generation + 1;

    return new Monster(game, this, pos.x, pos.y, generation);
  }

  /// Generate the list of breeds spawned by this breed.
  ///
  /// Each item in the list represents a breed that should spawn a single
  /// monster. Takes into account this breed's count and minions.
  public List<Breed> spawnAll()
  {
    var breeds = new List<Breed>();

    // This breed.
    var count = Rng.rng.inclusive(countMin, countMax);
    for (var i = 0; i < count; i++)
    {
      breeds.Add(this);
    }

    if (minions != null)
    {
      // Minions are weaker than the main breed.
      var minionDepth = Math.Floor(depth * 0.9);
      minions!.spawnBreed((int)minionDepth, breeds.Add);
    }

    return breeds;
  }

  public override string ToString() => name;
}

// TODO: Should this affect how the monster moves during the game too?
/// Where in the dungeon the breed prefers to spawn.
public enum SpawnLocation
{
  anywhere,

  /// Away from walls.
  open,

  /// Adjacent to a wall.
  wall,

  /// Adjacent to multiple walls.
  corner,
}

// typedef AddMonster = void Function(Breed breed);

public abstract class Spawn
{
  public abstract void spawnBreed(int depth, System.Action<Breed> addMonster);
}

public class BreedFlags
{
  public bool berzerk;
  public bool cowardly;
  public bool fearless;
  public bool immobile;
  public bool protective;
  public bool unique;

  BreedFlags(bool berzerk, bool cowardly, bool fearless,
            bool immobile, bool protective, bool unique)
  {
    this.berzerk = berzerk;
    this.cowardly = cowardly;
    this.fearless = fearless;
    this.immobile = immobile;
    this.protective = protective;
    this.unique = unique;
  }

  /// The way this set of flags affects the experience gained when killing a
  /// monster.
  public double experienceScale
  {
    get
    {
      var scale = 1.0;

      if (berzerk) scale *= 1.1;
      if (cowardly) scale *= 0.9;
      if (fearless) scale *= 1.05;
      if (immobile) scale *= 0.7;
      if (protective) scale *= 1.1;

      return scale;
    }

  }

  public static BreedFlags fromSet(List<string> names)
  {
    var kk = new HashSet<string>(names);
    return fromSet(kk);
  }

  public static BreedFlags fromSet(HashSet<string> names)
  {
    //names = names.toSet();

    var flags = new BreedFlags(
        berzerk: names.Remove("berzerk"),
        cowardly: names.Remove("cowardly"),
        fearless: names.Remove("fearless"),
        immobile: names.Remove("immobile"),
        protective: names.Remove("protective"),
        unique: names.Remove("unique"));

    if (names.Count != 0)
    {
      throw new System.ArgumentException($"Unknown flags \"{string.Join(", ", names)}\"");
    }

    return flags;
  }

  public override string ToString()
  {
    List<string> rt = new List<string>();
    if (berzerk) rt.Add("berzerk");
    if (cowardly) rt.Add("cowardly");
    if (fearless) rt.Add("fearless");
    if (immobile) rt.Add("immobile");
    if (protective) rt.Add("protective");
    if (unique) rt.Add("unique");
    return string.Join(" ", rt);
  }
}