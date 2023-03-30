using System.Collections;
using System.Collections.Generic;

using System.Linq;

/// The game's live play area.
public class Stage
{
  public Game game;

  public List<Actor> _actors = new List<Actor>();
  public Lighting _lighting;
  public Sound _sound;

  int _currentActorIndex = 0;

  public int width => tiles.width;

  public int height => tiles.height;

  public Rect bounds => tiles.bounds;

  public List<Actor> actors => _actors;

  public Actor currentActor => _actors[_currentActorIndex];

  public Array2D<Tile> tiles;

  public Dictionary<Vec, Inventory> _itemsByTile = new Dictionary<Vec, Inventory>();

  /// A spatial partition to let us quickly locate an actor by tile.
  ///
  /// This is a performance bottleneck since pathfinding needs to ensure it
  /// doesn't step on other actors.
  public Array2D<Actor> _actorsByTile;

  public Stage(int width, int height, Game game)
  {
    this.game = game;

    tiles = Array2D<Tile>.generated(width, height, (_) => new Tile());
    _actorsByTile = new Array2D<Actor>(width, height, null);

    _lighting = new Lighting(this);
    _sound = new Sound(this);
  }

  public Tile this[Vec pos] => tiles[pos];

  /// Iterates over every item on the ground on the stage.
  IEnumerator<Item> allItems
  {
    get
    {
      var rt = new List<Item>();
      foreach (var inventory in _itemsByTile.Values)
      {
        rt.AddRange(inventory._items);
      }
      return rt.GetEnumerator();
    }
  }

  public Tile get(int x, int y) => tiles._get(x, y);

  public void set(int x, int y, Tile tile) => tiles._set(x, y, tile);

  public void addActor(Actor actor)
  {
    Debugger.assert(_actorsByTile[actor.pos] == null);

    _actors.Add(actor);
    _actorsByTile[actor.pos] = actor;
  }

  /// Called when an [Actor]'s position has changed so the stage can track it.
  public void moveActor(Vec from, Vec to)
  {
    var actor = _actorsByTile[from];
    _actorsByTile[from] = null;
    _actorsByTile[to] = actor;
  }

  public void removeActor(Actor actor)
  {
    Debugger.assert(_actorsByTile[actor.pos] == actor);

    var index = _actors.IndexOf(actor);
    Debugger.assert(index >= 0 && index < _actors.Count);

    if (_currentActorIndex > index) _currentActorIndex--;
    _actors.RemoveAt(index);

    if (_currentActorIndex >= _actors.Count) _currentActorIndex = 0;
    _actorsByTile[actor.pos] = null;
  }

  public void advanceActor()
  {
    _currentActorIndex = (_currentActorIndex + 1) % _actors.Count;
  }

  public Actor actorAt(Vec pos) =>
   _actorsByTile[pos];

  public List<Item> placeDrops(Vec pos, Motility motility, Drop drop)
  {
    var items = new List<Item>();

    // Try to keep dropped items from overlapping.
    var flow = new MotilityFlow(this, pos, motility, avoidActors: false);

    drop.dropItem(game.depth, (item) =>
    {
      items.Add(item);

      // Prefer to not place under monsters or stack with other items.
      var itemPos = flow.bestWhere((pos) =>
      {
        // Some chance to place on occupied tiles.
        if (Rng.rng.oneIn(5)) return true;

        return actorAt(pos) == null && !isItemAt(pos);
      });

      // If that doesn't work, pick any nearby tile.
      if (itemPos == null)
      {
        var allowed = flow.reachable.Take(10).ToList();
        if (allowed.Count > 0)
        {
          itemPos = Rng.rng.item(allowed);
        }
        else
        {
          // Nowhere to place it.
          // TODO: If the starting position doesn't allow the motility (as in
          // when opening a barrel), this does the wrong thing. What should we
          // do then?
          itemPos = pos;
        }
      }

      addItem(item, itemPos!);
    });

    return items;
  }

  public void addItem(Item item, Vec pos)
  {
    // Get the inventory for the tile.
    Inventory inventory = null;
    if (_itemsByTile.TryGetValue(pos, out inventory) == false)
    {
      inventory = new Inventory(ItemLocation.onGround);
      _itemsByTile[pos] = inventory;
    }
    var result = inventory.tryAdd(item);
    // Inventory is unlimited, so should always succeed.
    Debugger.assert(result.remaining == 0);

    // If a light source is dropped, we need to light the floor.
    if (item.emanationLevel > 0) floorEmanationChanged();
  }

