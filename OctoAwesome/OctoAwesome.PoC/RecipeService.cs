using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.PoC;

public class RecipeService
{
    public IReadOnlyCollection<Recipe> Recipes { get; }

    List<Recipe> recipes = new(); //rässipie

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
        throw new NotImplementedException();
    }

    internal Recipe? GetByInput(RecipeItem input, IReadOnlyCollection<Recipe> recipes)
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

    public Recipe? GetByInputs(IReadOnlyCollection<RecipeItem> inputs, IReadOnlyCollection<Recipe> recipes)
    {
        foreach (var recipe in recipes) //Order by most limited
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
                return recipe;
        }

        return null;
    }
    public IReadOnlyCollection<Recipe> GetMultipleByInputs(List<RecipeItem> inputs, IReadOnlyCollection<Recipe> recipes)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<Recipe> GetByInput(RecipeItem input)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<Recipe> GetByOutput(RecipeItem output)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<Recipe> Search(string query)
    {
        throw new NotImplementedException();
    }
}
