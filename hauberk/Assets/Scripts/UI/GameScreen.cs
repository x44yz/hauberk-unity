using System;
using System.Collections.Generic;
using System.Linq;
using UnityTerminal;
using KeyCode = UnityEngine.KeyCode;

class GameScreen : UnityTerminal.Screen
{
  public Game game;

  /// When the hero is in the dungeon, this is their save state before entering.
  /// If they die or forfeit, their current state is discarded and this one is
  /// used instead.
  public HeroSave _storageSave;
  public Storage _storage;
  public LogPanel _logPanel;
  public ItemPanel itemPanel;
  public SidebarPanel _sidebarPanel;

  public StagePanel stagePanel => _stagePanel;
  public StagePanel _stagePanel;

  /// The number of ticks left to wait before restarting the game loop after
  /// coming back from a dialog where the player chose an action for the hero.
  int _pause = 0;

  Actor _targetActor;
  Vec _target;

  UsableSkill _lastSkill;

  /// The portal for the tile the hero is currently standing on.
  ///
  /// When this changes, we know the hero has stepped onto a new one.
  TilePortal _portal;

  public void targetActor(Actor value)
  {
    if (_targetActor != value) Dirty();

    _targetActor = value;
    _target = null;
  }

  /// Targets the floor at [pos].
  public void targetFloor(Vec pos)
  {
    if (_targetActor != null || _target != pos) Dirty();

    _targetActor = null;
    _target = pos;
  }

  /// Gets the currently targeted position.
  ///
  /// If targeting an actor, gets the actor's position.
  public Vec currentTarget
  {
    get
    {
      // If we're targeting an actor, use its position.
      var actor = currentTargetActor;
      return actor?.pos ?? _target;
    }
  }

  /// The currently targeted actor, if any.
  public Actor currentTargetActor
  {
    get
    {
      // Forget the target if it dies or goes offscreen.
      var actor = _targetActor;
      if (actor != null)
      {
        if (!actor.isAlive || !game.hero.canPerceive(actor))
        {
          _targetActor = null;
        }
      }

      if (_targetActor != null) return _targetActor;

      // If we're targeting the floor, see if there is an actor there.
      if (_target != null)
      {
        return game.stage.actorAt(_target!);
      }

      return null;
    }
  }

  public Rect cameraBounds => _stagePanel.cameraBounds;

  public Color heroColor
  {
    get
    {
      var hero = game.hero;
      if (hero.health < hero.maxHealth / 4f) return Hues.red;
      if (hero.poison.isActive) return Hues.peaGreen;
      if (hero.cold.isActive) return Hues.lightBlue;
      if (hero.health < hero.maxHealth / 2f) return Hues.pink;
      if (hero.stomach == 0 && hero.health < hero.maxHealth) return Hues.sandal;
      return Hues.ash;
    }
  }

  public GameScreen(Storage _storage, Game game, HeroSave _storageSave)
  {
    this._storage = _storage;
    this.game = game;
    this._storageSave = _storageSave;
    _logPanel = new LogPanel(game.log);
    itemPanel = new ItemPanel(game);
    _sidebarPanel = new SidebarPanel(this);
    _stagePanel = new StagePanel(this);

    Debugger.bindGameScreen(this);
  }

  public static void town(Storage storage, Content content, HeroSave save,
    System.Action<GameScreen> callback)
  {
    var game = new Game(content, save, 0, width: 60, height: 34);
    var tt = Main.Inst.StartCoroutine(game.generate(()=>{
        callback.Invoke(new GameScreen(storage, game, null));
    }));
  }

  /// Draws [Glyph] at [x], [y] in [Stage] coordinates onto the stage panel.
  public void drawStageGlyph(Terminal terminal, int x, int y, Glyph glyph)
  {
    _stagePanel.drawStageGlyph(terminal, x, y, glyph);
  }

