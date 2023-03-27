using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// Generates the town.
class Town
{
  public Stage stage;

  public Town(Stage stage)
  {
    this.stage = stage;
  }

  public IEnumerator buildStage(System.Action<Vec> placeHero)
  {
    foreach (var pos in stage.bounds)
    {
      // TODO: Grass? Something else?
      stage[pos].type = Tiles.flagstoneFloor;
    }

    foreach (var pos in stage.bounds.trace())
    {
      // TODO: Impenetrable wall when we add destructibility.
      stage[pos].type = Tiles.flagstoneWall;
    }

    var entrances = new List<TileType>{
      Tiles.dungeonEntrance,
      Tiles.home,
      Tiles.shop1,
      Tiles.shop2,
      Tiles.shop3,
      Tiles.shop4,
      Tiles.shop5,
      Tiles.shop6,
    };

    // TODO: Place more interestingly.
    for (var i = 0; i < entrances.Count; i++)
    {
      var x = (i % 4) * 13 + 5;
      var y = (i / 4) * 14 + 6;

      var rect = new Rect(x, y, 11, 8);

      foreach (var pos in rect)
      {
        stage[pos].type = Tiles.flagstoneWall;
      }

      Vec door;
      if ((i / 4).isOdd())
      {
        door = (rect.topLeft + rect.topRight) / 2;
      }
      else
      {
        door = ((rect.bottomLeft + rect.bottomRight) / 2).offsetY(-1);
      }

      stage[door].type = entrances[i];
      stage[door.offsetX(-1)].type = Tiles.wallTorch;
      stage[door.offsetX(1)].type = Tiles.wallTorch;
    }

    // The town is always fully lit and explored.
    foreach (var pos in stage.bounds)
    {
      var tile = stage[pos];

      tile.updateExplored(force: true);

      if (tile.isFlyable) tile.addEmanation(64);
    }

    placeHero(stage.bounds.center);
    yield return null;
  }
}
