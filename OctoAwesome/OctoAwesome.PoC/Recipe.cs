namespace OctoAwesome.PoC;

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
    
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public bool Enabled { get; set; }
    public string Description { get; set; }
    public string[] Keywords { get; set; }

    public string[] Overwrites { get; set; }
}
