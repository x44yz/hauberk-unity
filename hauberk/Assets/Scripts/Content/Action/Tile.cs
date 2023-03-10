using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// Base class for actions that open a container tile.
abstract class _OpenTileAction : Action {
  public Vec _pos;

  public _OpenTileAction(Vec _pos)
  {
    this._pos = _pos;
  }

  string _name;

  TileType _openTile;

  // TODO: Do something more sophisticated. Take into account the theme where
  // the tile is.

  int _minDepthEmptyChance;

  int _maxDepthEmptyChance;

  public override ActionResult onPerform() {
    game.stage[_pos].type = _openTile;
    addEvent(EventType.openBarrel, pos: _pos);

    // TODO: Chance of monster in it?
    // TODO: Traps. Locks.
    if (Rng.rng.percent(MathUtils.lerpInt(game.depth, 1, Option.maxDepth,
        _minDepthEmptyChance, _maxDepthEmptyChance))) {
      log($"The {_name} is empty.", actor);
    } else {
      game.stage.placeDrops(_pos, Motility.walk, _createDrop());

      log($"{1} open[s] the {_name}.", actor);
    }

    return ActionResult.success;
  }

  public abstract Drop _createDrop();
}

/// Open a barrel and place its drops.
class OpenBarrelAction : _OpenTileAction {
  public OpenBarrelAction(Vec pos) : base(pos)
  {
  }

  string _name => "barrel";

  TileType _openTile => Tiles.openBarrel;

  int _minDepthEmptyChance => 40;

  int _maxDepthEmptyChance => 10;

  // TODO: More sophisticated drop.
  public override Drop _createDrop() => DropUtils.parseDrop("food", depth: game.depth);
}

/// Open a chest and place its drops.
class OpenChestAction : _OpenTileAction {
  public OpenChestAction(Vec pos) : base(pos)
  {

  }

  string _name => "chest";

  TileType _openTile => Tiles.openChest;

  int _minDepthEmptyChance => 20;

  int _maxDepthEmptyChance => 2;

  // TODO: Drop more than one item sometimes.
  public override Drop _createDrop() => DropUtils.dropOneOf(new Dictionary<Drop, double>(){
        {DropUtils.parseDrop("treasure", depth: game.depth), 0.5},
        {DropUtils.parseDrop("magic", depth: game.depth), 0.2},
        {DropUtils.parseDrop("equipment", depth: game.depth), 0.3}
      });
}
