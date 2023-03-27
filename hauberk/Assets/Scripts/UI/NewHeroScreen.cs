using System;
using System.Collections.Generic;
using UnityTerminal;
using KeyCode = UnityEngine.KeyCode;
using Mathf = UnityEngine.Mathf;
using System.Linq;

class _Field
{
  public const int name = 0;
  public const int race = 1;
  public const int heroClass = 2;
  public const int count = 3;
}

// TODO: Update to handle resizable UI.
class NewHeroScreen : UnityTerminal.Screen
{

  // From: http://medieval.stormthecastle.com/medieval-names.htm.
  private static string[] _defaultNames = new string[]{
    "Merek",
    "Carac",
    "Ulric",
    "Tybalt",
    "Borin",
    "Sadon",
    "Terrowin",
    "Rowan",
    "Forthwind",
    "Althalos",
    "Fendrel",
    "Brom",
    "Hadrian",
    "Crewe",
    "Bolbec",
    "Fenwick",
    "Mowbray",
    "Drake",
    "Bryce",
    "Leofrick",
    "Letholdus",
    "Lief",
    "Barda",
    "Rulf",
    "Robin",
    "Gavin",
    "Terrin",
    "Jarin",
    "Cedric",
    "Gavin",
    "Josef",
    "Janshai",
    "Doran",
    "Asher",
    "Quinn",
    "Xalvador",
    "Favian",
    "Destrian",
    "Dain",
    "Millicent",
    "Alys",
    "Ayleth",
    "Anastas",
    "Alianor",
    "Cedany",
    "Ellyn",
    "Helewys",
    "Malkyn",
    "Peronell",
    "Thea",
    "Gloriana",
    "Arabella",
    "Hildegard",
    "Brunhild",
    "Adelaide",
    "Beatrix",
    "Emeline",
    "Mirabelle",
    "Helena",
    "Guinevere",
    "Isolde",
    "Maerwynn",
    "Catrain",
    "Gussalen",
    "Enndolynn",
    "Krea",
    "Dimia",
    "Aleida"
  };

  public const int _maxNameLength = 20;

  public Content content;
  public Storage storage;

  int _field = _Field.name;
  string _name = "";
  string _defaultName;
  int _race;
  int _class;

  public NewHeroScreen(Content content, Storage storage)
  {
    this.content = content;
    this.storage = storage;

    _defaultName = Rng.rng.item<string>(_defaultNames);
    _race = Rng.rng.range(content.races.Count);
    _class = Rng.rng.range(content.classes.Count);
  }

  public override void Render(Terminal terminal)
  {
    terminal.Clear();

    _renderName(terminal);
    _renderRace(terminal);
    _renderClass(terminal);
    _renderMenu(terminal);

    var help = new List<string>() { "[Tab] Next field" };
    switch (_field)
    {
      case _Field.name:
        help.Add("[A-Z Del] Edit name");
        break;
      case _Field.race:
        help.Add("[↕] Select race");
        break;
      case _Field.heroClass:
        help.Add("[↕] Select class");
        break;
    }

    help.Add("[Enter] Create hero");
    help.Add("[Esc] Cancel");
    terminal.WriteAt(0, terminal.height - 1, string.Join(", ", help), UIHue.helpText);
  }

  void _renderName(Terminal terminal)
  {
    terminal = terminal.Rect(0, 0, 40, 10);

    TerminalUtils.DrawFrame(terminal, 0, 0, terminal.width, terminal.height,
        _field == _Field.name ? UIHue.selection : Hues.darkCoolGray);

    terminal.WriteAt(1, 0, "Name", _field == _Field.name ? UIHue.selection : UIHue.text);
    terminal.WriteAt(1, 2, "Out of the mists of history, a hero", UIHue.text);
    terminal.WriteAt(1, 3, "appears named...", UIHue.text);

    TerminalUtils.DrawBox(terminal, 2, 5, 23, 3,
        _field == _Field.name ? UIHue.selection : UIHue.disabled);

    if (string.IsNullOrEmpty(_name) == false)
    {
      terminal.WriteAt(3, 6, _name, UIHue.primary);
      if (_field == _Field.name)
      {
        terminal.WriteAt(3 + _name.Length, 6, " ", Color.black, UIHue.selection);
      }
    }
    else
    {
      if (_field == _Field.name)
      {
        terminal.WriteAt(3, 6, _defaultName, Color.black, UIHue.selection);
      }
      else
      {
        terminal.WriteAt(3, 6, _defaultName, UIHue.primary);
      }
    }
  }

  void _renderRace(Terminal terminal)
  {
    terminal = terminal.Rect(0, 10, 40, 29);

    TerminalUtils.DrawFrame(terminal, 0, 0, terminal.width, terminal.height,
        _field == _Field.race ? UIHue.selection : Hues.darkCoolGray);
    terminal.WriteAt(1, 0, "Race", _field == _Field.race ? UIHue.selection : UIHue.text);

    var race = content.races[_race];
    terminal.WriteAt(1, 2, race.name, UIHue.primary);

    var y = 4;
    foreach (var line in Log.wordWrap(38, race.description))
    {
      terminal.WriteAt(1, y, line, UIHue.text);
      y++;
    }

    y = 18;
    foreach (var stat in Stat.all)
    {
      terminal.WriteAt(2, y, stat.name, UIHue.secondary);
      var width = 25 * race.stats[stat]! / 45;
      terminal.WriteAt(12, y, new string(' ', width), Hues.ash, Hues.red);
      terminal.WriteAt(12 + width, y, new string(' ', 25 - width), Hues.ash, Hues.maroon);
      y += 2;
    }
  }

