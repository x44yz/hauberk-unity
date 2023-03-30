using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;

/// A lazy, generic pathfinder.
///
/// It can be used to find the cost from a starting point to a goal, or
/// find the directions to reach the nearest goals meeting some predicate.
///
/// Internally, it lazily runs Dijkstra's algorithm. It only processes outward
/// as far as needed to answer the query. In practice, this means it often does
/// less than 10% of the iterations of a full eager search.
///
/// This abstract base class does not know the cost to enter a tile. Subclasses
/// provide a method that determines that.
///
/// See:
///
/// * http://www.roguebasin.com/index.php?title=The_Incredible_Power_of_Dijkstra_Maps
public abstract class Flow
{
  public const int _unknown = -2;
  public const int _unreachable = -1;

  public Stage stage;
  public Vec _start;

  public Array2D<int> _costs;

  /// The position of the array's top-level corner relative to the stage.
  public Vec _offset;

  /// The cells whose neighbors still remain to be processed.
  public BucketQueue<Vec> _open = new BucketQueue<Vec>();

  /// The list of reachable cells that have been found so far, in order of
  /// increasing distance.
  ///
  /// Coordinates are local to [_costs], not the [Stage].
  public List<Vec> _found = new List<Vec>();

  /// Gets the bounds of the [Flow] in stage coordinates.
  public Rect bounds => Rect.posAndSize(_offset, _costs.size);

  /// Gets the starting position in stage coordinates.
  public Vec start => _start;

  public bool includeDiagonals => true;

  public Flow(Stage stage, Vec _start, int? maxDistance = null)
  {
    this.stage = stage;
    this._start = _start;

    int width;
    int height;

    // TODO: Distinguish between maxDistance and maxCost. Once cost is no longer
    // unit for every step, the two can diverge.
    if (maxDistance == null)
    {
      // Inset by one since we can assume the edges are impassable.
      _offset = new Vec(1, 1);
      width = stage.width - 2;
      height = stage.height - 2;
    }
    else
    {
      var left = Mathf.Max(1, _start.x - maxDistance.Value);
      var top = Mathf.Max(1, _start.y - maxDistance.Value);
      var right = Mathf.Min(stage.width - 1, _start.x + maxDistance.Value + 1);
      var bottom = Mathf.Min(stage.height - 1, _start.y + maxDistance.Value + 1);
      _offset = new Vec(left, top);
      width = right - left;
      height = bottom - top;
    }

    _costs = new Array2D<int>(width, height, _unknown);

    // Seed it with the starting position.
    var start = _start - _offset;
    _open.add(start, 0);
    _costs[start] = 0;
  }

  /// Lazily iterates over all reachable tiles in order of increasing cost.
  public List<Vec> reachable
  {
    get
    {
      List<Vec> rt = new List<Vec>();
      for (var i = 0; ; i++)
      {
        // Lazily find the next reachable tile.
        while (i >= _found.Count)
        {
          // If we run out of tiles to search, stop.
          if (!_processNext())
            return rt;
        }

        rt.Add(_found[i] + _offset);
      }
    }
  }

  /// Returns the nearest position to start that meets [predicate].
  ///
  /// If there are multiple equivalent positions, chooses one randomly. If
  /// there are none, returns null.
  public Vec bestWhere(System.Func<Vec, bool> predicate)
  {
    var results = _findAllBestWhere(predicate);
    if (results.Count == 0) return null;

    return Rng.rng.item(results) + _offset;
  }

  /// Gets the cost from the starting position to [pos], or `null` if there is
  /// no path to it.
  public int? costAt(Vec pos)
  {
    pos -= _offset;
    if (!_costs.bounds.contains(pos)) return null;

    // Lazily search until we reach the tile in question or run out of paths to
    // try.
    while (_costs[pos] == _unknown && _processNext()) { }

    var distance = _costs[pos];
    if (distance == _unknown || distance == _unreachable) return null;
    return distance;
  }

  /// Chooses a random direction from [start] that gets closer to [pos].
  Direction directionTo(Vec pos)
  {
    var directions = _directionsTo(new List<Vec> { pos - _offset });
    if (directions.Count == 0) return Direction.none;
    return Rng.rng.item(directions);
  }

  /// Chooses a random direction from [start] that gets closer to one of the
  /// best positions matching [predicate].
  ///
  /// Returns [Direction.none] if no matching positions were found.
  public Direction directionToBestWhere(System.Func<Vec, bool> predicate)
  {
    var directions = directionsToBestWhere(predicate);
    if (directions.Count == 0) return Direction.none;
    return Rng.rng.item(directions);
  }

  /// Find all directions from [start] that get closer to one of the best
  /// positions matching [predicate].
  ///
  /// Returns an empty list if no matching positions were found.
  List<Direction> directionsToBestWhere(System.Func<Vec, bool> predicate)
  {
    return _directionsTo(_findAllBestWhere(predicate));
  }

