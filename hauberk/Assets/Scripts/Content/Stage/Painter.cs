using System;
using System.Collections.Generic;
using System.Linq;

/// The procedural interface exposed by [Decorator] to let a [PaintStyle]
/// modify the stage.
public class Painter
{
  public Decorator _decorator;
  public Architect _architect;
  public Architecture _architecture;
  int _painted = 0;

  public Painter(Decorator _decorator, Architect _architect, Architecture _architecture)
  {
    this._decorator = _decorator;
    this._architect = _architect;
    this._architecture = _architecture;
  }

  public Rect bounds => _architect.stage.bounds;

  public List<Vec> ownedTiles => _decorator.tilesFor(_architecture);

  public int paintedCount => _painted;

  public int depth => _architect.depth;

  public bool ownsTile(Vec pos) => _architect.ownerAt(pos) == _architecture;

  public TileType getTile(Vec pos)
  {
    return _architect.stage[pos].type;
  }

  public void setTile(Vec pos, TileType type)
  {
    Debugger.assert(_architect.ownerAt(pos) == _architecture);
    _architect.stage[pos].type = type;
    _painted++;
  }

  public bool hasActor(Vec pos) => _architect.stage.actorAt(pos) != null;

  public Breed chooseBreed(int depth, string tag = null, bool? includeParentTags = null)
  {
    return _decorator.chooseBreed(depth,
        tag: tag, includeParentTags: includeParentTags);
  }

  public void spawnMonster(Vec pos, Breed breed)
  {
    _decorator.spawnMonster(pos, breed);
  }
}

/// Each style is a custom "look" that translates the semantic temporary
/// tiles into specific concrete tile types.
public class PaintStyle
{
  static public PaintStyle rock = new PaintStyle();
  static public PaintStyle flagstone = new PaintStyle(
      floor: new List<TileType> { Tiles.flagstoneFloor },
      wall: new List<TileType> { Tiles.flagstoneWall },
      closedDoor: Tiles.closedDoor,
      openDoor: Tiles.openDoor);
  static public PaintStyle granite = new PaintStyle(
      floor: new List<TileType> { Tiles.graniteFloor },
      wall: new List<TileType> { Tiles.graniteWall },
      closedDoor: Tiles.closedSquareDoor);
  static public PaintStyle stoneJail = new PaintStyle(closedDoor: Tiles.closedBarredDoor);

  static public Dictionary<TileType, List<TileType>> _defaultTypes = new Dictionary<TileType, List<TileType>>(){
    {Tiles.solidWet, new List<TileType>{Tiles.water}},
    {Tiles.passageWet, new List<TileType>{Tiles.bridge}}
  };

  static public List<TileType> _defaultWalls = new List<TileType>(){
    Tiles.granite1,
    Tiles.granite2,
    Tiles.granite3
  };

  public List<TileType> _floor;
  public List<TileType> _wall;
  public TileType _closedDoor;
  public TileType _openDoor;

  public PaintStyle(
      List<TileType> floor = null,
      List<TileType> wall = null,
      TileType closedDoor = null,
      TileType openDoor = null)
  {
    _floor = floor;
    _wall = wall;
    _closedDoor = closedDoor;
    _openDoor = openDoor;
  }

  public TileType paintTile(Painter painter, Vec pos)
  {
    var tile = painter.getTile(pos);

    if (tile == Tiles.open || tile == Tiles.passage) return _floorTile();

    if (tile == Tiles.solid)
    {
      if (_wall != null) return Rng.rng.item(_wall!);
      return Rng.rng.item(_defaultWalls);
    }

    if (tile == Tiles.doorway)
    {
      if (_closedDoor != null && _openDoor != null)
      {
        switch (Rng.rng.range(6))
        {
          case 0:
            return _openDoor!;
          case 1:
            return _floorTile();
          default:
            return _closedDoor!;
        }
      }
      else if (_closedDoor != null)
      {
        return _closedDoor!;
      }
      else if (_openDoor != null)
      {
        return _openDoor!;
      }
      else
      {
        return _floorTile();
      }
    }

    if (_defaultTypes.ContainsKey(tile)) return Rng.rng.item(_defaultTypes[tile]!);

    return tile;
  }

  TileType _floorTile()
  {
    if (_floor != null) return Rng.rng.item(_floor!);

    return Tiles.flagstoneFloor;
  }
}
