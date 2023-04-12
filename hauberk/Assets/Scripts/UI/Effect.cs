using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using Mathf = UnityEngine.Mathf;
using UnityTerminal;

// typedef DrawGlyph = void Function(int x, int y, Glyph glyph);

abstract class Effect
{
  // TODO: Effects need to take background color into effect better: should be
  // black when over unexplored tiles, unlit over unlit, etc.

  public static Dictionary<Direction, char> _directionLines = new Dictionary<Direction, char>(){
    {Direction.n, '|'},
    {Direction.ne, '/'},
    {Direction.e, '-'},
    {Direction.se, '\\'},
    {Direction.s, '|'},
    {Direction.sw, '/'},
    {Direction.w, '-'},
    {Direction.nw, '\\'}
  };

  /// Adds an [Effect]s that should be displayed when [event] happens.
  public static void addEffects(List<Effect> effects, Event evt)
  {
    if (evt.type == EventType.pause)
    {
      // Do nothing.
    }
    else if (evt.type == EventType.bolt)
    {
      // TODO: Assumes all none-element bolts are arrows. Do something better?
      if (evt.element == Element.none)
      {
        var chars = new Dictionary<Direction, string>() {
            {Direction.none, "•"},
            {Direction.n, "|"},
            {Direction.ne, "/"},
            {Direction.e, "-"},
            {Direction.se, "\\"},
            {Direction.s, "|"},
            {Direction.sw, "/"},
            {Direction.w, "-"},
            {Direction.nw, "\\"},
          };
        effects.Add(new FrameEffect(evt.pos!, chars[evt.dir], Hues.sandal, life: 2));
      }
      else
      {
        effects.Add(new ElementEffect(evt.pos!, evt.element));
      }
    }

    else if (evt.type == EventType.cone)
    {
      effects.Add(new ElementEffect(evt.pos!, evt.element));
    }

    else if (evt.type == EventType.toss)
    {
      effects.Add(new ItemEffect(evt.pos!, evt.other as Item));
    }

    else if (evt.type == EventType.hit)
    {
      effects
          .Add(new DamageEffect(evt.actor!, evt.element, (int)evt.other));
    }

    else if (evt.type == EventType.die)
    {
      // TODO: Make number of particles vary based on monster health.
      for (var i = 0; i < 10; i++)
      {
        // TODO: Different blood colors for different breeds.
        effects.Add(ParticleEffect.create(evt.actor!.x, evt.actor!.y, Hues.red));
      }
    }

    else if (evt.type == EventType.heal)
    {
      effects.Add(new HealEffect(evt.actor!.pos.x, evt.actor!.pos.y));
    }

    else if (evt.type == EventType.detect)
    {
      effects.Add(new DetectEffect(evt.pos!));
    }

    else if (evt.type == EventType.perceive)
    {
      // TODO: Make look different.
      effects.Add(new DetectEffect(evt.actor!.pos));
    }

    else if (evt.type == EventType.map)
    {
      effects.Add(new MapEffect(evt.pos!));
    }

    else if (evt.type == EventType.teleport)
    {
      var numParticles = (evt.actor!.pos - evt.pos!).kingLength * 2;
      for (var i = 0; i < numParticles; i++)
      {
        effects.Add(TeleportEffect.create(evt.pos!, evt.actor!.pos));
      }
    }

    else if (evt.type == EventType.spawn)
    {
      // TODO: Something more interesting.
      effects.Add(new FrameEffect(evt.actor!.pos, "*", Hues.ash));
    }

    else if (evt.type == EventType.polymorph)
    {
      // TODO: Something more interesting.
      effects.Add(new FrameEffect(evt.actor!.pos, "*", Hues.ash));
    }

    else if (evt.type == EventType.howl)
    {
      effects.Add(new HowlEffect(evt.actor!));
    }

    else if (evt.type == EventType.awaken)
    {
      effects.Add(new BlinkEffect(evt.actor!, new Glyph('!', Hues.ash)));
    }

    else if (evt.type == EventType.frighten)
    {
      effects.Add(new BlinkEffect(evt.actor!, new Glyph('!', Hues.gold)));
    }

    else if (evt.type == EventType.wind)
    {
      // TODO: Do something.
    }

    else if (evt.type == EventType.knockBack)
    {
      // TODO: Something more interesting.
      effects.Add(new FrameEffect(evt.pos!, "*", Hues.buttermilk));
    }

    else if (evt.type == EventType.slash ||
            evt.type == EventType.stab)
    {
      var line = _directionLines[evt.dir]!;

      var color = Hues.ash;
      if (evt.other != null)
      {
        color = (evt.other as Glyph).fore;
      }
      // TODO: If monsters starting using this, we'll need some other way to
      // color it.

      effects.Add(new FrameEffect(evt.pos!, line.ToString(), color));
    }

    else if (evt.type == EventType.gold)
    {
      effects.Add(new TreasureEffect(evt.pos!, evt.other as Item));
    }

    else if (evt.type == EventType.openBarrel)
    {
      effects.Add(new FrameEffect(evt.pos!, "*", Hues.sandal));
    }
  }

