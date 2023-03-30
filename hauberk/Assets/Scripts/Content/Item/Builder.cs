using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TossItemUse = System.Func<Vec, Action>;
using UnityTerminal;

class _BaseBuilder
{
  public List<Skill> _skills = new List<Skill>();
  public Dictionary<Element, int> _destroyChance = new Dictionary<Element, int>();

  public int? _maxStack;
  public Element _tossElement;
  public int? _tossDamage;
  public int? _tossRange;
  public TossItemUse _tossUse;
  public int? _emanation;
  public int? _fuel;

  /// Percent chance of objects in the current category breaking when thrown.
  public int? _breakage;

  public void stack(int stack)
  {
    _maxStack = stack;
  }

  /// Makes items in the category throwable.
  public void toss(int? damage = null, Element element = null, int? range = null, int? breakage = null)
  {
    _tossDamage = damage;
    _tossElement = element;
    _tossRange = range;
    _breakage = breakage;
  }

  public void tossUse(TossItemUse use)
  {
    _tossUse = use;
  }

  public void destroy(Element element, int chance, int? fuel = null)
  {
    _destroyChance[element] = chance;
    // TODO: Per-element fuel.
    _fuel = fuel;
  }

  public void skill(string skill)
  {
    _skills.Add(Skills.find(skill));
  }

  public void skills(List<string> skills)
  {
    _skills.AddRange(skills.Select(x => Skills.find(x)));
  }
}

class _CategoryBuilder : _BaseBuilder
{

  public static _CategoryBuilder _category;
  public static _CategoryBuilder category(int glyph, string verb = null, int? stack = null)
  {
    _ItemBuilder.finishItem();

    _category = new _CategoryBuilder(glyph, verb);
    _category._maxStack = stack;

    return _category;
  }

  /// The current glyph's character code. Any items defined will use this.
  public int _glyph;
  public string _verb;

  public string _equipSlot;
  public string _weaponType;
  public string _tag;
  public bool _isTreasure = false;
  public bool _isTwoHanded = false;

  static string[] tagEquipSlots = new string[]{
      "hand",
      "ring",
      "necklace",
      "body",
      "cloak",
      "helm",
      "gloves",
      "boots"
};

  _CategoryBuilder(int _glyph, string _verb)
  {
    this._glyph = _glyph;
    this._verb = _verb;
  }

  public void tag(string tagPath)
  {
    // Define the tag path and store the leaf tag which is what gets used by
    // the item types.
    Items.types.defineTags($"item/{tagPath}");
    var tags = tagPath.Split('/').ToList();
    _tag = tags.Last();

    if (tags.Contains("shield") || tags.Contains("light"))
    {
      _equipSlot = "hand";
    }
    else if (tags.Contains("weapon"))
    {
      // TODO: Handle two-handed weapons.
      _equipSlot = "hand";
      _weaponType = tags[tags.IndexOf("weapon") + 1];
    }
    else
    {
      foreach (var equipSlot in tagEquipSlots)
      {
        if (tags.Contains(equipSlot))
        {
          _equipSlot = equipSlot;
          break;
        }
      }
    }

    // TODO: Hacky. We need a matching tag hiearchy for affixes so that, for
    // example, a "sword" item will match a "weapon" affix.
    Affixes.defineItemTag(tagPath);
  }

  public void treasure()
  {
    _isTreasure = true;
  }

  public void twoHanded()
  {
    _isTwoHanded = true;
  }
}

class _ItemBuilder : _BaseBuilder
{

  public static int _sortIndex = 0;
  public static _ItemBuilder _item;
  public static _ItemBuilder item(string name, Color color,
      double frequency = 1.0, int price = 0)
  {
    finishItem();

    return _item = new _ItemBuilder(name, color, frequency, price);
  }

  public string _name;
  public Color _color;
  public double _frequency;
  public int _price;
  ItemUse _use;
  Attack _attack;
  Defense _defense;
  int? _weight;
  int? _heft;
  int? _armor;

  // TODO: Instead of late public, initialize these in item() instead of depth().
  public int _minDepth;
  public int _maxDepth;

  public static _CategoryBuilder _category => _CategoryBuilder._category;

  _ItemBuilder(string _name, Color _color, double _frequency, int _price)
  {
    this._name = _name;
    this._color = _color;
    this._frequency = _frequency;
    this._price = _price;
  }

  /// Sets the item's minimum depth to [from]. If [to] is given, then the item
  /// has the given depth range. Otherwise, its max is [Option.maxDepth].
  public void depth(int from, int? to = null)
  {
    _minDepth = from;
    _maxDepth = to ?? Option.maxDepth;
  }

  public void defense(int amount, string message)
  {
    Debugger.assert(_defense == null);
    _defense = new Defense(amount, message);
  }

  public void armor(int armor, int? weight = null)
  {
    _armor = armor;
    _weight = weight;
  }

  public void weapon(int damage, int heft, Element element = null)
  {
    _attack = new Attack(null, _category._verb!, damage, 0, element);
    _heft = heft;
  }

