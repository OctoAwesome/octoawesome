using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OctoAwesome.Crafting;


public class Recipe
{
    public string Name { get; set; }
    public string Type { get; set; }
    public RecipeItem[] Inputs { get; set; }
    public RecipeItem[] Outputs { get; set; }

    // 10 x Wood Amount
    public int? Energy { get; set; }
    //XOR Realtime Milliseconds, 400 per Wood
    public int? Time { get; set; }

    // Optional
    public int? MinTime { get; set; }

    public RecipeCategory[] Category { get; set; }
    public bool Enabled { get; set; }
    public string Description { get; set; }
    public string[] Keywords { get; set; }

    public string[] Overwrites { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; set; }
}

