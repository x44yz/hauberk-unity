using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

  Affix(this.name, this.displayName,
      {double? heftScale,
      int? weightBonus,
      int? strikeBonus,
      double? damageScale,
      int? damageBonus,
      Element? brand,
      int? armor,
      int? priceBonus,
      double? priceScale})
      : heftScale = heftScale ?? 1.0,
        weightBonus = weightBonus ?? 0,
        strikeBonus = strikeBonus ?? 0,
        damageScale = damageScale ?? 1.0,
        damageBonus = damageBonus ?? 1,
        brand = brand ?? Element.none,
        armor = armor ?? 0,
        priceBonus = priceBonus ?? 0,
        priceScale = priceScale ?? 1.0;

  int resistance(Element element) => _resists[element] ?? 0;

  void resist(Element element, int power) {
    _resists[element] = power;
  }

  int statBonus(Stat stat) => _statBonuses[stat] ?? 0;

  void setStatBonus(Stat stat, int bonus) {
    _statBonuses[stat] = bonus;
  }

  string toString() => name;
}
