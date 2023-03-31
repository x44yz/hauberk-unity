using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

/// Modal dialog for letting the user perform an [Action] on an [Item]
/// accessible to the [Hero].
class ItemDialog : Screen
{
  public GameScreen _gameScreen;

  /// The command the player is trying to perform on an item.
  public _ItemCommand _command;

  /// The current location being shown to the player.
  ItemLocation _location = ItemLocation.inventory;

  /// If the player needs to select a quantity for an item they have already
  /// chosen, this will be the index of the item.
  Item _selectedItem;

  /// The number of items the player selected.
  int _count = -1;

  /// Whether the shift key is currently pressed.
  public bool _shiftDown = false;

  /// The current item being inspected or `null` if there is none.
  public Item _inspected;

  public override bool isTransparent => true;

  /// True if the item dialog supports tabbing between item lists.
  bool canSwitchLocations => _command.allowedLocations.Count > 1;

  public static ItemDialog drop(GameScreen _gameScreen)
    => new ItemDialog(_gameScreen, new _DropItemCommand(), ItemLocation.inventory);

  public static ItemDialog use(GameScreen _gameScreen)
    => new ItemDialog(_gameScreen, new _UseItemCommand(), ItemLocation.inventory);

  public static ItemDialog toss(GameScreen _gameScreen)
    => new ItemDialog(_gameScreen, new _TossItemCommand(), ItemLocation.inventory);

  public static ItemDialog pickUp(GameScreen _gameScreen)
    => new ItemDialog(_gameScreen, new _PickUpItemCommand(), ItemLocation.onGround);

  public static ItemDialog equip(GameScreen _gameScreen)
    => new ItemDialog(_gameScreen, new _EquipItemCommand(), ItemLocation.inventory);

  public static ItemDialog sell(GameScreen _gameScreen, Inventory shop)
    => new ItemDialog(_gameScreen, new _SellItemCommand(shop), ItemLocation.inventory);

  public static ItemDialog put(GameScreen _gameScreen)
    => new ItemDialog(_gameScreen, new _PutItemCommand(), ItemLocation.inventory);

  public ItemDialog(GameScreen gameScreen, _ItemCommand command, ItemLocation location)
  {
    this._gameScreen = gameScreen;
    this._command = command;
    this._location = location;
  }

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    if (keyCode == InputX.ok)
    {
      if (_selectedItem != null)
      {
        _command.selectItem(this, _selectedItem!, _count, _location);
        return true;
      }
    }
    else if (keyCode == InputX.cancel)
    {
      if (_selectedItem != null)
      {
        // Go back to selecting an item.
        _selectedItem = null;
        Dirty();
      }
      else
      {
        terminal.Pop();
      }
      return true;
    }
    else if (keyCode == InputX.n)
    {
      if (_selectedItem != null)
      {
        if (_count < _selectedItem!.count)
        {
          _count++;
          Dirty();
        }
        return true;
      }
    }
    else if (keyCode == InputX.s)
    {
      if (_selectedItem != null)
      {
        if (_count > 1)
        {
          _count--;
          Dirty();
        }
        return true;
      }
    }
    // end of handleinput

    if (Input.GetKeyDown(KeyCode.LeftShift))
    {
      _shiftDown = true;
      Dirty();
      return true;
    }

    if (alt)
      return false;

    if (_shiftDown && Input.GetKeyDown(KeyCode.Escape))
    {
      _inspected = null;
      Dirty();
      return true;
    }

    // Can't switch view or select an item while selecting a count.
    if (_selectedItem != null) return false;

    if (keyCode >= KeyCode.A && keyCode <= KeyCode.Z)
    {
      _selectItem(keyCode - KeyCode.A);
      return true;
    }

