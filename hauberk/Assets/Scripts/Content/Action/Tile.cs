using System.Collections;
using System.Collections.Generic;

/// Base class for actions that open a container tile.
abstract class _OpenTileAction : Action
{
  public Vec m_pos;

  public _OpenTileAction(Vec _pos)
  {
    this.m_pos = _pos;
  }

  public virtual string _name => "";

  public virtual TileType _openTile => TileType.uninitialized;

  // TODO: Do something more sophisticated. Take into account the theme where
  // the tile is.

  public virtual int _minDepthEmptyChance => 0;

  public virtual int _maxDepthEmptyChance => 0;

  public override ActionResult onPerform()
  {
    game.stage[m_pos].type = _openTile;
    addEvent(EventType.openBarrel, pos: m_pos);

    // TODO: Chance of monster in it?
    // TODO: Traps. Locks.
    if (Rng.rng.percent(MathUtils.lerpInt(game.depth, 1, Option.maxDepth,
        _minDepthEmptyChance, _maxDepthEmptyChance)))
    {
      log($"The {_name} is empty.", actor);
    }
    else
    {
      game.stage.placeDrops(m_pos, Motility.walk, _createDrop());

      log($"{1} open[s] the {_name}.", actor);
    }

    return ActionResult.success;
  }

  public abstract Drop _createDrop();
}

/// Open a barrel and place its drops.
class OpenBarrelAction : _OpenTileAction
{
  public OpenBarrelAction(Vec pos) : base(pos)
  {
  }

  public override string _name => "barrel";

  public override TileType _openTile => Tiles.openBarrel;

  public override int _minDepthEmptyChance => 40;

  public override int _maxDepthEmptyChance => 10;

  // TODO: More sophisticated drop.
  public override Drop _createDrop() => DropUtils.parseDrop("food", depth: game.depth);
}

/// Open a chest and place its drops.
class OpenChestAction : _OpenTileAction
{
  public OpenChestAction(Vec pos) : base(pos)
  {

  }

  public override string _name => "chest";

  public override TileType _openTile => Tiles.openChest;

  public override int _minDepthEmptyChance => 20;

  public override int _maxDepthEmptyChance => 2;

  // TODO: Drop more than one item sometimes.
  public override Drop _createDrop() => DropUtils.dropOneOf(new Dictionary<Drop, double>(){
        {DropUtils.parseDrop("treasure", depth: game.depth), 0.5},
        {DropUtils.parseDrop("magic", depth: game.depth), 0.2},
        {DropUtils.parseDrop("equipment", depth: game.depth), 0.3}
      });
}
