using System.Collections;
using System.Collections.Generic;

/// Fires a bolt, a straight line of an elemental attack that stops at the
/// first [Actor] is hits or opaque tile.
class BoltAction : LosAction
{
  public Hit _hit;
  public bool _canMiss;
  public int? _range;

  public override int range => _range ?? _hit.range;

  public BoltAction(Vec target, Hit _hit, bool canMiss = false, int? range = null)
    : base(target)
  {
    this._hit = _hit;
    this._canMiss = canMiss;
    this._range = range;
  }


  public override void onStep(Vec previous, Vec pos)
  {
    addEvent(EventType.bolt,
        element: _hit.element,
        pos: pos,
        dir: (pos - previous).nearestDirection);
  }

  public override bool onHitActor(Vec pos, Actor target)
  {
    // TODO: Should range increase odds of missing? If so, do that here. Also
    // need to tweak enemy AI then since they shouldn't always try to maximize
    // distance.
    _hit.perform(this, actor, target, canMiss: _canMiss);
    return true;
  }
}