  public void ranged(string noun,
    int heft, int damage, int range)
  {
    _attack = new Attack(new Noun(noun), "pierce[s]", damage, range);
    // TODO: Make this per-item once it does something.
    _heft = heft;
  }

  void use(string description, System.Func<Action> createAction)
  {
    _use = new ItemUse(description, createAction);
  }

  public void food(int amount)
  {
    use($"Provides {amount} turns of food.", () => new EatAction(amount));
  }

  public void detection(List<DetectType> types, int? range = null)
  {
    // TODO: Hokey. Do something more general if more DetectTypes are added.
    var typeDescription = "exits and items";
    if (types.Count == 1)
    {
      if (types[0] == DetectType.exit)
      {
        typeDescription = "exits";
      }
      else
      {
        typeDescription = "items";
      }
    }

    var description = $"Detects {typeDescription}";
    if (range != null)
    {
      description += $" up to {range} steps away";
    }

    use($"{description}.", () => new DetectAction(types, range));
  }

  public void perception(int duration = 5, int distance = 16)
  {
    // TODO: Better description.
    use("Perceive monsters.", () => new PerceiveAction(duration, distance));
  }

  public void resistSalve(Element element)
  {
    use($"Grantes resistance to {element} for 40 turns.",
        () => new ResistAction(40, element));
  }

  public void mapping(int distance, bool illuminate = false)
  {
    var description =
        $"Imparts knowledge of the dungeon up to {distance} steps from the hero.";
    if (illuminate)
    {
      description += " Illuminates the dungeon.";
    }

    use(description, () => new MappingAction(distance, illuminate: illuminate));
  }

  public void haste(int amount, int duration)
  {
    use($"Raises speed by {amount} for {duration} turns.",
        () => new HasteAction(amount, duration));
  }

  public void teleport(int distance)
  {
    use($"Attempts to teleport up to {distance} steps away.",
        () => new TeleportAction(distance));
  }

  // TODO: Take list of conditions to cure?
  public void heal(int amount, bool curePoison = false)
  {
    use($"Instantly heals {amount} lost health.",
        () => new HealAction(amount, curePoison: curePoison));
  }

  /// Sets a use and toss use that creates an expanding ring of elemental
  /// damage.
  public void ball(Element element, string noun, string verb, int damage,
      int range = 3)
  {
    var attack = new Attack(new Noun(noun), verb, damage, range, element);

    use(
        $"Unleashes a ball of {element} that inflicts {damage} damage out to {range} steps from the hero.",
        () => new RingSelfAction(attack));
    tossUse((pos) => new RingFromAction(attack, pos));
  }

  /// Sets a use and toss use that creates a flow of elemental damage.
  public void flow(Element element, string noun, string verb, int damage,
      int range = 5, bool fly = false)
  {
    var attack = new Attack(new Noun(noun), verb, damage, range, element);

    var motility = Motility.walk;
    if (fly) motility |= Motility.fly;

    use(
        $"Unleashes a flow of {element} that inflicts {damage} damage out to " +
        $"{range} steps from the hero.",
        () => new FlowSelfAction(attack, motility));
    tossUse((pos) => new FlowFromAction(attack, pos, motility));
  }

  public void lightSource(int level, int? range)
  {
    _emanation = level;

    if (range != null)
    {
      use($"Illuminates out to a range of {range}.",
          () => new IlluminateSelfAction(range.Value));
    }
  }

  public static void finishItem()
  {
    var builder = _item;
    if (builder == null) return;

    var appearance = new Glyph(_category._glyph, builder._color);

    Toss toss = null;
    var tossDamage = builder._tossDamage ?? _category._tossDamage;
    if (tossDamage != null)
    {
      var noun = new Noun($"the {builder._name.ToLower()}");
      var verb = "hits";
      if (_category._verb != null)
      {
        verb = Log.conjugate(_category._verb!, Pronoun.it);
      }

      var range = builder._tossRange ?? _category._tossRange;
      Debugger.assert(range != null);
      var element =
          builder._tossElement ?? _category._tossElement ?? Element.none;
      var use = builder._tossUse ?? _category._tossUse;
      var breakage = _category._breakage ?? builder._breakage ?? 0;

      var tossAttack = new Attack(noun, verb, tossDamage.Value, range.Value, element);
      toss = new Toss(breakage, tossAttack, use);
    }

    var itemType = new ItemType(
        builder._name,
        appearance,
        builder._minDepth,
        _sortIndex++,
        _category._equipSlot,
        _category._weaponType,
        builder._use,
        builder._attack,
        toss,
        builder._defense,
        builder._armor ?? 0,
        builder._price,
        builder._maxStack ?? _category._maxStack ?? 1,
        weight: builder._weight ?? 0,
        heft: builder._heft ?? 0,
        emanation: builder._emanation ?? _category._emanation,
        fuel: builder._fuel ?? _category._fuel,
        treasure: _category._isTreasure,
        twoHanded: _category._isTwoHanded);

    foreach (var kv in _category._destroyChance) itemType.destroyChance.Add(kv.Key, kv.Value);
    foreach (var kv in builder._destroyChance) itemType.destroyChance.Add(kv.Key, kv.Value);

    itemType.skills.AddRange(_category._skills);
    itemType.skills.AddRange(builder._skills);

    Items.types.addRanged(itemType,
        name: itemType.name,
        start: builder._minDepth,
        end: builder._maxDepth,
        startFrequency: builder._frequency,
        tags: _category._tag);

    _item = null;
  }
}

