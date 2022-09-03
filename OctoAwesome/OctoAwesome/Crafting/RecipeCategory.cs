using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OctoAwesome.Crafting;

/// <summary>
/// Class containing information about the category a recipe belongs to.
/// </summary>
public class RecipeCategory
{
    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the name of the sub-category.
    /// </summary>
    public string SubCategory { get; set; }
    /// <summary>
    /// Gets or sets the additional data that can be set by extensions.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; set; }
}

