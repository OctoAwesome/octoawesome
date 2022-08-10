using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace OctoAwesome.Crafting;

public class RecipeItem
{

    public string ItemName { get; set; }
    public int Count { get; set; }
    public string? MaterialName { get; set; }
    public string? MaterialId { get; set; } 

    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; set; }

    public RecipeItem(string itemName, int count, string? materialName, string? materialId)
    {
        ItemName = itemName;
        Count = count;
        MaterialName = materialName;
        MaterialId = materialId;
    }
}
