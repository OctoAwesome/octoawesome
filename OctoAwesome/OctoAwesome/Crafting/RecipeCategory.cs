using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OctoAwesome.Crafting;

public class RecipeCategory
{
    public string Name { get; set; }
    public string SubCategory { get; set; }
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; set; }
}

