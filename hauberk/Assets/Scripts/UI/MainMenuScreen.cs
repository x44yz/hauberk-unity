using System;
using System.Collections.Generic;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

class MainMenuScreen : UnityTerminal.Screen
{
  string[] _chars = new string[]{
    "______ ______                     _____                          _____",
    "\\ .  / \\  . /                     \\ . |                          \\  .|",
    " | .|   |. |                       | .|                           |. |",
    " |. |___| .|   _____  _____ _____  |. | ___     ______  ____  ___ | .|  ____",
    " |:::___:::|   \\::::\\ \\:::| \\:::|  |::|/:::\\   /::::::\\ \\:::|/:::\\|::| /::/",
    " |xx|   |xx|  ___ \\xx| |xx|  |xx|  |xx|  \\xx\\ |xx|__)xx| |xx|  \\x||xx|/x/",
    " |xx|   |xx| /xxx\\|xx| |xx|  |xx|  |xx|   |xx||xx|\\xxxx| |xx|     |xxxxxx\\",
    " |XX|   |XX||XX(__|XX| |XX\\__|XX|  |XX|__/XXX||XX|_____  |XX|     |XX| \\XX\\_",
    " |XX|   |XX| \\XXXX/\\XX\\ \\XXX/|XXX\\/XXX/\\XXXX/  \\XXXXXX/ /XXXX\\  /XXXX\\ \\XXX\\",
    " |XX|   |XX|_________________________________________________________________",
    " |XX|   |XX||XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX\\",
    "_|XX|   |XX|_",
    "\\XXX|   |XXX/",
    " \\XX|   |XX/",
    "  \\X|   |X/",
    "   \\|   |/",
  };

  string[] _charColors = new string[]{
    "LLLLLL LLLLLL                     LLLLL                          LLLLL",
    "ERRRRE ERRRRE                     ERRRE                          ERRRE",
    " ERRE   ERRE                       ERRE                           ERRE",
    " ERRELLLERRE   LLLLL  LLLLL LLLLL  ERRE LLL     LLLLLL  LLLL  LLL ERRE  LLLL",
    " ERRREEERRRE   ERRRRL ERRRE ERRRE  ERREERRRL   LRRRRRRL ERRRLLRRRLERRE LRRE",
    " ERRE   ERRE  LLL ERRE ERRE  ERRE  ERRE  ERRL ERRELLERRE ERRE  EREERRELRE",
    " EOOE   EOOE LOOOEEOOE EOOE  EOOE  EOOE   EOOEEOOEEOOOOE EOOE     EOOOOOOL",
    " EGGE   EGGEEGGELLEGGE EGGLLLEGGE  EGGELLLGGGEEGGELLLLL  EGGE     EGGE EGGLL",
    " EYYE   EYYE EYYYYEEYYE EYYYEEYYYLLYYYEEYYYYE  EYYYYYYE LYYYYL   LYYYYL EYYYL",
    " EYYE   EYYELLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLLL",
    " EYYE   EYYEEYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYYL",
    "LEYYE   EYYEL",
    "EYYYE   EYYYE",
    " EYYE   EYYE",
    "  EYE   EYE",
    "   EE   EE",
  };

  Dictionary<string, Color> _colors = new Dictionary<string, Color>(){
    {"L", Hues.lightWarmGray},
    {"E", Hues.warmGray},
    {"R", Hues.red},
    {"O", Hues.carrot},
    {"G", Hues.gold},
    {"Y", Hues.yellow}
  };

  public Content content;
  public Storage storage;
  int selectedHero = 0;

  public MainMenuScreen(Content content)
  {
    this.content = content;
    storage = new Storage(content);
  }

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == InputX.n)
    {
      _changeSelection(-1);
      return true;
    }
    else if (keyCode == InputX.s)
    {
      _changeSelection(1);
      return true;
    }
    else if (keyCode == KeyCode.L)
    {
      if (selectedHero < storage.heroes.Count)
      {
        var save = storage.heroes[selectedHero];
        GameScreen.town(storage, content, save, (scene)=>{
          terminal.Push(scene);
        });
      }
      return true;
    }

    if (shift || alt) return false;

    if (keyCode == KeyCode.N)
    {
      terminal.Push(new NewHeroScreen(content, storage));
      return true;
    }
    else if (keyCode == KeyCode.D)
    {
      if (selectedHero < storage.heroes.Count)
      {
        var name = storage.heroes[selectedHero].name;
        terminal.Push(
            new ConfirmPopup($"Are you sure you want to delete {name}?", "delete"));
      }
      return true;
    }
    return false;
  }

  public override void Active(UnityTerminal.Screen popped, object result)
  {
    if (popped is ConfirmPopup && result.Equals("delete"))
    {
      storage.heroes.RemoveAt(selectedHero);
      if (selectedHero > 0 && selectedHero >= storage.heroes.Count)
      {
        selectedHero--;
      }
      storage.save();
      Dirty();
    }
  }

  public override void Render(Terminal terminal)
  {
    base.Render(terminal);

    // Center everything horizontally.
    var pterminal = terminal.Rect((terminal.width - 78) / 2, 0, 80, terminal.height);

    pterminal.WriteAt(0,
        pterminal.height - 1,
        "[L] Select a hero, [↕] Change selection, [N] Create a new hero, [D] Delete hero",
        UIHue.helpText);

    // Center the content vertically.
    pterminal = pterminal.Rect(0, (pterminal.height - 40) / 2, pterminal.width, 40);
    for (var y = 0; y < _chars.Length; y++)
    {
      for (var x = 0; x < _chars[y].Length; x++)
      {
        if (_chars[y][x] == CharCode.space)
          continue;
        var k = _charColors[y][x].ToString();
        if (_colors.ContainsKey(k) == false)
        {
          // UnityEngine.Debug.LogWarning($"not exit key > {y} {x} > " + k + " - " + _chars[y][x].ToString());
          continue;
        }
        var color = _colors[k];
        pterminal.WriteAt(x + 1, y + 1, _chars[y][x], color);
      }
    }

    pterminal.WriteAt(10, 18, "Which hero shall you play?", UIHue.text);

    if (storage.heroes.Count == 0)
    {
      pterminal.WriteAt(10, 20, "(No heroes. Please create a new one.)", UIHue.helpText);
    }

    for (var i = 0; i < storage.heroes.Count; i++)
    {
      var hero = storage.heroes[i];

      var primary = UIHue.primary;
      var secondary = UIHue.secondary;
      if (i == selectedHero)
      {
        primary = UIHue.selection;
        secondary = UIHue.selection;

        pterminal.WriteAt(9, 20 + i, CharCode.blackRightPointingPointer, UIHue.selection);
      }

      pterminal.WriteAt(10, 20 + i, hero.name, primary);
      pterminal.WriteAt(30, 20 + i, $"Level {hero.level}", secondary);
      pterminal.WriteAt(40, 20 + i, hero.race.name, secondary);
      pterminal.WriteAt(50, 20 + i, hero.heroClass.name, secondary);
    }
  }

  void _changeSelection(int offset)
  {
    selectedHero = (selectedHero + offset) % storage.heroes.Count;
    Dirty();
  }
}
