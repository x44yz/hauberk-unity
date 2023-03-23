using System;
using System.Collections.Generic;
using System.Linq;
using Color = UnityEngine.Color;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;



// typedef DrawGlyph = void Function(int x, int y, Glyph glyph);

abstract class Effect {
// TODO: Effects need to take background color into effect better: should be
// black when over unexplored tiles, unlit over unlit, etc.

  public static Dictionary<Direction, char> _directionLines = new Dictionary<Direction, char>(){
    {Direction.n, "|"},
    {Direction.ne, "/"},
    {Direction.e, "-"},
    {Direction.se, "\\"},
    {Direction.s, "|"},
    {Direction.sw, "/"},
    {Direction.w, "-"},
    {Direction.nw, "\\"}
  };

  /// Adds an [Effect]s that should be displayed when [event] happens.
  public static void addEffects(List<Effect> effects, Event evt) {
    switch (evt.type) {
      case EventType.pause:
        // Do nothing.
        break;

      case EventType.bolt:
        // TODO: Assumes all none-element bolts are arrows. Do something better?
        if (evt.element == Element.none) {
          var chars = new Dictionary<Direction, char>() {
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
          effects.add(FrameEffect(evt.pos!, chars[evt.dir], sandal, life: 2));
        } else {
          effects.add(ElementEffect(evt.pos!, evt.element));
        }
        break;

      case EventType.cone:
        effects.add(ElementEffect(evt.pos!, evt.element));
        break;

      case EventType.toss:
        effects.add(ItemEffect(evt.pos!, evt.other as Item));
        break;

      case EventType.hit:
        effects
            .add(DamageEffect(evt.actor!, evt.element, evt.other as int));
        break;

      case EventType.die:
        // TODO: Make number of particles vary based on monster health.
        for (var i = 0; i < 10; i++) {
          // TODO: Different blood colors for different breeds.
          effects.add(ParticleEffect(evt.actor!.x, evt.actor!.y, red));
        }
        break;

      case EventType.heal:
        effects.add(HealEffect(evt.actor!.pos.x, evt.actor!.pos.y));
        break;

      case EventType.detect:
        effects.add(DetectEffect(evt.pos!));
        break;

      case EventType.perceive:
        // TODO: Make look different.
        effects.add(DetectEffect(evt.actor!.pos));
        break;

      case EventType.map:
        effects.add(MapEffect(evt.pos!));
        break;

      case EventType.teleport:
        var numParticles = (evt.actor!.pos - evt.pos!).kingLength * 2;
        for (var i = 0; i < numParticles; i++) {
          effects.add(TeleportEffect(evt.pos!, evt.actor!.pos));
        }
        break;

      case EventType.spawn:
        // TODO: Something more interesting.
        effects.add(FrameEffect(evt.actor!.pos, '*', ash));
        break;

      case EventType.polymorph:
        // TODO: Something more interesting.
        effects.add(FrameEffect(evt.actor!.pos, '*', ash));
        break;

      case EventType.howl:
        effects.add(HowlEffect(evt.actor!));
        break;

      case EventType.awaken:
        effects.add(BlinkEffect(evt.actor!, Glyph('!', ash)));
        break;

      case EventType.frighten:
        effects.add(BlinkEffect(evt.actor!, Glyph("!", gold)));
        break;

      case EventType.wind:
        // TODO: Do something.
        break;

      case EventType.knockBack:
        // TODO: Something more interesting.
        effects.add(FrameEffect(evt.pos!, "*", buttermilk));
        break;

      case EventType.slash:
      case EventType.stab:
        var line = _directionLines[evt.dir]!;

        var color = ash;
        if (evt.other != null) {
          color = (evt.other as Glyph).fore;
        }
        // TODO: If monsters starting using this, we'll need some other way to
        // color it.

        effects.add(FrameEffect(evt.pos!, line, color));
        break;

      case EventType.gold:
        effects.add(TreasureEffect(evt.pos!, evt.other as Item));
        break;

      case EventType.openBarrel:
        effects.add(FrameEffect(evt.pos!, '*', sandal));
        break;
    }
  }

  /// Creates a list of [Glyph]s for each combination of [chars] and [colors].
  public static List<Glyph> _glyphs(string chars, List<Color> colors) {
    var results = new List<Glyph>();
    for (int i = 0; i < chars.Length; ++i) {
      var ch = chars[i];
      foreach (var color in colors) {
        results.add(new Glyph(ch, color));
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
      _glyphs("x+", new List<Color>(){Hues.ash, buttermilk}),
      _glyphs(":;\"'`,", new List<Color>(){Hues.buttermilk, gold}),
      _glyphs(".", new List<Color>(){Hues.lightCoolGray, buttermilk})
    }},
    {Elements.spirit, new List<List<Glyph>>(){
      _glyphs("Oo*+", new List<Color>(){Hues.lilac, lightCoolGray}),
      _glyphs("o+", new List<Color>(){Hues.purple, peaGreen}),
      _glyphs("•.", new List<Color>(){Hues.violet, sherwood, sherwood})
    }},
  };

  public abstract bool update(Game game);

  public abstract void render(Game game, DrawGlyph drawGlyph);
}


/// Draws a motionless particle for an [Element] that fades in intensity over
/// time.
class ElementEffect : Effect {
  public Vec _pos;
  public List<List<Glyph>> _sequence;
  int _age = 0;

