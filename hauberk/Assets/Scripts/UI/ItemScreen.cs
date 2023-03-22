using System;
using System.Collections.Generic;
using System.Linq;
using Color = UnityEngine.Color;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

// TODO: Home screen is confusing when empty.
// TODO: The home (get) and shop (buy) screens handle selecting a count
// completely differently from the ItemDialogs (put, sell, etc.). Different
// code and different user interface. Unify those.

abstract class ItemScreen : Screen {
  public GameScreen _gameScreen;

  // TODO: Move this and _transfer() to an intermediate class instead of making
  // this nullable?
  /// The place items are being transferred to or `null` if this is just a
  /// view.
  ItemCollection? _destination => null;

  /// Whether the shift key is currently pressed.
  bool _shiftDown = false;

  /// Whether this screen is on top.
  // TODO: Maintaining this manually is hacky. Maybe have malison expose it?
  bool _isActive = true;

  /// The item currently being inspected or `null` if none.
  Item? _inspected;

//  /// If the crucible contains a complete recipe, this will be it. Otherwise,
//  /// this will be `null`.
//  Recipe completeRecipe;

  string? _error;

  ItemCollection _items;

  HeroSave _save => _gameScreen.game.hero.save;

  string _headerText;

  public virtual Dictionary<string, string> _helpKeys => null;

  public ItemScreen(GameScreen _gameScreen)
  {
    this._gameScreen = _gameScreen;
  }

  public override bool isTransparent => true;

  public static ItemScreen home(GameScreen gameScreen) => new _HomeViewScreen(gameScreen);

  public static ItemScreen shop(GameScreen gameScreen, Inventory shop) =>
      new _ShopViewScreen(gameScreen, shop);

  bool _canSelectAny => false;
  bool _showPrices => false;

  bool _canSelect(Item item) {
    if (_shiftDown) return true;

    return canSelect(item);
  }

  bool canSelect(Item item) => true;

  public override bool HandleInput() {
    _error = null;

    bool shift = Input.GetKey(KeyCode.LeftShift);
    bool alt = Input.GetKey(KeyCode.LeftAlt);

    if (Input.GetKeyDown(InputX.cancel)) {
      terminal.Pop();
      return true;
    }
    else if (Input.anyKeyDown)
    {

    }

    return false;
  }

  // bool keyDown(int keyCode, {required bool shift, required bool alt}) {
  //   _error = null;

  //   if (keyCode == KeyCode.shift) {
  //     _shiftDown = true;
  //     dirty();
  //     return true;
  //   }

  //   if (alt) return false;


  //   if (_shiftDown && keyCode == KeyCode.escape) {
  //     _inspected = null;
  //     dirty();
  //     return true;
  //   }

  //   if (keyCode >= KeyCode.a && keyCode <= KeyCode.z) {
  //     var index = keyCode - KeyCode.a;
  //     if (index >= _items.slots.length) return false;
  //     var item = _items.slots.elementAt(index);
  //     if (item == null) return false;

  //     if (_shiftDown) {
  //       _inspected = item;
  //       dirty();
  //     } else {
  //       if (!_canSelectAny || !canSelect(item)) return false;

  //       // Prompt the user for a count if the item is a stack.
  //       if (item.count > 1) {
  //         _isActive = false;
  //         ui.push(_CountScreen(_gameScreen, this as _ItemVerbScreen, item));
  //         return true;
  //       }

  //       if (_transfer(item, 1)) {
  //         ui.pop();
  //         return true;
  //       }
  //     }
  //   }

  //   return false;
  // }

  // bool keyUp(int keyCode, {required bool shift, required bool alt}) {
  //   if (keyCode == KeyCode.shift) {
  //     _shiftDown = false;
  //     dirty();
  //     return true;
  //   }

  //   return false;
  // }

  public override void Active(Screen popped, object result) {
    _isActive = true;
    _inspected = null;

    var countScreen = popped as _CountScreen;
    if (countScreen != null && result != null) {
      if (_transfer(countScreen._item, (int)result)) {
        terminal.Pop();
      }
    }
  }