  /// Creates a list of [Glyph]s for each combination of [chars] and [colors].
  private static List<Glyph> _glyphs(string chars, List<Color> colors)
  {
    var results = new List<Glyph>();
    for (int i = 0; i < chars.Length; ++i)
    {
      var ch = chars[i];
      foreach (var color in colors)
      {
        results.Add(new Glyph(ch, color));
      }
    }

    return results;
  }

  // TODO: Design custom sprites for these.
  public Dictionary<Element, List<List<Glyph>>> _elementSequences = new Dictionary<Element, List<List<Glyph>>>(){
    {Element.none, new List<List<Glyph>>(){
      _glyphs("•", new List<Color>(){Hues.sandal}),
      _glyphs("•", new List<Color>(){Hues.sandal}),
      _glyphs("•", new List<Color>(){Hues.tan})
    }},
    {Elements.air, new List<List<Glyph>>(){
      _glyphs("Oo", new List<Color>(){Hues.ash, Hues.lightAqua}),
      _glyphs(".", new List<Color>(){Hues.lightAqua}),
      _glyphs(".", new List<Color>(){Hues.lightBlue})
    }},
    {Elements.earth, new List<List<Glyph>>(){
      _glyphs("*%", new List<Color>(){Hues.sandal, Hues.gold}),
      _glyphs("*%", new List<Color>(){Hues.tan, Hues.brown}),
      _glyphs("•*", new List<Color>(){Hues.tan}),
      _glyphs("•", new List<Color>(){Hues.brown})
    }},
    {Elements.fire, new List<List<Glyph>>(){
      _glyphs("▲^", new List<Color>(){Hues.gold, Hues.buttermilk}),
      _glyphs("*^", new List<Color>(){Hues.carrot}),
      _glyphs("^", new List<Color>(){Hues.red}),
      _glyphs("^", new List<Color>(){Hues.brown, Hues.red}),
      _glyphs(".", new List<Color>(){Hues.brown, Hues.red})
    }},
    {Elements.water, new List<List<Glyph>>(){
      _glyphs("Oo", new List<Color>(){Hues.lightAqua, Hues.lightBlue}),
      _glyphs("o•^", new List<Color>(){Hues.lightBlue, Hues.blue}),
      _glyphs("•^", new List<Color>(){Hues.blue, Hues.darkBlue}),
      _glyphs("^~", new List<Color>(){Hues.blue, Hues.darkBlue}),
      _glyphs("~", new List<Color>(){Hues.darkBlue}),
      _glyphs(".", new List<Color>(){Hues.darkBlue, Hues.violet})
    }},
    {Elements.acid, new List<List<Glyph>>(){
      _glyphs("Oo", new List<Color>(){Hues.buttermilk, Hues.gold}),
      _glyphs("o•~", new List<Color>(){Hues.lima, Hues.gold}),
      _glyphs(":,", new List<Color>(){Hues.lima, Hues.olive}),
      _glyphs(".", new List<Color>(){Hues.lima})
    }},
    {Elements.cold, new List<List<Glyph>>(){
      _glyphs("*", new List<Color>(){Hues.ash}),
      _glyphs("+x", new List<Color>(){Hues.lightAqua, Hues.ash}),
      _glyphs("+x", new List<Color>(){Hues.lightBlue, Hues.lightCoolGray}),
      _glyphs(".", new List<Color>(){Hues.coolGray, Hues.darkBlue})
    }},
    {Elements.lightning, new List<List<Glyph>>(){
      _glyphs("*", new List<Color>(){Hues.lilac}),
      _glyphs("-|\\/", new List<Color>(){Hues.purple, Hues.ash}),
      _glyphs(".", new List<Color>(){Hues.darkerCoolGray, Hues.darkerCoolGray, Hues.darkerCoolGray, Hues.lilac})
    }},
    {Elements.poison, new List<List<Glyph>>(){
      _glyphs("Oo", new List<Color>(){Hues.mint, Hues.lima}),
      _glyphs("o•", new List<Color>(){Hues.peaGreen, Hues.peaGreen, Hues.olive}),
      _glyphs("•", new List<Color>(){Hues.sherwood, Hues.olive}),
      _glyphs(".", new List<Color>(){Hues.sherwood})
    }},
    {Elements.dark, new List<List<Glyph>>(){
      _glyphs("*%", new List<Color>(){Hues.darkerCoolGray, Hues.darkerCoolGray, Hues.darkCoolGray}),
      _glyphs("•", new List<Color>(){Hues.darkerCoolGray, Hues.darkerCoolGray, Hues.lightCoolGray}),
      _glyphs(".", new List<Color>(){Hues.darkerCoolGray}),
      _glyphs(".", new List<Color>(){Hues.darkerCoolGray})
    }},
    {Elements.light, new List<List<Glyph>>(){
      _glyphs("*", new List<Color>(){Hues.ash}),
      _glyphs("x+", new List<Color>(){Hues.ash, Hues.buttermilk}),
      _glyphs(":;\"'`,", new List<Color>(){Hues.buttermilk, Hues.gold}),
      _glyphs(".", new List<Color>(){Hues.lightCoolGray, Hues.buttermilk})
    }},
    {Elements.spirit, new List<List<Glyph>>(){
      _glyphs("Oo*+", new List<Color>(){Hues.lilac, Hues.lightCoolGray}),
      _glyphs("o+", new List<Color>(){Hues.purple, Hues.peaGreen}),
      _glyphs("•.", new List<Color>(){Hues.violet, Hues.sherwood, Hues.sherwood})
    }},
  };

