using System;
using System.Collections.Generic;
using num = System.Double;

/// Spawns a new [Monster] of the same [Breed] adjacent to this one.
class SpawnMove : Move
{
  public bool _preferStraight;

  public SpawnMove(num rate, bool? preferStraight = null) : base(rate)
  {
    _preferStraight = preferStraight ?? false;
  }

  public override num experience => 6.0;

  public override bool shouldUse(Monster monster)
  {
    // Don't breed offscreen since it can end up filling the room before the
    // hero gets there.
    if (!monster.isVisibleToHero) return false;

    // Look for an open adjacent tile.
    foreach (var neighbor in monster.pos.neighbors)
    {
      if (monster.willEnter(neighbor)) return true;
    }

    return false;
  }

  public override Action onGetAction(Monster monster)
  {
    // Pick an open adjacent tile.
    var dirs = new List<Direction> { };

    // If we want to spawn in straight-ish lines, bias the directions towards
    // ones that continue existing lines.
    if (_preferStraight)
    {
      foreach (var dir in Direction.all)
      {
        if (!monster.willEnter(monster.pos + dir)) continue;

        bool checkNeighbor(Direction neighbor)
        {
          var other = monster.game.stage.actorAt(monster.pos + dir);
          return other != null &&
              other is Monster &&
              (other as Monster).breed == monster.breed;
        }

        if (checkNeighbor(dir.rotate180))
        {
          dirs.AddRange(new Direction[] { dir, dir, dir, dir, dir });
        }

        if (checkNeighbor(dir.rotate180.rotateLeft45))
        {
          dirs.Add(dir);
        }

        if (checkNeighbor(dir.rotate180.rotateRight45))
        {
          dirs.Add(dir);
        }
      }
    }

    if (dirs.Count == 0)
    {
      foreach (var dir in Direction.all)
      {
        if (!monster.willEnter(monster.pos + dir)) continue;
        dirs.Add(dir);
      }
    }

    return new SpawnAction(monster.pos + Rng.rng.item(dirs), monster.breed);
  }

  public override string ToString() => $"Spawn rate: {rate}";
}
