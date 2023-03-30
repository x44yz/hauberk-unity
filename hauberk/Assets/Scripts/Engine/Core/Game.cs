using System;
using System.Collections;
using System.Collections.Generic;


/// Root class for the game engine. All game state is contained within this.
public class Game
{
  public Content content;

  public HeroSave _save;
  public Log log = new Log();

  public List<Action> _actions = new List<Action>();
  public List<Action> _reactions = new List<Action>();

  /// The events that have occurred since the last call to [update()].
  public List<Event> _events = new List<Event>();

  /// The energy that tracks when the substances are ready to update.
  public Energy _substanceEnergy = new Energy();

  /// Substances work like a cellular automata. A normal cellular automata
  /// updates all cells simultaneously using double buffering. That wouldn't
  /// play nice with the game's action system, which likes to process each
  /// single action and its consequences to completion before moving to the
  /// next one.
  ///
  /// To handle that, we instead update substance cells one at a time. To avoid
  /// visible skew and artifacts from updating sequentially through the dungeon,
  /// we shuffle the cells and update them in random order. This is that order.
  public List<Vec> _substanceUpdateOrder = new List<Vec>();

  /// While the game is processing substance tiles, this is the index of the
  /// current tile's position in [_substanceUpdateOrder]. Otherwise, this is
  /// `null`.
  int? _substanceIndex;

  public int depth;

  public Stage stage => _stage;
  Stage _stage;
  public Hero hero;

  public Game(Content content, HeroSave _save, int depth, int width = 80, int height = 60)
  {
    this.content = content;
    this._save = _save;
    this.depth = depth;
    // TODO: Vary size?
    _stage = new Stage(width, height, this);

    _substanceUpdateOrder.AddRange(_stage.bounds.inflate(-1));
    Rng.rng.shuffle(_substanceUpdateOrder);
  }

  public IEnumerator generate(System.Action callback)
  {
    // TODO: Do something useful with depth.
    Vec heroPos = Vec.zero;
    yield return Main.Inst.StartCoroutine(content.buildStage(_save.lore, _stage, depth, (pos) =>
    {
      heroPos = pos;
    }));

    hero = new Hero(this, heroPos, _save);
    _stage.addActor(hero);

    yield return "Calculating visibility";
    _stage.refreshView();
    callback.Invoke();
  }

  public GameResult update()
  {
    var madeProgress = false;

    while (true)
    {
      // Process any ongoing or pending actions.
      while (_actions.Count > 0)
      {
        var action = _actions[0];

        var result = action.perform();

        // Cascade through the alternates until we hit bottom.
        while (result.alternative != null)
        {
          _actions.RemoveAt(0);
          action = result.alternative!;
          _actions.Insert(0, action);

          result = action.perform();
        }

        while (_reactions.Count > 0)
        {
          var reaction = _reactions[_reactions.Count - 1];
          _reactions.RemoveAt(_reactions.Count - 1);
          var rt = reaction.perform();

          // Cascade through the alternates until we hit bottom.
          while (rt.alternative != null)
          {
            reaction = rt.alternative!;
            rt = reaction.perform();
          }

          Debugger.assert(rt.succeeded, "Reactions should never fail.");
        }

        stage.refreshView();
        madeProgress = true;

        if (result.done)
        {
          _actions.RemoveAt(0);

          if (result.succeeded && action.consumesEnergy)
          {
            action.actor!.finishTurn(action);
            stage.advanceActor();
          }

          // Refresh every time the hero takes a turn.
          if (action.actor == hero) return makeResult(madeProgress);
        }

        if (_events.Count > 0) return makeResult(madeProgress);
      }

      // If we are in the middle of updating substances, keep working through
      // them.
      if (_substanceIndex != null)
      {
        _updateSubstances();
      }

      // If we get here, all pending actions are done, so advance to the next
      // tick until an actor moves.
      while (_actions.Count == 0)
      {
        var actor = stage.currentActor;

        // If we are still waiting for input for the actor, just return (again).
        if (actor.energy.canTakeTurn && actor.needsInput)
        {
          return makeResult(madeProgress);
        }

        if (actor.energy.canTakeTurn || actor.energy.gain(actor.speed))
        {
          // If the actor can move now, but needs input from the user, just
          // return so we can wait for it.
          if (actor.needsInput) return makeResult(madeProgress);

          _actions.Add(actor.getAction());
        }
        else
        {
          // This actor doesn't have enough energy yet, so move on to the next.
          stage.advanceActor();
        }

        // Each time we wrap around, process "idle" things that are ongoing and
        // speed independent.
        if (actor == hero)
        {
          if (_substanceEnergy.gain(Energy.normalSpeed))
          {
            _substanceEnergy.spend();
            _substanceIndex = 0;
            _updateSubstances();
          }
          //          trySpawnMonster();
        }
      }
    }
  }

  public void addAction(Action action)
  {
    if (action.isImmediate)
    {
      _reactions.Add(action);
    }
    else
    {
      _actions.Add(action);
    }
  }

  public void addEvent(EventType type,
      Actor actor = null,
      Element element = null,
      object other = null,
      Vec pos = null,
      Direction dir = null)
  {
    _events.Add(new Event(type, actor, element ?? Element.none, pos, dir, other));
  }

  GameResult makeResult(bool madeProgress)
  {
    var result = new GameResult(madeProgress);
    result.events.AddRange(_events);
    _events.Clear();
    return result;
  }

  void _updateSubstances()
  {
    while (_substanceIndex! < _substanceUpdateOrder.Count)
    {
      var pos = _substanceUpdateOrder[_substanceIndex!.Value];
      var action = content.updateSubstance(stage, pos);
      _substanceIndex = _substanceIndex! + 1;

      if (action != null)
      {
        action.bindPassive(this, pos);
        _actions.Add(action);
        return;
      }
    }

    // If we reach the end, we are done with them for now.
    _substanceIndex = null;
  }

