using System.Collections;
using System.Collections.Generic;


/// Base class for an [Action] that traces a path from the actor along a [Line].
abstract class LosAction : Action
{
  public Vec _target;
  public Vec _lastPos;

  private IEnumerator<Vec> m_los = null;
  public IEnumerator<Vec> _los
  {
    get
    {
      if (m_los == null)
        m_los = _initIterator();
      return m_los;
    }
  }

  /// Override this to provide the range of the line.
  public virtual int range => 0;

  public override bool isImmediate => false;

  public LosAction(Vec _target)
  {
    this._target = _target;
  }

  public override ActionResult onPerform()
  {
    var pos = _los.Current;

    // Stop if we hit a wall or went out of range.
    if (!game.stage[pos].isFlyable || pos - actor!.pos > range)
    {
      onEnd(_lastPos);
      return succeed();
    }

    onStep(_lastPos, pos);

    // See if there is an actor there.
    var target = game.stage.actorAt(pos);
    if (target != null && target != actor)
    {
      if (onHitActor(pos, target)) return ActionResult.success;
    }

    if (pos == _target)
    {
      if (onTarget(pos)) return ActionResult.success;
    }

    _lastPos = pos;
    return doneIf(!_los.MoveNext());
  }

  /// Override this to handle the LOS reaching an open tile.
  public abstract void onStep(Vec previous, Vec pos);

  /// Override this to handle the LOS hitting an [Actor].
  ///
  /// Return `true` if the LOS should stop here or `false` if it should keep
  /// going.
  public virtual bool onHitActor(Vec pos, Actor target) => true;

  /// Override this to handle the LOS hitting a wall or going out of range.
  ///
  /// [pos] is the position on the path *before* failure. It's the last good
  /// position. It may be the actor's position if the LOS hit a wall directly
  /// adjacent to the actor.
  public virtual void onEnd(Vec pos) { }

  /// Override this to handle the LOS reaching the target on an open tile.
  ///
  /// If this returns `true`, the LOS will stop there. Otherwise it will
  /// continue until it reaches the end of its range or hits something.
  public virtual bool onTarget(Vec pos) => false;

  IEnumerator<Vec> _initIterator()
  {
    var iterator = new Line(actor!.pos, _target).iterator;

    // Advance to the first tile.
    iterator.MoveNext();

    _lastPos = actor!.pos;
    return iterator;
  }
}
