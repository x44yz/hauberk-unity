using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// [Action] for a melee attack from one [Actor] to another.
class AttackAction : Action 
{
    public Actor defender;

    AttackAction(Actor defender)
    {
        this.defender = defender;
    }

    public override ActionResult onPerform() 
    {
        foreach (var hit in actor!.createMeleeHits(defender)) {
            hit.perform(this, actor, defender);
            if (!defender.isAlive) break;
        }

        return ActionResult.success;
    }

    public override double noise => Sound.attackNoise;

    string toString() => "$actor attacks $defender";
}
