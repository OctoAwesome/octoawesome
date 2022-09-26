using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using OctoAwesome.Extension;

namespace OctoAwesome.Crafting;

/// <summary>
/// Class containing information about the category a recipe belongs to.
/// </summary>
public class RecipeCategory
{
    private string? name, subCategory;

    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    public string Name
    {
        get => NullabilityHelper.NotNullAssert(name, $"{nameof(Name)} was not initialized!");
        set => name = NullabilityHelper.NotNullAssert(value, $"{nameof(Name)} cannot be initialized with null!");
    }

    /// <summary>
    /// Gets or sets the name of the sub-category.
    /// </summary>
    public string SubCategory
    {
        get => NullabilityHelper.NotNullAssert(subCategory, $"{nameof(SubCategory)} was not initialized!");
        set => subCategory = NullabilityHelper.NotNullAssert(value, $"{nameof(SubCategory)} cannot be initialized with null!");
    }

    /// <summary>
    /// Gets or sets the additional data that can be set by extensions.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }
}

