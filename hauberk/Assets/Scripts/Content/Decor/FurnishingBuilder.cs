using System.Collections;
using System.Collections.Generic;
using System.Linq;

abstract partial class Decor {
  enum Symmetry {
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
      string themes, double? frequency = null, Dictionary<string, Cell>? cells = null) {
    _themes = themes;
    _categoryFrequency = frequency;
    _categoryCells = cells;
  }

  public static Cell applyOpen(TileType type) => new Cell(apply: type, motility: Motility.walk);

  public static Cell apply(TileType type, TileType? over) => new Cell(apply: type, require: over);

  public static Cell require(TileType type) => new Cell(require: type);

  public static void furnishing(
      double? frequency, Symmetry? symmetry, string template) {
    _furnishingFrequency = frequency;
    symmetry ??= Symmetry.none;

    var lines = template.Split("\n").map((line) => line.trim()).toList();
    _singleFurnishing(lines);

    if (symmetry == Symmetry.mirrorHorizontal ||
        symmetry == Symmetry.mirrorBoth) {
      var mirrorLines = lines.toList();
      for (var i = 0; i < lines.length; i++) {
        mirrorLines[i] = _mapString(
            string.fromCharCodes(lines[i].codeUnits.reversed),
            _mirrorCharHorizontal);
      }

      _singleFurnishing(mirrorLines);
    }

    if (symmetry == Symmetry.mirrorVertical || symmetry == Symmetry.mirrorBoth) {
      var mirrorLines = lines.toList();
      for (var i = 0; i < lines.length; i++) {
        mirrorLines[lines.length - i - 1] =
            _mapString(lines[i], _mirrorCharVertical);
      }

      _singleFurnishing(mirrorLines);
    }

    if (symmetry == Symmetry.mirrorBoth ||
        symmetry == Symmetry.rotate180 ||
        symmetry == Symmetry.rotate90) {
      var mirrorLines = lines.toList();
      for (var i = 0; i < lines.length; i++) {
        mirrorLines[lines.length - i - 1] = _mapString(
            string.fromCharCodes(lines[i].codeUnits.reversed), _mirrorCharBoth);
      }

      _singleFurnishing(mirrorLines);
    }

    if (symmetry == Symmetry.rotate90) {
      // Rotate 90°.
      var rotateLines = <string>[];
      for (var x = 0; x < lines[0].length; x++) {
        var buffer = StringBuffer();
        for (var y = 0; y < lines.length; y++) {
          buffer.write(_rotateChar90(lines[y][x]));
        }
        rotateLines.add(buffer.toString());
      }

      _singleFurnishing(rotateLines);

      // Rotate 270° by mirroring the 90°.
      var mirrorLines = rotateLines.toList();
      for (var i = 0; i < rotateLines.length; i++) {
        mirrorLines[rotateLines.length - i - 1] = _mapString(
            string.fromCharCodes(rotateLines[i].codeUnits.reversed),
            _mirrorCharBoth);
      }

      _singleFurnishing(mirrorLines);
    }
  }

  public static string _mapString(string input, string Function(string) map) {
    var buffer = StringBuffer();
    for (var i = 0; i < input.length; i++) {
      buffer.write(map(input[i]));
    }
    return buffer.toString();
  }

  public static string _mirrorCharBoth(string input) =>
      _mirrorCharHorizontal(_mirrorCharVertical(input));

  public static string _mirrorCharHorizontal(string input) {
    for (var mirror in _mirrorHorizontal) {
      var index = mirror.indexOf(input);
      if (index != -1) return mirror[1 - index];
    }

    // Tile doesn't change.
    return input;
  }

  public static string _mirrorCharVertical(string input) {
    for (var mirror in _mirrorVertical) {
      var index = mirror.indexOf(input);
      if (index != -1) return mirror[1 - index];
    }

    // Tile doesn't change.
    return input;
  }

  public static string _rotateChar90(string input) {
    for (var rotate in _rotate) {
      var index = rotate.indexOf(input);
      if (index != -1) return rotate[(index + 1) % 4];
    }

    // Tile doesn't change.
    return input;
  }

  public static void _singleFurnishing(List<string> lines) {
    var cells =
        Array2D<Cell>(lines.first.length, lines.length, Cell.uninitialized);
    for (var y = 0; y < lines.length; y++) {
      for (var x = 0; x < lines.first.length; x++) {
        var char = lines[y][x];
        Cell cell;
        if (_categoryCells != null && _categoryCells!.containsKey(char)) {
          cell = _categoryCells![char]!;
        } else if (_applyCells.containsKey(char)) {
          cell = _applyCells[char]!;
        } else {
          cell = _requireCells[char]!;
        }

        cells.set(x, y, cell);
      }
    }

    var furnishing = Furnishing(cells);
    Decor.all.add(furnishing,
        frequency: _categoryFrequency ?? _furnishingFrequency ?? 1.0,
        tags: _themes);
  }
}