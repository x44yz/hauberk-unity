using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;

/// An active entity in the game. Includes monsters and the hero.
public abstract class Actor : Noun
{
  public Game game;
  public Energy energy = new Energy();

  /// Haste raises speed.
  public Condition haste = new HasteCondition();

  /// Cold lowers speed.
  public Condition cold = new ColdCondition();

  /// Poison inflicts damage each turn.
  public Condition poison = new PoisonCondition();

  /// Makes it hard for the actor to see.
  public Condition blindness = new BlindnessCondition();

  /// Makes it hard for the actor to see.
  public Condition dazzle = new BlindnessCondition();

  public Condition perception = new PerceiveCondition();

  // TODO: Wrap this in a method that returns a non-nullable result.
  // Temporary resistance to elements.
  public Dictionary<Element, ResistCondition> resistances = new Dictionary<Element, ResistCondition>();

  // All [Condition]s for the actor.
  List<Condition> conditions
  {
    get
    {
      var rt = new List<Condition>() {
        haste,
        cold,
        poison,
        blindness,
        dazzle,
        perception,
      };
      rt.AddRange(resistances.Values);
      return rt;
    }
  }

  Vec _pos;
  public Vec pos
  {
    get { return _pos; }
    set
    {
      if (value != _pos)
      {
        changePosition(_pos, value);
        _pos = value;
      }
    }
  }

  public int x
  {
    get { return pos.x; }
    set
    {
      pos = new Vec(value, y);
    }
  }

  public int y
  {
    get { return pos.y; }
    set
    {
      pos = new Vec(x, value);
    }
  }

  int _health = 0;
  public int health
  {
    get { return _health; }
    set
    {
      _health = Mathf.Clamp(value, 0, maxHealth);
    }
  }

  public Actor(Game game, int x, int y)
  : base("")
  {
    this.game = game;
    _pos = new Vec(x, y);

    foreach (var element in game.content.elements)
    {
      resistances[element] = new ResistCondition(element);
    }

    foreach (var condition in conditions)
    {
      condition.bind(this);
    }
  }

  public virtual object appearance => null;

  // string nounText;

  public override Pronoun pronoun => Pronoun.it;

  public bool isAlive => health > 0;

  /// Whether or not the actor can be seen by the hero.
  public bool isVisibleToHero => game.stage[pos].isVisible;

  /// Whether the actor's vision is currently impaired.
  public bool isBlinded => blindness.isActive || dazzle.isActive;

  public virtual bool needsInput => false;

  public virtual Motility motility => null;

  public virtual int maxHealth => 0;

  /// Gets the actor's current speed, taking into any account any active
  /// [Condition]s.
  public int speed
  {
    get
    {
      var speed = baseSpeed;
      speed += haste.intensity;
      speed -= cold.intensity;
      return speed;
    }
  }

  /// Additional ways the actor can avoid a hit beyond dodging it.
  public List<Defense> defenses
  {
    get
    {
      var rt = new List<Defense>();

      var dodge = baseDodge;

      // Hard to dodge an attack you can't see coming.
      if (isBlinded) dodge /= 2;

      if (dodge != 0) rt.Add(new Defense(dodge, "{1} dodge[s] {2}."));

      rt.AddRange(onGetDefenses());
      return rt;
    }
  }

  /// The amount of protection against damage the actor has.
  public virtual int armor => 0;

  /// The amount of light emanating from this actor.
  ///
  /// This is not a raw emanation value, but a "level" to be passed to
  /// [Lighting.emanationForLevel()].
  public virtual int emanationLevel => 0;

  /// Called when the actor's position is about to change from [from] to [to].
  public virtual void changePosition(Vec from, Vec to)
  {
    game.stage.moveActor(from, to);

    if (emanationLevel > 0) game.stage.actorEmanationChanged();
  }

  public virtual int baseSpeed => 0;

  /// The actor's base dodge ability. This is the percentage chance of a melee
  /// attack missing the actor.
  public virtual int baseDodge => 0;

  public abstract List<Defense> onGetDefenses();

  public Action getAction()
  {
    var action = onGetAction();
    action.bind(this);
    return action;
  }

  public abstract Action onGetAction();

  /// Create a new [Hit] for this [Actor] to attempt to hit [defender].
  ///
  /// Note that [defender] may be null if this hit is being created for
  /// something like a bolt attack or whether the targeted actor isn't known.
  public List<Hit> createMeleeHits(Actor defender)
  {
    var hits = onCreateMeleeHits(defender);
    foreach (var hit in hits)
    {
      modifyHit(hit, HitType.melee);
    }
    return hits;
  }

