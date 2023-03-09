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

  public Affix(string name, string displayName,
      double heftScale = 1.0,
      int weightBonus = 0,
      int strikeBonus = 0,
      double damageScale = 1.0,
      int damageBonus = 1,
      Element brand = null,
      int armor = 0,
      int priceBonus = 0,
      double priceScale = 1.0)
    {
      this.name = name;
      this.displayName = displayName;

      this.heftScale = heftScale;
      this.weightBonus = weightBonus;
      this.strikeBonus = strikeBonus;
      this.damageScale = damageScale;
      this.damageBonus = damageBonus;
      this.brand = brand ?? Element.none;
      this.armor = armor;
      this.priceBonus = priceBonus;
      this.priceScale = priceScale;
    }

  public int resistance(Element element)
  {
    int val = 0;
    _resists.TryGetValue(element, out val);
    return val;
  }

  public void resist(Element element, int power) {
    _resists[element] = power;
  }

  public int statBonus(Stat stat)
  {
    int val = 0;
    _statBonuses.TryGetValue(stat, out val);
    return val;
  }

  public void setStatBonus(Stat stat, int bonus) {
    _statBonuses[stat] = bonus;
  }

  string toString() => name;
}