  void _renderClass(Terminal terminal)
  {
    terminal = terminal.Rect(40, 10, 40, 29);

    TerminalUtils.DrawFrame(terminal, 0, 0, terminal.width, terminal.height,
        _field == _Field.heroClass ? UIHue.selection : Hues.darkCoolGray);
    terminal.WriteAt(1, 0, "Class",
        _field == _Field.heroClass ? UIHue.selection : UIHue.text);

    var heroClass = content.classes[_class];
    terminal.WriteAt(1, 2, heroClass.name, UIHue.primary);

    var y = 4;
    foreach (var line in Log.wordWrap(38, heroClass.description))
    {
      terminal.WriteAt(1, y, line, UIHue.text);
      y++;
    }
  }

  void _renderMenu(Terminal terminal)
  {
    terminal = terminal.Rect(40, 0, 40, 10);

    TerminalUtils.DrawFrame(terminal, 0, 0, terminal.width, terminal.height);

    if (_field == _Field.name) return;

    string label = "";
    var items = new List<string>();
    int selected;
    if (_field == _Field.race)
    {
      label = "race";
      items.AddRange(content.races.Select((race) => race.name));
      selected = _race;
    }
    else
    {
      label = "class";
      items.AddRange(content.classes.Select((c) => c.name));
      selected = _class;
    }

    terminal.WriteAt(1, 0, $"Choose a {label}:", UIHue.selection);

    var y = 2;
    for (var i = 0; i < items.Count; i++)
    {
      var item = items[i];
      var isSelected = i == selected;
      terminal.WriteAt(2, y, item, isSelected ? UIHue.selection : UIHue.primary);
      if (isSelected)
      {
        terminal.WriteAt(1, y, "►", UIHue.selection);
      }
      y++;
    }
  }

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == InputX.cancel)
    {
      terminal.Pop();
      return true;
    }

    if (_field == _Field.race)
    {
      if (keyCode == InputX.n)
      {
        _changeRace(-1);
        return true;
      }
      else if (keyCode == InputX.s)
      {
        _changeRace(1);
        return true;
      }
    }
    else if (_field == _Field.heroClass)
    {
      if (keyCode == InputX.n)
      {
        _changeClass(-1);
        return true;
      }
      else if (keyCode == InputX.s)
      {
        _changeClass(1);
        return true;
      }
    }

    if (keyCode == KeyCode.Return)
    {
      var hero = content.createHero(_name.isNotEmpty() ? _name : _defaultName,
          content.races[_race], content.classes[_class]);
      storage.heroes.Add(hero);
      storage.save();
      GameScreen.town(storage, content, hero, (scene)=>{
        terminal.GoTo(scene);
      });
      return true;
    }
    else if (keyCode == KeyCode.Tab)
    {
      if (shift)
      {
        _changeField(-1);
      }
      else
      {
        _changeField(1);
      }
      return true;
    }
    else if (keyCode == KeyCode.Space)
    {
      if (_field == _Field.name)
      {
        // TODO: Handle modifiers.
        _appendToName(" ");
      }
      return true;
    }
    else if (keyCode == KeyCode.Delete)
    {
      if (_field == _Field.name)
      {
        if (_name.isNotEmpty())
        {
          _name = _name.Substring(0, _name.Length - 1);

          // Pick a new default name.
          if (_name.isEmpty())
          {
            _defaultName = Rng.rng.item<string>(_defaultNames);
          }

          Dirty();
        }
      }
      return true;
    }
    else
    {
      if (_field == _Field.name && !alt)
      {
        if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
        {
          var charCode = 'a' + (keyCode - KeyCode.A);
          // TODO: Handle other modifiers.
          if (shift)
          {
            charCode = 'A' + (keyCode - KeyCode.A);
          }
          _appendToName(char.ConvertFromUtf32(charCode));
          return true;
        }
        else if (keyCode >= KeyCode.Alpha0 && keyCode <= KeyCode.Alpha9)
        {
          var charCode = '0' + (keyCode - KeyCode.Alpha0);
          _appendToName(char.ConvertFromUtf32(charCode));
          return true;
        }
      }
    }

    return false;
  }

  void _changeField(int offset)
  {
    _field = (_field + offset + _Field.count) % _Field.count;
    Dirty();
  }

  void _appendToName(string text)
  {
    _name += text;
    if (_name.Length > _maxNameLength)
    {
      _name = _name.Substring(0, _maxNameLength);
    }

    Dirty();
  }

  void _changeRace(int offset)
  {
    var race = Mathf.Clamp(_race + offset, 0, content.races.Count - 1);
    if (race != _race)
    {
      _race = race;
      Dirty();
    }
  }

  void _changeClass(int offset)
  {
    var heroClass = Mathf.Clamp(_class + offset, 0, content.classes.Count - 1);
    if (heroClass != _class)
    {
      _class = heroClass;
      Dirty();
    }
  }
}
