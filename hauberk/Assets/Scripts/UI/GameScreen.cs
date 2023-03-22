using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTerminal;

class GameScreen : UnityTerminal.Screen {
  public Game game;

  /// When the hero is in the dungeon, this is their save state before entering.
  /// If they die or forfeit, their current state is discarded and this one is
  /// used instead.
  public HeroSave? _storageSave;
  public Storage _storage;
  public LogPanel _logPanel;
  public ItemPanel itemPanel;
  public SidebarPanel _sidebarPanel;

  public StagePanel stagePanel => _stagePanel;
  public StagePanel _stagePanel;

  /// The number of ticks left to wait before restarting the game loop after
  /// coming back from a dialog where the player chose an action for the hero.
  int _pause = 0;

  Actor? _targetActor;
  Vec? _target;

  UsableSkill? _lastSkill;

  /// The portal for the tile the hero is currently standing on.
  ///
  /// When this changes, we know the hero has stepped onto a new one.
  TilePortal? _portal;

  void targetActor(Actor? value) {
    if (_targetActor != value) Dirty();

    _targetActor = value;
    _target = null;
  }

  /// Targets the floor at [pos].
  void targetFloor(Vec? pos) {
    if (_targetActor != null || _target != pos) Dirty();

    _targetActor = null;
    _target = pos;
  }

  /// Gets the currently targeted position.
  ///
  /// If targeting an actor, gets the actor's position.
  Vec? currentTarget {
    get {
      // If we're targeting an actor, use its position.
      var actor = currentTargetActor;
      return actor?.pos ?? _target;
    }
  }

  /// The currently targeted actor, if any.
  Actor? currentTargetActor {
    get {
    // Forget the target if it dies or goes offscreen.
      var actor = _targetActor;
      if (actor != null) {
        if (!actor.isAlive || !game.hero.canPerceive(actor)) {
          _targetActor = null;
        }
      }

      if (_targetActor != null) return _targetActor;

      // If we're targeting the floor, see if there is an actor there.
      if (_target != null) {
        return game.stage.actorAt(_target!);
      }

      return null;
    }
  }

  Rect cameraBounds => _stagePanel.cameraBounds;

  Color heroColor {
    get {
      var hero = game.hero;
      if (hero.health < hero.maxHealth / 4) return Hues.red;
      if (hero.poison.isActive) return Hues.peaGreen;
      if (hero.cold.isActive) return Hues.lightBlue;
      if (hero.health < hero.maxHealth / 2) return Hues.pink;
      if (hero.stomach == 0 && hero.health < hero.maxHealth) return Hues.sandal;
      return Hues.ash;
    }
  }

  public GameScreen(Storage _storage, Game game, HeroSave _storageSave)
  {
    this._storage = _storage;
    this.game = game;
    this._storageSave = _storageSave;
    _logPanel = LogPanel(game.log);
    itemPanel = ItemPanel(game);
    _sidebarPanel = SidebarPanel(this);
    _stagePanel = StagePanel(this);

    Debugger.bindGameScreen(this);
  }

  public static GameScreen town(Storage storage, Content content, HeroSave save) {
    var game = new Game(content, save, 0, width: 60, height: 34);
    foreach (var _ in game.generate()) {}

    return new GameScreen(storage, game, null);
  }

  /// Draws [Glyph] at [x], [y] in [Stage] coordinates onto the stage panel.
  void drawStageGlyph(Terminal terminal, int x, int y, Glyph glyph) {
    _stagePanel.drawStageGlyph(terminal, x, y, glyph);
  }

