using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GameContent : Content {
    public static Content createContent() {
        // Note: The order is significant here. For example, monster drops will
        // reference items, which need to have already been created.
        Items.initialize();
        Monsters.initialize();
        Affixes.initialize();
        Shops.initialize();
        FloorDrops.initialize();
        ArchitecturalStyle.initialize();
        Decor.initialize();

        return new GameContent();
    }

  Iterable<string> buildStage(
      Lore lore, Stage stage, int depth, Function(Vec) placeHero) {
    if (depth == 0) return Town(stage).buildStage(placeHero);
    return Architect(lore, stage, depth).buildStage(placeHero);
  }

  Affix findAffix(string name) => Affixes.find(name);
  Breed? tryFindBreed(string name) => Monsters.breeds.tryFind(name);
  ItemType? tryFindItem(string name) => Items.types.tryFind(name);
  Skill findSkill(string name) => Skills.find(name);

  Iterable<Breed> get breeds => Monsters.breeds.all;
  List<HeroClass> get classes => Classes.all;
  Iterable<Element> get elements => Elements.all;
  Iterable<ItemType> get items => Items.types.all;
  List<Race> get races => Races.all;
  Iterable<Skill> get skills => Skills.all;
  Map<string, Shop> get shops => Shops.all;

  HeroSave createHero(string name, [Race? race, HeroClass? heroClass]) {
    race ??= Races.human;
    heroClass ??= Classes.adventurer;

    var hero = HeroSave(name, race, heroClass);

    var initialItems = {
      "Mending Salve": 3,
      "Scroll of Sidestepping": 2,
      "Tallow Candle": 4,
      "Loaf of Bread": 5
    };

    initialItems.forEach((type, amount) {
      hero.inventory.tryAdd(Item(Items.types.find(type), amount));
    });

    heroClass.startingItems.dropItem(1, hero.inventory.tryAdd);

    // TODO: Instead of giving the player access to all shops at once, consider
    // letting the rescue shopkeepers from the dungeon to unlock better and
    // better shops over time.
    // Populate the shops.
    for (var shop in shops.values) {
      hero.shops[shop] = shop.create();
    }

    return hero;
  }

  // TODO: Putting this right here in content is kind of lame. Is there a
  // better place for it?
  Action? updateSubstance(Stage stage, Vec pos) {
    // TODO: More interactions:
    // fire:
    // - burns fuel (amount) and goes out when it hits zero
    // - if tile is not burning but is burnable and has nearby fire, random
    //   chance of catching fire
    // - burns actors (set on fire?)
    // - changes tiles -> table to ashes, etc.
    // water:
    // - diffuses
    //   - will put out fire
    // - tile has absorption rate that reduces amount each turn
    // - move actors?
    // - move items
    // - make tile non-walkable
    // poison:
    // - diffuses like gas
    // - disappates
    // - does not diffuse into fire or water
    // - poisons actors
    // cold:
    // - does not spread much, if at all
    // - slowly thaws
    // - freezes actors
    var tile = stage[pos];

    if (tile.substance == 0) {
      // TODO: Water first.

      if (_tryToIgniteTile(stage, pos, tile)) {
        // Done.
      } else {
        _spreadPoison(stage, pos, tile);
      }

      // TODO: Cold?
    } else {
      if (tile.element == Elements.fire) {
        // Consume fuel.
        tile.substance--;

        if (tile.substance <= 0) {
          // If the floor itself burns, change its type. If it's only burning
          // because of items on it, don't.
          if (Tiles.ignition(tile.type) > 0) {
            tile.type = rng.item(Tiles.burnResult(tile.type));
          }

          stage.floorEmanationChanged();
        } else {
          return BurningFloorAction(pos);
        }
      } else if (tile.element == Elements.poison) {
        _spreadPoison(stage, pos, tile);

        if (tile.substance > 0) return PoisonedFloorAction(pos);
      }

      // TODO: Cold.
      // TODO: Water.
    }

    return null;
  }

  /// Attempts to catch [tile] on fire.
  bool _tryToIgniteTile(Stage stage, Vec pos, Tile tile) {
    // See if this tile catches fire.
    var ignition = Tiles.ignition(tile.type);
    if (ignition == 0) return false;

    // The more neighboring tiles on fire, the greater the chance of this
    // one catching fire.
    var fire = 0;

    void neighbor(int x, int y, int amount) {
      var neighbor = stage.get(pos.x + x, pos.y + y);
      if (neighbor.substance == 0) return;
      if (neighbor.element == Elements.fire) fire += amount;
    }

    neighbor(-1, 0, 3);
    neighbor(1, 0, 3);
    neighbor(0, -1, 3);
    neighbor(0, 1, 3);
    neighbor(-1, -1, 2);
    neighbor(-1, 1, 2);
    neighbor(1, -1, 2);
    neighbor(1, 1, 2);

    // TODO: Subtract neighboring cold?

    if (fire <= rng.range(50 + ignition)) return false;

    var fuel = Tiles.fuel(tile.type);
    tile.substance = rng.range(fuel ~/ 2, fuel);
    tile.element = Elements.fire;
    stage.floorEmanationChanged();
    return true;
  }

  void _spreadPoison(Stage stage, Vec pos, Tile tile) {
    if (!tile.isFlyable) return;

    // Average the poison with this tile and its neighbors.
    var poison = tile.element == Elements.poison ? tile.substance * 4 : 0;
    var open = 4;

    void neighbor(int x, int y) {
      var neighbor = stage.get(pos.x + x, pos.y + y);

      if (neighbor.isFlyable) {
        open++;
        if (neighbor.element == Elements.poison) poison += neighbor.substance;
      }
    }

    neighbor(-1, 0);
    neighbor(1, 0);
    neighbor(0, -1);
    neighbor(0, 1);

    // Round down so that poison gradually decays.
    poison = (poison / open).round();

    tile.element = Elements.poison;
    tile.substance = (poison - 1).clamp(0, 255);
  }
}