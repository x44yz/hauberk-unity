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

  Condition condition => actor!.haste;

  int getIntensity() => _speed;
  public override int getDuration() => _duration;
  public override void onActivate() => log("{1} start[s] moving faster.", actor);
  public override void onExtend() => log("{1} [feel]s the haste lasting longer.", actor);
  public override void onIntensify() => log("{1} move[s] even faster.", actor);
}

class FreezeActorAction : ConditionAction
{
  public int _damage;
  public DestroyActionMixin _destroyMixin;

  public FreezeActorAction(int _damage)
  {
    this._damage = _damage;
    _destroyMixin = new DestroyActionMixin(this);
  }

  Condition condition => actor!.cold;

  public override ActionResult onPerform()
  {
    _destroyMixin.destroyHeldItems(Elements.cold);
    return base.onPerform();
  }

  int getIntensity() => 1 + _damage / 40;
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

  Condition condition => actor!.poison;

  int getIntensity() => 1 + _damage / 20;
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

  Condition condition => actor!.blindness;

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

  Condition condition => actor!.dazzle;

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

  Condition condition => actor!.resistances[_element]!;

  public override int getDuration() => _duration;
  // TODO: Resistances of different intensity.
  public override void onActivate() => log("{1} [are|is] resistant to $_element.", actor);
  public override void onExtend() => log("{1} feel[s] the resistance extend.", actor);
}
