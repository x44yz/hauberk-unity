using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SenseItems : Spell {
  public override string description => "Detect nearby items.";
  public override string name => "Sense Items";
  public override int baseComplexity => 17;
  public override int baseFocusCost => 40;
  public override int range => 20;

  Action onGetAction(Game game, int level) =>
      new DetectAction(new List<DetectType>{DetectType.item}, range);
}

