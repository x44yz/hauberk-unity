using System;
using System.Collections.Generic;
using System.Linq;
using Input = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using UnityTerminal;

class ItemPanel : Panel
{
  public Game _game;

  public ItemPanel(Game _game)
  {
    this._game = _game;
  }

  public int equipmentTop => 0;
  public int inventoryTop => _game.hero.equipment.slots.Count + 2;
  public int onGroundTop =>
      _game.hero.equipment.slots.Count + Option.inventoryCapacity + 4;

  public bool onGroundVisible => bounds.height > 50;

  public override void renderPanel(Terminal terminal)
  {
    var hero = _game.hero;
    _drawItems(
        terminal, equipmentTop, hero.equipment.slots.Count, hero.equipment);

    _drawItems(
        terminal, inventoryTop, Option.inventoryCapacity, hero.inventory);

    // Don't show the on the ground panel if the height is too short for it.
    if (onGroundVisible)
    {
      var onGround = _game.stage.itemsAt(hero.pos);
      _drawItems(terminal, onGroundTop, 5, onGround);
    }

    // TODO: Show something useful down here. Maybe mini-map or monster info.
    var restTop = onGroundVisible ? onGroundTop + 7 : onGroundTop;
    Draw.box(terminal, 0, restTop, terminal.width, terminal.height - restTop);
  }

  void _drawItems(
      Terminal terminal, int y, int itemSlotCount, ItemCollection items)
  {
    var view = new _ItemPanelItemView(_game, items);
    view.render(terminal, 0, y, terminal.width, itemSlotCount);
  }
}

class _ItemPanelItemView : ItemView
{
  public Game _game;
  public override ItemCollection items => _items;
  ItemCollection _items;

  public _ItemPanelItemView(Game _game, ItemCollection items)
  {
    this._game = _game;
    this._items = items;
  }

  public override HeroSave save => _game.hero.save;

  public override bool showLetters => false;

  public override bool canSelectAny => false;
}
