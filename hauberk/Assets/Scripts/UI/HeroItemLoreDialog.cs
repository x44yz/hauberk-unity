using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using Mathf = UnityEngine.Mathf;
using UnityTerminal;

class HeroItemLoreDialog : HeroInfoDialog
{
  public const int _rowCount = 11;

  public List<ItemType> _items = new List<ItemType>();
  _Sort _sort = _Sort.type;
  int _selection = 0;
  int _scroll = 0;

  public HeroItemLoreDialog(Content content, HeroSave hero)
      : base(content, hero)
  {
    _listItems();
  }

  public override string name => "Item Lore";

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
      _listItems();
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
          "──────────────────────────────────────────────────── ───── ─────── " +
          "───── ─────",
          color);
    }

    terminal.WriteAt(2, 1, "Items", Hues.gold);
    terminal.WriteAt(20, 1, $"({_sort.description})".PadLeft(34), Hues.darkCoolGray);
    terminal.WriteAt(55, 1, "Depth   Price Found  Used", Hues.coolGray);

    for (var i = 0; i < _rowCount; i++)
    {
      var y = i * 2 + 3;
      writeLine(y + 1, Hues.darkerCoolGray);

      var index = _scroll + i;
      if (index >= _items.Count) continue;
      var item = _items[index];

      var fore = UIHue.text;
      if (index == _selection)
      {
        fore = UIHue.selection;
        terminal.WriteAt(1, y, "►", fore);
      }

      var found = hero.lore.foundItems(item);
      if (found > 0)
      {
        terminal.WriteAt(0, y, item.appearance as Glyph);
        terminal.WriteAt(2, y, item.name, fore);

        terminal.WriteAt(55, y, item.depth.ToString().PadLeft(5), fore);
        terminal.WriteAt(61, y, DartUtils.formatMoney(item.price).PadLeft(7), fore);
        terminal.WriteAt(69, y, found.ToString().PadLeft(5), fore);

        if (item.use != null)
        {
          var used = hero.lore.usedItems(item);
          terminal.WriteAt(75, y, used.ToString().PadLeft(5), fore);
        }
        else
        {
          terminal.WriteAt(75, y, "--".PadLeft(5), fore);
        }
      }
      else
      {
        terminal.WriteAt(
            2, y, $"(undiscovered {_scroll + i + 1})", UIHue.disabled);
      }
    }

    writeLine(2, Hues.darkCoolGray);

    _showItem(terminal, _items[_selection]);
  }

  void _showItem(Terminal terminal, ItemType item)
  {
    terminal = terminal.Rect(0, terminal.height - 15, terminal.width, 14);

    Draw.frame(terminal, 0, 1, 80, terminal.height - 1);
    terminal.WriteAt(1, 0, "┌─┐", Hues.darkCoolGray);
    terminal.WriteAt(1, 1, "╡ ╞", Hues.darkCoolGray);
    terminal.WriteAt(1, 2, "└─┘", Hues.darkCoolGray);

    // TODO: Get working.
    var found = hero.lore.foundItems(item);
    if (found == 0)
    {
      terminal.WriteAt(
          1, 3, "You have not found this item yet.", UIHue.disabled);
      return;
    }

    terminal.WriteAt(2, 1, item.appearance as Glyph);
    terminal.WriteAt(4, 1, item.name, UIHue.selection);

    // TODO: Show item details. Can we reuse the code from item_view.dart?
  }

  void _select(int offset)
  {
    _selection = Mathf.Clamp(_selection + offset, 0, _items.Count - 1);

    // Keep the selected row on screen.
    _scroll = Mathf.Clamp(_scroll, _selection - _rowCount + 1, _selection);
    Dirty();
  }

  void _listItems()
  {
    // Try to keep the current item type selected, if there is one.
    ItemType selectedItem = null;
    if (_items.isNotEmpty())
    {
      selectedItem = _items[_selection];
    }

    _items.Clear();
    _items.AddRange(content.items);

    int compareSort(ItemType a, ItemType b) =>
        a.sortIndex.CompareTo(b.sortIndex);

    int compareDepth(ItemType a, ItemType b) => a.depth.CompareTo(b.depth);

    int comparePrice(ItemType a, ItemType b) => a.price.CompareTo(b.price);

    var comparisons = new List<System.Func<ItemType, ItemType, int>>();
    if (_sort.IsEquals(_Sort.type))
    {
      comparisons.Add(compareSort);
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
    else if (_sort.IsEquals(_Sort.price))
    {
      comparisons.Add(comparePrice);
    }

    // TODO: Price. Damage for weapons, weight, heft, etc.

    _items.Sort((a, b) =>
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
    if (selectedItem != null)
    {
      _selection = _items.IndexOf(selectedItem);

      // TODO
      //      // It may not be found since the unique page doesn't show all breeds.
      //      if (_selection == -1) _selection = 0;
    }
    _select(0);
  }

  class _Sort
  {
    /// The default order they are created in in the content.
    public static _Sort type = new _Sort("ordered by type", "Sort by type");

    /// Sort by depth.
    public static _Sort depth = new _Sort("ordered by depth", "Sort by depth");

    /// Sort alphabetically by name.
    public static _Sort name = new _Sort("ordered by name", "Sort by name");

    /// Sort by price.
    public static _Sort price = new _Sort("ordered by price", "Sort by price");

    public static _Sort[] all = new _Sort[] { type, depth, name, price };

    public string description;
    public string helpText;

    _Sort(string description, string helpText)
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


