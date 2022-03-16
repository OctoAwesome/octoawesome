using OctoAwesome.Definitions.Items;
using OctoAwesome.EntityComponents;

using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.PoC;

public static class Program
{
    class Workbench
    {
        private readonly RecipeService recipeService;
        private IReadOnlyCollection<Recipe> recipes;

        public List<RecipeItem> Inputs { get; } = new();
        public List<RecipeItem> Outputs { get; } = new();

        public Workbench(RecipeService recipeService)
        {
            this.recipeService = recipeService;
        }

        public void Initialize()
        {
            recipes = recipeService.GetByType("workbench");
        }

        public void Update()
        {
            if (!Inputs.Any())
                return;

            recipeService.GetByInputs(Inputs, recipes);
        }
    }

    class Furnace
    {
        private readonly RecipeService recipeService;
        private IReadOnlyCollection<Recipe> recipes;
        private Recipe? currentRecipe;
        private int requiredForRecipe;

        public InventoryComponent Input { get; set; } = new();
        public InventoryComponent Outputs { get; } = new();

        public Furnace(RecipeService recipeService)
        {
            this.recipeService = recipeService;
        }

        public void Initialize()
        {
            recipes = recipeService.GetByType("furnace");
        }

        public void OnInputSlotUpdate()
        {
            if (Input is null)
                return;
            var inputSlot = Input.Inventory.FirstOrDefault();
            if (inputSlot is null)
                return;
            var recipe = recipeService.GetByInput(new RecipeItem(inputSlot.Definition.Name, inputSlot.Amount), recipes);
            if (recipe is null)
                return;
            currentRecipe = recipe;

            requiredForRecipe = currentRecipe.Energy ?? currentRecipe.Time!.Value;
            if (requiredForRecipe < 0)
                return;
        }

        public void Update()
        {

            if (requiredForRecipe <= 0)
            {
                //TODO Fragen die sich Maxi stellen tut:
                /*
                    - Wie also klar Zeitberechnung im Update?
                    - Auch die Energieberchnung? Theoretisch
                    - Output vom Input aus Berechnen / generieren / ...?
                    - Betriebsmittel (Kleber, Rohöl, Feuer, ...)?
                    - Für den Rezeptservice:
                        - Wie wird das relevanteste Rezept selektiert? (Kriterien, Demokratie?)
                 */

                Input = null;
                Outputs.AddSlot(currentRecipe!.Outputs);
            }
        }
    }

    static void Main()
    {

        Console.ReadLine();
    }
}
