﻿using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.PoC;

public class RecipeService
{
    public IReadOnlyCollection<Recipe> Recipes { get; }

    List<Recipe> recipes = new(); //rässipie

    public void Load(string[] paths)
    {
        foreach (string path in paths)
        {
            var recipes = Directory.GetFiles(path, "*.json");
            foreach (var item in recipes)
            {
                var recipe = System.Text.Json.JsonSerializer.Deserialize<Recipe>(item);
                //TODO Check validity of recipe before adding and write exception otherwise
                this.recipes.Add(recipe);
            }
        }
    }

    public IReadOnlyCollection<Recipe> GetByType(string type)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<Recipe> GetByCategory(string categorie)
    {
        throw new NotImplementedException();
    }

    internal Recipe? GetByInput(RecipeItem input, IReadOnlyCollection<Recipe> recipes)
    {
        foreach (var recipe in recipes)
        {
            foreach (var inputItem in recipe.Inputs)
            {
                if (inputItem == input)
                    return recipe;
            }
        }

        return null;
    }

    public Recipe? GetByInputs(List<RecipeItem> inputs, IReadOnlyCollection<Recipe> recipes)
    {
        throw new NotImplementedException();
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