  public override bool KeyDown(KeyCode keyCode, bool shift, bool alt)
  {
    Action action = null;
    if (keyCode == InputX.quit)
    {
      var portal = game.stage[game.hero.pos].portal;
      if (portal == TilePortals.exit)
      {
        // Should have a storage because there are no exits in the town.
        terminal.Push(new ExitPopup(_storageSave!, game));
      }
      else
      {
        game.log.error("You are not standing on an exit.");
        Dirty();
      }
    }
    else if (keyCode == InputX.forfeit && shift)
    {
      terminal.Push(new ForfeitPopup(isTown: game.depth == 0));
    }
    else if (keyCode == InputX.selectSkill)
    {
      terminal.Push(new SelectSkillDialog(this));
    }
    else if (keyCode == InputX.editSkills && shift)
    {
      terminal.Push(SkillDialog.create(game.hero.save));
    }
    else if (keyCode == InputX.heroInfo)
    {
      terminal.Push(HeroInfoDialog.create(game.content, game.hero.save));
    }
    else if (keyCode == InputX.drop)
    {
      terminal.Push(ItemDialog.drop(this));
    }
    else if (keyCode == InputX.use)
    {
      terminal.Push(ItemDialog.use(this));
    }
    else if (keyCode == InputX.toss)
    {
      terminal.Push(ItemDialog.toss(this));
    }
    else if (keyCode == InputX.rest && shift)
    {
      if (!game.hero.rest())
      {
        // Show the message.
        Dirty();
      }
    }
    else if (keyCode == InputX.open && shift)
    {
      _open();
    }
    else if (keyCode == InputX.close)
    {
      _closeDoor();
    }
    else if (keyCode == InputX.pickUp)
    {
      _pickUp();
    }
    else if (keyCode == InputX.equip)
    {
      terminal.Push(ItemDialog.equip(this));
    }
    else if (keyCode == InputX.nw)
    {
      action = new WalkAction(Direction.nw);
    }
    else if (keyCode == InputX.n)
    {
      action = new WalkAction(Direction.n);
    }
    else if (keyCode == InputX.ne)
    {
      action = new WalkAction(Direction.ne);
    }
    else if (keyCode == InputX.w)
    {
      action = new WalkAction(Direction.w);
    }
    else if (keyCode == InputX.ok)
    {
      action = new WalkAction(Direction.none);
    }
    else if (keyCode == InputX.e)
    {
      action = new WalkAction(Direction.e);
    }
    else if (keyCode == InputX.sw)
    {
      action = new WalkAction(Direction.sw);
    }
    else if (keyCode == InputX.s)
    {
      action = new WalkAction(Direction.s);
    }
    else if (keyCode == InputX.se)
    {
      action = new WalkAction(Direction.se);
    }
    else if (keyCode == InputX.runNW && shift)
    {
      game.hero.run(Direction.nw);
    }
    else if (keyCode == InputX.runN && shift)
    {
      game.hero.run(Direction.n);
    }
    else if (keyCode == InputX.runNE && shift)
    {
      game.hero.run(Direction.ne);
    }
    else if (keyCode == InputX.runW && shift)
    {
      game.hero.run(Direction.w);
    }
    else if (keyCode == InputX.runE && shift)
    {
      game.hero.run(Direction.e);
    }
    else if (keyCode == InputX.runSW && shift)
    {
      game.hero.run(Direction.sw);
    }
    else if (keyCode == InputX.runS && shift)
    {
      game.hero.run(Direction.s);
    }
    else if (keyCode == InputX.runSE && shift)
    {
      game.hero.run(Direction.se);
    }
    else if (keyCode == InputX.fireNW && alt)
    {
      _fireTowards(Direction.nw);
    }
    else if (keyCode == InputX.fireN && alt)
    {
      _fireTowards(Direction.n);
    }
    else if (keyCode == InputX.fireNE && alt)
    {
      _fireTowards(Direction.ne);
    }
    else if (keyCode == InputX.fireW && alt)
    {
      _fireTowards(Direction.w);
    }
    else if (keyCode == InputX.fireE && alt)
    {
      _fireTowards(Direction.e);
    }
    else if (keyCode == InputX.fireSW && alt)
    {
      _fireTowards(Direction.sw);
    }
    else if (keyCode == InputX.fireS && alt)
    {
      _fireTowards(Direction.s);
    }
    else if (keyCode == InputX.fireSE && alt)
    {
      _fireTowards(Direction.se);
    }
    else if (keyCode == InputX.fire && alt)
    {
      if (_lastSkill is TargetSkill)
      {
        var targetSkill = _lastSkill as TargetSkill;
        if (currentTargetActor != null)
        {
          // If we still have a visible target, use it.
          _fireAtTarget(_lastSkill as TargetSkill);
        }
        else
        {
          // No current target, so ask for one.
          _openTargetDialog(targetSkill);
        }
      }
      else if (_lastSkill is DirectionSkill)
      {
        // Ask user to pick a direction.
        terminal.Push(new SkillDirectionDialog(this, _fireTowards));
      }
      else if (_lastSkill is ActionSkill)
      {
        var actionSkill = _lastSkill as ActionSkill;
        game.hero.setNextAction(
            actionSkill.getAction(game, game.hero.skills.level(actionSkill as Skill)));
      }
      else
      {
        game.log.error("No skill selected.");
        Dirty();
      }
    }
    else if (keyCode == InputX.swap)
    {
      var unequipped = game.hero.inventory.lastUnequipped;
      if (unequipped == null)
      {
        game.log.error("You aren't holding an unequipped item to swap.");
        Dirty();
      }
      else
      {
        action = new EquipAction(ItemLocation.inventory, unequipped);
      }
    }
    else if (keyCode == InputX.wizard && shift && alt)
    {
      if (Debugger.enabled)
      {
        terminal.Push(new WizardDialog(game));
      }
      else
      {
        game.log.cheat("No cheating in non-debug builds. Cheater.");
        Dirty();
      }
    }

    if (Debugger.debugSelectDepth && keyCode == KeyCode.RightBracket)
      terminal.Push(new SelectDepthPopup(game.content, game.hero.save));

    if (action != null)
      game.hero.setNextAction(action);

    return true;
  }

