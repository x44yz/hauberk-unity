using System.Collections;
using System.Collections.Generic;

/// An [Action] that flows out and maps tiles within a certain distance.
class MappingAction : Action
{
  public int _maxDistance;
  public bool _illuminate;
  int _currentDistance = 0;

  /// The different distances (squared) that contain tiles, in reverse order
  /// for easy removal of the nearest distance.
  List<List<Vec>> m_tilesByDistance = null;
  public List<List<Vec>> _tilesByDistance
  {
    get
    {
      if (m_tilesByDistance == null)
        m_tilesByDistance = _findTiles();
      return m_tilesByDistance;
    }
  }

  public override bool isImmediate => false;

  public MappingAction(int _maxDistance, bool illuminate = false)
  {
    this._maxDistance = _maxDistance;
    this._illuminate = illuminate;
  }

  public override ActionResult onPerform()
  {
    for (var i = 0; i < 2; i++)
    {
      // If we've shown all the tiles, we're done.
      if (_currentDistance >= _tilesByDistance.Count)
      {
        return ActionResult.success;
      }

      foreach (var pos in _tilesByDistance[_currentDistance])
      {
        game.stage.explore(pos, force: true);
        addEvent(EventType.map, pos: pos);

        if (_illuminate)
        {
          game.stage[pos].addEmanation(255);
          game.stage.floorEmanationChanged();
        }

        // Update the neighbors too mainly so that walls get explored.
        foreach (var neighbor in pos.neighbors)
        {
          game.stage.explore(neighbor, force: true);
        }
      }

      _currentDistance++;
    }

    return ActionResult.notDone;
  }

  /// Finds all the tiles that should be detected and organizes them from
  /// farthest to nearest.
  List<List<Vec>> _findTiles()
  {
    var result = new List<List<Vec>>() { new List<Vec>() };
    result[0].Add(actor!.pos);

    var flow = new MappingFlow(game.stage, actor!.pos, _maxDistance);

    foreach (var pos in flow.reachable)
    {
      var distance = flow.costAt(pos)!;
      for (var i = result.Count; i <= distance; i++)
      {
        result.Add(new List<Vec>());
      }

      result[distance.Value].Add(pos);
    }

    for (var i = 0; i < result.Count; i++)
    {
      Rng.rng.shuffle(result[i]);
    }

    return result;
  }
}

/// Flows through any visible or traversable tiles, treating diagonals as a
/// little longer to give a nice round edge to the perimeter.
class MappingFlow : Flow
{
  public int _maxDistance;

  public MappingFlow(Stage stage, Vec start, int _maxDistance)
      : base(stage, start, maxDistance: _maxDistance)
  {
  }

  /// The cost to enter [tile] at [pos] or `null` if the tile cannot be entered.
  public override int? tileCost(int parentCost, Vec pos, Tile tile, bool isDiagonal)
  {
    // Can't enter impassable tiles.
    if (!tile.canEnter(Motility.doorAndFly)) return null;

    // TODO: Assumes cost == distance.
    // Can't reach if it's too far.
    if (parentCost >= _maxDistance * 2) return null;

    return isDiagonal ? 3 : 2;
  }
}

