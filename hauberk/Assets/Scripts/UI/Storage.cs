using System;
using System.Collections.Generic;
using System.Linq;
using PlayerPrefs = UnityEngine.PlayerPrefs;
using Debug = UnityEngine.Debug;
// using LitJson;
// using SimpleJSON;
using SimpleJson;

/// The entrypoint for all persisted save data.
class Storage
{
  public Content content;
  public List<HeroSave> heroes = new List<HeroSave>();

  public Storage(Content content)
  {
    this.content = content;
    _load();
  }

  void _load()
  {
    // TODO: For debugging. If the query is "?clear", then ditch saved heroes.
    // if (html.window.location.search == '?clear') {
    //   save();
    //   return;
    // }

    var storage = PlayerPrefs.GetString("heroes", null);
    if (storage == null || string.IsNullOrEmpty(storage)) return;

    var data = SimpleJson.SimpleJson.DeserializeObject(storage) as IDictionary<string, object>;
    if (data == null)
      return;

    // TODO: Check version.

    var heros = data["heroes"] as IList<object>;
    foreach (var hero_ in heros)
    {
      var hero = hero_ as IDictionary<string, object>;

      var name = hero["name"] as string;
      var race = _loadRace(hero["race"] as IDictionary<string, object>);

      HeroClass heroClass;
      if (hero["class"] == null)
      {
        // TODO: Temp for characters before classes.
        heroClass = content.classes[0];
      }
      else
      {
        var className = hero["class"] as string;
        heroClass = content.classes.First((c) => c.name == className);
      }

      var inventoryItems = _loadItems(hero["inventory"] as List<object>);
      var inventory = new Inventory(
          ItemLocation.inventory, Option.inventoryCapacity, inventoryItems);

      var equipment = new Equipment();
      foreach (var item in _loadItems(hero["equipment"] as List<object>))
      {
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
      var shops = new Dictionary<Shop, Inventory> { };
      if (hero.ContainsKey("shops"))
      {
        foreach (var kv in content.shops)
        {
          var shopName = kv.Key;
          var shop = kv.Value;

          var kk = hero["shops"] as IDictionary<string, object>;
          var shopData = kk[shopName] as IList<object>;
          if (shopData != null)
          {
            shops[shop] = shop.load(_loadItems(shopData));
          }
          else
          {
            Debugger.logError($"No data for {shopName}, so regenerating.");
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

      var experience = Convert.ToInt32(hero["experience"]);

      var levels = new Dictionary<Skill, int> { };
      var points = new Dictionary<Skill, int> { };
      var skills = hero["skills"] as IDictionary<string, object>;
      if (skills != null)
      {
        foreach (var skillName in skills.Keys)
        {
          var skill = content.findSkill(skillName);
          // Handle old storage without points.
          // TODO: Remove when no longer needed.
          if (skills[skillName] is int)
          {
            levels[skill] = Convert.ToInt32(skills[skillName]);
            points[skill] = 0;
          }
          else
          {
            var skillInfo = skills[skillName] as IDictionary<string, object>;
            levels[skill] = Convert.ToInt32(skillInfo["level"]);
            points[skill] = Convert.ToInt32(skillInfo["points"]);
          }
        }
      }

      var skillSet = new SkillSet(levels, points);

      var lore = _loadLore(hero["lore"] as IDictionary<string, object>);

      var gold = Convert.ToInt32(hero["gold"]);
      var maxDepth = 0;
      if (hero.ContainsKey("maxDepth"))
        maxDepth = Convert.ToInt32(hero["maxDepth"]);

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
    }
  }

  RaceStats _loadRace(IDictionary<string, object> data)
  {
    // TODO: Temp to handle heros from before races.
    if (data == null)
    {
      return content.races.elementAt(4).rollStats();
    }

    var name = data["name"] as string;
    var race = content.races.First((race) => race.name == name);

    var statData = data["stats"] as IDictionary<string, object>;
    var stats = new Dictionary<Stat, int> { };

    foreach (var stat in Stat.all)
    {
      if (statData.ContainsKey(stat.name))
      {
        stats[stat] = Convert.ToInt32(statData[stat.name]);
      }
    }

    // TODO: 1234 is temp for characters without seed.
    var seed = 1234;
    if (data.ContainsKey("seed"))
      seed = Convert.ToInt32(data["seed"]);

    return new RaceStats(race, stats, seed);
  }

  List<Item> _loadItems(IList<object> data)
  {
    var items = new List<Item>();
    if (data == null)
      return items;

    foreach (var itemData in data)
    {
      var item = _loadItem(itemData as IDictionary<string, object>);
      if (item != null) items.Add(item);
    }

    return items;
  }

  Item _loadItem(IDictionary<string, object> data)
  {
    var type = content.tryFindItem(data["type"] as string);
    if (type == null)
    {
      Debugger.logError($"Couldn't find item type {data["type"]}, discarding item.");
      return null;
    }

    var count = 1;
    // Existing save files don't store count, so allow it to be missing.
    if (data.ContainsKey("count"))
    {
      count = Convert.ToInt32(data["count"]);
    }

    Affix prefix = null;
    if (data.ContainsKey("prefix"))
    {
      // TODO: Older save from back when affixes had types.
      if (data["prefix"] is string)
      {
        prefix = content.findAffix(data["prefix"] as string);
      }
      else
      {
        var kk = data["prefix"] as IDictionary<string, object>;
        prefix = content.findAffix(kk["name"] as string);
      }
    }

    Affix suffix = null;
    if (data.ContainsKey("suffix"))
    {
      // TODO: Older save from back when affixes had types.
      if (data["suffix"] is string)
      {
        suffix = content.findAffix(data["suffix"] as string);
      }
      else
      {
        var kk = data["suffix"] as IDictionary<string, object>;
        suffix = content.findAffix(kk["name"] as string);
      }
    }

    return new Item(type, count, prefix, suffix);
  }

  Lore _loadLore(IDictionary<string, object> data)
  {
    var seenBreeds = new Dictionary<Breed, int> { };
    var slain = new Dictionary<Breed, int> { };
    var foundItems = new Dictionary<ItemType, int> { };
    var foundAffixes = new Dictionary<Affix, int> { };
    var usedItems = new Dictionary<ItemType, int> { };

    // TODO: Older saves before lore.
    if (data != null)
    {
      var seenMap = data["seen"] as IDictionary<string, object>;
      if (seenMap != null)
      {
        foreach (var kv in seenMap)
        {
          var breedName = kv.Key;
          var count = Convert.ToInt32(kv.Value);

          var breed = content.tryFindBreed(breedName);
          if (breed != null) seenBreeds[breed] = count;
        };
      }

      var slainMap = data["slain"] as IDictionary<string, object>;
      if (slainMap != null)
      {
        foreach (var kv in slainMap)
        {
          var breedName = kv.Key;
          var count = Convert.ToInt32(kv.Value);

          var breed = content.tryFindBreed(breedName);
          if (breed != null) slain[breed] = count;
        };
      }

      if (data.ContainsKey("foundItems"))
      {
        var foundItemMap = data["foundItems"] as IDictionary<string, object>;
        if (foundItemMap != null)
        {
          foreach (var kv in foundItemMap)
          {
            var itemName = kv.Key;
            var count = Convert.ToInt32(kv.Value);

            var itemType = content.tryFindItem(itemName);
            if (itemType != null) foundItems[itemType] = count;
          };
        }
      }

      if (data.ContainsKey("foundAffixes"))
      {
        var foundAffixMap = data["foundAffixes"] as IDictionary<string, object>;
        if (foundAffixMap != null)
        {
          foreach (var kv in foundAffixMap)
          {
            var affixName = kv.Key;
            var count = Convert.ToInt32(kv.Value);

            var affix = content.findAffix(affixName);
            if (affix != null) foundAffixes[affix] = count;
          };
        }
      }

      if (data.ContainsKey("usedItems"))
      {
        var usedItemMap = data["usedItems"] as IDictionary<string, object>;
        if (usedItemMap != null)
        {
          foreach (var kv in usedItemMap)
          {
            var itemName = kv.Key;
            var count = Convert.ToInt32(kv.Value);

            var itemType = content.tryFindItem(itemName);
            if (itemType != null) usedItems[itemType] = count;
          };
        }
      }
    }

    return new Lore(seenBreeds, slain, foundItems, foundAffixes, usedItems);
  }

  public void save()
  {
    var heroData = new List<object>();
    foreach (var hero in heroes)
    {
      var raceStats = new Dictionary<string, object> { };
      foreach (var stat in Stat.all)
      {
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

      var shops = new Dictionary<string, object> { };
      foreach (var kv in hero.shops)
      {
        var shop = kv.Key;
        shops[shop.name] = _saveItems(kv.Value);
      };

      var skills = new Dictionary<string, object> { };
      foreach (var skill in hero.skills.discovered)
      {
        skills[skill.name] = new Dictionary<string, object>(){
          {"level", hero.skills.level(skill)},
          {"points", hero.skills.points(skill)}
        };
      }

      var seen = new Dictionary<string, object> { };
      var slain = new Dictionary<string, object> { };
      var lore = new Dictionary<string, object>{
        {"seen", seen},
        {"slain", slain}
      };
      foreach (var breed in content.breeds)
      {
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
    var edata = SimpleJson.SimpleJson.SerializeObject(data);
    PlayerPrefs.SetString("heroes", edata);
    PlayerPrefs.Save();
    Debugger.log("Saved.");
  }

  List<Dictionary<string, object>> _saveItems(IEnumerable<Item> items)
  {
    var rt = new List<Dictionary<string, object>>();
    foreach (var item in items)
      rt.Add(_saveItem(item));
    return rt;
  }

  Dictionary<string, object> _saveItem(Item item)
  {
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