    if (keyCode == KeyCode.Tab && !_shiftDown && canSwitchLocations)
    {
      _advanceLocation(shift ? -1 : 1);
      Dirty();
      return true;
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

  public override void Render(Terminal terminal)
  {
    var itemCount = 0;
    if (_location == ItemLocation.inventory)
      itemCount = Option.inventoryCapacity;
    else if (_location == ItemLocation.equipment)
      itemCount = _gameScreen.game.hero.equipment.slots.Count;
    else if (_location == ItemLocation.onGround)
      // TODO: Define this constant somewhere. Make the game engine try not
      // to place more than this many items per tile.
      itemCount = 5;

    int itemsLeft;
    int itemsTop;
    int itemsWidth;
    if (_gameScreen.itemPanel.isVisible)
    {
      if (_location == ItemLocation.inventory)
        itemsTop = _gameScreen.itemPanel.inventoryTop;
      else if (_location == ItemLocation.equipment)
        itemsTop = _gameScreen.itemPanel.equipmentTop;
      else if (_location == ItemLocation.onGround)
      {
        if (_gameScreen.itemPanel.onGroundVisible)
        {
          itemsTop = _gameScreen.itemPanel.onGroundTop;
        }
        else
        {
          itemsTop = 0;
        }
      }
      else
        throw new System.Exception("Unexpected location.");

      // Always make it at least 2 wider than the item panel. That way, with
      // the letters, the items stay in the same position.
      itemsWidth = Math.Max(
          ItemView.preferredWidth, _gameScreen.itemPanel.bounds.width + 2);
      itemsLeft = terminal.width - itemsWidth;
    }
    else
    {
      itemsWidth = ItemView.preferredWidth;
      itemsLeft = _gameScreen.stagePanel.bounds.right - itemsWidth;
      itemsTop = _gameScreen.stagePanel.bounds.y;
    }

    var itemView = new _ItemDialogItemView(this);
    itemView.render(terminal, itemsLeft, itemsTop, itemsWidth, itemCount);

    string query;
    if (_selectedItem == null)
    {
      if (_shiftDown)
      {
        query = "Inspect which item?";
      }
      else
      {
        query = _command.query(_location);
      }
    }
    else
    {
      query = _command.queryCount(_location) + $" {_count}";
    }

    _renderHelp(terminal, query);
  }

  void _renderHelp(Terminal terminal, string query)
  {
    Dictionary<string, string> helpKeys = new Dictionary<string, string>();
    if (_selectedItem == null)
    {
      if (_shiftDown)
      {
        helpKeys["A-Z"] = "Inspect item";
        if (_inspected != null) helpKeys["Esc"] = "Hide inspector";
      }
      else
      {
        helpKeys["A-Z"] = "Select item";
        helpKeys["Shift"] = "Inspect item";
        if (canSwitchLocations) helpKeys["Tab"] = "Switch view";
      }
    }
    else
    {
      helpKeys["↕"] = "Change quantity";
    }

    Draw.helpKeys(terminal, helpKeys, query);
  }

  public bool _canSelect(Item item)
  {
    if (_shiftDown) return true;

    if (_selectedItem != null) return item == _selectedItem;
    return _command.canSelect(item);
  }

  public void _selectItem(int index)
  {
    var items = _getItems().slots.ToList();
    if (index >= items.Count) return;

    // Can't select an empty equipment slot.
    var item = items[index];
    if (item == null) return;

    if (_shiftDown)
    {
      _inspected = item;
      Dirty();
    }
    else
    {
      if (!_command.canSelect(item)) return;

      if (item.count > 1 && _command.needsCount)
      {
        _selectedItem = item;
        _count = item.count;
        Dirty();
      }
      else
      {
        // Either we don't need a count or there's only one item.
        _command.selectItem(this, item, 1, _location);
      }
    }
  }

  public ItemCollection _getItems()
  {
    if (_location == ItemLocation.inventory)
      return _gameScreen.game.hero.inventory;
    else if (_location == ItemLocation.equipment)
      return _gameScreen.game.hero.equipment;
    else if (_location == ItemLocation.onGround)
      return _gameScreen.game.stage.itemsAt(_gameScreen.game.hero.pos);
    else
      throw new System.Exception("Unexpected location.");
  }

  /// Rotates through the viewable locations the player can select an item from.
  void _advanceLocation(int offset)
  {
    var index = _command.allowedLocations.IndexOf(_location);
    var count = _command.allowedLocations.Count;
    _location = _command.allowedLocations[(index + count + offset) % count];
  }
}

class _ItemDialogItemView : ItemView
{
  public ItemDialog _dialog;

  public override HeroSave save => _dialog._gameScreen.game.hero.save;

  public override ItemCollection items => _dialog._getItems();

