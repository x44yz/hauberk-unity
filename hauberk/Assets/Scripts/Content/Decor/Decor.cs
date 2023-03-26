using System.Collections;
using System.Collections.Generic;
using System.Linq;

// TODO: Generate magical shrine/chests that let the player choose from one
// of a few items. This should help reduce the number of useless-for-this-hero
// items that are dropped.

abstract partial class Decor
{
  public static void initialize()
  {
    all.defineTags("built/room/dungeon");
    all.defineTags("built/room/keep");
    all.defineTags("catacomb");
    all.defineTags("cave/glowing-moss");
    all.defineTags("water");

    caveDecor();
    catacombDecor();
    roomDecor();
    waterDecor();

    // TODO: Doesn't look great. Remove or redo.
    //    all.addUnnamed(Blast(), 1, 0.01, "laboratory");
  }

  public static Decor choose(int depth, string theme)
  {
    if (!all.tagExists(theme)) return null;
    return all.tryChoose(depth, tag: theme);
  }

  static public ResourceSet<Decor> all = new ResourceSet<Decor>();

  public abstract bool canPlace(Painter painter, Vec pos);

  /// Adds this decor at [pos].
  public abstract void place(Painter painter, Vec pos);
}
