using System.Collections;
using System.Collections.Generic;

public enum Missive
{
  clumsy,
  insult,
  screech
}

class MissiveAction : Action
{

  public static Dictionary<Missive, List<string>> _messages = new Dictionary<Missive, List<string>>(){
        {Missive.clumsy, new List<string>{
            "{1} forget[s] what {1 he} was doing.",
            "{1} lurch[es] around.",
            "{1} stumble[s] awkwardly.",
            "{1} trip[s] over {1 his} own feet!",
        }},
        {Missive.insult, new List<string>{
            "{1} insult[s] {2 his} mother!",
            "{1} jeer[s] at {2}!",
            "{1} mock[s] {2} mercilessly!",
            "{1} make[s] faces at {2}!",
            "{1} laugh[s] at {2}!",
            "{1} sneer[s] at {2}!",
        }},
        {Missive.screech, new List<string>{
            "{1} screeches at {2}!",
            "{1} taunts {2}!",
            "{1} cackles at {2}!"
        }},
    };

  public Actor target;
  public Missive missive;

  public MissiveAction(Actor target, Missive missive)
  {
    this.target = target;
    this.missive = missive;
  }

  public override ActionResult onPerform()
  {
    var message = Rng.rng.item<string>(_messages[missive]);

    return succeed(message, actor, target);
  }
}