  public override void HandleInput() {
    bool shift = Input.GetKey(KeyCode.LeftShift);
    bool alt = Input.GetKey(KeyCode.LeftAlt);

    Action action = null;
    if (Input.GetKeyDown(InputX.quit))
    {
        var portal = game.stage[game.hero.pos].portal;
        if (portal == TilePortals.exit) {
          // Should have a storage because there are no exits in the town.
          terminal.Push(new ExitPopup(_storageSave!, game));
        } else {
          game.log.error("You are not standing on an exit.");
          Dirty();
        }
    }
    else if (Input.GetKeyDown(InputX.forfeit) && shift)
    {
      terminal.Push(new ForfeitPopup(isTown: game.depth == 0));
    }
    else if (Input.GetKeyDown(InputX.selectSkill))
    {
      throw new System.NotImplementedException();
      // ui.push(SelectSkillDialog(this));
    }
    else if (Input.GetKeyDown(InputX.editSkills) && shift)
    {
      throw new System.NotImplementedException();
      // ui.push(SkillDialog(game.hero.save));
    }
    else if (Input.GetKeyDown(InputX.heroInfo))
    {
      throw new System.NotImplementedException();
      // ui.push(HeroInfoDialog(game.content, game.hero.save));
    }
    else if (Input.GetKeyDown(InputX.drop))
    {
      throw new System.NotImplementedException();
      // ui.push(ItemDialog.drop(this));
    }
    else if (Input.GetKeyDown(InputX.use))
    {
      throw new System.NotImplementedException();
      // ui.push(ItemDialog.use(this));
    }
    else if (Input.GetKeyDown(InputX.toss))
    {
      throw new System.NotImplementedException();
      // ui.push(ItemDialog.toss(this));
    }
    else if (Input.GetKeyDown(InputX.rest) && shift)
    {
      throw new System.NotImplementedException();
        // if (!game.hero.rest()) {
        //   // Show the message.
        //   Dirty();
        // }
    }
    else if (Input.GetKeyDown(InputX.open) && shift)
    {
      _open();
    }
    else if (Input.GetKeyDown(InputX.close))
    {
      _closeDoor();
    }
    else if (Input.GetKeyDown(InputX.pickUp))
    {
      _pickUp();
    }
    else if (Input.GetKeyDown(InputX.equip))
    {
      throw new System.NotImplementedException();
        // ui.push(ItemDialog.equip(this));
    }
    else if (Input.GetKeyDown(InputX.nw))
    {
      throw new System.NotImplementedException();
        // action = WalkAction(Direction.nw);
    }
    else if (Input.GetKeyDown(InputX.n))
    {
      throw new System.NotImplementedException();
        // action = WalkAction(Direction.n);
    }
    else if (Input.GetKeyDown(InputX.ne))
    {
      throw new System.NotImplementedException();
        // action = WalkAction(Direction.ne);
    }
    else if (Input.GetKeyDown(InputX.w))
    {
      throw new System.NotImplementedException();
        // action = WalkAction(Direction.w);
    }
    else if (Input.GetKeyDown(InputX.ok))
    {
      throw new System.NotImplementedException();
        // action = WalkAction(Direction.none);
    }
    else if (Input.GetKeyDown(InputX.e))
    {
      throw new System.NotImplementedException();
        // action = WalkAction(Direction.e);
    }
    else if (Input.GetKeyDown(InputX.sw))
    {
      throw new System.NotImplementedException();
        // action = WalkAction(Direction.sw);
    }
    else if (Input.GetKeyDown(InputX.s))
    {
      throw new System.NotImplementedException();
        // action = WalkAction(Direction.s);
    }
    else if (Input.GetKeyDown(InputX.se))
    {
      throw new System.NotImplementedException();
        // action = WalkAction(Direction.se);
    }
    else if (Input.GetKeyDown(InputX.runNW) && shift)
    {
      throw new System.NotImplementedException();
        // game.hero.run(Direction.nw);
    }
    else if (Input.GetKeyDown(InputX.runN) && shift)
    {
      throw new System.NotImplementedException();
        // game.hero.run(Direction.n);
    }
    else if (Input.GetKeyDown(InputX.runNE) && shift)
    {
      throw new System.NotImplementedException();
        // game.hero.run(Direction.ne);
    }
    else if (Input.GetKeyDown(InputX.runW) && shift)
    {
      throw new System.NotImplementedException();
        // game.hero.run(Direction.w);
    }
    else if (Input.GetKeyDown(InputX.runE) && shift)
    {
      throw new System.NotImplementedException();
        // game.hero.run(Direction.e);
    }
    else if (Input.GetKeyDown(InputX.runSW) && shift)
    {
      throw new System.NotImplementedException();
        // game.hero.run(Direction.sw);
    }
    else if (Input.GetKeyDown(InputX.runS) && shift)
    {
      throw new System.NotImplementedException();
        // game.hero.run(Direction.s);
    }
    else if (Input.GetKeyDown(InputX.runSE) && shift)
    {
      throw new System.NotImplementedException();
        // game.hero.run(Direction.se);
    }
    else if (Input.GetKeyDown(InputX.fireNW) && alt)
    {
      throw new System.NotImplementedException();
        // _fireTowards(Direction.nw);
    }
    else if (Input.GetKeyDown(InputX.fireN) && alt)
    {
      throw new System.NotImplementedException();
        //_fireTowards(Direction.n);
    }
    else if (Input.GetKeyDown(InputX.fireNE) && alt)
    {
      throw new System.NotImplementedException();
        //_fireTowards(Direction.ne);
    }
    else if (Input.GetKeyDown(InputX.fireW) && alt)
    {
      throw new System.NotImplementedException();
        //_fireTowards(Direction.w);
    }
    else if (Input.GetKeyDown(InputX.fireE) && alt)
    {
      throw new System.NotImplementedException();
        //_fireTowards(Direction.e);
    }
    else if (Input.GetKeyDown(InputX.fireSW) && alt)
    {
      throw new System.NotImplementedException();
        //_fireTowards(Direction.sw);
    }
    else if (Input.GetKeyDown(InputX.fireS) && alt)
    {
      throw new System.NotImplementedException();
        //_fireTowards(Direction.s);
    }
    else if (Input.GetKeyDown(InputX.fireSE) && alt)
    {
      throw new System.NotImplementedException();
        //_fireTowards(Direction.se);
    }
    else if (Input.GetKeyDown(InputX.fire) && alt)
    {
      throw new System.NotImplementedException();
    // if (_lastSkill is TargetSkill) {
    //         var targetSkill = _lastSkill as TargetSkill;
    //         if (currentTargetActor != null) {
    //           // If we still have a visible target, use it.
    //           _fireAtTarget(_lastSkill as TargetSkill);
    //         } else {
    //           // No current target, so ask for one.
    //           _openTargetDialog(targetSkill);
    //         }
    //       } else if (_lastSkill is DirectionSkill) {
    //         // Ask user to pick a direction.
    //         ui.push(SkillDirectionDialog(this, _fireTowards));
    //       } else if (_lastSkill is ActionSkill) {
    //         var actionSkill = _lastSkill as ActionSkill;
    //         game.hero.setNextAction(
    //             actionSkill.getAction(game, game.hero.skills.level(actionSkill)));
    //       } else {
    //         game.log.error("No skill selected.");
    //         Dirty();
    //       }
    }
    else if (Input.GetKeyDown(InputX.swap))
    {
      throw new System.NotImplementedException();
        // var unequipped = game.hero.inventory.lastUnequipped;
        // if (unequipped == null) {
        //   game.log.error("You aren't holding an unequipped item to swap.");
        //   Dirty();
        // } else {
        //   action = EquipAction(ItemLocation.inventory, unequipped);
        // }
    }
    else if (Input.GetKeyDown(InputX.wizard) && shift && alt)
    {
        throw new System.NotImplementedException();
        // if (Debugger.enabled) {
        //   ui.push(WizardDialog(game));
        // } else {
        //   game.log.cheat("No cheating in non-debug builds. Cheater.");
        //   Dirty();
        // }
    }

    if (action != null) 
      game.hero.setNextAction(action);
  }

