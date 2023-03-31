using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

// TODO: Home screen is confusing when empty.
// TODO: The home (get) and shop (buy) screens handle selecting a count
// completely differently from the ItemDialogs (put, sell, etc.). Different
// code and different user interface. Unify those.

abstract class ItemScreen : Screen
{
  public GameScreen _gameScreen;

  // TODO: Move this and _transfer() to an intermediate class instead of making
  // this nullable?
  /// The place items are being transferred to or `null` if this is just a
  /// view.
  public virtual ItemCollection _destination => null;

  /// Whether the shift key is currently pressed.
  public bool _shiftDown = false;

  /// Whether this screen is on top.
  // TODO: Maintaining this manually is hacky. Maybe have malison expose it?
  public bool _isActive = true;

  /// The item currently being inspected or `null` if none.
  public Item _inspected;

  //  /// If the crucible contains a complete recipe, this will be it. Otherwise,
  //  /// this will be `null`.
  //  Recipe completeRecipe;

  string _error;

  public virtual ItemCollection _items => null;

  public HeroSave _save => _gameScreen.game.hero.save;

  public virtual string _headerText => "";

  public virtual Dictionary<string, string> _helpKeys => null;

  public ItemScreen(GameScreen _gameScreen)
  {
    this._gameScreen = _gameScreen;
  }

  public override bool isTransparent => true;

  public static ItemScreen home(GameScreen gameScreen) => new _HomeViewScreen(gameScreen);

  public static ItemScreen shop(GameScreen gameScreen, Inventory shop) =>
      new _ShopViewScreen(gameScreen, shop);

  public virtual bool _canSelectAny => false;
  public virtual bool _showPrices => false;

  public virtual bool _canSelect(Item item)
  {
    if (_shiftDown) return true;

    return canSelect(item);
  }

  public virtual bool canSelect(Item item) => true;

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == InputX.cancel)
    {
      terminal.Pop();
      return true;
    }
    // end of hanleInput

    _error = null;

    if (keyCode == KeyCode.LeftShift)
    {
      _shiftDown = true;
      Dirty();
      return true;
    }

    if (alt) return false;

    //    if (keyCode == KeyCode.space && completeRecipe != null) {
    //      _save.crucible.clear();
    //      completeRecipe.result.spawnDrop(_save.crucible.tryAdd);
    //      refreshRecipe();
    //
    //      // The player probably wants to get the item out of the crucible.
    //      _mode = Mode.get;
    //      dirty();
    //      return true;
    //    }

    if (_shiftDown && keyCode == KeyCode.Escape)
    {
      _inspected = null;
      Dirty();
      return true;
    }

    if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
    {
      var index = keyCode - KeyCode.A;
      if (index >= _items.slots.Count()) return false;
      var item = _items.slots.elementAt(index);
      if (item == null) return false;

      if (_shiftDown)
      {
        _inspected = item;
        Dirty();
      }
      else
      {
        if (!_canSelectAny || !canSelect(item)) return false;

        // Prompt the user for a count if the item is a stack.
        if (item.count > 1)
        {
          _isActive = false;
          terminal.Push(new _CountScreen(_gameScreen, this as _ItemVerbScreen, item));
          return true;
        }

        if (_transfer(item, 1))
        {
          terminal.Pop();
          return true;
        }
      }
    }

    return false;
  }

  public override bool KeyUp(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == KeyCode.LeftShift)
    {
      _shiftDown = false;
      Dirty();
      return true;
    }

    return false;
  }

  public override void Active(Screen popped, object result)
  {
    _isActive = true;
    _inspected = null;

    var countScreen = popped as _CountScreen;
    if (countScreen != null && result != null)
    {
      if (_transfer(countScreen._item, (int)result))
      {
        terminal.Pop();
      }
    }
  }

  public override void Render(Terminal terminal)
  {
    // Don't show the help if another dialog (like buy or sell) is on top with
    // its own help.
    if (_isActive)
    {
      if (_shiftDown)
      {
        var helpKeys = new Dictionary<string, string>(){
              {"A-Z", "Inspect item"},
        };
        if (_inspected != null)
          helpKeys["Esc"] = "Hide inspector";
        Draw.helpKeys(
            terminal,
            helpKeys,
            "Inspect which item?");
      }
      else
      {
        Draw.helpKeys(terminal, _helpKeys, _headerText);
      }
    }

    var view = new _TownItemView(this);
    var width =
        Math.Min(ItemView.preferredWidth, _gameScreen.stagePanel.bounds.width);
    view.render(terminal, _gameScreen.stagePanel.bounds.x,
        _gameScreen.stagePanel.bounds.y, width, _items.length);

    //    if (completeRecipe != null) {
    //      terminal.writeAt(59, 2, "Press [Space] to forge item!", UIHue.selection);
    //
    //      var itemCount = _place.items(this).length;
    //      for (var i = 0; i < completeRecipe.produces.length; i++) {
    //        terminal.writeAt(50, itemCount + i + 4,
    //            completeRecipe.produces.elementAt(i), UIHue.text);
    //      }
    //    }

    if (_error != null)
    {
      terminal.WriteAt(0, 32, _error!, Hues.red);
    }
  }

  /// The default count to move when transferring a stack from [_items].
  public virtual int _initialCount(Item item) => item.count;

  /// The maximum number of items in the stack of [item] that can be
  /// transferred from [_items].
  public virtual int _maxCount(Item item) => item.count;

  /// By default, don't show the price.
  public virtual int? _itemPrice(Item item) => null;

  bool _transfer(Item item, int count)
  {
    var destination = _destination!;
    if (!destination.canAdd(item))
    {
      _error = $"Not enough room for {item.clone(count)}.";
      Dirty();
      return false;
    }

    if (count == item.count)
    {
      // Moving the entire stack.
      destination.tryAdd(item);
      _items.remove(item);
    }
    else
    {
      // Splitting the stack.
      destination.tryAdd(item.splitStack(count));
      _items.countChanged();
    }

    _afterTransfer(item, count);
    // TODO
    //    } else if (_place == _Place.crucible) {
    //      refreshRecipe();
    //    }

    return true;
  }

  /// Called after [count] of [item] has been transferred out of [_items].
  protected virtual void _afterTransfer(Item item, int count) { }
}

