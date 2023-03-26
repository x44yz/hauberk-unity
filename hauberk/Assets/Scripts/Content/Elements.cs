using System.Collections;
using System.Collections.Generic;

class Elements
{
  // TODO: Teleport items.
  static public Element air = new Element("air", "Ai", 1.2, attack: (_) => new WindAction());
  static public Element earth = new Element("earth", "Ea", 1.1);

  static public Element fire = new Element("fire", "Fi", 1.2,
      emanates: true,
      destroyMessage: "burns up",
      attack: (_) => new BurnActorAction(),
      floor: (pos, hit, distance, fuel) =>
          new BurnFloorAction(pos, (int)hit.averageDamage, fuel));

  // TODO: Push back attack action.
  // TODO: Move items on floor.
  static public Element water = new Element("water", "Wa", 1.3);

  // TODO: Destroy items on floor and in inventory.
  static public Element acid = new Element("acid", "Ac", 1.4);

  static public Element cold = new Element("cold", "Co", 1.2,
      destroyMessage: "shatters",
      attack: (damage) => new FreezeActorAction(damage),
      floor: (pos, hit, distance, _) => new FreezeFloorAction(pos));

  // TODO: Break glass items. Recharge some items?
  static public Element lightning = new Element("lightning", "Ln", 1.1);

  static public Element poison = new Element("poison", "Po", 2.0,
      attack: (damage) => new PoisonAction(damage),
      floor: (pos, hit, distance, _) =>
          new PoisonFloorAction(pos, (int)hit.averageDamage));

  // TODO: Remove tile emanation.
  static public Element dark =
      new Element("dark", "Dk", 1.5, attack: (damage) => new BlindAction(damage));

  static public Element light = new Element("light", "Li", 1.5,
      attack: (damage) => new DazzleAction(damage),
      floor: (pos, hit, distance, _) => new LightFloorAction(pos, hit, distance));

  // TODO: Drain experience.
  static public Element spirit = new Element("spirit", "Sp", 3.0);

  static public List<Element> all = new List<Element>(){
    Element.none,
    air,
    earth,
    fire,
    water,
    acid,
    cold,
    lightning,
    poison,
    dark,
    light,
    spirit
  };
}

