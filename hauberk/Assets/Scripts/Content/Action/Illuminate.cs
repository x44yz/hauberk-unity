using System.Collections;
using System.Collections.Generic;
using num = System.Double;

/// Creates a swath of emanation that radiates out from a point.
class IlluminateAction : RayActionBase
{
  public override int range => _range;
  int _range;

  public IlluminateAction(int range, Vec center) : base(center, center, 1.0)
  {
    this._range = range;
  }

  public override void reachStartTile(Vec pos)
  {
    reachTile(pos, 0);
  }

  public override void reachTile(Vec pos, num distance)
  {
    game.stage[pos].maxEmanation(Lighting.emanationForLevel(3));
    game.stage.floorEmanationChanged();
    addEvent(EventType.pause);
  }
}

/// Creates an expanding ring of emanation centered on the [Actor].
///
/// This class mainly exists as an [Action] that [Item]s can use.
class IlluminateSelfAction : Action
{
  public int _range;

  public IlluminateSelfAction(int _range)
  {
    this._range = _range;
  }

  public override bool isImmediate => false;

  public override ActionResult onPerform()
  {
    game.stage[actor!.pos].maxEmanation(Lighting.emanationForLevel(3));
    game.stage.floorEmanationChanged();
    addEvent(EventType.pause);

    return alternate(new IlluminateAction(_range, actor!.pos));
  }
}

