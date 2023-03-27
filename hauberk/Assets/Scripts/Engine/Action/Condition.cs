using System.Collections;
using System.Collections.Generic;


/// Base class for an [Action] that applies (or extends/intensifies) a
/// [Condition]. It handles cases where the condition is already in effect with
/// possibly a different intensity.
abstract class ConditionAction : Action
{
  /// The [Condition] on the actor that should be affected.
  protected virtual Condition condition => null;

  /// The intensity of the condition to apply.
  protected virtual int getIntensity() => 1;

  /// The number of turns the condition should last.
  public abstract int getDuration();

  /// Override this to log the message when the condition is first applied.
  public abstract void onActivate();

  /// Override this to log the message when the condition is already in effect
  /// and its duration is extended.
  public abstract void onExtend();

  /// Override this to log the message when the condition is already in effect
  /// at a weaker intensity and the intensity increases.
  public virtual void onIntensify() { }

  public override ActionResult onPerform()
  {
    var intensity = getIntensity();
    var duration = getDuration();

    if (!condition.isActive)
    {
      condition.activate(duration, intensity);
      onActivate();
      return ActionResult.success;
    }

    if (condition.intensity >= intensity)
    {
      // Scale down the new duration by how much weaker the new intensity is.
      duration = (duration * intensity) / condition.intensity;

      // Compounding doesn't add as much as the first one.
      duration /= 2;
      if (duration == 0) return succeed();

      condition.extend(duration);
      onExtend(); 
      return ActionResult.success;
    }

    // Scale down the existing duration by how much stronger the new intensity
    // is.
    var oldDuration = (condition.duration * condition.intensity) / intensity;

    condition.activate(oldDuration + duration / 2, intensity);
    onIntensify();
    return ActionResult.success;
  }
}