  // TODO: Decide if we want to keep this. Now that there is hunger forcing the
  // player to explore, it doesn't seem strictly necessary.
  /// Over time, new monsters will appear in unexplored areas of the dungeon.
  /// This is to encourage players to not waste time: the more they linger, the
  /// more dangerous the remaining areas become.
  //  void trySpawnMonster() {
  //    if (!rng.oneIn(Option.spawnMonsterChance)) return;
  //
  //    // Try to place a new monster in unexplored areas.
  //    Vec pos = rng.vecInRect(stage.bounds);
  //
  //    public tile = stage[pos];
  //    if (tile.visible || tile.isExplored || !tile.isPassable) return;
  //    if (stage.actorAt(pos) != null) return;
  //
  //    stage.spawnMonster(area.pickBreed(level), pos);
  //  }
}

/// Defines the actual content for the game: the breeds, items, etc. that
/// define the play experience.
public abstract class Content
{
  // TODO: Temp. Figure out where dungeon generator lives.
  // TODO: Using a callback to set the hero position is kind of hokey.
  public abstract IEnumerator buildStage(
      Lore lore, Stage stage, int depth, Action<Vec> placeHero);

  public abstract Affix findAffix(string name);

  public abstract Breed tryFindBreed(string name);

  public abstract ItemType tryFindItem(string name);

  public abstract Skill findSkill(string name);

  public virtual List<Breed> breeds => null;

  public virtual List<HeroClass> classes => null;

  public virtual List<Element> elements => null;

  public virtual List<ItemType> items => null;

  public virtual List<Race> races => null;

  public virtual List<Skill> skills => null;

  public virtual Dictionary<string, Shop> shops => null;

  public abstract HeroSave createHero(string name, Race race = null, HeroClass heroClass = null);

  public abstract Action updateSubstance(Stage stage, Vec pos);
}

/// Each call to [Game.update()] will return a [GameResult] object that tells
/// the UI what happened during that update and what it needs to do.
public class GameResult
{
  /// The "interesting" events that occurred in this update.
  public List<Event> events = new List<Event>();

  /// Whether or not any game state has changed. If this is `false`, then no
  /// game processing has occurred (i.e. the game is stuck waiting for user
  /// input for the [Hero]).
  public bool madeProgress;

  /// Returns `true` if the game state has progressed to the point that a change
  /// should be shown to the user.
  public bool needsRefresh => madeProgress || events.Count > 0;

  public GameResult(bool madeProgress)
  {
    this.madeProgress = madeProgress;
  }
}

/// Describes a single "interesting" thing that occurred during a call to
/// [Game.update()]. In general, events correspond to things that a UI is likely
/// to want to display visually in some form.
public class Event
{
  public EventType type;
  // TODO: Having these all be nullable leads to a lot of "!" in effects.
  // Consider a better way to model this.
  public Actor actor;
  public Element element;
  public object other;
  public Vec pos;
  public Direction dir;

  public Event(EventType type, Actor actor, Element element, Vec pos,
            Direction dir, object other)
  {
    this.type = type;
    this.actor = actor;
    this.element = element;
    this.pos = pos;
    this.dir = dir;
    this.other = other;
  }
}

// TODO: Move to content.
/// A kind of [Event] that has occurred.
public class EventType
{
  public static EventType pause = new EventType("pause");

  /// One step of a bolt.
  public static EventType bolt = new EventType("bolt");

  /// The leading edge of a cone.
  public static EventType cone = new EventType("cone");

  /// A thrown item in flight.
  public static EventType toss = new EventType("toss");

  /// An [Actor] was hit.
  public static EventType hit = new EventType("hit");

  /// An [Actor] died.
  public static EventType die = new EventType("die");

  /// An [Actor] was healed.
  public static EventType heal = new EventType("heal");

  /// Something in the level was detected.
  public static EventType detect = new EventType("detect");

  /// An actor was perceived.
  public static EventType perceive = new EventType("perceive");

  /// A floor tile was magically explored.
  public static EventType map = new EventType("map");

  /// An [Actor] teleported.
  public static EventType teleport = new EventType("teleport");

  /// A new [Actor] was spawned by another.
  public static EventType spawn = new EventType("spawn");

  /// [Actor] has polymorphed into another breed.
  public static EventType polymorph = new EventType("polymorph");

  /// An [Actor] howls.
  public static EventType howl = new EventType("howl");

  /// An [Actor] wakes up.
  public static EventType awaken = new EventType("awaken");

  /// An [Actor] becomes afraid.
  public static EventType frighten = new EventType("frighten");

  /// An [Actor] was blown by wind.
  public static EventType wind = new EventType("wind");

  /// A club's bash attack moves an actor.
  public static EventType knockBack = new EventType("knockBack");

  /// An axe's slash attack hits a tile.
  public static EventType slash = new EventType("slash");

  /// A spear's stab attack hits a tile.
  public static EventType stab = new EventType("stab");

  /// The hero picks up gold worth [Event.other].
  public static EventType gold = new EventType("gold");

  public static EventType openBarrel = new EventType("openBarrel");

  public string _name;

  EventType(string _name)
  {
    this._name = _name;
  }

  public override string ToString() => _name;

  public static bool operator ==(EventType a, EventType b)
  {
    return a._name.Equals(b._name);
  }

  public static bool operator !=(EventType a, EventType b)
  {
    return a._name.Equals(b._name) == false;
  }
  public override bool Equals(object obj)
  {
    if (obj is EventType)
    {
      var evt = obj as EventType;
      return this == evt;
    }
    return false;
  }
  public override int GetHashCode()
  {
    return _name.GetHashCode();
  }
}