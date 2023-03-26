using System.Collections;
using System.Collections.Generic;
using System.Linq;

enum DetectType
{
  exit,
  item,
}

/// An [Action] that marks all tiles containing [Item]s explored.
class DetectAction : Action
{
  public HashSet<DetectType> _types;
  public int? _maxDistance;

  /// The different distances (squared) that contain tiles, in reverse order
  /// for easy removal of the nearest distance.
  public List<List<Vec>> m_tilesByDistance = null;
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

  public DetectAction(IEnumerable<DetectType> types, int? _maxDistance)
  {
    _types = new HashSet<DetectType>();
    foreach (var k in types)
      _types.Add(k);
    this._maxDistance = _maxDistance;
  }

  public override ActionResult onPerform()
  {
    // If we've shown all the tiles, we're done.
    if (_tilesByDistance.Count == 0) return ActionResult.success;

    var tiles = _tilesByDistance.Last();
    _tilesByDistance.RemoveAt(_tilesByDistance.Count - 1);
    foreach (var pos in tiles)
    {
      game.stage.explore(pos, force: true);
      addEvent(EventType.detect, pos: pos);
    }

    return ActionResult.notDone;
  }

  /// Finds all the tiles that should be detected and organizes them from
  /// farthest to nearest.
  List<List<Vec>> _findTiles()
  {
    var distanceMap = new Dictionary<int, List<Vec>>();

    void addTile(Vec pos)
    {
      var distance = (actor!.pos - pos).lengthSquared;
      if (_maxDistance != null && distance > _maxDistance! * _maxDistance!)
      {
        return;
      }

      if (distanceMap.ContainsKey(distance) == false)
      {
        distanceMap[distance] = new List<Vec>();
      }
      distanceMap[distance]!.Add(pos);
    }

    var foundExits = 0;
    if (_types.Contains(DetectType.exit))
    {
      foreach (var pos in game.stage.bounds)
      {
        // Ignore already found ones.
        if (game.stage[pos].isExplored) continue;

        if (game.stage[pos].portal != TilePortals.exit) continue;

        foundExits++;
        addTile(pos);
      }
    }

    var foundItems = 0;
    if (_types.Contains(DetectType.item))
    {
      game.stage.forEachItem((item, pos) =>
      {
        // Ignore items already found.
        if (game.stage[pos].isExplored) return;

        foundItems++;
        addTile(pos);
      });
    }

    if (foundExits > 0)
    {
      if (foundItems > 0)
      {
        log("{1} sense[s] hidden secrets in the dark!", actor);
      }
      else
      {
        log("{1} sense[s] places to escape!", actor);
      }
    }
    else
    {
      if (foundItems > 0)
      {
        log("{1} sense[s] the treasures held in the dark!", actor);
      }
      else
      {
        log("The darkness holds no secrets.");
      }
    }

    var distances = distanceMap.Keys.ToList();
    distances.Sort((a, b) => b.CompareTo(a));

    return distances.Select((distance) => distanceMap[distance]!).ToList();
  }
}
