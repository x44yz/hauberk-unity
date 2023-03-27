using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mathf = UnityEngine.Mathf;

/// Places a single blob filled with monsters.
class Pit : Architecture
{
  public string _monsterGroup;

  /// The minimum chamber size.
  public int _minSize;

  /// The maximum chamber size.
  public int _maxSize;

  public List<Vec> _monsterTiles = new List<Vec>();

  public override PaintStyle paintStyle => PaintStyle.stoneJail;

  public Pit(string _monsterGroup, int? minSize = null, int? maxSize = null)
  {
    this._monsterGroup = _monsterGroup;

    _minSize = minSize ?? 12;
    _maxSize = maxSize ?? 24;
  }

  public override IEnumerator build()
  {
    for (var i = 0; i < 20; i++)
    {
      var size = Rng.rng.range(_minSize, _maxSize);
      var cave = Blob.make(size);

      var bounds = _tryPlaceCave(cave, this.bounds);
      if (bounds != null)
      {
        yield return "pit";

        foreach (var pos in cave.bounds)
        {
          if (cave[pos])
          {
            _monsterTiles.Add(pos + bounds.topLeft);
          }
        }

        yield return Main.Inst.StartCoroutine(_placeAntechambers(bounds));
        yield break;
      }
    }
  }

  public override bool spawnMonsters(Painter painter)
  {
    // Boost the depth some.
    var depth = Mathf.CeilToInt((float)(painter.depth * Rng.rng.rfloat(1.0, 1.4)));

    foreach (var pos in _monsterTiles)
    {
      if (!painter.getTile(pos).isWalkable) continue;

      // Leave a ring of open tiles around the edge of the pit.
      var openNeighbors = true;
      foreach (var neighbor in pos.neighbors)
      {
        if (!painter.getTile(neighbor).isWalkable)
        {
          openNeighbors = false;
          break;
        }
      }
      if (!openNeighbors) continue;

      if (painter.hasActor(pos)) continue;

      var breed = painter.chooseBreed(depth,
          tag: _monsterGroup, includeParentTags: false);
      painter.spawnMonster(pos, breed);
    }

    return true;
  }

  Rect _tryPlaceCave(Array2D<bool> cave, Rect bounds)
  {
    if (bounds.width < cave.width) return null;
    if (bounds.height < cave.height) return null;

    for (var j = 0; j < 200; j++)
    {
      var x = Rng.rng.range(bounds.left, bounds.right - cave.width);
      var y = Rng.rng.range(bounds.top, bounds.bottom - cave.height);

      if (_tryPlaceCaveAt(cave, x, y))
      {
        return new Rect(x, y, cave.width, cave.height);
      }
    }

    return null;
  }

  // TODO: Copied from Cavern.
  bool _tryPlaceCaveAt(Array2D<bool> cave, int x, int y)
  {
    foreach (var pos in cave.bounds)
    {
      if (cave[pos])
      {
        if (!canCarve(pos.offset(x, y))) return false;
      }
    }

    foreach (var pos in cave.bounds)
    {
      if (cave[pos]) carve(pos.x + x, pos.y + y);
    }

    return true;
  }

  /// Try to place a few small caves around the main pit. This gives some
  /// foreshadowing that the hero is about to enter a pit.
  IEnumerator _placeAntechambers(Rect pitBounds)
  {
    for (var i = 0; i < 8; i++)
    {
      var size = Rng.rng.range(6, 10);
      var cave = Blob.make(size);

      var allowed = Rect.leftTopRightBottom(
          pitBounds.left - cave.width,
          pitBounds.top - cave.height,
          pitBounds.right + cave.width,
          pitBounds.bottom + cave.height);
      allowed = Rect.intersect(allowed, bounds.inflate(-1));

      if (_tryPlaceCave(cave, allowed) != null) yield return "antechamber";
    }
  }
}
