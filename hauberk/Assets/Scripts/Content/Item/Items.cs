using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Static class containing all of the [ItemType]s.
class Items {
    public static ResourceSet<ItemType> types = new ResourceSet<ItemType>();

    public static void litter() {
        category(CharCode.latinCapitalLetterCWithCedilla, stack: 10)
            ..tag("item")
            ..toss(damage: 3, range: 7, element: Elements.earth, breakage: 10);
        item("Rock", tan, frequency: 0.1).depth(1);

        category(CharCode.latinSmallLetterUWithDiaeresis, stack: 4)
            ..tag("item")
            ..toss(damage: 2, range: 5, breakage: 30);
        item("Skull", lightCoolGray, frequency: 0.1).depth(1);
    }

  static void initialize() {
    types.defineTags("item");

    litter();
    treasure();
    pelts();
    food();
    lightSources();
    potions();
    scrolls();
    spellBooks();
    // TODO: Rings.
    // TODO: Amulets.
    // TODO: Wands.
    weapons();
    helms();
    bodyArmor();
    cloaks();
    gloves();
    shields();
    boots();

    // CharCode.latinSmallLetterIWithDiaeresis // ring
    // CharCode.latinSmallLetterIWithCircumflex // wand

    finishItem();
  }
}