  public override void Active(UnityTerminal.Screen popped, object result) {
    if (!game.hero.needsInput) {
      // The player is coming back from a screen where they selected an action
      // for the hero. Give them a bit to visually reorient themselves before
      // kicking off the action.
      _pause = 10;
    }

    if (popped is ExitPopup) {
      // TODO: Hero should start next to dungeon entrance.
      _storageSave!.takeFrom(game.hero);

      // Update shops.
      foreach (var kv in game.hero.save.shops)
      {
        var shop = kv.Key;
        var inventory = kv.Value;
        shop.update(inventory);
      };

      _storage.save();
      terminal.GoTo(GameScreen.town(_storage, game.content, _storageSave!));
    }
    /*
    else if (popped is SelectDepthPopup && result is int) {
      // Enter the dungeon.
      _storage.save();
      ui.push(LoadingDialog(game.hero.save, game.content, result));
    }
    else if (popped is LoadingDialog) {
      ui.goTo(GameScreen(_storage, result as Game, game.hero.save));
    } 
    else if (popped is ForfeitPopup && result == true) {
      if (game.depth > 0) {
        // Forfeiting, so return to the town and discard the current hero.
        // TODO: Hero should start next to dungeon entrance.
        ui.goTo(GameScreen.town(_storage, game.content, _storageSave!));
      } else {
        // Leaving the town. Save just to be safe.
        _storage.save();
        ui.pop();
      }
    } 
    else if (popped is ItemScreen) {
      // Always save when leaving home or a shop.
      _storage.save();
    } 
    else if (popped is ItemDialog) {
      // Save after changing items in the town.
      if (game.depth == 0) _storage.save();
    } 
    else if (popped is SkillDialog) {
      // TODO: Once skills can be learned on the SkillDialog again, make this
      // work.
//      game.hero.updateSkills(result);
    } 
    else if (popped is SelectSkillDialog && result != null) {
      if (result is TargetSkill) {
        _openTargetDialog(result);
      } else if (result is DirectionSkill) {
        ui.push(SkillDirectionDialog(this, (dir) {
          _lastSkill = result;
          _fireTowards(dir);
        }));
      } else if (result is ActionSkill) {
        _lastSkill = result;
        game.hero.setNextAction(
            result.getAction(game, game.hero.skills.level(result)));
      }
    }
    */
  }

