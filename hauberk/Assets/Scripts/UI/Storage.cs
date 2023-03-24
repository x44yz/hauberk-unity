using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// The entrypoint for all persisted save data.
class Storage {
  public Content content;
  public List<HeroSave> heroes = new List<HeroSave>();

  public Storage(Content content) {
    this.content = content;
    _load();
  }

  void _load() {
    // TODO: For debugging. If the query is "?clear", then ditch saved heroes.
    // if (html.window.location.search == '?clear') {
    //   save();
    //   return;
    // }

    var storage = PlayerPrefs.GetString("heroes");
    if (storage == null) return;

    var data = JsonUtility.FromJson<Dictionary<string, object>>(storage);

    // TODO: Check version.
    
    var heros = data["heroes"] as List<object>;
    foreach (var heroObj in heros) {
      try {
        var hero = heroObj as Dictionary<string, object>;

        var name = hero["name"] as string;
        var race = _loadRace(hero["race"] as Dictionary<string, object>);

        HeroClass heroClass;
        if (hero["class"] == null) {
          // TODO: Temp for characters before classes.
          heroClass = content.classes[0];
        } else {
          var className = hero["class"] as string;
          heroClass = content.classes.First((c) => c.name == className);
        }

        var inventoryItems = _loadItems(hero["inventory"] as List<object>);
        var inventory = new Inventory(
            ItemLocation.inventory, Option.inventoryCapacity, inventoryItems);

        var equipment = new Equipment();
        foreach (var item in _loadItems(hero["equipment"] as List<object>)) {
          // TODO: If there are multiple slots of the same type, this may
          // shuffle items around.
          equipment.equip(item);
        }

        var homeItems = _loadItems(hero["home"] as List<object>);
        var home = new Inventory(ItemLocation.home, Option.homeCapacity, homeItems);

        var crucibleItems = _loadItems(hero["crucible"] as List<object>);
        var crucible = new Inventory(
            ItemLocation.crucible, Option.crucibleCapacity, crucibleItems);

        // TODO: What if shops are added or changed?
        var shops = new Dictionary<Shop, Inventory>{};
        if (hero.ContainsKey("shops")) {
          foreach (var kv in content.shops) {
            var shopName = kv.Key;
            var shop = kv.Value;

            var kk = hero["shops"] as Dictionary<string, object>;
            var shopData = kk[shopName] as List<object>;
            if (shopData != null) {
              shops[shop] = shop.load(_loadItems(shopData));
            } else {
              Debug.LogError($"No data for {shopName}, so regenerating.");
              shops[shop] = shop.create();
            }
          };
        }

        // Clean up legacy heroes before item stacks.
        // TODO: Remove this once we don't need to worry about it anymore.
        inventory.countChanged();
        home.countChanged();
        crucible.countChanged();

        // Defaults are to support legacy saves.

        var experience = (int)hero["experience"];

        var levels = new Dictionary<Skill, int>{};
        var points = new Dictionary<Skill, int>{};
        var skills = hero["skills"] as Dictionary<string, object>;
        if (skills != null) {
          foreach (var skillName in skills.Keys) {
            var skill = content.findSkill(skillName);
            // Handle old storage without points.
            // TODO: Remove when no longer needed.
            if (skills[skillName] is int) {
              levels[skill] = (int)skills[skillName];
              points[skill] = 0;
            } else {
              var kk = skills[skillName] as Dictionary<string, int>;
              levels[skill] = (int)kk["level"];
              points[skill] = (int)kk["points"];
            }
          }
        }

        var skillSet = new SkillSet(levels, points);

        var lore = _loadLore(hero["lore"] as Dictionary<string, object>);

        var gold = (int)hero["gold"];
        var maxDepth = 0;
        if (hero.ContainsKey("maxDepth"))
          maxDepth = (int)hero["maxDepth"];

        var heroSave = new HeroSave(
            name,
            race,
            heroClass,
            inventory,
            equipment,
            home,
            crucible,
            shops,
            experience,
            skillSet,
            lore,
            gold,
            maxDepth);
        heroes.Add(heroSave);
      } catch (Exception ex) {
        Debug.LogError("Could not load hero. Data:");
        Debug.Log(JsonUtility.ToJson(heroObj));
        Debug.LogError($"Error:{ex}");
      }
    }
  }

  RaceStats _loadRace(Dictionary<string, object> data) {
    // TODO: Temp to handle heros from before races.
    if (data == null) {
      return content.races.elementAt(4).rollStats();
    }

    var name = data["name"] as string;
    var race = content.races.First((race) => race.name == name);

    var statData = data["stats"] as Dictionary<string, object>;
    var stats = new Dictionary<Stat, int>{};

    foreach (var stat in Stat.all) {
      if (statData.ContainsKey(stat.name)) {
        stats[stat] = (int)statData[stat.name];
      }
    }

    // TODO: 1234 is temp for characters without seed.
    var seed = 1234;
    if (data.ContainsKey("seed"))
      seed = (int)data["seed"];

    return new RaceStats(race, stats, seed);
  }

  List<Item> _loadItems(List<object> data) {
    var items = new List<Item>();
    if (data == null)
      return items;

    foreach (var itemData in data) {
      var item = _loadItem(itemData as Dictionary<string, object>);
      if (item != null) items.Add(item);
    }

    return items;
  }

