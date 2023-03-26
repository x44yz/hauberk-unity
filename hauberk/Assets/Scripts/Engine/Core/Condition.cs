using System.Collections;
using System.Collections.Generic;


// TODO: To reinforce the session-oriented play style of the game, maybe these
// shouldn't wear off?

/// A temporary condition that modifies some property of an [Actor] while it
/// is in effect.
public abstract class Condition
{
  /// The [Actor] that this condition applies to.
  public Actor actor => _actor;
  public Actor _actor;

  /// The number of turns that the condition while remain in effect for.
  public int _turnsRemaining = 0;

  /// The "intensity" of this condition. The interpretation of this varies from
  /// condition to condition.
  public int _intensity = 0;

  /// Gets whether the condition is currently in effect.
  public bool isActive => _turnsRemaining > 0;

  public int duration => _turnsRemaining;

  /// The condition's current intensity, or zero if not active.
  public int intensity => _intensity;

  /// Binds the condition to the actor that it applies to. Must be called and
  /// can only be called once.
  public void bind(Actor actor)
  {
    _actor = actor;
  }

  /// Processes one turn of the condition.
  public void update(Action action)
  {
    if (isActive)
    {
      _turnsRemaining--;
      if (isActive)
      {
        onUpdate(action);
      }
      else
      {
        onDeactivate();
        _intensity = 0;
      }
    }
  }

  /// Extends the condition by [duration].
  public void extend(int duration)
  {
    _turnsRemaining += duration;
  }

  /// Activates the condition for [duration] turns at [intensity].
  public void activate(int duration, int intensity = 1)
  {
    _turnsRemaining = duration;
    _intensity = intensity;
  }

  /// Cancels the condition immediately. Does not deactivate the condition.
  public void cancel()
  {
    _turnsRemaining = 0;
    _intensity = 0;
  }

  // TODO: Instead of modifying the given action, should this create a reaction?
  protected virtual void onUpdate(Action action) { }

  protected virtual void onDeactivate() { }
}

// TODO: Move these to content?

/// A condition that temporarily boosts the actor's speed.
class HasteCondition : Condition
{
  protected override void onDeactivate()
  {
    actor.log("{1} slow[s] back down.", actor);
  }
}

/// A condition that temporarily lowers the actor's speed.
class ColdCondition : Condition
{
  protected override void onDeactivate()
  {
    actor.log("{1} warm[s] back up.", actor);
  }
}

/// A condition that inflicts damage every turn.
class PoisonCondition : Condition
{
  protected override void onUpdate(Action action)
  {
    // TODO: Apply resistances. If resistance lowers intensity to zero, end
    // condition and log message.

    if (!actor.takeDamage(action, intensity, new Noun("the poison")))
    {
      actor.log("{1} [are|is] hurt by poison!", actor);
    }
  }

  protected override void onDeactivate()
  {
    actor.log("{1} [are|is] no longer poisoned.", actor);
  }
}

/// A condition that impairs vision.
public class BlindnessCondition : Condition
{
  protected override void onDeactivate()
  {
    actor.log("{1} can see clearly again.", actor);
    if (actor == actor.game.hero) actor.game.stage.heroVisibilityChanged();
  }
}

/// A condition that provides resistance to an element.
public class ResistCondition : Condition
{
  public Element _element;

  public ResistCondition(Element element)
  {
    this._element = element;
  }

  protected override void onDeactivate()
  {
    actor.log("{1} feel[s] susceptible to $_element.", actor);
  }
}

/// A condition that provides non-visual perception of nearby monsters.
public class PerceiveCondition : Condition
{
  protected override void onDeactivate()
  {
    actor.log("{1} no longer perceive[s] monsters.", actor);
  }
}