class _AffixBuilder
{

  public static string _affixTag;
  public static _AffixBuilder _affix;
  public static void affixCategory(string tag)
  {
    finishAffix();
    _affixTag = tag;
  }

  public static _AffixBuilder affix(string name, double frequency)
  {
    finishAffix();

    bool isPrefix;
    if (name.EndsWith(" _"))
    {
      name = name.Substring(0, name.Length - 2);
      isPrefix = true;
    }
    else if (name.StartsWith("_ "))
    {
      name = name.Substring(2);
      isPrefix = false;
    }
    else
    {
      throw new System.Exception($"Affix \"{name}\" must start or end with \"_\".");
    }

    return _affix = new _AffixBuilder(name, isPrefix, frequency);
  }


  public string _name;
  public bool _isPrefix;
  int? _minDepth;
  int? _maxDepth;
  public double _frequency;

  double? _heftScale;
  int? _weightBonus;
  int? _strikeBonus;
  double? _damageScale;
  int? _damageBonus;
  Element _brand;
  int? _armor;
  int? _priceBonus;
  double? _priceScale;

  public Dictionary<Element, int> _resists = new Dictionary<Element, int>();
  public Dictionary<Stat, int> _statBonuses = new Dictionary<Stat, int>();

  _AffixBuilder(string _name, bool _isPrefix, double _frequency)
  {
    this._name = _name;
    this._isPrefix = _isPrefix;
    this._frequency = _frequency;
  }

  /// Sets the affix's minimum depth to [from]. If [to] is given, then the
  /// affix has the given depth range. Otherwise, its max range is
  /// [Option.maxDepth].
  public void depth(int from, int to = Option.maxDepth)
  {
    _minDepth = from;
    _maxDepth = to;
  }

  public void heft(double scale)
  {
    _heftScale = scale;
  }

  public void weight(int bonus)
  {
    _weightBonus = bonus;
  }

  void strike(int bonus)
  {
    _strikeBonus = bonus;
  }

  public void damage(double? scale = null, int? bonus = null)
  {
    _damageScale = scale;
    _damageBonus = bonus;
  }

  public void brand(Element element, int resist = 1)
  {
    _brand = element;

    // By default, branding also grants resistance.
    _resists[element] = resist;
  }

  public void armor(int armor)
  {
    _armor = armor;
  }

  public void resist(Element element, int power = 1)
  {
    _resists[element] = power;
  }

  public void strength(int bonus)
  {
    _statBonuses[Stat.strength] = bonus;
  }

  void agility(int bonus)
  {
    _statBonuses[Stat.agility] = bonus;
  }

  public void fortitude(int bonus)
  {
    _statBonuses[Stat.fortitude] = bonus;
  }

  public void intellect(int bonus)
  {
    _statBonuses[Stat.intellect] = bonus;
  }

  public void will(int bonus)
  {
    _statBonuses[Stat.will] = bonus;
  }

  public void price(int bonus, double scale)
  {
    _priceBonus = bonus;
    _priceScale = scale;
  }

  public static void finishAffix()
  {
    var builder = _affix;
    if (builder == null) return;

    var affixes = builder._isPrefix ? Affixes.prefixes : Affixes.suffixes;

    var displayName = builder._name;
    var fullName = $"{displayName} ({_affixTag})";
    var index = 1;

    // Generate a unique name for it.
    while (affixes.tryFind(fullName) != null)
    {
      index++;
      fullName = $"{displayName} ({_affixTag} {index})";
    }

    var affix = new Affix(fullName, displayName,
        heftScale: builder._heftScale,
        weightBonus: builder._weightBonus,
        strikeBonus: builder._strikeBonus,
        damageScale: builder._damageScale,
        damageBonus: builder._damageBonus,
        brand: builder._brand,
        armor: builder._armor,
        priceBonus: builder._priceBonus,
        priceScale: builder._priceScale);

    foreach (var kv in builder._resists)
      affix.resist(kv.Key, kv.Value);
    foreach (var kv in builder._statBonuses)
      affix.setStatBonus(kv.Key, kv.Value);

    affixes.addRanged(affix,
        name: fullName,
        start: builder._minDepth,
        end: builder._maxDepth,
        startFrequency: builder._frequency,
        endFrequency: builder._frequency,
        tags: _affixTag);
    _affix = null;
  }
}


