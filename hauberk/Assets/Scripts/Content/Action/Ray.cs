using System.Collections;
using System.Collections.Generic;
using System.Linq;
using num = System.Double;
using Mathf = UnityEngine.Mathf;

/// Base class for an action that touches a conical or circular swath of tiles.
abstract class RayActionBase : Action
{
  /// The centerpoint that the cone is radiating from.
  public Vec _from;

  /// The tile being targeted. The arc of the cone will center on a line from
  /// [_from] to this.
  public Vec _to;

  /// The tiles that have already been touched by the effect. Used to make sure
  /// we don't hit the same tile multiple times.
  public List<Vec> _hitTiles = new List<Vec> { };

  /// The cone incrementally spreads outward. This is how far we currently are.
  public float _radius = 1.0f;

  /// We "fill" the cone by tracing a number of rays, each of which can get
  /// obstructed. This is the angle of each ray still being traced.
  public List<double> _rays = new List<double> { };

  public override bool isImmediate => false;

  public virtual int range => 0;

  /// Creates a [RayActionBase] radiating from [_from] centered on [_to] (which
  /// may be the same as [_from] if the ray is a full circle. The rays cover a
  /// chord whose width is [fraction] which varies from 0 (an infinitely narrow
  /// line) to 1.0 (a full circle).
  public RayActionBase(Vec _from, Vec _to, double fraction)
  {
    this._from = _from;
    this._to = _to;

    // We "fill" the cone by tracing a number of rays. We need enough of them
    // to ensure there are no gaps when the cone is at its maximum extent.
    var circumference = Mathf.PI * 2 * range;
    var numRays = Mathf.Ceil((float)(circumference * fraction * 2.0f));

    if (fraction < 1.0)
    {
      // Figure out the center angle of the cone.
      var offset = _to - _from;
      // TODO: Make atan2 getter on Vec?
      var centerTheta = 0.0;
      if (_from != _to)
      {
        centerTheta = Mathf.Atan2(offset.x, offset.y);
      }

      // Create the rays centered on the line from [_from] to [_to].
      for (var i = 0; i < numRays; i++)
      {
        var theta = (i * 1f / (numRays - 1)) - 0.5;
        _rays.Add(centerTheta + theta * (Mathf.PI * 2.0 * fraction));
      }
    }
    else
    {
      // Create the rays all the way around the circle.
      var thetaStep = Mathf.PI * 2.0 / numRays;
      for (var i = 0; i < numRays; i++)
      {
        _rays.Add(i * thetaStep);
      }
    }
  }

  public override ActionResult onPerform()
  {
    if (_radius == 0.0)
    {
      reachStartTile(_from);
      _radius += 1.0f;
      return ActionResult.notDone;
    }

    // TODO: When using this for casting light, should really hit the hero's
    // tile too.

    // See which new tiles each ray hit now.
    _rays.RemoveAll((ray) =>
    {
      var pos = new Vec(_from.x + Mathf.RoundToInt(Mathf.Sin((float)ray) * _radius),
          _from.y + Mathf.RoundToInt(Mathf.Cos((float)ray) * _radius));

      // TODO: Support rays that hit closed doors but do not go past them. That
      // would let fire attacks set closed doors on fire.

      // Kill the ray if it's obstructed.
      if (!game.stage[pos].isFlyable) return true;

      // Don't hit the same tile twice.
      if (_hitTiles.Contains(pos)) return false;
      _hitTiles.Add(pos);

      reachTile(pos, (pos - _from).length);
      return false;
    });

    _radius += 1.0f;
    if (_radius > range || _rays.Count == 0) return ActionResult.success;

    // Still going.
    return ActionResult.notDone;
  }

  public virtual void reachStartTile(Vec pos) { }

  public abstract void reachTile(Vec pos, num distance);
}

/// Creates a swath of damage that radiates out from a point.
class RayAction : RayActionBase, ElementActionMixin
{
  public Hit _hit;
  public override int range => _hit.range;

  /// A 45° cone of [hit] centered on the line from [from] to [to].
  public static RayAction cone(Vec from, Vec to, Hit hit) =>
      new RayAction(hit, from, to, 1.0 / 8.0);

  /// A complete ring of [hit] radiating outwards from [center].
  public static RayAction ring(Vec center, Hit hit) =>
      new RayAction(hit, center, center, 1.0);

  public RayAction(Hit _hit, Vec from, Vec to, double fraction)
      : base(from, to, fraction)
  {
    this._hit = _hit;
  }

  public override void reachTile(Vec pos, num distance)
  {
    (this as ElementActionMixin).hitTile(_hit, pos, distance);
  }
}

/// Creates an expanding ring of damage centered on the [Actor].
///
/// This class mainly exists as an [Action] that [Item]s can use.
class RingSelfAction : Action
{
  public Attack _attack;

  public RingSelfAction(Attack _attack)
  {
    this._attack = _attack;
  }

  public override bool isImmediate => false;

  public override ActionResult onPerform()
  {
    return alternate(RayAction.ring(actor!.pos, _attack.createHit()));
  }
}

class RingFromAction : Action
{
  public Attack _attack;
  public Vec m_pos;

  public RingFromAction(Attack _attack, Vec _pos)
  {
    this._attack = _attack;
    this.m_pos = _pos;
  }

  public override bool isImmediate => false;

  public override ActionResult onPerform()
  {
    return alternate(RayAction.ring(m_pos, _attack.createHit()));
  }
}