  void update() {
    if (_enterPortal()) return;

    if (_pause > 0) {
      _pause--;
      return;
    }

    var result = game.update();

    // See if the hero died.
    if (!game.hero.isAlive) {
      terminal.GoTo(new GameOverScreen(game.log));
      return;
    }

    if (_stagePanel.update(result.events)) Dirty();

    if (result.needsRefresh) Dirty();
  }

  public override void Resize(int width, int height){
    var size = new Vec(width, height);

    var leftWidth = 21;

    if (size > 160) {
      leftWidth = 29;
    } else if (size > 150) {
      leftWidth = 25;
    }

    var centerWidth = size.x - leftWidth;

    itemPanel.hide();
    if (size.x >= 100) {
      var nwidth = Math.Min(50, 20 + (size.x - 100) / 2);
      itemPanel.show(new Rect(size.x - nwidth, 0, nwidth, size.y));
      centerWidth = size.x - leftWidth - nwidth;
    }

    _sidebarPanel.show(new Rect(0, 0, leftWidth, size.y));

    var logHeight = 6 + (size.y - 40) / 2;
    logHeight = Math.Min(logHeight, 20);

    stagePanel.show(new Rect(leftWidth, 0, centerWidth, size.y - logHeight));
    _logPanel.show(new Rect(leftWidth, size.y - logHeight, centerWidth, logHeight));
  }

  public override void Render() {
    terminal.Clear();

    _stagePanel.render(terminal);
    _logPanel.render(terminal);
    // Note, this must be rendered after the stage panel so that the visible
    // monsters are correctly calculated first.
    _sidebarPanel.render(terminal);
    itemPanel.render(terminal);
  }

  /// Handle the hero stepping onto a portal tile.
  bool _enterPortal() {
    var portal = game.stage[game.hero.pos].portal;
    if (portal == _portal) return false;
    _portal = portal;

    if (portal == TilePortals.dungeon)
        terminal.Push(new SelectDepthPopup(game.content, game.hero.save));
    else if (portal == TilePortals.home)
        terminal.Push(ItemScreen.home(this));
    else if (portal == TilePortals.shop1)
        _enterShop(0);
    else if (portal == TilePortals.shop2)
        _enterShop(1);
    else if (portal == TilePortals.shop3)
        _enterShop(2);
    else if (portal == TilePortals.shop4)
        _enterShop(3);
    else if (portal == TilePortals.shop5)
        _enterShop(4);
    else if (portal == TilePortals.shop6)
        _enterShop(5);
    else if (portal == TilePortals.shop7)
        _enterShop(6);
    else if (portal == TilePortals.shop8)
        _enterShop(7);
    else if (portal == TilePortals.shop9)
        _enterShop(8);
      // TODO: No crucible right now.
//        ui.push(new ItemScreen.crucible(content, save));

    return true;
  }

