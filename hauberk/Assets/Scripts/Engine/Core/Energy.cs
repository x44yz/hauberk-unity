using System.Collections;
using System.Collections.Generic;


/// Energy is used to control the rate that actors move relative to other
/// actors. Each game turn, every actor will accumulate energy based on their
/// speed. When it reaches a threshold, that actor can take a turn.
public class Energy
{
  public const int minSpeed = 0;
  public const int normalSpeed = 6;
  public const int maxSpeed = 12;

  public const int actionCost = 240;

  // How much energy is gained each game turn for each speed.
  public static int[] gains = new int[]{
        15, // 1/4 normal speed
        20, // 1/3 normal speed
        24, // 2/5 normal speed
        30, // 1/2 normal speed
        40, // 2/3 normal speed
        50, // 5/6 normal speed
        60, // normal speed
        80, // 4/3 normal speed
        100, // 5/3 normal speed
        120, // 2x normal speed
        150, // 3/2 normal speed
        180, // 3x normal speed
        240 // 4x normal speed
    };

  public int energy = 0;

  public bool canTakeTurn => energy >= actionCost;

  /// Advances one game turn and gains an appropriate amount of energy. Returns
  /// `true` if there is enough energy to take a turn.
  public bool gain(int speed)
  {
    energy += gains[speed];
    return canTakeTurn;
  }

  /// Spends a turn's worth of energy.
  public void spend()
  {
    Debugger.assert(energy >= actionCost);
    energy -= actionCost;
  }
}

