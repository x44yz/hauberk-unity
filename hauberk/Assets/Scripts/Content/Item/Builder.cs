using System.Collections;
using System.Collections.Generic;
using System.Linq;

class _BaseBuilder {
  public List<Skill> _skills = new List<Skill>();
  public Dictionary<Element, int> _destroyChance = new Dictionary<Element, int>();

  int? _maxStack;
  Element? _tossElement;
  int? _tossDamage;
  int? _tossRange;
  TossItemUse? _tossUse;
  int? _emanation;
  int? _fuel;

  /// Percent chance of objects in the current category breaking when thrown.
  int? _breakage;

  void stack(int stack) {
    _maxStack = stack;
  }

  /// Makes items in the category throwable.
  void toss(int? damage, Element? element, int? range, int? breakage) {
    _tossDamage = damage;
    _tossElement = element;
    _tossRange = range;
    _breakage = breakage;
  }

  public void tossUse(TossItemUse use) {
    _tossUse = use;
  }

  void destroy(Element element, int chance, int? fuel) {
    _destroyChance[element] = chance;
    // TODO: Per-element fuel.
    _fuel = fuel;
  }

  void skill(string skill) {
    _skills.add(Skills.find(skill));
  }

  void skills(List<string> skills) {
    _skills.addAll(skills.map(Skills.find));
  }
}

class _CategoryBuilder : _BaseBuilder {

    public static _CategoryBuilder _category;
    _CategoryBuilder category(int glyph, string verb = null, int? stack = null) {
        finishItem();

        _category = new _CategoryBuilder(glyph, verb);
        _category._maxStack = stack;

        return _category;
    }

  /// The current glyph's character code. Any items defined will use this.
  public int _glyph;
  public string? _verb;

  string? _equipSlot;
  string? _weaponType;
    public string _tag;
  bool _isTreasure = false;
  bool _isTwoHanded = false;

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

  void tag(string tagPath) {
    // Define the tag path and store the leaf tag which is what gets used by
    // the item types.
    Items.types.defineTags($"item/{tagPath}");
    var tags = tagPath.Split('/').ToList();
    _tag = tags.Last();

    if (tags.Contains("shield") || tags.Contains("light")) {
      _equipSlot = "hand";
    } else if (tags.Contains("weapon")) {
      // TODO: Handle two-handed weapons.
      _equipSlot = "hand";
      _weaponType = tags[tags.IndexOf("weapon") + 1];
    } else {
      foreach (var equipSlot in tagEquipSlots) {
        if (tags.Contains(equipSlot)) {
          _equipSlot = equipSlot;
          break;
        }
      }
    }

    // TODO: Hacky. We need a matching tag hiearchy for affixes so that, for
    // example, a "sword" item will match a "weapon" affix.
    Affixes.defineItemTag(tagPath);
  }

  void treasure() {
    _isTreasure = true;
  }

  void twoHanded() {
    _isTwoHanded = true;
  }
}

class _ItemBuilder : _BaseBuilder {

public static int _sortIndex = 0;
public static _ItemBuilder? _item;
public static _ItemBuilder item(string name, Color color,
    double frequency = 1.0, int price = 0) {
  finishItem();

  return _item = new _ItemBuilder(name, color, frequency, price);
}

  public string _name;
  public Color _color;
  public double _frequency;
  public int _price;
  ItemUse? _use;
  Attack? _attack;
  Defense? _defense;
  int? _weight;
  int? _heft;
  int? _armor;

  // TODO: Instead of late final, initialize these in item() instead of depth().
  public int _minDepth;
  public int _maxDepth;

  _ItemBuilder(string _name, Color _color, double _frequency, int _price)
  {
    this._name = _name;
    this._color = _color;
    this._frequency = _frequency;
    this._price = _price;
  }

  /// Sets the item's minimum depth to [from]. If [to] is given, then the item
  /// has the given depth range. Otherwise, its max is [Option.maxDepth].
  void depth(int from, int? to) {
    _minDepth = from;
    _maxDepth = to ?? Option.maxDepth;
  }

  void defense(int amount, string message) {
    DartUtils.assert(_defense == null);
    _defense = new Defense(amount, message);
  }

  void armor(int armor, int? weight) {
    _armor = armor;
    _weight = weight;
  }

  void weapon(int damage, int heft, Element? element) {
    _attack = new Attack(null, _category._verb!, damage, 0, element);
    _heft = heft;
  }

  void ranged(string noun,
    int heft, int damage, int range) 
    {
    _attack = new Attack(new Noun(noun), "pierce[s]", damage, range);
    // TODO: Make this per-item once it does something.
    _heft = heft;
  }

  void use(string description, System.Func<Action> createAction) {
    _use = new ItemUse(description, createAction);
  }

  void food(int amount) {
    use($"Provides {amount} turns of food.", () => new EatAction(amount));
  }

  void detection(List<DetectType> types, int? range) {
    // TODO: Hokey. Do something more general if more DetectTypes are added.
    var typeDescription = "exits and items";
    if (types.length == 1) {
      if (types[0] == DetectType.exit) {
        typeDescription = "exits";
      } else {
        typeDescription = "items";
      }
    }

    var description = $"Detects {typeDescription}";
    if (range != null) {
      description += $" up to {range} steps away";
    }

    use($"{description}.", () => new DetectAction(types, range));
  }

  void perception(int duration = 5, int distance = 16) {
    // TODO: Better description.
    use("Perceive monsters.", () => new PerceiveAction(duration, distance));
  }

  void resistSalve(Element element) {
    use($"Grantes resistance to {element} for 40 turns.",
        () => new ResistAction(40, element));
  }

