using System.Collections;
using System.Collections.Generic;
using System.Linq;

partial class Decor
{
  public static void caveDecor()
  {
    category(themes: "glowing-moss", cells: new Dictionary<string, Cell>(){
      {"*", applyOpen(Tiles.glowingMoss)},
    });

    furnishing(symmetry: Symmetry.rotate90, template: @"
      #
      *");

    furnishing(symmetry: Symmetry.rotate90, template: @"
      ##
      #*");

    furnishing(template: @"
      ?.?
      .*.
      ?.?");
  }
}
