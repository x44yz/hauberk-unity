using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// A template-based decor that applies a set of tiles if it matches a set of
/// existing tiles.
class Furnishing : Decor
{
  // static void initialize() {
  /*
  // Fountains.
  // TODO: Can these be found anywhere else?
  category(0.03, apply: "≈PIl", themes: "aquatic");
  furnishing(Symmetry.none, """
  .....
  .≈≈≈.
  .≈P≈.
  .≈≈≈.
  .....""");

  furnishing(Symmetry.rotate90, """
  #####
  .≈P≈.
  .≈≈≈.
  .....""");

  furnishing(Symmetry.rotate90, """
  ##I##
  .≈P≈.
  .≈≈≈.
  .....""");

  furnishing(Symmetry.rotate90, """
  #I#I#
  .≈P≈.
  .≈≈≈.
  .....""");

  furnishing(Symmetry.rotate90, """
  ##I#I##
  .≈≈P≈≈.
  ..≈≈≈..
  ?.....?""");

  furnishing(Symmetry.rotate90, """
  #######
  .l≈P≈l.
  ..≈≈≈..
  ?.....?""");

  furnishing(Symmetry.rotate90, """
  ##I##
  .≈≈P#
  ..≈≈I
  ?..≈#""");

  // Streams.
  category(10.0, apply: "≈≡", themes: "aquatic");
  furnishing(Symmetry.rotate90, """
  #...#
  #≈≡≈#
  #...#""");

  furnishing(Symmetry.rotate90, """
  #....#
  #≈≈≡≈#
  #....#""");

  furnishing(Symmetry.rotate90, """
  #.....#
  #≈≈≡≈≈#
  #.....#""");

  furnishing(Symmetry.rotate90, """
  #.....#
  #≈≡≈≡≈#
  #.....#""");

  furnishing(Symmetry.rotate90, """
  #......#
  #......#
  #≈≈≡≈≈≈#
  #......#
  #......#""");

  furnishing(Symmetry.rotate90, """
  #......#
  #......#
  #≈≡≈≈≡≈#
  #......#
  #......#""");

  furnishing(Symmetry.rotate90, """
  #.......#
  #≈≈≈≡≈≈≈#
  #.......#
  #.......#""");

  furnishing(Symmetry.rotate90, """
  #.......#
  #.......#
  #≈≈≡≈≡≈≈#
  #.......#
  #.......#""");

  furnishing(Symmetry.rotate90, """
  #.......#
  #.......#
  #≈≡≈≈≈≡≈#
  #.......#
  #.......#""");

  furnishing(Symmetry.rotate90, """
  #........#
  #........#
  #≈≈≈≡≈≈≈≈#
  #........#
  #........#""");

  furnishing(Symmetry.rotate90, """
  #........#
  #........#
  #≈≈≡≈≈≡≈≈#
  #........#
  #........#""");

  furnishing(Symmetry.rotate90, """
  #.........#
  #.........#
  #≈≈≈≈≡≈≈≈≈#
  #.........#
  #.........#""");

  furnishing(Symmetry.rotate90, """
  #.........#
  #.........#
  #≈≈≡≈≈≈≡≈≈#
  #.........#
  #.........#""");

  furnishing(Symmetry.rotate90, """
  #.........#
  #.........#
  #≈≈≈≈≡≈≈≈≈#
  #≈≈≈≈≡≈≈≈≈#
  #.........#
  #.........#""");

  furnishing(Symmetry.rotate90, """
  #.........#
  #.........#
  #≈≈≡≈≈≈≡≈≈#
  #≈≈≡≈≈≈≡≈≈#
  #.........#
  #.........#""");

  // TODO: Fireplaces.
  */
  // }

  public Array2D<Cell> _cells;

  public Furnishing(Array2D<Cell> _cells)
  {
    this._cells = _cells;
  }

  public override bool canPlace(Painter painter, Vec pos)
  {
    for (var y = 0; y < _cells.height; y++)
    {
      for (var x = 0; x < _cells.width; x++)
      {
        var absolute = pos.offset(x, y);

        // Should skip these checks for cells that have no requirement.
        if (!painter.bounds.contains(absolute)) return false;
        if (!painter.ownsTile(absolute)) return false;

        if (!_cells._get(x, y).meetsRequirement(painter.getTile(absolute)))
        {
          return false;
        }
      }
    }

    return true;
  }

  public override void place(Painter painter, Vec pos)
  {
    for (var y = 0; y < _cells.height; y++)
    {
      for (var x = 0; x < _cells.width; x++)
      {
        _cells._get(x, y).apply(painter, pos.offset(x, y));
      }
    }
  }
}

public class Cell
{
  static public Cell uninitialized = new Cell();

  public TileType _apply;
  public Motility _motility;
  public List<TileType> _require = new List<TileType> { };

  public Cell(
      TileType apply = null,
      Motility motility = null,
      TileType require = null,
      List<TileType> requireAny = null)
  {
    _apply = apply;
    _motility = motility;

    if (require != null) _require.Add(require);
    if (requireAny != null) _require.AddRange(requireAny);
  }

  public bool meetsRequirement(TileType tile)
  {
    if (_motility != null && !tile.canEnter(_motility!)) return false;
    if (_require.isNotEmpty<TileType>() && !_require.Contains(tile)) return false;
    return true;
  }

  public void apply(Painter painter, Vec pos)
  {
    if (_apply != null) painter.setTile(pos, _apply!);
  }
}