  void mapping(int distance, bool illuminate = false) {
    var description =
        "Imparts knowledge of the dungeon up to $distance steps from the hero.";
    if (illuminate) {
      description += " Illuminates the dungeon.";
    }

    use(description, () => new MappingAction(distance, illuminate: illuminate));
  }

  void haste(int amount, int duration) {
    use("Raises speed by $amount for $duration turns.",
        () => new HasteAction(amount, duration));
  }

  void teleport(int distance) {
    use("Attempts to teleport up to $distance steps away.",
        () => new TeleportAction(distance));
  }

  // TODO: Take list of conditions to cure?
  void heal(int amount, bool curePoison = false) {
    use("Instantly heals $amount lost health.",
        () => new HealAction(amount, curePoison: curePoison));
  }

  /// Sets a use and toss use that creates an expanding ring of elemental
  /// damage.
  void ball(Element element, string noun, string verb, int damage,
      int range = 3) {
    var attack = new Attack(new Noun(noun), verb, damage, range, element);

    use(
        $"Unleashes a ball of {element} that inflicts {damage} damage out to {range} steps from the hero.",
        () => new RingSelfAction(attack));
    tossUse((pos) => new RingFromAction(attack, pos));
  }

  /// Sets a use and toss use that creates a flow of elemental damage.
  void flow(Element element, string noun, string verb, int damage,
      int range = 5, bool fly = false) {
    var attack = new Attack(new Noun(noun), verb, damage, range, element);

    var motility = Motility.walk;
    if (fly) motility |= Motility.fly;

    use(
        $"Unleashes a flow of {element} that inflicts {damage} damage out to " +
        $"{range} steps from the hero.",
        () => new FlowSelfAction(attack, motility));
    tossUse((pos) => FlowFromAction(attack, pos, motility));
  }

  void lightSource(required int level, int? range) {
    _emanation = level;

    if (range != null) {
      use("Illuminates out to a range of $range.",
          () => new IlluminateSelfAction(range));
    }
  }

    public static void finishItem() {
    var builder = _item;
    if (builder == null) return;

    var appearance = Glyph.fromCharCode(_category._glyph, builder._color);

    Toss? toss;
    var tossDamage = builder._tossDamage ?? _category._tossDamage;
    if (tossDamage != null) {
        var noun = Noun("the ${builder._name.toLowerCase()}");
        var verb = "hits";
        if (_category._verb != null) {
        verb = Log.conjugate(_category._verb!, Pronoun.it);
        }

        var range = builder._tossRange ?? _category._tossRange;
        assert(range != null);
        var element =
            builder._tossElement ?? _category._tossElement ?? Element.none;
        var use = builder._tossUse ?? _category._tossUse;
        var breakage = _category._breakage ?? builder._breakage ?? 0;

        var tossAttack = Attack(noun, verb, tossDamage, range, element);
        toss = Toss(breakage, tossAttack, use);
    }

    var itemType = ItemType(
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

    itemType.destroyChance.addAll(_category._destroyChance);
    itemType.destroyChance.addAll(builder._destroyChance);

    itemType.skills.addAll(_category._skills);
    itemType.skills.addAll(builder._skills);

    Items.types.addRanged(itemType,
        name: itemType.name,
        start: builder._minDepth,
        end: builder._maxDepth,
        startFrequency: builder._frequency,
        tags: _category._tag);

    _item = null;
    }
}

class _AffixBuilder {

public static string _affixTag;
public static _AffixBuilder? _affix;
public static void affixCategory(string tag) {
  finishAffix();
  _affixTag = tag;
}

public static _AffixBuilder affix(string name, double frequency) {
  finishAffix();

  bool isPrefix;
  if (name.EndsWith(" _")) {
    name = name.Substring(0, name.Length - 2);
    isPrefix = true;
  } else if (name.StartsWith("_ ")) {
    name = name.Substring(2);
    isPrefix = false;
  } else {
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
  Element? _brand;
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
  void depth(int from, int to = Option.maxDepth) {
    _minDepth = from;
    _maxDepth = to;
  }

  void heft(double scale) {
    _heftScale = scale;
  }

  void weight(int bonus) {
    _weightBonus = bonus;
  }

  void strike(int bonus) {
    _strikeBonus = bonus;
  }

  void damage(double? scale, int? bonus) {
    _damageScale = scale;
    _damageBonus = bonus;
  }

  void brand(Element element, int resist = 1) {
    _brand = element;

    // By default, branding also grants resistance.
    _resists[element] = resist;
  }

  void armor(int armor) {
    _armor = armor;
  }

  void resist(Element element, int power = 1) {
    _resists[element] = power;
  }

  void strength(int bonus) {
    _statBonuses[Stat.strength] = bonus;
  }

  void agility(int bonus) {
    _statBonuses[Stat.agility] = bonus;
  }

  void fortitude(int bonus) {
    _statBonuses[Stat.fortitude] = bonus;
  }

  void intellect(int bonus) {
    _statBonuses[Stat.intellect] = bonus;
  }

  void will(int bonus) {
    _statBonuses[Stat.will] = bonus;
  }

  void price(int bonus, double scale) {
    _priceBonus = bonus;
    _priceScale = scale;
  }

  public static void finishAffix() {
    var builder = _affix;
    if (builder == null) return;

    var affixes = builder._isPrefix ? Affixes.prefixes : Affixes.suffixes;

    var displayName = builder._name;
    var fullName = $"{displayName} ({_affixTag})";
    var index = 1;

    // Generate a unique name for it.
    while (affixes.tryFind(fullName) != null) {
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

    builder._resists.forEach(affix.resist);
    builder._statBonuses.forEach(affix.setStatBonus);

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


