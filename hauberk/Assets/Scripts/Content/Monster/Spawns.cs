using System.Collections;
using System.Collections.Generic;
using AddMonster = System.Action<Breed>;

public static class SpawnUtils
{
  public static Spawn spawnBreed(string name) => new _BreedSpawn(new BreedRef(name));

  public static Spawn spawnTag(string tag) => new _TagSpawn(tag);

  public static Spawn repeatSpawn(int min, int max, Spawn spawn) =>
      new _RepeatSpawn(min, max, spawn);

  public static Spawn spawnAll(List<Spawn> spawns) => new _AllOfSpawn(spawns);
}

/// Spawns a monster of a given breed.
class _BreedSpawn : Spawn
{
  public BreedRef _breed;

  public _BreedSpawn(BreedRef _breed)
  {
    this._breed = _breed;
  }

  public override void spawnBreed(int depth, AddMonster addMonster)
  {
    addMonster(_breed.breed);
  }
}

/// Drops a randomly selected breed with a given tag.
class _TagSpawn : Spawn
{
  /// The tag to choose from.
  public string _tag;

  public _TagSpawn(string _tag)
  {
    this._tag = _tag;
  }

  // TODO: Should the spawn be able to override or modify the depth?
  public override void spawnBreed(int depth, AddMonster addMonster)
  {
    for (var tries = 0; tries < 10; tries++)
    {
      var breed =
          Monsters.breeds.tryChoose(depth, tag: _tag, includeParents: false);
      if (breed == null) continue;
      if (breed.flags.unique) continue;

      addMonster(breed);
      break;
    }
  }
}

/// A [Spawn] that repeats a spawn more than once.
class _RepeatSpawn : Spawn
{
  public int _minCount;
  public int _maxCount;
  public Spawn _spawn;

  public _RepeatSpawn(int _minCount, int _maxCount, Spawn _spawn)
  {
    this._minCount = _minCount;
    this._maxCount = _maxCount;
    this._spawn = _spawn;
  }

  public override void spawnBreed(int depth, AddMonster addMonster)
  {
    var taper = 5;
    if (_maxCount > 3) taper = 4;
    if (_maxCount > 6) taper = 3;

    var count = Rng.rng.inclusive(_minCount, _maxCount) + Rng.rng.taper(0, taper);
    for (var i = 0; i < count; i++)
    {
      _spawn.spawnBreed(depth, addMonster);
    }
  }
}

/// A [Spawn] that spawns all of a list of child spawns.
class _AllOfSpawn : Spawn
{
  public List<Spawn> _spawns;

  public _AllOfSpawn(List<Spawn> _spawns)
  {
    this._spawns = _spawns;
  }

  public override void spawnBreed(int depth, AddMonster addMonster)
  {
    foreach (var spawn in _spawns)
    {
      spawn.spawnBreed(depth, addMonster);
    }
  }
}