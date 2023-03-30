using System.Collections;
using System.Collections.Generic;
using num = System.Double;
using System.Text.RegularExpressions;
using UnityTerminal;

public class _MBaseBuilder
{
 public static Regex collapseNewlines = new Regex("\n\\s*");

  public static Dictionary<Element, List<string>> _elementText = new Dictionary<Element, List<string>>(){
        {Elements.air, new List<string>{"the wind", "buffets"}},
        {Elements.earth, new List<string>{"the soil", "buries"}},
        {Elements.fire, new List<string>{"the flame", "burns"}},
        {Elements.water, new List<string>{"the water", "blasts"}},
        {Elements.acid, new List<string>{"the acid", "melts"}},
        {Elements.cold, new List<string>{"the ice", "freezes"}},
        {Elements.lightning, new List<string>{"the lightning", "shocks"}},
        {Elements.poison, new List<string>{"the poison", "chokes"}},
        {Elements.dark, new List<string>{"the darkness", "crushes"}},
        {Elements.light, new List<string>{"the light", "sears"}},
        {Elements.spirit, new List<string>{"the spirit", "haunts"}},
    };

  /// The last builder that was created. It gets implicitly finished when the
  /// next family or breed starts, or at the end of initialization. This way, we
  /// don't need an explicit `build()` call at the end of each builder.
  public static _BreedBuilder _builder;

  public static _FamilyBuilder _family;

  public static _FamilyBuilder family(string character,
      double? frequency = null,
      int? meander = null,
      int? speed = null,
      int? dodge = null,
      int? tracking = null,
      string flags = null)
  {
    finishBreed();

    _family = new _FamilyBuilder(frequency, character);
    _family._meander = meander;
    _family._speed = speed;
    _family._dodge = dodge;
    _family._tracking = tracking;
    _family._flags = flags;

    return _family;
  }

  public static void finishBreed()
  {
    var builder = _builder;
    if (builder == null) return;

    var tags = new List<string>();
    tags.AddRange(_family._groups);
    tags.AddRange(builder._groups);

    if (tags.Count == 0) tags.Add("monster");

    var breed = builder.build();

    Monsters.breeds.add(breed,
        name: breed.name,
        depth: breed.depth,
        frequency: builder._frequency ?? _family._frequency,
        tags: string.Join(" ", tags));
    _builder = null;
  }

  // TODO: Move more named params into builder methods?
  public static _BreedBuilder breed(string name, int depth, Color color, int health,
      double? frequency = null, int speed = 0, int? dodge = null, int? meander = null)
  {
    finishBreed();

    var glyph = new Glyph(_family._character[0], color);
    var builder = new _BreedBuilder(name, depth, frequency, glyph, health);
    builder._speed = speed;
    builder._dodge = dodge;
    builder._meander = meander;
    _builder = builder;
    return builder;
  }

  public static void describe(string description)
  {
    // description = description.replaceAll(collapseNewlines, " ");
    description = Regex.Replace(description, collapseNewlines.ToString(), " ");
    _builder!._description = description;
  }

  ///////////////////////////////////////////////
  public double? _frequency;

  public int? _tracking;

  // Default to walking.
  // TODO: Are there monsters that cannot walk?
  public Motility _motility = Motility.walk;

  // TODO: Get this working again.
  public SpawnLocation? _location;

  /// The default speed for breeds in the current family. If the breed
  /// specifies a speed, it offsets the family's speed.
  public int? _speed;

  /// The default meander for breeds in the current family. If the breed
  /// specifies a meander, it offset's the family's meander.
  public int? _meander;

  public int? _dodge;

  public List<Defense> _defenses = new List<Defense>();
  public List<string> _groups = new List<string>();

  // TODO: Make flags strongly typed here too?
  public string _flags;

  public int? _countMin;
  public int? _countMax;

  public TileType _stain;

  public int? _emanationLevel;

  public int? _vision;
  public int? _hearing;

  public _MBaseBuilder(double? _frequency)
  {
    this._frequency = _frequency;
  }

  public void flags(string flags)
  {
    // TODO: Allow negated flags.
    _flags = flags;
  }

  public void emanate(int level)
  {
    _emanationLevel = level;
  }

  public void sense(int? see = null, int? hear = null)
  {
    _vision = see;
    _hearing = hear;
  }

  public void preferWall()
  {
    _location = SpawnLocation.wall;
  }

  public void preferCorner()
  {
    _location = SpawnLocation.corner;
  }

  public void preferOpen()
  {
    _location = SpawnLocation.open;
  }

  /// How many monsters of this kind are spawned.
  public void count(int minOrMax, int? max = null)
  {
    if (max == null)
    {
      _countMin = 1;
      _countMax = minOrMax;
    }
    else
    {
      _countMin = minOrMax;
      _countMax = max;
    }
  }

