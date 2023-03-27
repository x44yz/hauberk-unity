using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// Uses a cellular automata to carve out rounded open cavernous areas.
class Lake : Architecture
{
  public override IEnumerator build()
  {
    var lakeCount = Rng.rng.inclusive(1, 2);
    for (var i = 0; i < lakeCount; i++)
    {
      _placeLake(Blob.make(Rng.rng.range(16, 32)));
      yield return "Placing lake";
    }
  }

  void _placeLake(Array2D<bool> lake)
  {
    var x = Rng.rng.range(0, width - lake.width);
    var y = Rng.rng.range(0, height - lake.height);

    foreach (var pos in lake.bounds)
    {
      if (lake[pos]) placeWater(pos.offset(x, y));
    }
  }
}
