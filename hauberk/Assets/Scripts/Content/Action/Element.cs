using System.Collections;
using System.Collections.Generic;
using num = System.Double;
using System.Linq;
using Mathf  = UnityEngine.Mathf;

public class ElementActionMixin : ActionMixin {
   public ElementActionMixin(Action action) : base(action)
  {
  }

  public void hitTile(Hit hit, Vec pos, num distance, int fuel = 0) {
    // Open tiles if the given motility lets us go through them.
    var tile = game.stage[pos];
    if (tile.type.canOpen) {
      addAction(tile.type.onOpen!(pos));
    }

    addEvent(EventType.cone, element: hit.element, pos: pos);

    // See if there is an actor there.
    var target = game.stage.actorAt(pos);
    if (target != null && target != actor) {
      // TODO: Modify damage based on range?
      hit.perform(this.action, actor, target, canMiss: false);
    }

    // Hit stuff on the floor too.
    var action = hit.element.floorAction(pos, hit, distance, fuel);
    if (action != null) addAction(action);
  }
}

/// Side-effect action when an actor has been hit with an [Elements.fire]
/// attack.
class BurnActorAction : Action {
    public DestroyActionMixin _destroyMixin;

    public BurnActorAction()
    {
        _destroyMixin = new DestroyActionMixin(this);
    }

  public override ActionResult onPerform() {
    _destroyMixin.destroyHeldItems(Elements.fire);

    // Being burned "cures" cold.
    if (actor!.cold.isActive) {
      actor!.cold.cancel();
      return succeed("The fire warms {1} back up.", actor);
    }

    return ActionResult.success;
  }
}

/// Side-effect action when an [Elements.fire] area attack sweeps over a tile.
class BurnFloorAction : Action {
  // public Vec _pos;
  public int _damage;
  public int _fuel;

  public DestroyActionMixin _destroyMixin;

  public BurnFloorAction(Vec _pos, int _damage, int _fuel)
  {
    this._pos = _pos;
    this._damage = _damage;
    this._fuel = _fuel;

    _destroyMixin = new DestroyActionMixin(this);
  }

  public override ActionResult onPerform() {
    var fuel = _fuel + _destroyMixin.destroyFloorItems(_pos, Elements.fire);

    // Try to set the tile on fire.
    var tile = game.stage[_pos];
    var ignition = Tiles.ignition(tile.type);
    if (fuel > 0 || ignition > 0 && _damage > Rng.rng.range(ignition)) {
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
class BurningFloorAction : Action {
  // public Vec _pos;

  public DestroyActionMixin _destroyMixin;

  public BurningFloorAction(Vec _pos)
  {
    this._pos = _pos;
    _destroyMixin = new DestroyActionMixin(this);
  }

  public override ActionResult onPerform() {
    // See if there is an actor there.
    var target = game.stage.actorAt(_pos);
    if (target != null) {
      // TODO: What should the damage be?
      var hit = new Attack(new Noun("fire"), "burns", 10, 0, Elements.fire).createHit();
      hit.perform(this, null, target, canMiss: false);
    }

    // Try to burn items.
    game.stage[_pos].substance += _destroyMixin.destroyFloorItems(_pos, Elements.fire);
    return ActionResult.success;
  }
}

/// Side-effect action when an [Elements.cold] area attack sweeps over a tile.
class FreezeFloorAction : Action {
  // public Vec _pos;

  public DestroyActionMixin _destroyMixin;

  public FreezeFloorAction(Vec _pos)
  {
    this._pos = _pos;
    _destroyMixin = new DestroyActionMixin(this);
  }

  public override ActionResult onPerform() {
    _destroyMixin.destroyFloorItems(_pos, Elements.cold);

    // TODO: Put out fire.

    return ActionResult.success;
  }
}

/// Side-effect action when an [Elements.poison] area attack sweeps over a tile.
class PoisonFloorAction : Action {
  // public Vec _pos;
  public int _damage;

  public DestroyActionMixin _destroyMixin;

  public PoisonFloorAction(Vec _pos, int _damage)
  {
    this._pos = _pos;
    this._damage = _damage;
    _destroyMixin = new DestroyActionMixin(this);
  }

  public override ActionResult onPerform() {
    var tile = game.stage[_pos];

    // Fire beats poison.
    if (tile.element == Elements.fire && tile.substance > 0) {
      return ActionResult.success;
    }

    // Try to fill the tile with poison gas.
    if (tile.isFlyable) {
      tile.element = Elements.poison;
      tile.substance = Mathf.Clamp(tile.substance + _damage * 16, 0, 255);
    }

    return ActionResult.success;
  }
}

/// Action created by the [Elements.poison] substance each turn a tile contains
/// poisonous gas.
class PoisonedFloorAction : Action {
  // public Vec _pos;

    public DestroyActionMixin _destroyMixin;
  public PoisonedFloorAction(Vec _pos)
  {
    this._pos = _pos;
    _destroyMixin = new DestroyActionMixin(this);
  }

  public override ActionResult onPerform() {
    // See if there is an actor there.
    var target = game.stage.actorAt(_pos);
    if (target != null) {
      // TODO: What should the damage be?
      var hit =
          new Attack(new Noun("poison"), "chokes", 4, 0, Elements.poison).createHit();
      hit.perform(this, null, target, canMiss: false);
    }

    return ActionResult.success;
  }
}

// TODO: Should interact with substances on tile: spead poison and fire, etc.
class WindAction : Action {
  /// Not immediate to ensure an actor doesn't get blown into the path of a
  /// yet-to-be-processed tile.
  public override bool isImmediate => false;

  public override ActionResult onPerform() {
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
class LightFloorAction : Action {
  // public Vec _pos;
  int _emanation;

  public LightFloorAction(Vec pos, Hit hit, num distance) {
    // The intensity fades from the center outward. Also, strong hits produce
    // more light.
    var min = Mathf.Clamp((float)(1.0 + (int)hit.averageDamage * 4.0), 0.0f, Lighting.max);
    var max = Mathf.Clamp((float)(128.0 + hit.averageDamage * 16.0), 0.0f, Lighting.max);
    
    this._pos = pos;
    this._emanation = (int)MathUtils.lerpDouble((float)(hit.range - distance), 0.0f, hit.range, min, max);
  }

  public LightFloorAction(Vec _pos, int _emanation)
  {
    this._pos = _pos;
    this._emanation = _emanation;
  }

  public override ActionResult onPerform() {
    game.stage[_pos].addEmanation(_emanation);
    game.stage.floorEmanationChanged();

    return ActionResult.success;
  }
}