  void _open() {
    // See how many adjacent closed doors there are.
    // TODO: Handle chests.
    var openable = new List<Vec>();
    foreach (var pos in game.hero.pos.neighbors) {
      if (game.stage[pos].type.canOpen) {
        openable.Add(pos);
      }
    }

    if (openable.isEmpty()) {
      game.log.error("You are not next to anything to open.");
      Dirty();
    } else if (openable.Count == 1) {
      var pos = openable.First();
      // TODO: This leaks information if the hero is next to unexplored tiles.
      game.hero.setNextAction(game.stage[pos].type.onOpen!(pos));
    } else {
      terminal.Push(OpenDialog(this));
    }
  }

  void _closeDoor() {
    // See how many adjacent open doors there are.
    var closeable = new List<Vec>();
    foreach (var pos in game.hero.pos.neighbors) {
      if (game.stage[pos].type.canClose) {
        closeable.Add(pos);
      }
    }

    if (closeable.isEmpty()) {
      game.log.error("You are not next to an open door.");
      Dirty();
    } else if (closeable.Count == 1) {
      var pos = closeable.First();
      // TODO: This leaks information if the hero is next to unexplored tiles.
      game.hero.setNextAction(game.stage[pos].type.onClose!(pos));
    } else {
      terminal.Push(CloseDialog(this));
    }
  }

  void _pickUp() {
    var items = game.stage.itemsAt(game.hero.pos);
    if (items.length > 1) {
      // Show item dialog if there are multiple things to pick up.
      terminal.Push(ItemDialog.pickUp(this));
    } else if (items.length == 1) {
      // Otherwise attempt to pick the one item.
      game.hero.setNextAction(PickUpAction(items.first));
    } else {
      game.log.error('There is nothing here.');
      Dirty();
    }
  }

  void _openTargetDialog(TargetSkill skill) {
    terminal.Push(
        TargetDialog(this, skill.getRange(game), (_) => _fireAtTarget(skill)));
  }

  void _fireAtTarget(TargetSkill skill) {
    if (currentTarget == game.hero.pos && !skill.canTargetSelf) {
      game.log.error("You can't target yourself.");
      Dirty();
      return;
    }

    _lastSkill = skill;
    // TODO: It's kind of annoying that we force the player to select a target
    // or direction for skills that spend focus/fury even when they won't be
    // able to perform it. Should do an early check first.
    game.hero.setNextAction(skill.getTargetAction(
        game, game.hero.skills.level(skill), currentTarget!));
  }

  void _fireTowards(Direction dir) {
    // If the user canceled, don't fire.
    if (dir == Direction.none) return;

    if (_lastSkill is DirectionSkill) {
      var directionSkill = _lastSkill as DirectionSkill;
      game.hero.setNextAction(directionSkill.getDirectionAction(
          game, game.hero.skills.level(directionSkill), dir));
    } else if (_lastSkill is TargetSkill) {
      var targetSkill = _lastSkill as TargetSkill;
      var pos = game.hero.pos + dir;

      // Target the monster that is in the fired direction, if any.
      late Vec previous;
      for (var step in Line(game.hero.pos, pos)) {
        // If we reached an actor, target it.
        var actor = game.stage.actorAt(step);
        if (actor != null) {
          targetActor(actor);
          break;
        }

        // If we hit a wall, target the floor tile before it.
        if (game.stage[step].blocksView) {
          targetFloor(previous);
          break;
        }

        // If we hit the end of the range, target the floor there.
        if ((step - game.hero.pos) >= targetSkill.getRange(game)) {
          targetFloor(step);
          break;
        }

        previous = step;
      }

      if (currentTarget != null) {
        game.hero.setNextAction(targetSkill.getTargetAction(
            game, game.hero.skills.level(targetSkill), currentTarget!));
      } else {
        var tile = game.stage[game.hero.pos + dir].type.name;
        game.log.error("There is a $tile in the way.");
        Dirty();
      }
    } else if (_lastSkill is ActionSkill) {
      game.log.error("${_lastSkill!.useName} does not take a direction.");
      Dirty();
    } else {
      // TODO: Better error message.
      game.log.error("No skill selected.");
      Dirty();
    }
  }

  void _enterShop(int index) {
    var shops = game.hero.save.shops.Keys.ToList();
    if (index >= shops.Count) return;

    terminal.Push(ItemScreen.shop(this, game.hero.save.shops[shops[index]]!));
  }
}