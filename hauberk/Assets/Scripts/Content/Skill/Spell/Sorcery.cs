using System.Collections;
using System.Collections.Generic;

class Icicle : Spell, TargetSkill
{
  public override string name => "Icicle";
  public override string description => "Launches a spear-like icicle.";
  public override int baseComplexity => 10;
  public override int baseFocusCost => 12;
  public override int damage => 8;
  public override int range => 8;

  public bool canTargetSelf() => false;
  public Action onGetTargetAction(Game game, int level, Vec target)
  {
    var attack =
        new Attack(new Noun("the icicle"), "pierce", damage, range, Elements.cold);
    return new BoltAction(target, attack.createHit());
  }
}

class BrilliantBeam : Spell, TargetSkill
{
  public override string name => "Brilliant Beam";
  public override string description => "Emits a blinding beam of radiance.";
  public override int baseComplexity => 14;
  public override int baseFocusCost => 24;
  public override int damage => 10;
  public override int range => 12;

  public bool canTargetSelf() => false;
  public Action onGetTargetAction(Game game, int level, Vec target)
  {
    var attack =
        new Attack(new Noun("the light"), "sear", damage, range, Elements.light);
    return RayAction.cone(game.hero.pos, target, attack.createHit());
  }
}

class Windstorm : Spell, ActionSkill
{
  public override string name => "Windstorm";
  public override string description =>
      "Summons a blast of air, spreading out from the sorceror.";
  public override int baseComplexity => 18;
  public override int baseFocusCost => 36;
  public override int damage => 10;
  public override int range => 6;

  public Action onGetAction(Game game, int level)
  {
    var attack = new Attack(new Noun("the wind"), "blast", damage, range, Elements.air);
    return new FlowAction(game.hero.pos, attack.createHit(), Motility.flyAndWalk);
  }
}

class FireBarrier : Spell, TargetSkill
{
  public override string name => "Fire Barrier";
  public override string description => "Creates a wall of fire.";
  public override int baseComplexity => 30;
  public override int baseFocusCost => 45;
  public override int damage => 10;
  public override int range => 8;

  public bool canTargetSelf() => false;
  public Action onGetTargetAction(Game game, int level, Vec target)
  {
    var attack = new Attack(new Noun("the fire"), "burn", damage, range, Elements.fire);
    return BarrierAction.create(game.hero.pos, target, attack.createHit());
  }
}

class TidalWave : Spell, ActionSkill
{
  public override string name => "Tidal Wave";
  public override string description => "Summons a giant tidal wave.";
  public override int baseComplexity => 40;
  public override int baseFocusCost => 70;
  public override int damage => 50;
  public override int range => 15;

  public Action onGetAction(Game game, int level)
  {
    var attack =
        new Attack(new Noun("the wave"), "inundate", damage, range, Elements.water);
    return new FlowAction(game.hero.pos, attack.createHit(),
        Motility.walk | Motility.door | Motility.swim,
        slowness: 2);
  }
}
