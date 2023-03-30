using System;
using System.Collections.Generic;
using System.Linq;


/// An incrementally-updated breadth-first search distance map.
///
/// Works basically like [Flow] except that when a tile is turned solid, only
/// the tiles that need to be updated are recalculated. This is much faster
/// when filling in passages during level generation.
class Reachability
{
  public const int _unknown = -2;
  public const int _unreachable = -1;

  public Stage stage;
  public Vec _start;

  public Array2D<int> _distances;
  public VecSet _affected;

  /// The number of unfillable tiles that are currently reachable.
  int _reachedOpenCount = 0;
  public int reachedOpenCount => _reachedOpenCount;

  List<_FillStep> _beforeFill = new List<_FillStep>();

  public Reachability(Stage stage, Vec _start)
  {
    this.stage = stage;
    this._start = _start;

    _distances = new Array2D<int>(stage.width, stage.height, _unknown);
    _affected = new VecSet(stage.width, stage.height);

    _setDistance(_start, 0);
    _process(new List<Vec>() { _start });
  }

  public bool isReachable(Vec pos) => _distances[pos] >= 0;

  int distanceAt(Vec pos) => _distances[pos];

  /// Mark the tile at [pos] as being solid and recalculate the distances of
  /// any affected tiles.
  public void fill(Vec pos)
  {
    var queue = new Queue<Vec>();
    _affected.clear();
    queue.Enqueue(pos);
    _affected.add(pos);

    _beforeFill = new List<_FillStep>() { new _FillStep(pos, _distances[pos]) };

    while (queue.Count > 0)
    {
      var pos2 = queue.Dequeue();
      var distance = _distances[pos2];
      foreach (var neighbor in pos2.cardinalNeighbors)
      {
        var neighborDistance = _distances[neighbor];
        if (neighborDistance == _unreachable) continue;

        // Ignore tiles that weren't reached from the parent tile.
        if (_distances[neighbor] != distance + 1) continue;

        // Don't get stuck in cycles.
        if (_affected.contains(neighbor)) continue;

        // Ignore tiles that we can get to from another path.
        if (_hasOtherPath(neighbor)) continue;

        queue.Enqueue(neighbor);
        _affected.add(neighbor);

        _beforeFill.Add(new _FillStep(neighbor, neighborDistance));
      }
    }

    // The starting tile is now blocked.
    _setDistance(pos, _unreachable);

    var border = _findBorder(pos);
    if (border.Count == 0)
    {
      // There are no other border tiles that are reachable, so the whole
      // affected area has been cut off.
      foreach (var pos2 in _affected)
      {
        _setDistance(pos2, _unreachable);
      }
    }
    else
    {
      // Clear the distances for the affected tiles.
      foreach (var here in _affected)
      {
        _setDistance(here, _unknown);
      }

      _setDistance(pos, _unreachable);

      // Recalculate the affected tiles.
      _process(border.ToList());
    }
  }

  /// Revert the previous call to [fill].
  public void undoFill()
  {
    foreach (var step in _beforeFill)
    {
      _setDistance(step.pos, step.distance);
    }

    _beforeFill = new List<_FillStep>();
  }

  // Returns true if there is a path to [pos] that doesn't go through an
  // affected tile.
  bool _hasOtherPath(Vec pos)
  {
    var distance = _distances[pos];
    foreach (var neighbor in pos.cardinalNeighbors)
    {
      if (!stage.bounds.contains(neighbor)) continue;

      // If there is an unaffected neighbor whose distance is one step shorter
      // that this one, we can go through that neighbor to get here.
      if (!_affected.contains(neighbor) &&
          _distances[neighbor] == distance - 1)
      {
        return true;
      }
    }

    return false;
  }

  /// Find all of the tiles around the affected tiles that do have a distance.
  /// We'll recalculate the affected tiles using paths from those.
  HashSet<Vec> _findBorder(Vec start)
  {
    var border = new HashSet<Vec> { };
    foreach (var here in _affected)
    {
      // Don't consider the initial filled tile.
      // TODO: This is kind of hokey. Would be better to eliminate pos from
      // affected set.
      if (here == start) continue;

      foreach (var neighbor in here.cardinalNeighbors)
      {
        if (_distances[neighbor] >= 0 && !_affected.contains(neighbor))
        {
          border.Add(neighbor);
        }
      }
    }

    return border;
  }

  /// Update the distances of all unknown tiles reachable from [starting].
  void _process(List<Vec> starting)
  {
    var frontier = new BucketQueue<Vec>();

    foreach (var pos in starting)
    {
      frontier.add(pos, _distances[pos]);
    }

    while (true)
    {
      var pos = frontier.removeNext();
      if (pos == null) break;

      var parentDistance = _distances[pos];

      // Propagate to neighboring tiles.
      foreach (var here in pos.cardinalNeighbors)
      {
        if (!_distances.bounds.contains(here)) continue;

        // Ignore tiles we've already reached.
        if (_distances[here] != _unknown) continue;

        if (stage[here].isWalkable)
        {
          var distance = parentDistance + 1;
          _setDistance(here, distance);
          frontier.add(here, distance);
        }
        else
        {
          _setDistance(here, _unreachable);
        }
      }
    }
  }

  void _setDistance(Vec pos, int distance)
  {
    // If we're on an open tile, update the running count.
    if (stage[pos].type == Tiles.open)
    {
      if (_distances[pos] >= 0) _reachedOpenCount--;
      if (distance >= 0) _reachedOpenCount++;
    }

    _distances[pos] = distance;
  }
}

/// An atomic change to the distance map, so that it can be undone.
class _FillStep
{
  public Vec pos;
  public int distance;

  public _FillStep(Vec pos, int distance)
  {
    this.pos = pos;
    this.distance = distance;
  }
}
