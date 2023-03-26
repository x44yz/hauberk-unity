using System.Collections;
using System.Collections.Generic;


// TODO: Should this be in content?
/// This contains all of the tunable game engine parameters. Tweaking these can
/// massively affect all aspects of gameplay.
class Option
{
  public const int maxDepth = 100;

  public const int skillPointsPerLevel = 3;

  /// How much damage an unarmed hero does.
  public const int heroPunchDamage = 3;

  /// The amount of gold a new hero starts with.
  public const int heroGoldStart = 60;

  public const int heroMaxStomach = 400;

  /// The maximum number of items the hero's inventory can contain.
  public const int inventoryCapacity = 24;

  /// The maximum number of items the hero's home inventory can contain.
  /// Note: To make this is more than 26, the home screen UI will need to be
  /// changed.
  public const int homeCapacity = 26;

  /// The maximum number of items the hero's crucible can contain.
  public const int crucibleCapacity = 8;
}