  public abstract bool update(Game game);

  public abstract void render(Game game, System.Action<int, int, Glyph> drawGlyph);
}


/// Draws a motionless particle for an [Element] that fades in intensity over
/// time.
class ElementEffect : Effect
{
  public Vec _pos;
  public List<List<Glyph>> _sequence;
  int _age = 0;

  public ElementEffect(Vec _pos, Element element)
  {
    this._pos = _pos;
    _sequence = _elementSequences[element]!;
  }

  public override bool update(Game game)
  {
    if (Rng.rng.oneIn(_age + 2)) _age++;
    return _age < _sequence.Count;
  }

  public override void render(Game game, System.Action<int, int, Glyph> drawGlyph)
  {
    drawGlyph(_pos.x, _pos.y, Rng.rng.item<Glyph>(_sequence[_age]));
  }
}

class FrameEffect : Effect
{
  public Vec pos;
  public string ch;
  public Color color;
  int life;

  public FrameEffect(Vec pos, string ch, Color color, int life = 4)
  {
    this.pos = pos;
    this.ch = ch;
    this.color = color;
    this.life = life;
  }

  public override bool update(Game game)
  {
    if (!game.stage[pos].isVisible) return false;

    return --life >= 0;
  }

  public override void render(Game game, System.Action<int, int, Glyph> drawGlyph)
  {
    drawGlyph(pos.x, pos.y, new Glyph(ch[0], color));
  }
}

/// Draws an [Item] as a given position. Used for thrown items.
class ItemEffect : Effect
{
  public Vec pos;
  public Item item;
  int _life = 2;

  public ItemEffect(Vec pos, Item item)
  {
    this.pos = pos;
    this.item = item;
  }

  public override bool update(Game game)
  {
    if (!game.stage[pos].isVisible) return false;

    return --_life >= 0;
  }

  public override void render(Game game, System.Action<int, int, Glyph> drawGlyph)
  {
    drawGlyph(pos.x, pos.y, item.appearance as Glyph);
  }
}

class DamageEffect : Effect
{
  public Actor actor;
  public Element element;
  public int _blinks;
  int _frame = 0;

  public DamageEffect(Actor actor, Element element, int damage)
  {
    this.actor = actor;
    this.element = element;
    _blinks = (int)Math.Ceiling(Math.Sqrt(damage / 5f));
  }

  public override bool update(Game game) => ++_frame < _blinks * _framesPerBlink;