  public abstract List<Hit> onCreateMeleeHits(Actor defender);

  /// Applies the hit modifications from the actor.
  public void modifyHit(Hit hit, HitType type)
  {
    // Hard to hit an actor you can't see.
    if (isBlinded)
    {
      switch (type)
      {
        case HitType.melee:
          hit.scaleStrike(0.5);
          break;
        case HitType.ranged:
          hit.scaleStrike(0.3);
          break;
        case HitType.toss:
          hit.scaleStrike(0.2);
          break;
      }
    }

    // Let the subclass also modify it.
    onModifyHit(hit, type);
  }

  public virtual void onModifyHit(Hit hit, HitType type) { }

  /// The amount of resistance the actor currently has to [element].
  ///
  /// Every level of resist reduces the damage taken by an attack of that
  /// element by 1/(resistance + 1), so that 1 resist is half damange, 2 is
  /// third, etc.
  public int resistance(Element element)
  {
    // TODO: What about negative resists?

    // Get the base resist from the subclass.
    var result = onGetResistance(element);

    // Apply temporary resistance.
    var resistance = resistances[element]!;
    if (resistance.isActive)
    {
      result += resistance.intensity;
    }

    return result;
  }

  public abstract int onGetResistance(Element element);

  /// Reduces the actor's health by [damage], and handles its death. Returns
  /// `true` if the actor died.
  public bool takeDamage(Action action, int damage, Noun attackNoun,
      Actor attacker = null)
  {
    health -= damage;
    onTakeDamage(action, attacker, damage);

    if (isAlive) return false;

    action.addEvent(EventType.die, actor: this);

    // TODO: Different verb for unliving monsters.
    action.log("{1} kill[s] {2}.", attackNoun, this);
    if (attacker != null) attacker.onKilled(action, this);

    onDied(attackNoun);

    return true;
  }

  /// Called when this actor has successfully hit [defender].
  public virtual void onGiveDamage(Action action, Actor defender, int damage)
  {
    // Do nothing.
  }

  /// Called when [attacker] has successfully hit this actor.
  ///
  /// [attacker] may be `null` if the damage is not the direct result of an
  /// attack (for example, poison).
  public virtual void onTakeDamage(Action action, Actor attacker, int damage)
  {
    // Do nothing.
  }

  /// Called when this Actor has been killed by [attackNoun].
  public virtual void onDied(Noun attackNoun)
  {
    // Do nothing.
  }

  /// Called when this Actor has killed [defender].
  public virtual void onKilled(Action action, Actor defender)
  {
    // Do nothing.
  }

  /// Called when this Actor has completed a turn.
  public virtual void onFinishTurn(Action action)
  {
    // Do nothing.
  }

  /// Whether it's possible for the actor to ever be on the tile at [pos].
  public bool canOccupy(Vec pos)
  {
    if (pos.x < 0) return false;
    if (pos.x >= game.stage.width) return false;
    if (pos.y < 0) return false;
    if (pos.y >= game.stage.height) return false;

    var tile = game.stage[pos];
    return tile.canEnter(motility);
  }

  /// Whether the actor ever desires to be on the tile at [pos].
  ///
  /// Takes into account that actors do not want to step into burning tiles,
  /// but does not care if the tile is occupied.
  public bool willOccupy(Vec pos) => canOccupy(pos) && game.stage[pos].substance == 0;

  /// Whether the actor can enter the tile at [pos] right now.
  ///
  /// This is true if the actor can occupy [pos] and no other actor already is.
  public bool canEnter(Vec pos) => canOccupy(pos) && game.stage.actorAt(pos) == null;

  /// Whether the actor desires to enter the tile at [pos].
  ///
  /// Takes into account that actors do not want to step into burning tiles.
  public bool willEnter(Vec pos) => canEnter(pos) && game.stage[pos].substance == 0;

  // TODO: Take resistance and immunities into account.

  public void finishTurn(Action action)
  {
    energy.spend();

    foreach (var condition in conditions)
    {
      condition.update(action);
    }

    if (isAlive) onFinishTurn(action);
  }

  /// Logs [message] if the actor is visible to the hero.
  public void log(string message, Noun noun1 = null, Noun noun2 = null, Noun noun3 = null)
  {
    if (!game.hero.canPerceive(this)) return;
    game.log.message(message, noun1, noun2, noun3);
  }

  public override string ToString() => nounText;
}