  /// Returns `true` if there is at least one item at [pos].
  public bool isItemAt(Vec pos) => _itemsByTile.ContainsKey(pos);

  /// Gets the [Item]s at [pos].
  public Inventory itemsAt(Vec pos)
  {
    if (_itemsByTile.ContainsKey(pos))
      return _itemsByTile[pos];
    return new Inventory(ItemLocation.onGround);
  }
  // TODO: This is kind of slow, probably from creating the inventory each time.
  // Use a const one for the empty case?

  /// Removes [item] from the stage at [pos].
  ///
  /// It is an error to call this if [item] is not on the ground at [pos].
  public void removeItem(Item item, Vec pos)
  {
    var inventory = _itemsByTile[pos]!;
    inventory.remove(item);

    // If a light source is picked up, we need to unlight the floor.
    if (item.emanationLevel > 0) floorEmanationChanged();

    // Discard empty inventories. Note that [isItemAt] assumes this is done.
    if (inventory.isEmpty) _itemsByTile.Remove(pos);
  }

  /// Iterates over every item on the stage and returns the item and its
  /// position.
  public void forEachItem(System.Action<Item, Vec> callback)
  {
    foreach (var kv in _itemsByTile)
    {
      var pos = kv.Key;
      var inventory = kv.Value;
      foreach (var item in inventory)
      {
        callback(item, pos);
      }
    }
  }

  /// Marks the illumination and field-of-view as needing recalculation.
  public void tileOpacityChanged()
  {
    _lighting.dirtyFloorLight();
    _lighting.dirtyActorLight();
    _lighting.dirtyVisibility();
    _sound.dirty();
  }

  /// Marks the floor illumination as needing recalculation.
  ///
  /// This should be called when a tile's emanation changes, or a
  /// light-emitting item is dropped or picked up.
  public void floorEmanationChanged()
  {
    _lighting.dirtyFloorLight();
  }

  /// Marks the actor illumination as needed recalculation.
  ///
  /// This should be called whenever an actor that emanates light moves or
  /// when its emanation changes (for example, the [Hero] equipping a light
  /// source).
  public void actorEmanationChanged()
  {
    _lighting.dirtyActorLight();
  }

  /// Marks the visibility as needing recalculation.
  ///
  /// This should be called whenever the [Hero] moves or their sight changes.
  public void heroVisibilityChanged()
  {
    _lighting.dirtyVisibility();
  }

  /// Marks the tile at [x],[y] as explored if the hero can see it and hasn't
  /// previously explored it.
  public void exploreAt(int x, int y, bool force = false)
  {
    var tile = tiles._get(x, y);
    if (tile.updateExplored(force: force))
    {
      if (tile.isVisible)
      {
        var actor = actorAt(new Vec(x, y));
        if (actor != null && actor is Monster)
        {
          game.hero.seeMonster(actor as Monster);
        }
      }
    }
  }

  public void explore(Vec pos, bool force = false)
  {
    exploreAt(pos.x, pos.y, force: force);
  }

  public void setVisibility(Vec pos, bool isOccluded, int fallOff)
  {
    var tile = tiles[pos];
    tile.updateVisibility(isOccluded, fallOff);
    if (tile.isVisible)
    {
      var actor = actorAt(pos);
      if (actor != null && actor is Monster)
      {
        game.hero.seeMonster(actor as Monster);
      }
    }
  }

  /// Recalculates any lighting or visibility state that needs it.
  public void refreshView()
  {
    _lighting.refresh();
  }

  // TODO: This is hackish and may fail to terminate.
  // TODO: Consider flyable tiles for flying monsters.
  /// Selects a random passable tile that does not have an [Actor] on it.
  public Vec findOpenTile()
  {
    while (true)
    {
      var pos = Rng.rng.vecInRect(bounds);

      if (!this[pos].isWalkable) continue;
      if (actorAt(pos) != null) continue;

      return pos;
    }
  }

  /// How loud the hero is from [pos] in terms of sound flow, up to
  /// [Sound.maxDistance].
  public double heroVolume(Vec pos) => _sound.heroVolume(pos);

  public double volumeBetween(Vec from, Vec to) => _sound.volumeBetween(from, to);
}
