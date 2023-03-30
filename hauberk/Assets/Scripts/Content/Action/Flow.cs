using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// Creates a swath of damage that flows out from a point through reachable
/// tiles.
class FlowAction : Action, ElementActionMixin
{
  /// The centerpoint that the flow is radiating from.
  public Vec _from;
  public Hit _hit;

  public Flow _flow;
  // TODO: Make this late public?
  List<Vec> _tiles;

  public Motility _motility;
  public int _slowness;

  public override bool isImmediate => false;

  int _frame = 0;

  // TODO: Support motilities that can flow into closed doors but not out of
  // them. That would let fire flow attacks that can set closed doors on fire.

  public FlowAction(Vec _from, Hit _hit, Motility _motility, int? slowness = null)
  {
    this._from = _from;
    this._hit = _hit;
    this._motility = _motility;
    this._slowness = slowness ?? 1;
  }

  public override ActionResult onPerform()
  {
    // Only animate 1/slowness frames.
    _frame = (_frame + 1) % _slowness;
    if (_frame != 0)
    {
      addEvent(EventType.pause);
      return ActionResult.notDone;
    }

    if (_tiles == null)
    {
      // TODO: Use a different flow that makes diagonal moves more expensive to
      // give more natural circular behavior?
      _flow = new MotilityFlow(game.stage, _from, _motility, avoidActors: false);

      _tiles = _flow.reachable
          .TakeWhile((pos) => _flow.costAt(pos)! <= _hit.range)
          .ToList();
    }

    // Hit all tiles at the same distance.
    var distance = _flow.costAt(_tiles!.First())!;
    int end;
    for (end = 0; end < _tiles!.Count; end++)
    {
      if (_flow.costAt(_tiles![end]) != distance) break;
    }

    foreach (var pos in _tiles!.GetRange(0, end))
    {
      (this as ElementActionMixin).hitTile(_hit, pos, distance.Value);
    }

    _tiles = _tiles!.GetRange(end, _tiles.Count - end);
    if (_tiles!.Count == 0) return ActionResult.success;

    return ActionResult.notDone;
  }
}

/// Creates an expanding flow of damage centered on the [Actor].
///
/// This class mainly exists as an [Action] that [Item]s can use.
class FlowSelfAction : Action
{
  public Attack _attack;
  public Motility _motility;

  public FlowSelfAction(Attack _attack, Motility _motility)
  {
    this._attack = _attack;
    this._motility = _motility;
  }

  public override ActionResult onPerform()
  {
    return alternate(new FlowAction(actor!.pos, _attack.createHit(), _motility));
  }
}

class FlowFromAction : Action
{
  public Attack _attack;
  public Vec m_pos;
  public Motility _motility;

  public FlowFromAction(Attack _attack, Vec _pos, Motility _motility)
  {
    this._attack = _attack;
    this.m_pos = _pos;
    this._motility = _motility;
  }

  public override ActionResult onPerform()
  {
    return alternate(new FlowAction(m_pos, _attack.createHit(), _motility));
  }
}