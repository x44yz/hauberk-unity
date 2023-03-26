using System.Collections;
using System.Collections.Generic;


/// A modifier that can be applied to an [Item] to change its capabilities.
/// For example, in a "Dagger of Wounding", the "of Wounding" part is an affix.
public class Affix
{
  /// The full unique name of the affixes.
  ///
  /// It's possible for different affixes to have the same name but different
  /// values or applying to different equipment. For storage, we need to know
  /// which one it actually is, which this distinguishes.
  public string name;

  /// The short, shown name of the affix.
  public string displayName;

  public double heftScale;
  public int weightBonus;
  public int strikeBonus;
  public double damageScale;
  public int damageBonus;
  public Element brand;
  public int armor;

  public Dictionary<Element, int> _resists = new Dictionary<Element, int>();
  public Dictionary<Stat, int> _statBonuses = new Dictionary<Stat, int>();

  public int priceBonus;
  public double priceScale;

  public Affix(string name, string displayName,
      double? heftScale,
      int? weightBonus,
      int? strikeBonus,
      double? damageScale,
      int? damageBonus,
      Element brand,
      int? armor,
      int? priceBonus,
      double? priceScale)
  {
    this.name = name;
    this.displayName = displayName;

    this.heftScale = heftScale ?? 1.0;
    this.weightBonus = weightBonus ?? 0;
    this.strikeBonus = strikeBonus ?? 0;
    this.damageScale = damageScale ?? 1.0;
    this.damageBonus = damageBonus ?? 1;
    this.brand = brand ?? Element.none;
    this.armor = armor ?? 0;
    this.priceBonus = priceBonus ?? 0;
    this.priceScale = priceScale ?? 1.0;
  }

  public int resistance(Element element)
  {
    int val = 0;
    _resists.TryGetValue(element, out val);
    return val;
  }

  public void resist(Element element, int power)
  {
    _resists[element] = power;
  }

  public int statBonus(Stat stat)
  {
    int val = 0;
    _statBonuses.TryGetValue(stat, out val);
    return val;
  }

  public void setStatBonus(Stat stat, int bonus)
  {
    _statBonuses[stat] = bonus;
  }

  public override string ToString() => name;
}
