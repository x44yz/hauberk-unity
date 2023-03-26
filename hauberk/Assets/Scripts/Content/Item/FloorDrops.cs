using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// Items that are spawned on the ground when a dungeon is first generated.
class FloorDrops
{
  public static ResourceSet<FloorDrop> _floorDrops = new ResourceSet<FloorDrop>();

  public static void floorDrop(
      double? startFrequency = null,
      double? endFrequency = null,
      SpawnLocation? location = null,
      Drop drop = null)
  {
    location ??= SpawnLocation.anywhere;
    var floorDrop = new FloorDrop(location.Value, drop);
    _floorDrops.addRanged(floorDrop,
        start: 1,
        end: 100,
        startFrequency: startFrequency,
        endFrequency: endFrequency);
  }

  public static void initialize()
  {
    // Add generic stuff at every depth.

    // TODO: Tune this.
    floorDrop(
        startFrequency: 2.0,
        location: SpawnLocation.wall,
        drop: DropUtils.dropAllOf(new List<Drop>{
          DropUtils.percentDrop(30, "Skull"),
          DropUtils.percentDrop(30, "treasure"),
          DropUtils.percentDrop(20, "weapon"),
          DropUtils.percentDrop(20, "armor"),
          DropUtils.percentDrop(20, "food"),
          DropUtils.percentDrop(15, "magic"),
          DropUtils.percentDrop(15, "magic")
  }));

    floorDrop(
        startFrequency: 5.0,
        location: SpawnLocation.wall,
        drop: DropUtils.parseDrop("magic"));

    floorDrop(startFrequency: 10.0, endFrequency: 1.0, drop: DropUtils.parseDrop("food"));

    floorDrop(
        startFrequency: 3.0,
        endFrequency: 0.01,
        location: SpawnLocation.corner,
        drop: DropUtils.parseDrop("Rock"));

    floorDrop(startFrequency: 10.0, drop: DropUtils.parseDrop("treasure"));

    floorDrop(startFrequency: 4.0, endFrequency: 0.1, drop: DropUtils.parseDrop("light"));

    floorDrop(
        startFrequency: 2.0,
        endFrequency: 5.0,
        location: SpawnLocation.anywhere,
        drop: DropUtils.parseDrop("item"));

    // TODO: Other stuff.
  }

  public static FloorDrop choose(int depth) => _floorDrops.tryChoose(depth)!;
}

class FloorDrop
{
  public SpawnLocation location;
  public Drop drop;

  public FloorDrop(SpawnLocation location, Drop drop)
  {
    this.location = location;
    this.drop = drop;
  }
}