  /// Get the lowest-cost positions that meet [predicate].
  ///
  /// Only returns more than one position if there are multiple equal-cost
  /// positions meeting the criteria. Returns an empty list if no valid
  /// positions are found. Returned positions are local to [_costs], not
  /// the [Stage].
  List<Vec> _findAllBestWhere(System.Func<Vec, bool> predicate)
  {
    var goals = new List<Vec>();

    int? lowestCost = null;
    for (var i = 0; ; i++)
    {
      // Lazily find the next open tile.
      while (i >= _found.Count)
      {
        // If we flowed everywhere and didn't find anything, give up.
        if (!_processNext()) return goals;
      }

      var pos = _found[i];
      if (!predicate(pos + _offset)) continue;

      var cost = _costs[pos];

      // Since pos was from _found, it should be reachable.
      Debugger.assert(cost >= 0);

      if (lowestCost == null || cost == lowestCost)
      {
        // Consider all goals at the nearest distance.
        lowestCost = cost;
        goals.Add(pos);
      }
      else
      {
        // We hit a tile that's worse than a valid goal, so we can stop looking.
        break;
      }
    }

    return goals;
  }

  /// Find all directions from [start] that get closer to one of positions in
  /// [goals].
  ///
  /// Returns an empty list if none of the goals can be reached.
  List<Direction> _directionsTo(List<Vec> goals)
  {
    var walked = new List<Vec>();
    var directions = new List<Direction>();

    // Starting at [pos], recursively walk along all paths that proceed towards
    // [start].
    void walkBack(Vec pos)
    {
      if (walked.Contains(pos)) return;
      walked.Add(pos);

      foreach (var dir in Direction.all)
      {
        var here = pos + dir;
        if (!_costs.bounds.contains(here)) continue;

        if (here == _start - _offset)
        {
          // If this step reached the target, mark the direction of the step.
          directions.Add(dir.rotate180);
        }
        else if (_costs[here] >= 0 && _costs[here] < _costs[pos])
        {
          walkBack(here);
        }
      }
    }

    // Trace all paths from the goals back to the target.
    goals.ForEach(walkBack);
    return directions;
  }

  /// Runs one iteration of the search, if there are still tiles left to search.
  ///
  /// Returns `false` if the queue is empty.
  bool _processNext()
  {
    var start = _open.removeNext();
    if (start == null) return false;

    var parentCost = _costs[start];

    // Propagate to neighboring tiles.
    void processNeighbor(Direction dir, bool isDiagonal)
    {
      var here = start + dir;

      if (!_costs.bounds.contains(here)) return;

      // Ignore tiles we've already reached.
      if (_costs[here] != _unknown) return;

      var tile = stage[here + _offset];
      var relative = tileCost(parentCost, here + _offset, tile, isDiagonal);

      if (relative == null)
      {
        _costs[here] = _unreachable;
      }
      else
      {
        var total = parentCost + relative.Value;
        _costs[here] = total;
        _found.Add(here);
        _open.add(here, total);
      }
    }

    processNeighbor(Direction.n, false);
    processNeighbor(Direction.s, false);
    processNeighbor(Direction.e, false);
    processNeighbor(Direction.w, false);
    if (includeDiagonals)
    {
      processNeighbor(Direction.nw, true);
      processNeighbor(Direction.ne, true);
      processNeighbor(Direction.sw, true);
      processNeighbor(Direction.se, true);
    }

    return true;
  }

  /// The cost to enter [tile] at [pos] or `null` if the tile cannot be entered.
  public abstract int? tileCost(int parentCost, Vec pos, Tile tile, bool isDiagonal);
}

/// A basic [Flow] implementation that flows through any tile permitting one of
/// a given [Motility].
public class MotilityFlow : Flow
{
  public Motility _motility;
  public bool _avoidActors;
  public bool _avoidSubstances;
  public int? _maxDistance;

  public MotilityFlow(Stage stage, Vec start, Motility _motility,
      int? maxDistance = null, bool? avoidActors = null, bool? avoidSubstances = null)
      : base(stage, start, maxDistance: maxDistance)
  {
    this._motility = _motility;

    this._maxDistance = maxDistance;
    this._avoidActors = avoidActors ?? true;
    this._avoidSubstances = avoidSubstances ?? false;
  }

  /// The cost to enter [tile] at [pos] or `null` if the tile cannot be entered.
  public override int? tileCost(int parentCost, Vec pos, Tile tile, bool isDiagonal)
  {
    // Can't enter impassable tiles.
    if (!tile.canEnter(_motility)) return null;

    // TODO: Should take resistances and immunity into account.
    if (_avoidSubstances && tile.substance > 0) return null;

    // Can't walk through other actors.
    if (_avoidActors && stage.actorAt(pos) != null) return null;

    // TODO: Assumes cost == distance.
    // Can't reach if it's too far.
    if (_maxDistance != null && parentCost >= _maxDistance!) return null;

    return 1;
  }
}