  public override void render(Game game, System.Action<int, int, Glyph> drawGlyph)
  {
    var frame = _frame % _framesPerBlink;
    if (frame < _framesPerBlink / 2)
    {
      drawGlyph(actor.x, actor.y, new Glyph('*', Hues.elementColor(element)));
    }
  }

  /// Blink faster as the number of blinks increases so that the effect doesn't
  /// get gratuitously long.
  int _framesPerBlink => MathUtils.lerpInt(_blinks, 1, 10, 16, 8);
}

class ParticleEffect : Effect
{
  float x;
  float y;
  float h;
  float v;
  int life;
  public Color color;

  public static ParticleEffect create(float x, float y, Color color)
  {
    var theta = Rng.rng.range(628) / 100f;
    var radius = Rng.rng.range(30, 40) / 100f;

    var h = Math.Cos(theta) * radius;
    var v = Math.Sin(theta) * radius;
    var life = Rng.rng.range(7, 15);
    return new ParticleEffect(x, y, (float)h, (float)v, life, color);
  }

  ParticleEffect(float x, float y, float h, float v, int life, Color color)
  {
    this.x = x;
    this.y = y;
    this.h = h;
    this.v = v;
    this.life = life;
    this.color = color;
  }

  public override bool update(Game game)
  {
    x += h;
    y += v;

    var pos = new Vec((int)x, (int)y);
    if (!game.stage.bounds.contains(pos)) return false;
    if (!game.stage[pos].isFlyable) return false;

    return life-- > 0;
  }

  public override void render(Game game, System.Action<int, int, Glyph> drawGlyph)
  {
    drawGlyph((int)x, (int)y, new Glyph('•', color));
  }
}

/// A particle that starts with a random initial velocity and arcs towards a
/// target.
class TeleportEffect : Effect
{
  float x;
  float y;
  float h;
  float v;
  int age = 0;
  public Vec target;

  public static Color[] _colors = new Color[] { Hues.lightAqua, Hues.lightBlue, Hues.lilac, Hues.ash };

  public static TeleportEffect create(Vec from, Vec target)
  {
    var x = from.x;
    var y = from.y;

    var theta = Rng.rng.range(628) / 100f;
    var radius = Rng.rng.range(10, 80) / 100f;

    var h = Math.Cos(theta) * radius;
    var v = Math.Sin(theta) * radius;

    return new TeleportEffect(x, y, (float)h, (float)v, target);
  }

  public TeleportEffect(float x, float y, float h, float v, Vec target)
  {
    this.x = x;
    this.y = y;
    this.h = h;
    this.v = v;
    this.target = target;
  }

  public override bool update(Game game)
  {
    var friction = 1.0f - age * 0.015f;
    h *= friction;
    v *= friction;

    var pull = age * 0.003f;
    h += (target.x - x) * pull;
    v += (target.y - y) * pull;

    x += h;
    y += v;

    age++;
    return (new Vec((int)x, (int)y) - target) > 1;
  }

  public override void render(Game game, System.Action<int, int, Glyph> drawGlyph)
  {
    var pos = new Vec((int)x, (int)y);
    if (!game.stage.bounds.contains(pos)) return;

    var char_ = _getChar(h, v);
    var color = Rng.rng.item<Color>(_colors);

    drawGlyph(pos.x, pos.y, new Glyph(char_, color));
  }

  /// Chooses a "line" character based on the vector [x], [y]. It will try to
  /// pick a line that follows the vector.
  int _getChar(float x, float y)
  {
    var velocity = new Vec((int)(x * 10), (int)(y * 10));
    if (velocity < 5) return CharCode.bullet;

    var angle = (int)Math.Floor(Math.Atan2(x, y) / (Math.PI * 2) * 16f + 8);
    return "|\\\\--//||\\\\--//||"[angle];
  }
}

class HealEffect : Effect
{
  int x;
  int y;
  int frame = 0;

  public HealEffect(int x, int y)
  {
    this.x = x;
    this.y = y;
  }

  public override bool update(Game game)
  {
    return frame++ < 24;
  }

  public override void render(Game game, System.Action<int, int, Glyph> drawGlyph)
  {
    if (game.stage.get(x, y).isOccluded) return;

    var back = new Color[] { Hues.darkerCoolGray, Hues.aqua, Hues.lightBlue, Hues.lightAqua }[(frame / 4) % 4];

    drawGlyph(x - 1, y, new Glyph('-', back));
    drawGlyph(x + 1, y, new Glyph('-', back));
    drawGlyph(x, y - 1, new Glyph('|', back));
    drawGlyph(x, y + 1, new Glyph('|', back));
  }
}

