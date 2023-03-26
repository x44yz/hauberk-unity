using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public abstract partial class Decor
{
  public enum Symmetry
  {
    none,
    mirrorHorizontal,
    mirrorVertical,
    mirrorBoth,
    rotate90,
    rotate180,
  }

  public static double? _categoryFrequency;
  public static double? _furnishingFrequency;
  public static string _themes;
  public static Dictionary<string, Cell> _categoryCells;

  public static Dictionary<string, Cell> _applyCells = new Dictionary<string, Cell>(){
    {"I", new Cell(
        apply: Tiles.wallTorch,
        requireAny: new List<TileType>{Tiles.flagstoneWall, Tiles.graniteWall})},
    {"l", new Cell(apply: Tiles.wallTorch, motility: Motility.walk)},
    {"P", new Cell(apply: Tiles.statue, motility: Motility.walk)},
    {"≈", new Cell(apply: Tiles.water, motility: Motility.walk)},
    {"%", new Cell(apply: Tiles.closedBarrel, motility: Motility.walk)},
    {"&", new Cell(apply: Tiles.closedChest, motility: Motility.walk)},
    {"*", new Cell(apply: Tiles.tallGrass, require: Tiles.grass)},
    {"=", new Cell(apply: Tiles.bridge, require: Tiles.water)},
    {"≡", new Cell(apply: Tiles.bridge, motility: Motility.walk)},
    {"•", new Cell(apply: Tiles.steppingStone, require: Tiles.water)}
  };

  public static Dictionary<string, Cell> _requireCells = new Dictionary<string, Cell>(){
    {"?", new Cell()},
    {".", new Cell(motility: Motility.walk)},
    {"#", new Cell(requireAny: new List<TileType>{
                              Tiles.flagstoneWall,
                              Tiles.graniteWall,
                              Tiles.granite1,
                              Tiles.granite2,
                              Tiles.granite3
                            })},
    {"┌", new Cell(require: Tiles.tableTopLeft)},
    {"─", new Cell(require: Tiles.tableTop)},
    {"┐", new Cell(require: Tiles.tableTopRight)},
    {"-", new Cell(require: Tiles.tableCenter)},
    {"│", new Cell(require: Tiles.tableSide)},
    {"╘", new Cell(require: Tiles.tableBottomLeft)},
    {"═", new Cell(require: Tiles.tableBottom)},
    {"╛", new Cell(require: Tiles.tableBottomRight)},
    {"╞", new Cell(require: Tiles.tableLegLeft)},
    {"╤", new Cell(require: Tiles.tableLeg)},
    {"╡", new Cell(require: Tiles.tableLegRight)},
    {"π", new Cell(require: Tiles.chair)},
    {"≈", new Cell(require: Tiles.water)},
    {"'", new Cell(requireAny: new List<TileType>{Tiles.grass, Tiles.tallGrass})},
  };

  public static List<string> _mirrorHorizontal = new List<string>(){
    "┌┐",
    "╛╘",
    "╞╡",
  };

  public static List<string> _mirrorVertical = new List<string>(){
    "┌╘",
    "┐╛",
    "─═",
  };

  public static List<string> _rotate = new List<string>(){
    "┌┐╛╘",
    "─│═│",
  };

  public static void category(
      string themes, double? frequency = null, Dictionary<string, Cell> cells = null)
  {
    _themes = themes;
    _categoryFrequency = frequency;
    _categoryCells = cells;
  }

  public static Cell applyOpen(TileType type) => new Cell(apply: type, motility: Motility.walk);

  public static Cell apply(TileType type, TileType over) => new Cell(apply: type, require: over);

  public static Cell require(TileType type) => new Cell(require: type);

  public static void furnishing(
      double? frequency = null, Symmetry? symmetry = null, string template = null)
  {
    _furnishingFrequency = frequency;
    symmetry ??= Symmetry.none;

    var lines = template.Trim().Split('\n').Select((line) => line.Trim()).ToList();
    _singleFurnishing(lines);

    if (symmetry == Symmetry.mirrorHorizontal ||
        symmetry == Symmetry.mirrorBoth)
    {
      var mirrorLines = lines.ToList();
      for (var i = 0; i < lines.Count; i++)
      {
        mirrorLines[i] = _mapString(
            new string(lines[i].Reverse().ToArray()),
            _mirrorCharHorizontal);
      }

      _singleFurnishing(mirrorLines);
    }

    if (symmetry == Symmetry.mirrorVertical || symmetry == Symmetry.mirrorBoth)
    {
      var mirrorLines = lines.ToList();
      for (var i = 0; i < lines.Count; i++)
      {
        mirrorLines[lines.Count - i - 1] =
            _mapString(lines[i], _mirrorCharVertical);
      }

      _singleFurnishing(mirrorLines);
    }

    if (symmetry == Symmetry.mirrorBoth ||
        symmetry == Symmetry.rotate180 ||
        symmetry == Symmetry.rotate90)
    {
      var mirrorLines = lines.ToList();
      for (var i = 0; i < lines.Count; i++)
      {
        mirrorLines[lines.Count - i - 1] = _mapString(
            new string(lines[i].Reverse().ToArray()), _mirrorCharBoth);
      }

      _singleFurnishing(mirrorLines);
    }

    if (symmetry == Symmetry.rotate90)
    {
      // Rotate 90°.
      var rotateLines = new List<string> { };
      for (var x = 0; x < lines[0].Length; x++)
      {
        var buffer = new StringBuilder();
        for (var y = 0; y < lines.Count; y++)
        {
          buffer.Append(_rotateChar90(lines[y][x].ToString()));
        }
        rotateLines.Add(buffer.ToString());
      }

      _singleFurnishing(rotateLines);

      // Rotate 270° by mirroring the 90°.
      var mirrorLines = rotateLines.ToList();
      for (var i = 0; i < rotateLines.Count; i++)
      {
        mirrorLines[rotateLines.Count - i - 1] = _mapString(
            new string(rotateLines[i].Reverse().ToArray()),
            _mirrorCharBoth);
      }

      _singleFurnishing(mirrorLines);
    }
  }

  public static string _mapString(string input, System.Func<string, string> map)
  {
    var buffer = new StringBuilder();
    for (var i = 0; i < input.Length; i++)
    {
      buffer.Append(map(input[i].ToString()));
    }
    return buffer.ToString();
  }

  public static string _mirrorCharBoth(string input) =>
      _mirrorCharHorizontal(_mirrorCharVertical(input));

  public static string _mirrorCharHorizontal(string input)
  {
    foreach (var mirror in _mirrorHorizontal)
    {
      var index = mirror.IndexOf(input);
      if (index != -1) return mirror[1 - index].ToString();
    }

    // Tile doesn't change.
    return input;
  }

  public static string _mirrorCharVertical(string input)
  {
    foreach (var mirror in _mirrorVertical)
    {
      var index = mirror.IndexOf(input);
      if (index != -1) return mirror[1 - index].ToString();
    }

    // Tile doesn't change.
    return input;
  }

  public static string _rotateChar90(string input)
  {
    foreach (var rotate in _rotate)
    {
      var index = rotate.IndexOf(input);
      if (index != -1) return rotate[(index + 1) % 4].ToString();
    }

    // Tile doesn't change.
    return input;
  }

  public static void _singleFurnishing(List<string> lines)
  {
    var cells =
        new Array2D<Cell>(lines.First().Length, lines.Count, Cell.uninitialized);
    for (var y = 0; y < lines.Count; y++)
    {
      for (var x = 0; x < lines.First().Length; x++)
      {
        var _char = lines[y][x].ToString();
        Cell cell;
        if (_categoryCells != null && _categoryCells!.ContainsKey(_char))
        {
          cell = _categoryCells![_char]!;
        }
        else if (_applyCells.ContainsKey(_char))
        {
          cell = _applyCells[_char]!;
        }
        else
        {
          cell = _requireCells[_char]!;
        }

        cells._set(x, y, cell);
      }
    }

    var furnishing = new Furnishing(cells);
    Decor.all.add(furnishing,
        frequency: _categoryFrequency ?? _furnishingFrequency ?? 1.0,
        tags: _themes);
  }
}