  public void stain(TileType type)
  {
    _stain = type;
  }

  public void fly()
  {
    _motility |= Motility.fly;
  }

  public void swim()
  {
    _motility |= Motility.swim;
  }

  public void openDoors()
  {
    _motility |= Motility.door;
  }

  public void defense(int amount, string message)
  {
    _defenses.Add(new Defense(amount, message));
  }

  public void groups(string names)
  {
    _groups.AddRange(names.Split(' '));
  }
}

public class _FamilyBuilder : _MBaseBuilder
{
  /// Character for the current monster.
  public string _character;

  public _FamilyBuilder(double? frequency, string _character) : base(frequency)
  {
    this._character = _character;
  }
}

public class _BreedBuilder : _MBaseBuilder
{
  public string _name;
  public int _depth;
  public object _appearance;
  public int _health;
  public List<Attack> _attacks = new List<Attack>();
  public List<Move> _moves = new List<Move>();
  public List<Drop> _drops = new List<Drop>();
  public List<Spawn> _minions = new List<Spawn>();
  public Pronoun _pronoun;
  public string _description;

  public _BreedBuilder(string _name, int _depth, double? frequency, object _appearance, int _health)
      : base(frequency)
  {
    this._name = _name;
    this._depth = _depth;
    this._appearance = _appearance;
    this._health = _health;
  }

  public void minion(string name, int? minOrMax = null, int? max = null)
  {
    Spawn spawn;
    if (Monsters.breeds.tagExists(name))
    {
      spawn = SpawnUtils.spawnTag(name);
    }
    else
    {
      spawn = SpawnUtils.spawnBreed(name);
    }

    if (max != null)
    {
      spawn = SpawnUtils.repeatSpawn(minOrMax.Value, max.Value, spawn);
    }
    else if (minOrMax != null)
    {
      spawn = SpawnUtils.repeatSpawn(1, minOrMax.Value, spawn);
    }

    _minions.Add(spawn);
  }

  public void attack(string verb, int damage, Element element = null, Noun noun = null)
  {
    _attacks.Add(new Attack(noun, verb, damage, 0, element));
  }

  /// Drops [name], which can be either an item type or tag.
  public void drop(string name,
      int percent = 100,
      int count = 1,
      int depthOffset = 0,
      int? affixChance = null)
  {
    var drop = DropUtils.percentDrop(percent, name, _depth + depthOffset, affixChance);
    if (count > 1) drop = DropUtils.repeatDrop(count, drop);
    _drops.Add(drop);
  }

  public void he()
  {
    _pronoun = Pronoun.he;
  }

  public void she()
  {
    _pronoun = Pronoun.she;
  }

  // TODO: Figure out some strategy for which of these parameters have defaults
  // and which don't.

  public void heal(num rate = 5, int amount = 0) =>
      _addMove(new HealMove(rate, amount));

  public void arrow(num rate = 5, int damage = 0) =>
      _bolt("the arrow", "hits", Element.none,
          rate: rate, damage: damage, range: 8);

  public void whip(num rate = 5, int damage = 0, int range = 2) =>
      _bolt(null, "whips", Element.none,
          rate: rate, damage: damage, range: range);

  void bolt(Element element,
      num rate, int damage, int range)
  {
    _bolt(_MBaseBuilder._elementText[element]![0], _MBaseBuilder._elementText[element]![1], element,
        rate: rate, damage: damage, range: range);
  }

  public void windBolt(num rate = 5, int damage = 0) =>
      bolt(Elements.air, rate: rate, damage: damage, range: 8);

  public void stoneBolt(num rate = 5, int damage = 0) =>
      _bolt("the stone", "hits", Elements.earth,
          rate: rate, damage: damage, range: 8);

  public void waterBolt(num rate = 5, int damage = 0) =>
      _bolt("the jet", "splashes", Elements.water,
          rate: rate, damage: damage, range: 8);

  public void sparkBolt(num rate, int damage, int range = 6) =>
      _bolt("the spark", "zaps", Elements.lightning,
          rate: rate, damage: damage, range: range);

  public void iceBolt(num rate = 5, int damage = 0, int range = 8) =>
      _bolt("the ice", "freezes", Elements.cold,
          rate: rate, damage: damage, range: range);

  public void fireBolt(num rate = 5, int damage = 0) =>
      bolt(Elements.fire, rate: rate, damage: damage, range: 8);

  public void lightningBolt(num rate = 5, int damage = 0) =>
      bolt(Elements.lightning, rate: rate, damage: damage, range: 10);

  public void acidBolt(num rate = 5, int damage = 0, int range = 8) =>
      bolt(Elements.acid, rate: rate, damage: damage, range: range);

