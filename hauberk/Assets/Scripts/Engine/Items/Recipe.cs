using System.Collections;
using System.Collections.Generic;

using System.Linq;

/// A recipe defines a set of items that can be placed into the crucible and
/// transmuted into something new.
class Recipe {
    /// Maps each required item type to the count of items of that type that are
    /// needed.
    public Dictionary<ItemType, int> ingredients;
    public Drop result;

    // TODO: Instead of hard-coding the word wrapping here, wrap it in the UI.
    /// If this recipe results in a specific item, [produces] will store that
    /// item's name. Otherwise, [produces] will be null.
    public List<string> produces;

    Recipe(Dictionary<ItemType, int> ingredients, Drop result, List<string> produces)
    {
        this.ingredients = ingredients;
        this.result = result;
        this.produces = produces;
    }

    /// Returns `true` if [items] are valid (but not necessarily complete)
    /// ingredients for this recipe.
    bool allows(List<Item> items) => _missingIngredients(items) != null;

    /// Returns `true` if [items] are the complete ingredients needed for this
    /// recipe.
    bool isComplete(List<Item> items) {
        var missing = _missingIngredients(items);
        return missing != null && missing.Count == 0;
    }

    /// Gets the remaining ingredients needed to complete this recipe given
    /// [items] ingredients. Returns `null` if [items] contains any ingredients
    /// that are not used by this recipe.
    Dictionary<ItemType, int> _missingIngredients(List<Item> items) {
        var missing = new Dictionary<ItemType, int>(ingredients);
        foreach (var item in items) {
            if (!missing.ContainsKey(item.type)) return null;
            missing[item.type] = missing[item.type]! - item.count;
        }

        // Remove the ingredients that are complete.
        foreach (var ingredient in missing.Keys.ToList()) {
            if (missing[ingredient]! <= 0) missing.Remove(ingredient);
        }

        return missing;
    }
}