  ElementEffect(Vec _pos, Element element)
  {
    this._pos = _pos;
    _sequence = _elementSequences[element]!;
  }

  public override bool update(Game game) {
    if (Rng.rng.oneIn(_age + 2)) _age++;
    return _age < _sequence.Count;
  }

  public override void render(Game game, DrawGlyph drawGlyph) {
    drawGlyph(_pos.x, _pos.y, Rng.rng.item<Glyph>(_sequence[_age]));
  }
}

class FrameEffect : Effect {
  final Vec pos;
  final String char;
  final Color color;
  int life;

  FrameEffect(this.pos, this.char, this.color, {this.life = 4});

  bool update(Game game) {
    if (!game.stage[pos].isVisible) return false;

    return --life >= 0;
  }

  void render(Game game, DrawGlyph drawGlyph) {
    drawGlyph(pos.x, pos.y, Glyph(char, color));
  }
}

/// Draws an [Item] as a given position. Used for thrown items.
class ItemEffect implements Effect {
  final Vec pos;
  final Item item;
  int _life = 2;

  ItemEffect(this.pos, this.item);

  bool update(Game game) {
    if (!game.stage[pos].isVisible) return false;

    return --_life >= 0;
  }

  void render(Game game, DrawGlyph drawGlyph) {
    drawGlyph(pos.x, pos.y, item.appearance as Glyph);
  }
}

class DamageEffect implements Effect {
  final Actor actor;
  final Element element;
  final int _blinks;
  int _frame = 0;

  DamageEffect(this.actor, this.element, int damage)
      : _blinks = math.sqrt(damage / 5).ceil();

  bool update(Game game) => ++_frame < _blinks * _framesPerBlink;

  void render(Game game, DrawGlyph drawGlyph) {
    var frame = _frame % _framesPerBlink;
    if (frame < _framesPerBlink ~/ 2) {
      drawGlyph(actor.x, actor.y, Glyph("*", elementColor(element)));
    }
  }

  /// Blink faster as the number of blinks increases so that the effect doesn't
  /// get gratuitously long.
  int get _framesPerBlink => lerpInt(_blinks, 1, 10, 16, 8);
}

class ParticleEffect implements Effect {
  num x;
  num y;
  num h;
  num v;
  int life;
  final Color color;

  factory ParticleEffect(num x, num y, Color color) {
    var theta = rng.range(628) / 100;
    var radius = rng.range(30, 40) / 100;

    var h = math.cos(theta) * radius;
    var v = math.sin(theta) * radius;
    var life = rng.range(7, 15);
    return ParticleEffect._(x, y, h, v, life, color);
  }

  ParticleEffect._(this.x, this.y, this.h, this.v, this.life, this.color);

  bool update(Game game) {
    x += h;
    y += v;

    var pos = Vec(x.toInt(), y.toInt());
    if (!game.stage.bounds.contains(pos)) return false;
    if (!game.stage[pos].isFlyable) return false;

    return life-- > 0;
  }

  void render(Game game, DrawGlyph drawGlyph) {
    drawGlyph(x.toInt(), y.toInt(), Glyph('•', color));
  }
}

/// A particle that starts with a random initial velocity and arcs towards a
/// target.
class TeleportEffect implements Effect {
  num x;
  num y;
  num h;
  num v;
  int age = 0;
  final Vec target;

  static final _colors = [lightAqua, lightBlue, lilac, ash];

  factory TeleportEffect(Vec from, Vec target) {
    var x = from.x;
    var y = from.y;

    var theta = rng.range(628) / 100;
    var radius = rng.range(10, 80) / 100;

    var h = math.cos(theta) * radius;
    var v = math.sin(theta) * radius;

    return TeleportEffect._(x, y, h, v, target);
  }

  TeleportEffect._(this.x, this.y, this.h, this.v, this.target);

  bool update(Game game) {
    var friction = 1.0 - age * 0.015;
    h *= friction;
    v *= friction;

    var pull = age * 0.003;
    h += (target.x - x) * pull;
    v += (target.y - y) * pull;

    x += h;
    y += v;

    age++;
    return (Vec(x.toInt(), y.toInt()) - target) > 1;
  }