  public void darkBolt(num rate = 5, int damage = 0) =>
      bolt(Elements.dark, rate: rate, damage: damage, range: 10);

  public void lightBolt(num rate = 5, int damage = 0) =>
      bolt(Elements.light, rate: rate, damage: damage, range: 10);

  void poisonBolt(num rate = 5, int damage = 0) =>
      bolt(Elements.poison, rate: rate, damage: damage, range: 8);

  public void cone(Element element, num? rate = null, int damage = 0, int? range = null)
  {
    if (_MBaseBuilder._elementText.ContainsKey(element) == false)
      Debugger.logError("cant exist key > " + element.ToString());

    _cone(_MBaseBuilder._elementText[element]![0], _MBaseBuilder._elementText[element]![1], element,
        rate: rate, damage: damage, range: range);
  }

  void windCone(num rate, int damage, int? range) =>
      cone(Elements.air, rate: rate, damage: damage, range: range);

  public void fireCone(num rate, int damage, int? range = null) =>
      cone(Elements.fire, rate: rate, damage: damage, range: range);

  public void iceCone(num rate, int damage, int? range = null) =>
      cone(Elements.cold, rate: rate, damage: damage, range: range);

  public void lightningCone(num rate, int damage, int? range = null) =>
      cone(Elements.lightning, rate: rate, damage: damage, range: range);

  public void lightCone(num rate, int damage, int? range = null) =>
      cone(Elements.light, rate: rate, damage: damage, range: range);

  public void darkCone(num rate, int damage, int? range = null) =>
      cone(Elements.dark, rate: rate, damage: damage, range: range);

  void waterCone(num rate, int damage, int? range) =>
      cone(Elements.water, rate: rate, damage: damage, range: range);

  public void missive(Missive missive, num rate = 5) =>
      _addMove(new MissiveMove(missive, rate));

  public void howl(num rate = 10, int range = 10, string verb = null) =>
      _addMove(new HowlMove(rate, range, verb));

  public void haste(num rate = 5, int duration = 10, int speed = 1) =>
      _addMove(new HasteMove(rate, duration, speed));

  public void teleport(num rate = 10, int range = 10) =>
      _addMove(new TeleportMove(rate, range));

  public void spawn(num rate = 10, bool? preferStraight = null) =>
      _addMove(new SpawnMove(rate, preferStraight));

  public void amputate(string body, string part, string message) =>
      _addMove(new AmputateMove(new BreedRef(body), new BreedRef(part), message));

  void _bolt(string noun, string verb, Element element,
      num rate, int damage, int range)
  {
    var nounObject = noun != null ? new Noun(noun) : null;
    _addMove(new BoltMove(rate, new Attack(nounObject, verb, damage, range, element)));
  }

  void _cone(string noun, string verb, Element element,
      num? rate, int damage, int? range)
  {
    rate ??= 5;
    range ??= 10;

    _addMove(new ConeMove(rate.Value, new Attack(new Noun(noun), verb, damage, range.Value, element)));
  }

  void _addMove(Move move)
  {
    _moves.Add(move);
  }

  public Breed build()
  {
    List<string> flags = new List<string>() { };
    // TODO: Use ?. and ...?.
    if (_family._flags != null)
      flags.AddRange(_family._flags!.Split(' '));
    if (_flags != null)
      flags.AddRange(_flags!.Split(' '));

    var dodge = _dodge ?? _family._dodge;
    if (flags.Contains("immobile")) dodge = 0;

    Spawn minions = null;
    if (_minions.Count == 1)
    {
      minions = _minions[0];
    }
    else if (_minions.Count > 1)
    {
      minions = SpawnUtils.spawnAll(_minions);
    }

    var breed = new Breed(
        _name,
        _pronoun ?? Pronoun.it,
        _appearance,
        _attacks,
        _moves,
        DropUtils.dropAllOf(_drops),
        _location ?? _family._location ?? SpawnLocation.anywhere,
        _family._motility | _motility,
        depth: _depth,
        maxHealth: _health,
        tracking: (_tracking ?? 0) + (_family._tracking ?? 10),
        vision: _vision ?? _family._vision,
        hearing: _hearing ?? _family._hearing,
        meander: _meander ?? _family._meander ?? 0,
        speed: (_speed ?? 0) + (_family._speed ?? 0),
        dodge: dodge,
        emanationLevel: _family._emanationLevel ?? _emanationLevel,
        countMin: _countMin ?? _family._countMin,
        countMax: _countMax ?? _family._countMax,
        minions: minions,
        stain: _stain ?? _family._stain,
        flags: BreedFlags.fromSet(flags),
        description: _description);

    breed.defenses.AddRange(_family._defenses);
    breed.defenses.AddRange(_defenses);

    breed.groups.AddRange(_family._groups);
    breed.groups.AddRange(_groups);

    return breed;
  }
}