  public override bool canSelectAny => true;

  public override bool capitalize => _dialog._shiftDown;

  public override bool showPrices => _dialog._command.showPrices;

  public override Item inspectedItem => _dialog._inspected;

  public override bool canSelect(Item item) => _dialog._canSelect(item);

  public override int? getPrice(Item item) => _dialog._command.getPrice(item);

  public _ItemDialogItemView(ItemDialog _dialog)
  {
    this._dialog = _dialog;
  }
}

/// The action the user wants to perform on the selected item.
abstract class _ItemCommand
{
  /// Locations of items that can be used with this command. When a command
  /// allows multiple locations, players can switch between them.
  public virtual List<ItemLocation> allowedLocations => new List<ItemLocation>() {
        ItemLocation.inventory,
        ItemLocation.onGround,
        ItemLocation.equipment
  };

  /// If the player must select how many items in a stack, returns `true`.
  public virtual bool needsCount => false;

  public virtual bool showPrices => false;

  /// The query shown to the user when selecting an item in this mode from the
  /// [ItemDialog].
  public abstract string query(ItemLocation location);

  /// The query shown to the user when selecting a quantity for an item in this
  /// mode from the [ItemDialog].
  public virtual string queryCount(ItemLocation location) => throw new System.NotImplementedException();

  /// Returns `true` if [item] is a valid selection for this command.
  public abstract bool canSelect(Item item);

  /// Called when a valid item has been selected.
  public abstract void selectItem(
      ItemDialog dialog, Item item, int count, ItemLocation location);

  public virtual int getPrice(Item item) => item.price;

  public void transfer(
      ItemDialog dialog, Item item, int count, ItemCollection destination)
  {
    if (!destination.canAdd(item))
    {
      dialog._gameScreen.game.log
          .error($"Not enough room for {item.clone(count)}.");
      dialog.Dirty();
      return;
    }

    if (count == item.count)
    {
      // Moving the entire stack.
      destination.tryAdd(item);
      dialog._getItems().remove(item);
    }
    else
    {
      // Splitting the stack.
      destination.tryAdd(item.splitStack(count));
      dialog._getItems().countChanged();
    }

    afterTransfer(dialog, item, count);
    dialog.terminal.Pop();
  }

  protected virtual void afterTransfer(ItemDialog dialog, Item item, int count) { }
}

class _DropItemCommand : _ItemCommand
{
  public override List<ItemLocation> allowedLocations =>
      new List<ItemLocation>() { ItemLocation.inventory, ItemLocation.equipment };

  public override bool needsCount => true;

  public override string query(ItemLocation location)
  {
    if (location == ItemLocation.inventory)
      return "Drop which item?";
    else if (location == ItemLocation.equipment)
      return "Unequip and drop which item?";
    throw new System.Exception("Unreachable.");
  }

  public override string queryCount(ItemLocation location) => "Drop how many?";

  public override bool canSelect(Item item) => true;

  public override void selectItem(
      ItemDialog dialog, Item item, int count, ItemLocation location)
  {
    dialog._gameScreen.game.hero
        .setNextAction(new DropAction(location, item, count));
    dialog.terminal.Pop();
  }
}

class _UseItemCommand : _ItemCommand
{
  public override bool needsCount => false;

  public override string query(ItemLocation location)
  {
    if (location == ItemLocation.inventory ||
        location == ItemLocation.equipment)
      return "Use which item?";
    else if (location == ItemLocation.onGround)
      return "Pick up and use which item?";

    throw new System.Exception("Unreachable.");
  }

  public override bool canSelect(Item item) => item.canUse;

  public override void selectItem(
      ItemDialog dialog, Item item, int count, ItemLocation location)
  {
    dialog._gameScreen.game.hero.setNextAction(new UseAction(location, item));
    dialog.terminal.Pop();
  }
}

class _EquipItemCommand : _ItemCommand
{
  public override bool needsCount => false;

  public override string query(ItemLocation location)
  {
    if (location == ItemLocation.inventory)
      return "Equip which item?";
    else if (location == ItemLocation.equipment)
      return "Unequip which item?";
    else if (location == ItemLocation.onGround)
      return "Pick up and equip which item?";

    throw new System.Exception("Unreachable.");
  }