  void render(Game game, DrawGlyph drawGlyph) {
    var pos = Vec(x.toInt(), y.toInt());
    if (!game.stage.bounds.contains(pos)) return;

    var char = _getChar(h, v);
    var color = rng.item(_colors);

    drawGlyph(pos.x, pos.y, Glyph.fromCharCode(char, color));
  }

  /// Chooses a "line" character based on the vector [x], [y]. It will try to
  /// pick a line that follows the vector.
  int _getChar(num x, num y) {
    var velocity = Vec((x * 10).toInt(), (y * 10).toInt());
    if (velocity < 5) return CharCode.bullet;

    var angle = math.atan2(x, y) / (math.pi * 2) * 16 + 8;
    return r"|\\--//||\\--//||".codeUnitAt(angle.floor());
  }
}

class HealEffect implements Effect {
  int x;
  int y;
  int frame = 0;

  HealEffect(this.x, this.y);

  bool update(Game game) {
    return frame++ < 24;
  }

  void render(Game game, DrawGlyph drawGlyph) {
    if (game.stage.get(x, y).isOccluded) return;

    var back = [darkerCoolGray, aqua, lightBlue, lightAqua][(frame ~/ 4) % 4];

    drawGlyph(x - 1, y, Glyph('-', back));
    drawGlyph(x + 1, y, Glyph('-', back));
    drawGlyph(x, y - 1, Glyph('|', back));
    drawGlyph(x, y + 1, Glyph('|', back));
  }
}

class DetectEffect implements Effect {
  static final _colors = [
    ash,
    buttermilk,
    gold,
    olive,
    darkOlive,
  ];

  final Vec pos;
  int life = 20;

  DetectEffect(this.pos);

  bool update(Game game) => --life >= 0;

  void render(Game game, DrawGlyph drawGlyph) {
    var radius = life ~/ 4;
    var glyph = Glyph("*", _colors[radius]);

    for (var pixel in Circle(pos, radius).edge) {
      drawGlyph(pixel.x, pixel.y, glyph);
    }
  }
}

class MapEffect implements Effect {
  final _maxLife = rng.range(10, 20);

  final Vec pos;
  int life = -1;

  MapEffect(this.pos) {
    life = _maxLife;
  }

  bool update(Game game) => --life >= 0;

  void render(Game game, DrawGlyph drawGlyph) {
    var glyph = game.stage[pos].type.appearance as Glyph;

    glyph = Glyph.fromCharCode(
        glyph.char,
        glyph.fore.blend(gold, life / _maxLife),
        glyph.back.blend(tan, life / _maxLife));

    drawGlyph(pos.x, pos.y, glyph);
  }
}

/// Floats a treasure item upward.
class TreasureEffect implements Effect {
  final int _x;
  int _y;
  final Item _item;
  int _life = 8;

  TreasureEffect(Vec pos, this._item)
      : _x = pos.x,
        _y = pos.y;

  bool update(Game game) {
    if (_life.isEven) {
      _y--;
      if (_y < 0) return false;
    }

    return --_life >= 0;
  }

  void render(Game game, DrawGlyph drawGlyph) {
    drawGlyph(_x, _y, _item.appearance as Glyph);
  }
}

class HowlEffect implements Effect {
  static final bang = Glyph("!", aqua);
  static final slash = Glyph("/", lightAqua);
  static final backslash = Glyph("\\", lightAqua);
  static final dash = Glyph("-", aqua);
  static final less = Glyph("<", aqua);
  static final greater = Glyph(">", aqua);

  final Actor _actor;
  int _age = 0;

  HowlEffect(this._actor);

  bool update(Game game) {
    return ++_age < 24;
  }

  void render(Game game, DrawGlyph drawGlyph) {
    var pos = _actor.pos;

    if ((_age ~/ 6).isEven) {
      drawGlyph(pos.x, pos.y, bang);
      drawGlyph(pos.x - 1, pos.y, greater);
      drawGlyph(pos.x + 1, pos.y, less);
    } else {
      drawGlyph(pos.x - 1, pos.y - 1, backslash);
      drawGlyph(pos.x - 1, pos.y + 1, slash);
      drawGlyph(pos.x + 1, pos.y - 1, slash);
      drawGlyph(pos.x + 1, pos.y + 1, backslash);
      drawGlyph(pos.x - 1, pos.y, dash);
      drawGlyph(pos.x + 1, pos.y, dash);
    }
  }
}

class BlinkEffect implements Effect {
  final Actor _actor;
  final Glyph _glyph;
  int _age = 0;

  BlinkEffect(this._actor, this._glyph);

  bool update(Game game) {
    if (!game.stage[_actor.pos].isVisible) return false;

    return ++_age < 24;
  }

  void render(Game game, DrawGlyph drawGlyph) {
    var pos = _actor.pos;

    if ((_age ~/ 6).isOdd) {
      drawGlyph(pos.x, pos.y, _glyph);
    }
  }
}
