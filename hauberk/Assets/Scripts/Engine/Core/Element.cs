using System;
using System.Collections;
using System.Collections.Generic;
using num = System.Double;

public class Element
{
  public static Element none = new Element("none", "No", 1.0);

  public string name;
  public string abbreviation;

  /// Text displayed when an item is destroyed by this element.
  public string destroyMessage;

  /// Whether this element emanates light when a substance on the ground.
  public bool emanates;

  /// The multiplier to experience gained when killing a monster with a move or
  /// attack using this element.
  public double experience;

  public string capitalized => $"{name[0].ToString().ToUpper()}{name.Substring(1)}";

  /// Creates a side-effect action to perform when an [Attack] of this element
  /// hits an actor for `damage` or `null` if this element has no side effect.
  public Func<int, Action> attackAction;

  /// Creates a side-effect action to perform when an area attack of this
  /// element hits a tile or `null` if this element has no effect.
  public Func<Vec, Hit, num, int, Action> floorAction;

  public Element(string name, string abbreviation, double experience,
      bool emanates = false, string destroyMessage = "",
      System.Func<int, Action> attack = null,
      System.Func<Vec, Hit, num, int, Action> floor = null)
  {
    this.name = name;
    this.abbreviation = abbreviation;
    this.experience = experience;
    this.emanates = emanates;
    this.destroyMessage = destroyMessage;
    this.attackAction = attack ?? ((_) => null);
    this.floorAction = floor ?? ((_, __, ___, ____) => null);
  }

  public override string ToString() => name;
}