  public override void Active(UnityTerminal.Screen popped, object result)
  {
    if (!game.hero.needsInput)
    {
      // The player is coming back from a screen where they selected an action
      // for the hero. Give them a bit to visually reorient themselves before
      // kicking off the action.
      _pause = 10;
    }

    if (popped is ExitPopup)
    {
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
      GameScreen.town(_storage, game.content, _storageSave!, (scene)=>{
        terminal.GoTo(scene);
      });
    }
    else if (popped is SelectDepthPopup && result is int)
    {
      // Enter the dungeon.
      _storage.save();
      terminal.Push(new LoadingDialog(game.hero.save, game.content, (int)result));
    }
    else if (popped is LoadingDialog)
    {
      terminal.GoTo(new GameScreen(_storage, result as Game, game.hero.save));
    }
    else if (popped is ForfeitPopup && result is bool && (bool)result == true)
    {
      if (game.depth > 0)
      {
        // Forfeiting, so return to the town and discard the current hero.
        // TODO: Hero should start next to dungeon entrance.
        GameScreen.town(_storage, game.content, _storageSave!, (scene)=>{
          terminal.GoTo(scene);
        });
      }
      else
      {
        // Leaving the town. Save just to be safe.
        _storage.save();
        terminal.Pop();
      }
    }
    else if (popped is ItemScreen)
    {
      // Always save when leaving home or a shop.
      _storage.save();
    }
    else if (popped is ItemDialog)
    {
      // Save after changing items in the town.
      if (game.depth == 0) _storage.save();
    }
    else if (popped is SkillDialog)
    {
      // TODO: Once skills can be learned on the SkillDialog again, make this
      // work.
      //      game.hero.updateSkills(result);
    }
    else if (popped is SelectSkillDialog && result != null)
    {
      if (result is TargetSkill)
      {
        _openTargetDialog((TargetSkill)result);
      }
      else if (result is DirectionSkill)
      {
        terminal.Push(new SkillDirectionDialog(this, (dir) =>
        {
          _lastSkill = (UsableSkill)result;
          _fireTowards(dir);
        }));
      }
      else if (result is ActionSkill)
      {
        _lastSkill = (UsableSkill)result;
        game.hero.setNextAction(
            (result as ActionSkill).getAction(game, game.hero.skills.level((Skill)result)));
      }
    }
  }

  public override void Tick(float dt)
  {
    if (_enterPortal()) return;

    if (_pause > 0)
    {
      _pause--;
      return;
    }

    var result = game.update();

    // See if the hero died.
    if (!game.hero.isAlive)
    {
      terminal.GoTo(new GameOverScreen(game.log));
      return;
    }

    if (_stagePanel.update(result.events)) Dirty();

    if (result.needsRefresh) Dirty();
  }

