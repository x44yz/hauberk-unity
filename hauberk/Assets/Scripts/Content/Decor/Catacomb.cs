using System.Collections;
using System.Collections.Generic;
using System.Linq;

partial class Decor
{
  public static void catacombDecor()
  {
    category(themes: "catacomb dungeon", cells: new Dictionary<string, Cell>(){
      {"!", applyOpen(Tiles.candle)},
    });

    // TODO: Looks kind of hokey.
    furnishing(template: @"
      ?.?
      .!.
      ?.?");
  }
}