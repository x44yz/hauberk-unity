using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using Mathf = UnityEngine.Mathf;
using UnityTerminal;

class HeroMonsterLoreDialog : HeroInfoDialog
{
  public const int _rowCount = 11;

  public List<Breed> _breeds = new List<Breed>();
  _Sort _sort = _Sort.appearance;
  int _selection = 0;
  int _scroll = 0;

  public HeroMonsterLoreDialog(Content content, HeroSave hero)
      : base(content, hero)
  {
    _listBreeds();
  }

  public override string name => "Monster Lore";

  public override string extraHelp => $"[↕] Scroll, [S] {_sort.next.helpText}";

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {

    if (keyCode == InputX.n)
    {
      _select(-1);
      return true;
    }
    else if (keyCode == InputX.s)
    {
      _select(1);
      return true;
    }
    else if (keyCode == InputX.runN)
    {
      _select(-(_rowCount - 1));
      return true;
    }
    else if (keyCode == InputX.runS)
    {
      _select(_rowCount - 1);
      return true;
    }

    if (!shift && !alt && keyCode == KeyCode.S)
    {
      _sort = _sort.next;
      _listBreeds();
      Dirty();
      return true;
    }

    return base.KeyDown(keyCode, shift: shift, alt: alt);
  }

  public override void Render(Terminal terminal)
  {
    base.Render(terminal);

    void writeLine(int y, Color color)
    {
      terminal.WriteAt(
          2,
          y,
          "──────────────────────────────────────────────────────────── ───── " +
          "───── ─────",
          color);
    }

    terminal.WriteAt(2, 1, "Monsters", Hues.gold);
    terminal.WriteAt(20, 1, $"({_sort.description})".PadLeft(42), Hues.darkCoolGray);
    terminal.WriteAt(63, 1, "Depth Seen Slain", Hues.coolGray);

    for (var i = 0; i < _rowCount; i++)
    {
      var y = i * 2 + 3;
      writeLine(y + 1, Hues.darkerCoolGray);

      var index = _scroll + i;
      if (index >= _breeds.Count) continue;
      var breed = _breeds[index];

      var fore = UIHue.text;
      if (index == _selection)
      {
        fore = UIHue.selection;
        terminal.WriteAt(1, y, "►", fore);
      }

      var seen = hero.lore.seenBreed(breed);
      var slain = hero.lore.slain(breed);
      if (seen > 0)
      {
        terminal.WriteAt(0, y, breed.appearance as Glyph);
        terminal.WriteAt(2, y, breed.name, fore);

        terminal.WriteAt(63, y, breed.depth.ToString().PadLeft(5), fore);
        if (breed.flags.unique)
        {
          terminal.WriteAt(69, y, "Yes".PadLeft(5), fore);
          terminal.WriteAt(75, y, (slain > 0 ? "Yes" : "No").PadLeft(5), fore);
        }
        else
        {
          terminal.WriteAt(69, y, seen.ToString().PadLeft(5), fore);
          terminal.WriteAt(75, y, slain.ToString().PadLeft(5), fore);
        }
      }
      else
      {
        terminal.WriteAt(
            2, y, $"(undiscovered {_scroll + i + 1})", UIHue.disabled);
      }
    }

    writeLine(2, Hues.darkCoolGray);

    _showMonster(terminal, _breeds[_selection]);
  }

  void _showMonster(Terminal terminal, Breed breed)
  {
    terminal = terminal.Rect(0, terminal.height - 15, terminal.width, 14);

    Draw.frame(terminal, 0, 1, 80, terminal.height - 1);
    terminal.WriteAt(1, 0, "┌─┐", Hues.darkCoolGray);
    terminal.WriteAt(1, 1, "╡ ╞", Hues.darkCoolGray);
    terminal.WriteAt(1, 2, "└─┘", Hues.darkCoolGray);

    var seen = hero.lore.seenBreed(breed);
    if (seen == 0)
    {
      terminal.WriteAt(
          1, 3, "You have not seen this breed yet.", UIHue.disabled);
      return;
    }

    terminal.WriteAt(2, 1, breed.appearance as Glyph);
    terminal.WriteAt(4, 1, breed.name, UIHue.selection);

    var y = 3;
    // TODO: Remove this check once all breeds have descriptions.
    if (breed.description != "")
    {
      foreach (var line in Log.wordWrap(terminal.width - 2, breed.description))
      {
        terminal.WriteAt(1, y, line, UIHue.text);
        y++;
      }

      y++;
    }

    var description = _describeBreed(breed);
    foreach (var line in Log.wordWrap(terminal.width - 2, description))
    {
      terminal.WriteAt(1, y, line, UIHue.text);
      y++;
    }
  }