  public override void Resize(int width, int height)
  {
    var size = new Vec(width, height);

    var leftWidth = 21;

    if (size > 160)
    {
      leftWidth = 29;
    }
    else if (size > 150)
    {
      leftWidth = 25;
    }

    var centerWidth = size.x - leftWidth;

    itemPanel.hide();
    if (size.x >= 100)
    {
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

  public override void Render(Terminal terminal)
  {
    terminal.Clear();

    _stagePanel.render(terminal);
    _logPanel.render(terminal);
    // Note, this must be rendered after the stage panel so that the visible
    // monsters are correctly calculated first.
    _sidebarPanel.render(terminal);
    itemPanel.render(terminal);
  }

  /// Handle the hero stepping onto a portal tile.
  bool _enterPortal()
  {
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

  void _open()
  {
    // See how many adjacent closed doors there are.
    // TODO: Handle chests.
    var openable = new List<Vec>();
    foreach (var pos in game.hero.pos.neighbors)
    {
      if (game.stage[pos].type.canOpen)
      {
        openable.Add(pos);
      }
    }

    if (openable.isEmpty())
    {
      game.log.error("You are not next to anything to open.");
      Dirty();
    }
    else if (openable.Count == 1)
    {
      var pos = openable.First();
      // TODO: This leaks information if the hero is next to unexplored tiles.
      game.hero.setNextAction(game.stage[pos].type.onOpen!(pos));
    }
    else
    {
      terminal.Push(new OpenDialog(this));
    }
  }

  void _closeDoor()
  {
    // See how many adjacent open doors there are.
    var closeable = new List<Vec>();
    foreach (var pos in game.hero.pos.neighbors)
    {
      if (game.stage[pos].type.canClose)
      {
        closeable.Add(pos);
      }
    }

    if (closeable.isEmpty())
    {
      game.log.error("You are not next to an open door.");
      Dirty();
    }
    else if (closeable.Count == 1)
    {
      var pos = closeable.First();
      // TODO: This leaks information if the hero is next to unexplored tiles.
      game.hero.setNextAction(game.stage[pos].type.onClose!(pos));
    }
    else
    {
      terminal.Push(new CloseDialog(this));
    }
  }

  void _pickUp()
  {
    var items = game.stage.itemsAt(game.hero.pos);
    if (items.length > 1)
    {
      // Show item dialog if there are multiple things to pick up.
      terminal.Push(ItemDialog.pickUp(this));
    }
    else if (items.length == 1)
    {
      // Otherwise attempt to pick the one item.
      game.hero.setNextAction(new PickUpAction(items.First()));
    }
    else
    {
      game.log.error("There is nothing here.");
      Dirty();
    }
  }

  void _openTargetDialog(TargetSkill skill)
  {
    terminal.Push(
        new TargetDialog(this, skill.getRange(game), (_) => _fireAtTarget(skill)));
  }

  void _fireAtTarget(TargetSkill skill)
  {
    if (currentTarget == game.hero.pos && !skill.canTargetSelf())
    {
      game.log.error("You can't target yourself.");
      Dirty();
      return;
    }

    _lastSkill = skill as UsableSkill;
    // TODO: It's kind of annoying that we force the player to select a target
    // or direction for skills that spend focus/fury even when they won't be
    // able to perform it. Should do an early check first.
    game.hero.setNextAction(skill.getTargetAction(
        game, game.hero.skills.level(skill as Skill), currentTarget!));
  }

  void _fireTowards(Direction dir)
  {
    // If the user canceled, don't fire.
    if (dir == Direction.none) return;

    if (_lastSkill is DirectionSkill)
    {
      var directionSkill = _lastSkill as DirectionSkill;
      game.hero.setNextAction(directionSkill.getDirectionAction(
          game, game.hero.skills.level(directionSkill as Skill), dir));
    }
    else if (_lastSkill is TargetSkill)
    {
      var targetSkill = _lastSkill as TargetSkill;
      var pos = game.hero.pos + dir;

      // Target the monster that is in the fired direction, if any.
      Vec previous = null;
      foreach (var step in new Line(game.hero.pos, pos))
      {
        // If we reached an actor, target it.
        var actor = game.stage.actorAt(step);
        if (actor != null)
        {
          targetActor(actor);
          break;
        }

        // If we hit a wall, target the floor tile before it.
        if (game.stage[step].blocksView)
        {
          targetFloor(previous);
          break;
        }

        // If we hit the end of the range, target the floor there.
        if ((step - game.hero.pos) >= targetSkill.getRange(game))
        {
          targetFloor(step);
          break;
        }

        previous = step;
      }

      if (currentTarget != null)
      {
        game.hero.setNextAction(targetSkill.getTargetAction(
            game, game.hero.skills.level(targetSkill as Skill), currentTarget!));
      }
      else
      {
        var tile = game.stage[game.hero.pos + dir].type.name;
        game.log.error($"There is a {tile} in the way.");
        Dirty();
      }
    }
    else if (_lastSkill is ActionSkill)
    {
      game.log.error($"{(_lastSkill as Skill).useName} does not take a direction.");
      Dirty();
    }
    else
    {
      // TODO: Better error message.
      game.log.error("No skill selected.");
      Dirty();
    }
  }

  void _enterShop(int index)
  {
    var shops = game.hero.save.shops.Keys.ToList();
    if (index >= shops.Count) return;

    terminal.Push(ItemScreen.shop(this, game.hero.save.shops[shops[index]]!));
  }
}
