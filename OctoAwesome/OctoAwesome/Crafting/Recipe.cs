using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OctoAwesome.Crafting;

/// <summary>
/// Describes how specific items can be crafted into different items under specified conditions.
/// </summary>
public class Recipe
{
    /// <summary>
    /// Gets or sets the name of this <see cref="Recipe"/>.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the <see cref="Recipe"/> type used by machines to find the recipes they can process.
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// Gets or sets an array of items needed by a machine to process this <see cref="Recipe"/>.
    /// </summary>
    public RecipeItem[] Inputs { get; set; }
    /// <summary>
    /// Gets or sets an array of items created by a machine after processing this <see cref="Recipe"/>-
    /// </summary>
    public RecipeItem[] Outputs { get; set; }

    /// <summary>
    /// Gets or sets the energy the recipe needs to be processed. <c>null</c> to use <see cref="Time"/> for processing.
    /// </summary>
    /// <remarks>Either <see cref="Energy"/> or <see cref="Time"/> needs to be set.</remarks>
    public int? Energy { get; set; }

    /// <summary>
    /// Gets or sets the time[ms] the recipe needs to be processed. Defaults to (Energy * 40).
    /// </summary>
    /// <remarks>Either <see cref="Energy"/> or <see cref="Time"/> needs to be set.</remarks>
    public int? Time { get; set; }

    /// <summary>
    /// Gets or sets the minimum time[ms] this <see cref="Recipe"/> takes to be processed. Default value is <c>null</c>.
    /// <remarks><c>null</c> means that the minimal processing time is </remarks>
    /// </summary>
    public int? MinTime { get; set; }

    /// <summary>
    /// Gets or sets an array of categories this <see cref="Recipe"/> belongs to.
    /// </summary>
    public RecipeCategory[] Category { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="Recipe"/> is enabled.
    /// </summary>
    public bool Enabled { get; set; }
    /// <summary>
    /// Gets or sets the description of this <see cref="Recipe"/>.
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Gets or sets an array of keywords which help for searching for this <see cref="Recipe"/>.
    /// </summary>
    public string[] Keywords { get; set; }

    /// <summary>
    /// Gets or sets an array of recipe names that will be overridden by this <see cref="Recipe"/>.
    /// </summary>
    public string[] Overrides { get; set; }

    /// <summary>
    /// Gets or sets the additional data that can be set by extensions.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> ExtensionData { get; set; }
}