  void _select(int offset)
  {
    _selection = Mathf.Clamp(_selection + offset, 0, _breeds.Count - 1);

    // Keep the selected row on screen.
    _scroll = Mathf.Clamp(_scroll, _selection - _rowCount + 1, _selection);
    Dirty();
  }

  string _describeBreed(Breed breed)
  {
    var sentences = new List<string>();
    var pronoun = breed.pronoun.subjective;
    var lore = hero.lore;

    // TODO: Breed descriptive text.
    // TODO: Multi-color output.

    var noun = "monster";
    if (breed.groups.isNotEmpty())
    {
      // TODO: Handle more than two groups.
      noun = string.Join(" ", breed.groups);
    }

    if (breed.flags.unique)
    {
      if (lore.slain(breed) > 0)
      {
        sentences.Add($"You have slain this unique {noun}.");
      }
      else
      {
        sentences.Add($"You have seen but not slain this unique {noun}.");
      }
    }
    else
    {
      sentences.Add($"You have seen {lore.seenBreed(breed)} and slain " +
          $"{lore.slain(breed)} of this {noun}.");
    }

    sentences.Add($"{pronoun} is worth {breed.experience} experience.");

    if (lore.slain(breed) > 0)
    {
      sentences.Add($"{pronoun} has {breed.maxHealth} health.");
    }

    // TODO: Other stats, moves, attacks, etc.

    return string.Join(" ", sentences
        .Select((sentence) =>
            sentence.Substring(0, 1).ToUpper() + sentence.Substring(1)));
  }

  void _listBreeds()
  {
    // Try to keep the current breed selected, if there is one.
    Breed selectedBreed = null;
    if (_breeds.isNotEmpty())
    {
      selectedBreed = _breeds[_selection];
    }

    _breeds.Clear();

    if (_sort == _Sort.uniques)
    {
      _breeds.AddRange(content.breeds.Where((breed) => breed.flags.unique));
    }
    else
    {
      _breeds.AddRange(content.breeds);
    }

    int compareGlyph(Breed a, Breed b)
    {
      var aChar = (a.appearance as Glyph).ch;
      var bChar = (b.appearance as Glyph).ch;

      bool isUpper(int c) => c >= CharCode.aUpper && c <= CharCode.zUpper;

      // Sort lowercase letters first even though they come later in character
      // code.
      if (isUpper(aChar) && !isUpper(bChar)) return 1;
      if (!isUpper(aChar) && isUpper(bChar)) return -1;

      return aChar.CompareTo(bChar);
    }

    int compareDepth(Breed a, Breed b) => a.depth.CompareTo(b.depth);

    var comparisons = new List<System.Func<Breed, Breed, int>>();
    if (_sort.IsEquals(_Sort.appearance))
    {
      comparisons.Add(compareGlyph);
      comparisons.Add(compareDepth);
    }
    else if (_sort.IsEquals(_Sort.name))
    {
      // No other comparisons.
    }
    else if (_sort.IsEquals(_Sort.depth))
    {
      comparisons.Add(compareDepth);
    }
    else if (_sort.IsEquals(_Sort.uniques))
    {
      comparisons.Add(compareDepth);
    }

    _breeds.Sort((a, b) =>
    {
      foreach (var comparison in comparisons)
      {
        var compare = comparison(a, b);
        if (compare != 0) return compare;
      }

      // Otherwise, sort by name.
      return a.name.ToLower().CompareTo(b.name.ToLower());
    });

    _selection = 0;
    if (selectedBreed != null)
    {
      _selection = _breeds.IndexOf(selectedBreed);

      // It may not be found since the unique page doesn't show all breeds.
      if (_selection == -1) _selection = 0;
    }
    _select(0);
  }

  class _Sort
  {
    /// The default order they are created in in the content.
    public static _Sort appearance =
        new _Sort("ordered by appearance", "Sort by appearance");

    /// Sort by depth.
    public static _Sort depth = new _Sort("ordered by depth", "Sort by depth");

    /// Sort alphabetically by name.
    public static _Sort name = new _Sort("ordered by name", "Sort by name");

    /// Show only uniques.
    public static _Sort uniques = new _Sort("uniques", "Show only uniques");

    public static _Sort[] all = new _Sort[] { appearance, depth, name, uniques };

    public string description;
    public string helpText;

    public _Sort(string description, string helpText)
    {
      this.description = description;
      this.helpText = helpText;
    }

    public _Sort next => all[(Array.IndexOf(all, this) + 1) % all.Length];

    public bool IsEquals(_Sort b)
    {
      return description.Equals(b.description) &&
        helpText.Equals(b.helpText);
    }
  }
}


