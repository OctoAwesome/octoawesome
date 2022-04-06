using engenious;

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
        private int requiredMillisecondsForRecipe;
        private int whEnergyUsage = 2000;

        private GameTime recipeEnd;

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
                return; //Reset recipe, time etc. pp.
            var inputSlot = Input.Inventory.FirstOrDefault();
            if (inputSlot is null)
                return; //Reset recipe, time etc. pp.
            var recipe = recipeService.GetByInput(new RecipeItem(inputSlot.Definition.DisplayName, inputSlot.Amount, inputSlot.Item.Material.DisplayName/*TODO Name not Displayname*/), recipes);
            if (recipe is null)
                return; //Reset recipe, time etc. pp.
            currentRecipe = recipe;

            requiredMillisecondsForRecipe = currentRecipe.Time ?? currentRecipe.Energy!.Value * 40;

            if(currentRecipe.MinTime is not null && requiredMillisecondsForRecipe < currentRecipe.MinTime)
                requiredMillisecondsForRecipe = currentRecipe.MinTime!.Value;

            if (requiredMillisecondsForRecipe < 0)
                return;
        }

        public void Update(GameTime currentGameTime)
        {
            //Idle to idle
            if (requiredMillisecondsForRecipe == 0)
                return;
            
            if(recipeEnd.TotalGameTime < currentGameTime.TotalGameTime 
                && recipeEnd.TotalGameTime.Add(currentGameTime.ElapsedGameTime) > currentGameTime.TotalGameTime)
            {
                //Has just finished, Input => Output
                //Check if next recipe can start => OnInputSlotUpdating, after Input => Output
                //or go to idle
                return;
            }

            //Idle to running
            //or Running to running Fuel Consumption

            if (requiredMillisecondsForRecipe <= 0)
            {
                //TODO Fragen die sich Maxi stellen tut:
                /*
                    - Wie also klar Zeitberechnung im Update? => GameTime when finished, etc.
                    - Auch die Energieberchnung? Theoretisch => Done
                    - Output vom Input aus Berechnen / generieren / ...?
                    - Betriebsmittel (Kleber, Rohöl, Schrauben, andere Öle, Brennmittel ...), was braucht die Maschine um ihre Arbeit zu verrichten?
                        - Seperate(r) Slot für Input
                        - Teil vom Block
                        - 
                    - Für den Rezeptservice:
                        - Wie wird das relevanteste Rezept selektiert? (Kriterien, Demokratie? Ja!)
                        - Priorisierung von kollidierenden Rezepten => Aktuell ungelöst, für den Anfang FirstOrDefault (Future: UI für Priorisierung / Sortierung, etc)
                        - Pay to Prioritize (Betterplace donations oder sowas), Abomodell
                
                    - Inventory überarbeiten, weil Slot hinzufügen / entfernen doof, wenn es fixe Slots geben sollte

                    - Placeholder für InputMaterial = Output Material
                    - Rezept als optionalen Input für Item => Dann können Rezepte von außen überschrieben werden vor der Verarbeitung des Inputs

                 */

                /*
                 Notes:
                    - Output Material = Input Material, when no explicit material is supplied
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
