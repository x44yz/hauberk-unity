using System.Collections;
using System.Collections.Generic;

/// Static class containing all of the [ItemType]s.
public partial class Items
{
  public static ResourceSet<ItemType> types = new ResourceSet<ItemType>();

  public static void initialize()
  {
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

    _ItemBuilder.finishItem();
  }
}
