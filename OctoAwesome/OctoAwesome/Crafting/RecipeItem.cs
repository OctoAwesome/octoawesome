using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace OctoAwesome.Crafting;

/// <summary>
/// Class describing an item used in a <see cref="Recipe"/>.
/// </summary>
public class RecipeItem
{
    /// <summary>
    /// Gets or sets the name of the item.
    /// </summary>
    public string ItemName { get; set; }
    /// <summary>
    /// Gets or sets the number of items needed.
    /// </summary>
    public int Count { get; set; }
    /// <summary>
    /// Gets or sets the name of the recipe item material.
    /// </summary>
    public string? MaterialName { get; set; }
    /// <summary>
    /// Gets or sets an identifier used to map recipe items between input and output items.
    /// </summary>
    public string? InputOutputMappingId { get; set; } 

    /// <summary>
    /// Gets or sets the additional data that can be set by extensions.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RecipeItem"/> class.
    /// </summary>
    /// <param name="itemName"></param>
    /// <param name="count"></param>
    /// <param name="materialName"></param>
    /// <param name="inputOutputMappingId"></param>
    public RecipeItem(string itemName, int count, string? materialName, string? inputOutputMappingId)
    {
        ItemName = itemName;
        Count = count;
        MaterialName = materialName;
        InputOutputMappingId = inputOutputMappingId;
    }
}
