using System.Collections;
using System.Collections.Generic;
using num = System.Double;
using System.Linq;
using Mathf = UnityEngine.Mathf;

public interface ElementActionMixin
{
}

public static class ElementActionMixinEx
{
  public static void hitTile(this ElementActionMixin mixin, Hit hit, Vec pos, num distance, int fuel = 0)
  {
    var a = mixin as Action;

    // Open tiles if the given motility lets us go through them.
    var tile = a.game.stage[pos];
    if (tile.type.canOpen)
    {
      a.addAction(tile.type.onOpen!(pos));
    }

    a.addEvent(EventType.cone, element: hit.element, pos: pos);

    // See if there is an actor there.
    var target = a.game.stage.actorAt(pos);
    if (target != null && target != a.actor)
    {
      // TODO: Modify damage based on range?
      hit.perform(a, a.actor, target, canMiss: false);
    }

    // Hit stuff on the floor too.
    var action = hit.element.floorAction(pos, hit, distance, fuel);
    if (action != null) a.addAction(action);
  }
}

/// Side-effect action when an actor has been hit with an [Elements.fire]
/// attack.
class BurnActorAction : Action, DestroyActionMixin
{
  public override ActionResult onPerform()
  {
    (this as DestroyActionMixin).destroyHeldItems(Elements.fire);

    // Being burned "cures" cold.
    if (actor!.cold.isActive)
    {
      actor!.cold.cancel();
      return succeed("The fire warms {1} back up.", actor);
    }

    return ActionResult.success;
  }
}

/// Side-effect action when an [Elements.fire] area attack sweeps over a tile.
class BurnFloorAction : Action, DestroyActionMixin
{
  public Vec m_pos;
  public int _damage;
  public int _fuel;

  public BurnFloorAction(Vec _pos, int _damage, int _fuel)
  {
    this.m_pos = _pos;
    this._damage = _damage;
    this._fuel = _fuel;
  }

  public override ActionResult onPerform()
  {
    var fuel = _fuel + (this as DestroyActionMixin).destroyFloorItems(m_pos, Elements.fire);

    // Try to set the tile on fire.
    var tile = game.stage[m_pos];
    var ignition = Tiles.ignition(tile.type);
    if (fuel > 0 || ignition > 0 && _damage > Rng.rng.range(ignition))
    {
      fuel += Tiles.fuel(tile.type);
      tile.substance = Rng.rng.range(fuel / 2, fuel);

      // Higher damage instantly burns off some of the fuel, leaving less to
      // burn over time.
      tile.substance -= _damage / 4;
      if (tile.substance <= 0) tile.substance = 1;

      tile.element = Elements.fire;
      game.stage.floorEmanationChanged();
    }

    return ActionResult.success;
  }
}

/// Action created by the [Elements.fire] substance each turn a tile continues
/// to burn.
class BurningFloorAction : Action, DestroyActionMixin
{
  public Vec m_pos;

  public BurningFloorAction(Vec _pos)
  {
    this.m_pos = _pos;
  }

  public override ActionResult onPerform()
  {
    // See if there is an actor there.
    var target = game.stage.actorAt(m_pos);
    if (target != null)
    {
      // TODO: What should the damage be?
      var hit = new Attack(new Noun("fire"), "burns", 10, 0, Elements.fire).createHit();
      hit.perform(this, null, target, canMiss: false);
    }

    // Try to burn items.
    game.stage[m_pos].substance += (this as DestroyActionMixin).destroyFloorItems(m_pos, Elements.fire);
    return ActionResult.success;
  }
}

/// Side-effect action when an [Elements.cold] area attack sweeps over a tile.
class FreezeFloorAction : Action, DestroyActionMixin
{
  public Vec m_pos;

  public FreezeFloorAction(Vec _pos)
  {
    this.m_pos = _pos;
  }

  public override ActionResult onPerform()
  {
    (this as DestroyActionMixin).destroyFloorItems(m_pos, Elements.cold);

    // TODO: Put out fire.

    return ActionResult.success;
  }
}

/// Side-effect action when an [Elements.poison] area attack sweeps over a tile.
class PoisonFloorAction : Action, DestroyActionMixin
{
  public Vec m_pos;
  public int _damage;
  
  public PoisonFloorAction(Vec _pos, int _damage)
  {
    this.m_pos = _pos;
    this._damage = _damage;
  }

  public override ActionResult onPerform()
  {
    var tile = game.stage[m_pos];

    // Fire beats poison.
    if (tile.element == Elements.fire && tile.substance > 0)
    {
      return ActionResult.success;
    }

    // Try to fill the tile with poison gas.
    if (tile.isFlyable)
    {
      tile.element = Elements.poison;
      tile.substance = Mathf.Clamp(tile.substance + _damage * 16, 0, 255);
    }

    return ActionResult.success;
  }
}

/// Action created by the [Elements.poison] substance each turn a tile contains
/// poisonous gas.
class PoisonedFloorAction : Action, DestroyActionMixin
{
  public Vec m_pos;

  public PoisonedFloorAction(Vec _pos)
  {
    this.m_pos = _pos;
  }

  public override ActionResult onPerform()
  {
    // See if there is an actor there.
    var target = game.stage.actorAt(m_pos);
    if (target != null)
    {
      // TODO: What should the damage be?
      var hit =
          new Attack(new Noun("poison"), "chokes", 4, 0, Elements.poison).createHit();
      hit.perform(this, null, target, canMiss: false);
    }

    return ActionResult.success;
  }
}

// TODO: Should interact with substances on tile: spead poison and fire, etc.
class WindAction : Action
{
  /// Not immediate to ensure an actor doesn't get blown into the path of a
  /// yet-to-be-processed tile.
  public override bool isImmediate => false;

  public override ActionResult onPerform()
  {
    // Move the actor to a random reachable tile. Flying actors get blown more.
    var distance = actor!.motility.overlaps(Motility.fly) ? 6 : 3;

    // Don't blow through doors.
    var motility = actor!.motility - Motility.door;
    var flow =
        new MotilityFlow(game.stage, actor!.pos, motility, maxDistance: distance);
    var positions =
        flow.reachable.Where((pos) => game.stage.actorAt(pos) == null).ToList();
    if (positions.Count == 0) return ActionResult.failure;

    log("{1} [are|is] thrown by the wind!", actor);
    addEvent(EventType.wind, actor: actor, pos: actor!.pos);
    actor!.pos = Rng.rng.item(positions);

    return ActionResult.success;
  }
}

/// Permanently illuminates the given tile.
class LightFloorAction : Action
{
  public Vec m_pos;
  int _emanation;

  public LightFloorAction(Vec pos, Hit hit, num distance)
  {
    // The intensity fades from the center outward. Also, strong hits produce
    // more light.
    var min = Mathf.Clamp((float)(1.0 + (int)hit.averageDamage * 4.0), 0.0f, Lighting.max);
    var max = Mathf.Clamp((float)(128.0 + hit.averageDamage * 16.0), 0.0f, Lighting.max);

    this.m_pos = pos;
    this._emanation = (int)MathUtils.lerpDouble((float)(hit.range - distance), 0.0f, hit.range, min, max);
  }

  public LightFloorAction(Vec _pos, int _emanation)
  {
    this.m_pos = _pos;
    this._emanation = _emanation;
  }

  public override ActionResult onPerform()
  {
    game.stage[m_pos].addEmanation(_emanation);
    game.stage.floorEmanationChanged();

    return ActionResult.success;
  }
}
