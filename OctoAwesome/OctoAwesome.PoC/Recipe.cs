namespace OctoAwesome.PoC;

public class Recipe
{
    public string Name { get; set; }
    public string Type { get; set; }
    public RecipeItem[] Inputs { get; set; }
    public RecipeItem[] Outputs { get; set; }
    public int? Energy { get; set; }
    public int? Time { get; set; }
    public int? MinTime { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public bool Enabled { get; set; }
    public string Description { get; set; }
    public string[] Keywords { get; set; }

    public string[] Overwrites { get; set; }
}