/// Base class for item views where the player is performing an action.
abstract class _ItemVerbScreen : ItemScreen
{
  public virtual string _verb => "";

  public _ItemVerbScreen(GameScreen gameScreen) : base(gameScreen)
  {

  }
}

class _TownItemView : ItemView
{
  public ItemScreen _screen;

  public _TownItemView(ItemScreen _screen)
  {
    this._screen = _screen;
  }

  public override HeroSave save => _screen._gameScreen.game.hero.save;

  public override ItemCollection items => _screen._items;

  public override bool capitalize => _screen._shiftDown;

  public override bool showPrices => _screen._showPrices;

  public override Item inspectedItem => _screen._isActive ? _screen._inspected : null;

  public override bool inspectorOnRight => true;

  public override bool canSelectAny => _screen._shiftDown || _screen._canSelectAny;

  public override bool canSelect(Item item) => _screen._canSelect(item);

  public override int? getPrice(Item item) => _screen._itemPrice(item);
}

class _HomeViewScreen : ItemScreen
{
  public override ItemCollection _items => _save.home;

  public override string _headerText => "Welcome home!";

  public override Dictionary<string, string> _helpKeys => new Dictionary<string, string>(){
        {"G", "Get item"},
        {"P", "Put item"},
        {"Shift", "Inspect item"},
        {"Esc", "Leave"}
      };

  public _HomeViewScreen(GameScreen gameScreen) : base(gameScreen)
  {
  }

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (base.KeyDown(keyCode, shift, alt))
      return true;

    if (shift || alt)
      return false;

    if (keyCode == KeyCode.G)
    {
      var screen = new _HomeGetScreen(_gameScreen);
      screen._inspected = _inspected;
      _isActive = false;
      terminal.Push(screen);
      return true;
    }
    else if (keyCode == KeyCode.P)
    {
      _isActive = false;
      terminal.Push(ItemDialog.put(_gameScreen));
      return true;
    }

    return false;
  }
}

/// Screen to items from the hero's home.
class _HomeGetScreen : _ItemVerbScreen
{
  public override string _headerText => "Get which item?";

  public override string _verb => "Get";

  public override Dictionary<string, string> _helpKeys => new Dictionary<string, string>() {
      {"A-Z", "Select item"},
      {"Shift", "Inspect item"},
      {"Esc", "Cancel"}
  };

  public override ItemCollection _items => _gameScreen.game.hero.save.home;

  public override ItemCollection _destination => _gameScreen.game.hero.inventory;

  public _HomeGetScreen(GameScreen gameScreen) : base(gameScreen)
  {
  }

  public override bool _canSelectAny => true;

  public override bool canSelect(Item item) => true;

  protected override void _afterTransfer(Item item, int count)
  {
    _gameScreen.game.log.message($"You {item.clone(count)}.");
    _gameScreen.game.hero.pickUp(item);
  }
}

/// Views the contents of a shop and lets the player choose to buy or sell.
class _ShopViewScreen : ItemScreen
{
  public Inventory _shop;

  public override ItemCollection _items => _shop;

  public override string _headerText => "What can I interest you in?";
  public override bool _showPrices => true;

