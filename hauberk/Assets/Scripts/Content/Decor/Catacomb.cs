using System.Collections;
using System.Collections.Generic;
using System.Linq;

void catacombDecor() {
  category(themes: "catacomb dungeon", cells: {
    "!": applyOpen(Tiles.candle),
  });

  // TODO: Looks kind of hokey.
  furnishing(template: """
    ?.?
    .!.
    ?.?""");
}