  public override void Render() {
    // Don't show the help if another dialog (like buy or sell) is on top with
    // its own help.
    if (_isActive) {
      if (_shiftDown) {
        var helpKeys = new Dictionary<string, string>(){
              {"A-Z", "Inspect item"},
        };
        if (_inspected != null) 
          helpKeys["Esc"] = "Hide inspector";
        Draw.helpKeys(
            terminal,
            helpKeys,
            "Inspect which item?");
      } else {
        Draw.helpKeys(terminal, _helpKeys, _headerText);
      }
    }

    var view = _TownItemView(this);
    var width =
        math.min(ItemView.preferredWidth, _gameScreen.stagePanel.bounds.width);
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

    if (_error != null) {
      terminal.WriteAt(0, 32, _error!, red);
    }
  }

  /// The default count to move when transferring a stack from [_items].
  int _initialCount(Item item) => item.count;

  /// The maximum number of items in the stack of [item] that can be
  /// transferred from [_items].
  int _maxCount(Item item) => item.count;

  /// By default, don't show the price.
  int? _itemPrice(Item item) => null;

  bool _transfer(Item item, int count) {
    var destination = _destination!;
    if (!destination.canAdd(item)) {
      _error = "Not enough room for ${item.clone(count)}.";
      Dirty();
      return false;
    }

    if (count == item.count) {
      // Moving the entire stack.
      destination.tryAdd(item);
      _items.remove(item);
    } else {
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
  void _afterTransfer(Item item, int count) {}
}

/// Base class for item views where the player is performing an action.
abstract class _ItemVerbScreen : ItemScreen {
  public virtual string _verb => "";

  public _ItemVerbScreen(GameScreen gameScreen) : base(gameScreen)
  {

  }
}

class _TownItemView : ItemView {
  public ItemScreen _screen;

  _TownItemView(this._screen);

  HeroSave save => _screen._gameScreen.game.hero.save;

  ItemCollection items => _screen._items;

  bool capitalize => _screen._shiftDown;

  bool showPrices => _screen._showPrices;

  Item? inspectedItem => _screen._isActive ? _screen._inspected : null;

  bool inspectorOnRight => true;

  bool canSelectAny => _screen._shiftDown || _screen._canSelectAny;

  bool canSelect(Item item) => _screen._canSelect(item);

  int? getPrice(Item item) => _screen._itemPrice(item);
}

class _HomeViewScreen : ItemScreen {
  ItemCollection _items => _save.home;

  string _headerText => "Welcome home!";

  public override Dictionary<string, string> _helpKeys => new Dictionary<string, string>(){
        {"G", "Get item"},
        {"P", "Put item"},
        {"Shift", "Inspect item"},
        {"Esc", "Leave"}
      };

  public _HomeViewScreen(GameScreen gameScreen) : base(gameScreen)
  {
  }

  public override void HandleInput()
  {
    
  }

  bool keyDown(int keyCode, {required bool shift, required bool alt}) {
    if (super.keyDown(keyCode, shift: shift, alt: alt)) return true;

    if (shift || alt) return false;

    switch (keyCode) {
      case KeyCode.g:
        var screen = _HomeGetScreen(_gameScreen);
        screen._inspected = _inspected;
        _isActive = false;
        ui.push(screen);
        return true;

      case KeyCode.p:
        _isActive = false;
        ui.push(ItemDialog.put(_gameScreen));
        return true;
    }

    return false;
  }
}

/// Screen to items from the hero's home.
class _HomeGetScreen extends _ItemVerbScreen {
  string _headerText => "Get which item?";

  string _verb => "Get";

  Map<string, string> _helpKeys =>
      {"A-Z": "Select item", "Shift": "Inspect item", "Esc": "Cancel"};

  ItemCollection _items => _gameScreen.game.hero.save.home;

  ItemCollection _destination => _gameScreen.game.hero.inventory;

  _HomeGetScreen(GameScreen gameScreen) : super(gameScreen);

  bool _canSelectAny => true;

  bool canSelect(Item item) => true;

  void _afterTransfer(Item item, int count) {
    _gameScreen.game.log.message("You ${item.clone(count)}.");
    _gameScreen.game.hero.pickUp(item);
  }
}

/// Views the contents of a shop and lets the player choose to buy or sell.
class _ShopViewScreen : ItemScreen {
  public Inventory _shop;

  ItemCollection _items => _shop;

  string _headerText => "What can I interest you in?";
  bool _showPrices => true;

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

  bool keyDown(int keyCode, {required bool shift, required bool alt}) {
    if (super.keyDown(keyCode, shift: shift, alt: alt)) return true;

    if (shift || alt) return false;

    switch (keyCode) {
      case KeyCode.b:
        var screen = _ShopBuyScreen(_gameScreen, _shop);
        screen._inspected = _inspected;
        _isActive = false;
        ui.push(screen);
        break;

      case KeyCode.s:
        _isActive = false;
        ui.push(ItemDialog.sell(_gameScreen, _shop));
        return true;
    }

    return false;
  }

  int? _itemPrice(Item item) => item.price;
}

/// Screen to buy items from a shop.
class _ShopBuyScreen : _ItemVerbScreen {
  public Inventory _shop;

  public override string _headerText => "Buy which item?";

  public override string _verb => "Buy";

  public override Dictionary<string, string> _helpKeys => new Dictionary<string, string>() {
      {"A-Z", "Select item"}, 
      {"Shift", "Inspect item"},
      {"Esc", "Cancel"}
  };

  ItemCollection _items => _shop;

  ItemCollection _destination => _gameScreen.game.hero.save.inventory;

  _ShopBuyScreen(GameScreen gameScreen, Inventory _shop) : base(gameScreen)
  {
    this._shop = _shop;
  }

  bool _canSelectAny => true;
  bool _showPrices => true;

  bool canSelect(Item item) => item.price <= _save.gold;

  int _initialCount(Item item) => 1;

  /// Don't allow buying more than the hero can afford.
  int _maxCount(Item item) => Math.Min(item.count, _save.gold / item.price);

  int? _itemPrice(Item item) => item.price;

  /// Pay for purchased item.
  void _afterTransfer(Item item, int count) {
    var price = item.price * count;
    _gameScreen.game.log
        .message("You buy ${item.clone(count)} for $price gold.");
    _save.gold -= price;

    // Acquiring an item may unlock skills.
    // TODO: Would be nice if hero handled this more automatically. Maybe make
    // Inventory and Equipment manage this?
    _gameScreen.game.hero.pickUp(item);
  }
}

/// Screen to let the player choose a count for a selected item.
class _CountScreen : ItemScreen {
  /// The [_ItemVerbScreen] that pushed this.
  public _ItemVerbScreen _parent;
  public Item _item;
  int _count;

  ItemCollection _items => _parent._items;

  string _headerText {
    get {
      var itemText = _item.clone(_count).ToString();
      var price = _parent._itemPrice(_item);
      if (price != null) {
        var priceString = formatMoney(price * _count);
        return $"{{{_parent._verb}}} {itemText} for {priceString} gold?";
      } else {
        return $"{{{_parent._verb}}} {itemText}?";
      }
    }
  }

  public override Dictionary<string, string> _helpKeys => new Dictionary<string, string>() {
      {"OK", _parent._verb}, 
      {"↕", "Change quantity"}, 
      {"Esc", "Cancel"}
  };

  _CountScreen(GameScreen gameScreen, _ItemVerbScreen _parent, Item _item)
  :base(gameScreen)
  {
    this._parent = _parent;
    this._item = _item;
    _count = _parent._initialCount(_item);
    _inspected = _item;
  }

  bool _canSelectAny => true;

  /// Highlight the item the user already selected.
  bool canSelect(Item item) => item == _item;

  bool keyDown(int keyCode, {required bool shift, required bool alt}) {
    // Don't allow the shift key to inspect items.
    if (keyCode == KeyCode.shift) return false;

    return super.keyDown(keyCode, shift: shift, alt: alt);
  }

  bool handleInput(Input input) {
    switch (input) {
      case Input.ok:
        ui.pop(_count);
        return true;

      case Input.cancel:
        ui.pop();
        return true;

      case Input.n:
        if (_count < _parent._maxCount(_item)) {
          _count++;
          dirty();
        }
        return true;

      case Input.s:
        if (_count > 1) {
          _count--;
          dirty();
        }
        return true;

      case Input.runN:
        _count = _parent._maxCount(_item);
        dirty();
        return true;

      case Input.runS:
        _count = 1;
        dirty();
        return true;

      // TODO: Allow typing number.
    }

    return false;
  }

  int? _itemPrice(Item item) => _parent._itemPrice(item);
}