  public override Dictionary<string, string> _helpKeys => new Dictionary<string, string>(){
        {"B", "Buy item"},
        {"S", "Sell item"},
        {"Shift", "Inspect item"},
        {"Esc", "Cancel"}
      };

  public _ShopViewScreen(GameScreen gameScreen, Inventory _shop) : base(gameScreen)
  {
    this._shop = _shop;
  }

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (base.KeyDown(keyCode, shift: shift, alt: alt)) return true;

    if (shift || alt) return false;

    switch (keyCode)
    {
      case KeyCode.B:
        var screen = new _ShopBuyScreen(_gameScreen, _shop);
        screen._inspected = _inspected;
        _isActive = false;
        terminal.Push(screen);
        break;

      case KeyCode.S:
        _isActive = false;
        terminal.Push(ItemDialog.sell(_gameScreen, _shop));
        return true;
    }

    return false;
  }

  public override int? _itemPrice(Item item) => item.price;
}

/// Screen to buy items from a shop.
class _ShopBuyScreen : _ItemVerbScreen
{
  public Inventory _shop;

  public override string _headerText => "Buy which item?";

  public override string _verb => "Buy";

  public override Dictionary<string, string> _helpKeys => new Dictionary<string, string>() {
      {"A-Z", "Select item"},
      {"Shift", "Inspect item"},
      {"Esc", "Cancel"}
  };

  public override ItemCollection _items => _shop;

  public override ItemCollection _destination => _gameScreen.game.hero.save.inventory;

  public _ShopBuyScreen(GameScreen gameScreen, Inventory _shop) : base(gameScreen)
  {
    this._shop = _shop;
  }

  public override bool _canSelectAny => true;
  public override bool _showPrices => true;

  public override bool canSelect(Item item) => item.price <= _save.gold;

  public override int _initialCount(Item item) => 1;

  /// Don't allow buying more than the hero can afford.
  public override int _maxCount(Item item) => Math.Min(item.count, _save.gold / item.price);

  public override int? _itemPrice(Item item) => item.price;

  /// Pay for purchased item.
  protected override void _afterTransfer(Item item, int count)
  {
    var price = item.price * count;
    _gameScreen.game.log
        .message($"You buy {item.clone(count)} for {price} gold.");
    _save.gold -= price;

    // Acquiring an item may unlock skills.
    // TODO: Would be nice if hero handled this more automatically. Maybe make
    // Inventory and Equipment manage this?
    _gameScreen.game.hero.pickUp(item);
  }
}

/// Screen to let the player choose a count for a selected item.
class _CountScreen : ItemScreen
{
  /// The [_ItemVerbScreen] that pushed this.
  public _ItemVerbScreen _parent;
  public Item _item;
  int _count;

  public override ItemCollection _items => _parent._items;

  public override string _headerText
  {
    get
    {
      var itemText = _item.clone(_count).ToString();
      var price = _parent._itemPrice(_item);
      if (price != null)
      {
        var priceString = DartUtils.formatMoney(price.Value * _count);
        return $"{_parent._verb} {itemText} for {priceString} gold?";
      }
      else
      {
        return $"{_parent._verb} {itemText}?";
      }
    }
  }

  public override Dictionary<string, string> _helpKeys => new Dictionary<string, string>() {
      {"OK", _parent._verb},
      {"↕", "Change quantity"},
      {"Esc", "Cancel"}
  };

  public _CountScreen(GameScreen gameScreen, _ItemVerbScreen _parent, Item _item)
  : base(gameScreen)
  {
    this._parent = _parent;
    this._item = _item;
    _count = _parent._initialCount(_item);
    _inspected = _item;
  }

  public override bool _canSelectAny => true;

  /// Highlight the item the user already selected.
  public override bool canSelect(Item item) => item == _item;

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == InputX.ok)
    {
      terminal.Pop(_count);
      return true;
    }
    else if (keyCode == InputX.cancel)
    {
      terminal.Pop();
      return true;
    }
    else if (keyCode == InputX.n)
    {
      if (_count < _parent._maxCount(_item))
      {
        _count++;
        Dirty();
      }
      return true;
    }
    else if (keyCode == InputX.s)
    {
      if (_count > 1)
      {
        _count--;
        Dirty();
      }
      return true;
    }
    else if (keyCode == InputX.runN && shift)
    {
      _count = _parent._maxCount(_item);
      Dirty();
      return true;
    }
    else if (keyCode == InputX.runS && shift)
    {
      _count = 1;
      Dirty();
      return true;

      // TODO: Allow typing number.
    }
    // en of handleInput

    // Don't allow the shift key to inspect items.
    if (keyCode == KeyCode.LeftShift) return false;

    return base.KeyDown(keyCode, shift: shift, alt: alt);
  }

  public override int? _itemPrice(Item item) => _parent._itemPrice(item);
}
