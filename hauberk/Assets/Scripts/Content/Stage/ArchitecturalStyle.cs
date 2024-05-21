using System.Collections;
using System.Collections.Generic;
using Mathf = UnityEngine.Mathf;
using Debug = UnityEngine.Debug;

public partial class ArchitecturalStyle
{
  public static ResourceSet<ArchitecturalStyle> styles = new ResourceSet<ArchitecturalStyle>();

  public static void initialize()
  {
    // Generic default dungeon style.
    // TODO: Variations on this.
    dungeon();

    // TODO: Decide if we should ever do full-size keeps anymore.
    //    addStyle("keep",
    //        startFrequency: 2.0,
    //        endFrequency: 5.0,
    //        decor: "keep",
    //        decorDensity: 0.07,
    //        create: () => Keep());

    // TODO: Define more.
    // TODO: More catacomb styles with different tile types and tuned params.
    catacomb("bat bug humanoid natural",
        startFrequency: 1.0, endFrequency: 2.0);
    cavern("animal bat bug natural", startFrequency: 0.2, endFrequency: 1.0);

    // TODO: Forest style that uses cavern-like CA to open an organic-shaped
    // area and then fills it with grass and trees. (Maybe just a specific
    // painter for Cavern?

    // TODO: Different liquid types including some that are dry.
    // TODO: Shore or islands?
    lake("animal herp", start: 1, end: 100);
    river("animal herp", start: 1, end: 100);

    // Pits.
    pit("bug", start: 1, end: 40);
    pit("jelly", start: 5, end: 50);
    pit("bat", start: 10, end: 40);
    pit("rodent", start: 1, end: 50);
    pit("snake", start: 8, end: 60);
    pit("plant", start: 15, end: 40);
    pit("eye", start: 20, end: 100);
    pit("dragon", start: 60, end: 100);

    // Keeps.
    keep("kobold", start: 2, end: 16);
    keep("goblin", start: 5, end: 23);
    keep("saurian", start: 10, end: 30);
    keep("orc", start: 28, end: 40);
    // TODO: More.
  }

  public static List<ArchitecturalStyle> pick(int depth)
  {
    var result = new List<ArchitecturalStyle> { };

    // TODO: Change count range based on depth?
    var count = Mathf.Min(Rng.rng.taper(1, 10), 5);
    var hasFillable = false;

    while (!hasFillable || result.Count < count)
    {
      var style = styles.tryChoose(depth)!;
      Debugger.log("style > " + style.name + " - " + style.canFill);

      // Make sure there's at least one style that can fill the entire stage.
      if (style.canFill) hasFillable = true;

      if (!result.Contains(style)) result.Add(style);
    }

    return result;
  }

  public string name;
  public string decorTheme;
  public double decorDensity;
  public List<string> monsterGroups;
  public double monsterDensity;
  public double itemDensity;
  public System.Func<Architecture> _factory;
  public bool canFill;

  public ArchitecturalStyle(
      string name,
      string decorTheme,
      double? decorDensity,
      List<string> monsterGroups,
      double? monsterDensity,
      double? itemDensity,
      System.Func<Architecture> _factory,
      bool? canFill = null)
  {
    this.name = name;
    this.decorTheme = decorTheme;
    this.monsterGroups = monsterGroups;
    this._factory = _factory;

    this.decorDensity = decorDensity ?? 0.1;
    this.monsterDensity = monsterDensity ?? 1.0;
    this.itemDensity = itemDensity ?? 1.0;
    this.canFill = canFill ?? true;
  }

  public Architecture create(Architect architect, Region region)
  {
    var architecture = _factory();
    architecture.bind(this, architect, region);
    return architecture;
  }
}
