using System.Collections;
using System.Collections.Generic;

class HasteAction : ConditionAction
{
  public int _speed;
  public int _duration;

  public HasteAction(int _speed, int _duration)
  {
    this._speed = _speed;
    this._duration = _duration;
  }

  protected override Condition condition => actor!.haste;

  protected override int getIntensity() => _speed;
  public override int getDuration() => _duration;
  public override void onActivate() => log("{1} start[s] moving faster.", actor);
  public override void onExtend() => log("{1} [feel]s the haste lasting longer.", actor);
  public override void onIntensify() => log("{1} move[s] even faster.", actor);
}

class FreezeActorAction : ConditionAction, DestroyActionMixin
{
  public int _damage;

  public FreezeActorAction(int _damage)
  {
    this._damage = _damage;
  }

  protected override Condition condition => actor!.cold;

  public override ActionResult onPerform()
  {
    (this as DestroyActionMixin).destroyHeldItems(Elements.cold);
    return base.onPerform();
  }

  protected override int getIntensity() => 1 + _damage / 40;
  public override int getDuration() => 3 + Rng.rng.triangleInt(_damage * 2, _damage / 2);
  public override void onActivate() => log("{1} [are|is] frozen!", actor);
  public override void onExtend() => log("{1} feel[s] the cold linger!", actor);
  public override void onIntensify() => log("{1} feel[s] the cold intensify!", actor);
}

class PoisonAction : ConditionAction
{
  public int _damage;

  public PoisonAction(int _damage)
  {
    this._damage = _damage;
  }

  protected override Condition condition => actor!.poison;

  protected override int getIntensity() => 1 + _damage / 20;
  public override int getDuration() => 1 + Rng.rng.triangleInt(_damage * 2, _damage / 2);
  public override void onActivate() => log("{1} [are|is] poisoned!", actor);
  public override void onExtend() => log("{1} feel[s] the poison linger!", actor);
  public override void onIntensify() => log("{1} feel[s] the poison intensify!", actor);
}

class BlindAction : ConditionAction
{
  public int _damage;

  public BlindAction(int _damage)
  {
    this._damage = _damage;
  }

  protected override Condition condition => actor!.blindness;

  public override int getDuration() => 3 + Rng.rng.triangleInt(_damage * 2, _damage / 2);

  public override void onActivate()
  {
    log("{1 his} vision dims!", actor);
    game.stage.heroVisibilityChanged();
  }

  public override void onExtend() => log("{1 his} vision dims!", actor);
}

class DazzleAction : ConditionAction
{
  public int _damage;

  public DazzleAction(int _damage)
  {
    this._damage = _damage;
  }

  protected override Condition condition => actor!.dazzle;

  public override int getDuration() => 3 + Rng.rng.triangleInt(_damage * 2, _damage / 2);
  public override void onActivate() => log("{1} [are|is] dazzled by the light!", actor);
  public override void onExtend() => log("{1} [are|is] dazzled by the light!", actor);
}

class ResistAction : ConditionAction
{
  public int _duration;
  public Element _element;

  public ResistAction(int _duration, Element _element)
  {
    this._duration = _duration;
    this._element = _element;
  }

  protected override Condition condition => actor!.resistances[_element]!;

  public override int getDuration() => _duration;
  // TODO: Resistances of different intensity.
  public override void onActivate() => log("{1} [are|is] resistant to $_element.", actor);
  public override void onExtend() => log("{1} feel[s] the resistance extend.", actor);
}
