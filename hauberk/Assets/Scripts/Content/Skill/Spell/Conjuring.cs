using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Flee : Spell {
  public override string description => "Teleports the hero a short distance away.";
  public override string name => "Flee";
  int baseComplexity => 10;
  int baseFocusCost => 16;
  int range => 8;

  Action onGetAction(Game game, int level) => new TeleportAction(range);
}

class Escape : Spell {
  public override string description => "Teleports the hero away.";
  public override string name => "Escape";
  int baseComplexity => 15;
  int baseFocusCost => 25;
  int range => 16;

  Action onGetAction(Game game, int level) => new TeleportAction(range);
}

class Disappear : Spell {
  public override string description => "Moves the hero across the dungeon.";
  public override string name => "Disappear";
  int baseComplexity => 30;
  int baseFocusCost => 50;
  int range => 100;

  Action onGetAction(Game game, int level) => new TeleportAction(range);
}

// TODO: These spells are all kind of similar and boring. Might be good if they
// had some differences. Maybe some could try to teleport specifically far away
// from monsters, etc.