  public override bool canSelect(Item item) => item.canEquip;

  public override void selectItem(
      ItemDialog dialog, Item item, int count, ItemLocation location)
  {
    dialog._gameScreen.game.hero.setNextAction(new EquipAction(location, item));
    dialog.terminal.Pop();
  }
}

class _TossItemCommand : _ItemCommand
{
  public override bool needsCount => false;

  public override string query(ItemLocation location)
  {
    if (location == ItemLocation.inventory)
      return "Throw which item?";
    else if (location == ItemLocation.equipment)
      return "Unequip and throw which item?";
    else if (location == ItemLocation.onGround)
      return "Pick up and throw which item?";

    throw new System.Exception("Unreachable.");
  }

  public override bool canSelect(Item item) => item.canToss;

  public override void selectItem(
      ItemDialog dialog, Item item, int count, ItemLocation location)
  {
    // Create the hit now so range modifiers can be calculated before the
    // target is chosen.
    var hit = item.toss!.attack.createHit();
    dialog._gameScreen.game.hero.modifyHit(hit, HitType.toss);

    // Now we need a target.
    dialog.terminal.GoTo(new TargetDialog(dialog._gameScreen, hit.range, (target) =>
    {
      dialog._gameScreen.game.hero
          .setNextAction(new TossAction(location, item, hit, target));
    }));
  }
}

// TODO: It queries for a count. But if there is only a single item, the hero
// automatically picks up the whole stack. Should it do the same here?
class _PickUpItemCommand : _ItemCommand
{
  public override List<ItemLocation> allowedLocations => new List<ItemLocation>()
    {ItemLocation.onGround};

  public override bool needsCount => true;

  public override string query(ItemLocation location) => "Pick up which item?";

  public override string queryCount(ItemLocation location) => "Pick up how many?";

  public override bool canSelect(Item item) => true;

  public override void selectItem(
      ItemDialog dialog, Item item, int count, ItemLocation location)
  {
    // Pick up item and return to the game
    dialog._gameScreen.game.hero.setNextAction(new PickUpAction(item));
    dialog.terminal.Pop();
  }
}

class _PutItemCommand : _ItemCommand
{
  public _PutItemCommand()
  {
  }

  public override List<ItemLocation> allowedLocations =>
      new List<ItemLocation>() { ItemLocation.inventory, ItemLocation.equipment };

  public override bool needsCount => true;

  public override string query(ItemLocation location) => "Put which item?";

  public override string queryCount(ItemLocation location) => "Put how many?";

  public override bool canSelect(Item item) => true;

  public override void selectItem(
      ItemDialog dialog, Item item, int count, ItemLocation location)
  {
    transfer(dialog, item, count, dialog._gameScreen.game.hero.save.home);
  }

  protected override void afterTransfer(ItemDialog dialog, Item item, int count)
  {
    dialog._gameScreen.game.log
        .message($"You put {item.clone(count)} safely into your home.");
  }
}

// TODO: Require confirmation when selling an item if it isn't a stack?
class _SellItemCommand : _ItemCommand
{
  public Inventory _shop;

  public _SellItemCommand(Inventory _shop)
  {
    this._shop = _shop;
  }

  public override List<ItemLocation> allowedLocations =>
      new List<ItemLocation>() { ItemLocation.inventory, ItemLocation.equipment };

  public override bool needsCount => true;

  public override bool showPrices => true;

  public override string query(ItemLocation location) => "Sell which item?";

  public override string queryCount(ItemLocation location) => "Sell how many?";

  public override bool canSelect(Item item) => item.price != 0;

  public override int getPrice(Item item) => (int)Math.Floor(item.price * 0.75);

  public override void selectItem(
      ItemDialog dialog, Item item, int count, ItemLocation location)
  {
    transfer(dialog, item, count, _shop);
  }

  protected override void afterTransfer(ItemDialog dialog, Item item, int count)
  {
    var itemText = item.clone(count).ToString();
    var price = getPrice(item) * count;
    // TODO: The help text overlaps the log pane, so this isn't very useful.
    dialog._gameScreen.game.log.message($"You sell {itemText} for {price} gold.");
    dialog._gameScreen.game.hero.gold += price;
  }
}
