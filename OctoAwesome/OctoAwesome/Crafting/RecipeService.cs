using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Crafting;

/// <summary>
/// Service for handling recipes.
/// </summary>
public class RecipeService
{
    /// <summary>
    /// Gets a collection of recipes managed by this service.
    /// </summary>
    public IReadOnlyCollection<Recipe> Recipes { get; }

    readonly List<Recipe> recipes = new();

    /// <summary>
    /// Load recipes from the given paths.
    /// </summary>
    /// <param name="paths">A parameter array of paths to load recipes from in .json files.</param>
    public void Load(params string[] paths)
    {
        foreach (string path in paths)
        {
            if (!Directory.Exists(path))
                continue;
            var recipes = Directory.GetFiles(path, "*.json");
            foreach (var item in recipes)
            {
                var recipe = System.Text.Json.JsonSerializer.Deserialize<Recipe>(File.ReadAllText(item));
                //TODO Check validity of recipe before adding and write exception otherwise
                this.recipes.Add(recipe);
            }
        }
    }

    /// <summary>
    /// Gets a collection of recipes matching the recipe type.
    /// </summary>
    /// <param name="type">The type name categorizing machines that can process the recipes.</param>
    /// <returns>A collection of recipes matching the recipe type.</returns>
    public IReadOnlyCollection<Recipe> GetByType(string type)
    {
        return recipes.Where(x => string.IsNullOrWhiteSpace(x.Type) || x.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).ToArray();

    }

    /// <summary>
    /// Gets a collection of recipes in the given category.
    /// </summary>
    /// <param name="category">The category to get the recipes in.</param>
    /// <returns>A collection of recipes in the given category.</returns>
    public IReadOnlyCollection<Recipe> GetByCategory(RecipeCategory category)
    {
        return recipes.Where(x => x.Category.Any(x=>x == category)).ToArray();
    }

    internal Recipe? GetByInput(IReadOnlyCollection<Recipe> recipes, RecipeItem input)
    {
        foreach (var recipe in recipes)
        {
            foreach (var inputItem in recipe.Inputs)
            {
                if (inputItem.Count <= input.Count
                    && (string.IsNullOrWhiteSpace(inputItem.ItemName) || inputItem.ItemName == input.ItemName)
                    && (string.IsNullOrWhiteSpace(inputItem.MaterialName) || inputItem.MaterialName == input.MaterialName))
                    return recipe;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets a <see cref="Recipe"/> from a given recipe collection which can be processed using the given input items.
    /// </summary>
    /// <param name="recipes">The <see cref="Recipe"/> collection to match a <see cref="Recipe"/> from.</param>
    /// <param name="inputs">The input items to filter by.</param>
    /// <returns>
    /// The recipe that can be processed using the given input,
    ///  or <c>null</c> if no matching <see cref="Recipe"/> is found.
    /// </returns>
    public static Recipe? GetByInputs(IReadOnlyCollection<Recipe> recipes, IReadOnlyCollection<RecipeItem> inputs)
    {
        Recipe? ret = null;
        int currScore = 0, currMatches = 0;

        foreach (var recipe in recipes.OrderBy(x => x.Inputs.Length))
        {
            int counter = 0, inputMatches = 0;
            
            foreach (var inputItem in recipe.Inputs)
            {
                foreach (var input in inputs)
                {
                    var count = inputItem.Count <= input.Count;
                    var itemName = string.IsNullOrWhiteSpace(inputItem.ItemName) || inputItem.ItemName == input.ItemName;
                    var materialName = string.IsNullOrWhiteSpace(inputItem.MaterialName) || inputItem.MaterialName == input.MaterialName;
                    if (count && itemName && materialName)
                    {
                        if (count)
                            counter++;

                        if (itemName)
                            counter++;

                        if (materialName)
                            counter++;

                        inputMatches++;
                    }
                }
            }

            if ((inputMatches >= recipe.Inputs.Length 
                && inputMatches > currMatches) 
                    || (inputMatches == currMatches && currScore < counter))
            {
                ret = recipe;
                currScore = counter;
                currMatches = inputMatches;
            }
        }

        return ret;
    }
    
    /// <summary>
    /// Gets a collection of recipes from a given recipe collection which can be processed using the given input items.
    /// </summary>
    /// <param name="recipes">The <see cref="Recipe"/> collection to match a collection of recipes from.</param>
    /// <param name="inputs">The input items to filter by.</param>
    /// <returns>
    /// The recipes that can be processed using the given input,
    /// </returns>
    public static IReadOnlyCollection<Recipe> GetMultipleByInputs(IReadOnlyCollection<Recipe> recipes, List<RecipeItem> inputs)
    {
        List<Recipe> retRecipes = new();
        foreach (var recipe in recipes)
        {
            int counter = 0;
            foreach (var inputItem in recipe.Inputs)
            {
                foreach (var input in inputs)
                {
                    if (inputItem.Count <= input.Count
                        && (string.IsNullOrWhiteSpace(inputItem.ItemName) || inputItem.ItemName == input.ItemName)
                        && (string.IsNullOrWhiteSpace(inputItem.MaterialName) || inputItem.MaterialName == input.MaterialName))
                    {
                        counter++;
                        break;
                    }

                }
            }
            if (counter >= recipe.Inputs.Length)
                retRecipes.Add(recipe);
        }

        return retRecipes;
    }
    
    /// <summary>
    /// Gets a collection of recipes which can be processed using the given input item.
    /// </summary>
    /// <param name="input">The input item to filter by.</param>
    /// <returns>
    /// The recipes that can be processed using the given input item,
    /// </returns>
    public IReadOnlyCollection<Recipe> GetByInput(RecipeItem input)
    {
        List<Recipe> retRecipes = new();
        foreach (var recipe in recipes)
        {
            foreach (var tinpuItem in recipe.Inputs)
            {
                if (tinpuItem.Count <= input.Count
                    && (string.IsNullOrWhiteSpace(tinpuItem.ItemName) || tinpuItem.ItemName == input.ItemName)
                    && (string.IsNullOrWhiteSpace(tinpuItem.MaterialName) || tinpuItem.MaterialName == input.MaterialName))
                {
                    retRecipes.Add(recipe);
                    break;
                }
            }
        }

        return retRecipes;
    }
    
    /// <summary>
    /// Gets a collection of recipes which can create the given output item.
    /// </summary>
    /// <param name="output">The output item to filter by.</param>
    /// <returns>
    /// The recipes that can create the given output item,
    /// </returns>
    public IReadOnlyCollection<Recipe> GetByOutput(RecipeItem output)
    {
        List<Recipe> retRecipes = new();
        foreach (var recipe in recipes)
        {
            foreach (var outputItem in recipe.Outputs)
            {
                if (outputItem.Count <= output.Count
                    && (string.IsNullOrWhiteSpace(outputItem.ItemName) || outputItem.ItemName == output.ItemName)
                    && (string.IsNullOrWhiteSpace(outputItem.MaterialName) || outputItem.MaterialName == output.MaterialName))
                {
                    retRecipes.Add(recipe);
                    break;
                }
            }
        }

        return retRecipes;
    }
}
