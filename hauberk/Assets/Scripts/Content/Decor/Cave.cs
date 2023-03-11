using System.Collections;
using System.Collections.Generic;
using System.Linq;

partial class Decor {
  void caveDecor() {
    category(themes: "glowing-moss", cells: {
      "*": applyOpen(Tiles.glowingMoss),
    });

    furnishing(symmetry: Symmetry.rotate90, template: """
      #
      *""");

    furnishing(symmetry: Symmetry.rotate90, template: """
      ##
      #*""");

    furnishing(template: """
      ?.?
      .*.
      ?.?""");
  }
}