class DetectEffect : Effect
{
  public static Color[] _colors = new Color[]{
    Hues.ash,
    Hues.buttermilk,
    Hues.gold,
    Hues.olive,
    Hues.darkOlive,
  };

  public Vec pos;
  int life = 20;

  public DetectEffect(Vec pos)
  {
    this.pos = pos;
  }

  public override bool update(Game game) => --life >= 0;

  public override void render(Game game, System.Action<int, int, Glyph> drawGlyph)
  {
    var radius = life / 4;
    var glyph = new Glyph('*', _colors[radius]);

    foreach (var pixel in new Circle(pos, radius).edge)
    {
      drawGlyph(pixel.x, pixel.y, glyph);
    }
  }
}

class MapEffect : Effect
{
  public int _maxLife;

  public Vec pos;
  int life = -1;

  public MapEffect(Vec pos)
  {
    this.pos = pos;
    _maxLife = Rng.rng.range(10, 20);
    life = _maxLife;
  }

  public override bool update(Game game) => --life >= 0;

  public override void render(Game game, System.Action<int, int, Glyph> drawGlyph)
  {
    var glyph = game.stage[pos].type.appearance as Glyph;

    glyph = new Glyph(
        glyph.ch,
        glyph.fore.Blend(Hues.gold, life * 1f / _maxLife),
        glyph.back.Blend(Hues.tan, life * 1f / _maxLife));

    drawGlyph(pos.x, pos.y, glyph);
  }
}

/// Floats a treasure item upward.
class TreasureEffect : Effect
{
  public int _x;
  int _y;
  public Item _item;
  int _life = 8;

  public TreasureEffect(Vec pos, Item _item)
  {
    this._item = _item;
    _x = pos.x;
    _y = pos.y;
  }

  public override bool update(Game game)
  {
    if (_life.isEven())
    {
      _y--;
      if (_y < 0) return false;
    }

    return --_life >= 0;
  }

  public override void render(Game game, System.Action<int, int, Glyph> drawGlyph)
  {
    drawGlyph(_x, _y, _item.appearance as Glyph);
  }
}

class HowlEffect : Effect
{
  static public Glyph bang = new Glyph('!', Hues.aqua);
  static public Glyph slash = new Glyph('/', Hues.lightAqua);
  static public Glyph backslash = new Glyph('\\', Hues.lightAqua);
  static public Glyph dash = new Glyph('-', Hues.aqua);
  static public Glyph less = new Glyph('<', Hues.aqua);
  static public Glyph greater = new Glyph('>', Hues.aqua);

  public Actor _actor;
  int _age = 0;

  public HowlEffect(Actor _actor)
  {
    this._actor = _actor;
  }

  public override bool update(Game game)
  {
    return ++_age < 24;
  }

  public override void render(Game game, System.Action<int, int, Glyph> drawGlyph)
  {
    var pos = _actor.pos;

    if ((_age / 6).isEven())
    {
      drawGlyph(pos.x, pos.y, bang);
      drawGlyph(pos.x - 1, pos.y, greater);
      drawGlyph(pos.x + 1, pos.y, less);
    }
    else
    {
      drawGlyph(pos.x - 1, pos.y - 1, backslash);
      drawGlyph(pos.x - 1, pos.y + 1, slash);
      drawGlyph(pos.x + 1, pos.y - 1, slash);
      drawGlyph(pos.x + 1, pos.y + 1, backslash);
      drawGlyph(pos.x - 1, pos.y, dash);
      drawGlyph(pos.x + 1, pos.y, dash);
    }
  }
}

class BlinkEffect : Effect
{
  public Actor _actor;
  public Glyph _glyph;
  int _age = 0;

  public BlinkEffect(Actor _actor, Glyph _glyph)
  {
    this._actor = _actor;
    this._glyph = _glyph;
  }

  public override bool update(Game game)
  {
    if (!game.stage[_actor.pos].isVisible) return false;

    return ++_age < 24;
  }

  public override void render(Game game, System.Action<int, int, Glyph> drawGlyph)
  {
    var pos = _actor.pos;

    if ((_age / 6).isOdd())
    {
      drawGlyph(pos.x, pos.y, _glyph);
    }
  }
}
