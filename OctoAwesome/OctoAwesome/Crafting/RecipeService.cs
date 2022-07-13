using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Crafting;

public class RecipeService
{
    public IReadOnlyCollection<Recipe> Recipes { get; }

    readonly List<Recipe> recipes = new();

    public void Load(params string[] paths)
    {
        foreach (string path in paths)
        {
            var recipes = Directory.GetFiles(path, "*.json");
            foreach (var item in recipes)
            {
                var recipe = System.Text.Json.JsonSerializer.Deserialize<Recipe>(File.ReadAllText(item));
                //TODO Check validity of recipe before adding and write exception otherwise
                this.recipes.Add(recipe);
            }
        }
    }

    public IReadOnlyCollection<Recipe> GetByType(string type)
    {
        return recipes.Where(x => string.IsNullOrWhiteSpace(x.Type) || x.Type.Equals(type, StringComparison.OrdinalIgnoreCase)).ToArray();

    }

    public IReadOnlyCollection<Recipe> GetByCategory(string category)
    {
        return recipes.Where(x => string.IsNullOrWhiteSpace(x.Category) || x.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToArray();
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

    public Recipe? GetByInputs(IReadOnlyCollection<Recipe> recipes, IReadOnlyCollection<RecipeItem> inputs)
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
    public IReadOnlyCollection<Recipe> GetMultipleByInputs(IReadOnlyCollection<Recipe> recipes, List<RecipeItem> inputs)
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

    public Recipe? Match(IReadOnlyCollection<Recipe> recipes, IReadOnlyCollection<RecipeItem> inputs, IReadOnlyCollection<RecipeItem> outputs, string category = "", string type = "")
    {
        foreach (var recipe in recipes.OrderBy(x => x.Inputs.Length))
        {
            if (!string.IsNullOrWhiteSpace(category) && recipe.Category != category)
                continue;
            if (!string.IsNullOrWhiteSpace(type) && recipe.Type != type)
                continue;

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
                return recipe;
        }

        return null;
    }

    public IReadOnlyCollection<Recipe> Search(string query)
    {
        throw new NotImplementedException();
    }
}
