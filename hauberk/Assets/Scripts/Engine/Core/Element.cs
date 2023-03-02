using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public string capitalized => "${name[0].toUpperCase()}${name.substring(1)}";

    /// Creates a side-effect action to perform when an [Attack] of this element
    /// hits an actor for `damage` or `null` if this element has no side effect.
    public Action<int> attackAction;

    /// Creates a side-effect action to perform when an area attack of this
    /// element hits a tile or `null` if this element has no effect.
    public Action<Vec, Hit, num, int> floorAction;

    Element(string name, string abbreviation, double experience)
    {
        this.name = name;
        this.abbreviation = abbreviation;
        this.experience = experience;
        this.emanates = false;
        this.destroyMessage = "";
        // Action? Function(int damage)? attack,
        // Action? Function(Vec pos, Hit hit, num distance, int fuel)? floor})
        // : attackAction = attack ?? ((_) => null),
        // floorAction = floor ?? ((_, __, ___, ____) => null);
    }

    string toString() => name;
}
