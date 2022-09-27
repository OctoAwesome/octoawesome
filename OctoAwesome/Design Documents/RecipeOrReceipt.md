### Cooking Recipe Sample
``` json
{
    "Name": "OctoAwesome.Basics:CookMeat",
    "Inputs":[
        {
            "ItemName":"RawMeat",
            "Count":1
        }
    ],
    "Outputs":[
        {
            "ItemName":"CookedMeat",
            "Count":1
        }
    ],
    "Time": 900,
    "MinTime": 180,
    "Categories": [ 
        {"Name": "Cooking", "SubCategory": "Food"},
    ], 
    "Enabled":true, //Selfexplenatory
    "Description":"Cooks raw meat into cooked meat", //INTL Text for user
    "Keywords":"Cooking,CookedMeat,RawMeat" //Search, Filter
}
```

### Iron Smelting Recipe Sample
``` json
{
    "Inputs":[
        {
            "ItemName":"IronOre",
            "Count":5
        }
    ],
    "Outputs":[
        {
            "ItemName":"Iron",
            "Count":2,
        },
        {
            "ItemName":"Slag",
            "Count":3,
        }
    ],
    "Energy":70,
    "Categories": [ 
        {"Name": "Smelting", "SubCategory": "Metal"},
    ], 
    "Enabled":true, //Selfexplenatory
    "Description":"Smelts Iron Ore into Iron", //INTL Text for user
    "Keywords":"Smelting,IronOre,Iron" //Search, Filter
}
```

1.4 W / g, 0.070min Schmelzzeit

31MJ/kg Koks
25t Eisen

8060kg Koks / 1t Eisen

____

* Energy: 500 wh für 200g Fleisch
* Zeit:   15 Minuten für 200g Fleisch
* Combined: Mind. 3 Minuten, Zeit 15 Minuten XOR 500 wh für 200g Fleisch

____
Entscheidung:
- Required: Zeit XOR Energy
- Optional: Mindestzeit

Rausgefallen:
- Required: Mindestzeit
- Optional: Energy OR Zeit 

[RecipeService.cs](https://github.com/OctoAwesome/octoawesome/blob/0a4e469fad1744f5c663b61779e4392d2fb117b1/OctoAwesome/OctoAwesome/Crafting/RecipeService.cs)
```csharp

//TODO Implement on a later point when needed or all discrepancies are solved
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

// TODO Is this needed, when match is finally implemented
public IReadOnlyCollection<Recipe> Search(string query)
{
    throw new NotImplementedException();
}