  Item _loadItem(Dictionary<string, object> data) {
    var type = content.tryFindItem(data["type"] as string);
    if (type == null) {
      Debug.LogError($"Couldn't find item type {data["type"]}, discarding item.");
      return null;
    }

    var count = 1;
    // Existing save files don't store count, so allow it to be missing.
    if (data.ContainsKey("count")) {
      count = (int)data["count"];
    }

    Affix prefix = null;
    if (data.ContainsKey("prefix")) {
      // TODO: Older save from back when affixes had types.
      if (data["prefix"] is string) {
        prefix = content.findAffix(data["prefix"] as string);
      } else {
        var kk = data["prefix"] as Dictionary<string, object>;
        prefix = content.findAffix(kk["name"] as string);
      }
    }

    Affix suffix = null;
    if (data.ContainsKey("suffix")) {
      // TODO: Older save from back when affixes had types.
      if (data["suffix"] is string) {
        suffix = content.findAffix(data["suffix"] as string);
      } else {
        var kk = data["suffix"] as Dictionary<string, object>;
        suffix = content.findAffix(kk["name"] as string);
      }
    }

    return new Item(type, count, prefix, suffix);
  }

  Lore _loadLore(Dictionary<string, object> data) {
    throw new System.NotImplementedException();
    // var seenBreeds = <Breed, int>{};
    // var slain = <Breed, int>{};
    // var foundItems = <ItemType, int>{};
    // var foundAffixes = <Affix, int>{};
    // var usedItems = <ItemType, int>{};

    // // TODO: Older saves before lore.
    // if (data != null) {
    //   var seenMap = data['seen'] as Map<string, dynamic>?;
    //   if (seenMap != null) {
    //     seenMap.forEach((breedName, dynamic count) {
    //       var breed = content.tryFindBreed(breedName);
    //       if (breed != null) seenBreeds[breed] = count as int;
    //     });
    //   }

    //   var slainMap = data['slain'] as Map<string, dynamic>?;
    //   if (slainMap != null) {
    //     slainMap.forEach((breedName, dynamic count) {
    //       var breed = content.tryFindBreed(breedName);
    //       if (breed != null) slain[breed] = count as int;
    //     });
    //   }

    //   var foundItemMap = data['foundItems'] as Map<string, dynamic>?;
    //   if (foundItemMap != null) {
    //     foundItemMap.forEach((itemName, dynamic count) {
    //       var itemType = content.tryFindItem(itemName);
    //       if (itemType != null) foundItems[itemType] = count as int;
    //     });
    //   }

    //   var foundAffixMap = data['foundAffixes'] as Map<string, dynamic>?;
    //   if (foundAffixMap != null) {
    //     foundAffixMap.forEach((affixName, dynamic count) {
    //       var affix = content.findAffix(affixName);
    //       if (affix != null) foundAffixes[affix] = count as int;
    //     });
    //   }

    //   var usedItemMap = data['usedItems'] as Map<string, dynamic>?;
    //   if (usedItemMap != null) {
    //     usedItemMap.forEach((itemName, dynamic count) {
    //       var itemType = content.tryFindItem(itemName);
    //       if (itemType != null) usedItems[itemType] = count as int;
    //     });
    //   }
    // }

    // return Lore.from(seenBreeds, slain, foundItems, foundAffixes, usedItems);
  }

  public void save() {
    var heroData = new List<object>();
    foreach (var hero in heroes) {
      var raceStats = new Dictionary<string, dynamic>{};
      foreach (var stat in Stat.all) {
        raceStats[stat.name] = hero.race.max(stat);
      }

      var race = new Dictionary<string, object>(){
        {"name", hero.race.name},
        {"seed", hero.race.seed},
        {"stats", raceStats}
      };

      var inventory = _saveItems(hero.inventory);
      var equipment = _saveItems(hero.equipment);
      var home = _saveItems(hero.home);
      var crucible = _saveItems(hero.crucible);

      var shops = new Dictionary<string, object>{};
      foreach (var kv in hero.shops) {
        var shop = kv.Key;
        shops[shop.name] = _saveItems(kv.Value);
      };

      var skills = new Dictionary<string, object>{};
      foreach (var skill in hero.skills.discovered) {
        skills[skill.name] = new Dictionary<string, object>(){
          {"level", hero.skills.level(skill)},
          {"points", hero.skills.points(skill)}
        };
      }

      var seen = new Dictionary<string, object>{};
      var slain = new Dictionary<string, object>{};
      var lore = new Dictionary<string, object>{
        {"seen", seen},
        {"slain", slain}
      };
      foreach (var breed in content.breeds) {
        var count = hero.lore.seenBreed(breed);
        if (count != 0) seen[breed.name] = count;

        count = hero.lore.slain(breed);
        if (count != 0) slain[breed.name] = count;
      }

      heroData.Add(new Dictionary<string, object>(){
        {"name", hero.name},
        {"race", race},
        {"class", hero.heroClass.name},
        {"inventory", inventory},
        {"equipment", equipment},
        {"home", home},
        {"crucible", crucible},
        {"shops", shops},
        {"experience", hero.experience},
        {"skills", skills},
        {"lore", lore},
        {"gold", hero.gold},
        {"maxDepth", hero.maxDepth}
      });
    }

    // TODO: Version.
    var data = new Dictionary<string, object>{
      {"heroes", heroData}
    };

    // html.window.localStorage['heroes'] = json.encode(data);
    var edata = JsonUtility.ToJson(data);
    PlayerPrefs.SetString("heroes", edata);
    Debug.Log("Saved.");
  }

  List<Dictionary<string, object>> _saveItems(IEnumerable<Item> items) {
    var rt = new List<Dictionary<string, object>>();
    foreach (var item in items)
      rt.Add(_saveItem(item));
    return rt;
  }

  Dictionary<string, object> _saveItem(Item item) {
    var kk = new Dictionary<string, object>{
      {"type", item.type.name},
      {"count", item.count},
    };
    if (item.prefix != null) 
      kk["prefix"] = item.prefix!.name;
    if (item.suffix != null)
      kk["suffix"] = item.suffix!.name;
    return kk;
  }
}
