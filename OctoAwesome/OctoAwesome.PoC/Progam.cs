using engenious;

using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.EntityComponents;
using OctoAwesome.Runtime;

using OpenTK.Windowing.Common.Input;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using static OctoAwesome.PoC.StateMachine;

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
        private readonly DefinitionManager manager;
        private IReadOnlyCollection<Recipe> recipes;
        private Recipe? currentRecipe;
        private int requiredMillisecondsForRecipe;
        private int whEnergyUsage = 2000;

        private GameTime recipeEnd;
        private StateMachine stateMachine;

        public InventoryComponent Input { get; set; } = new();
        public InventoryComponent Outputs { get; } = new();

        public Furnace(RecipeService recipeService, DefinitionManager manager)
        {
            this.recipeService = recipeService;
            this.manager = manager;
        }

        public void Initialize()
        {
            recipes = recipeService.GetByType("furnace");

            //Idle => Recipe => Running => Output => Recipe / Idle

            var idleState = new GenericNode("idle", (elapsed, total) => true);
            var running = new GenericNode("running", HasRecipeFinished);
            var recipe = new GenericNode("recipe", (elapsed, total) => true);
            var output = new GenericNode("output", GenerateOutput);

            stateMachine = new StateMachine(idleState);
            stateMachine.AddNodes(running, recipe, output);

            stateMachine.AddTransition(idleState, recipe, () => GetRecipe() is not null);
            stateMachine.AddTransition(recipe, running, () => true);
            stateMachine.AddTransition(running, output, () => true);
            stateMachine.AddTransition(output, recipe, () => GetRecipe() is not null);
            stateMachine.AddTransition(output, idleState, () => GetRecipe() is null);
        }

        private bool GenerateOutput(TimeSpan elapsed, TimeSpan total)
        {
            var firstInputSlot = Input.GetSlotAtIndex(0);

            foreach (var item in currentRecipe!.Outputs)
            {
                var def = manager.Definitions.FirstOrDefault(x => x.DisplayName == item.ItemName);/*TODO Name not Displayname*/

                if (def is IItemDefinition itemDef)
                {
                    IMaterialDefinition? mat;

                    if (string.IsNullOrWhiteSpace(item.MaterialName))
                        mat = firstInputSlot.Item.Material;
                    else
                        mat = manager.MaterialDefinitions.FirstOrDefault(x => x.DisplayName == item.MaterialName);

                    if (mat is null)
                        continue;
                    Outputs.Add(itemDef.Create(mat), item.Count);
                }
                else if (def is IBlockDefinition blockDefinition)
                {
                    Outputs.Add(blockDefinition, item.Count);
                }

            }
            firstInputSlot.Remove(currentRecipe.Inputs[0].Count);
            return true; //We should never be two updates in this state! That would be mist
        }

        private Recipe? GetRecipe()
        {
            if (Input is null)
                return null; //Reset recipe, time etc. pp.
            var inputSlot = Input.Inventory.FirstOrDefault();
            if (inputSlot is null)
                return null; //Reset recipe, time etc. pp.
            var recipe = recipeService.GetByInput(new RecipeItem(inputSlot.Definition.DisplayName, inputSlot.Amount, inputSlot.Item.Material.DisplayName/*TODO Name not Displayname*/), recipes);
            if (recipe is null)
                return null; //Reset recipe, time etc. pp.
            return recipe;
        }

        public void OnInputSlotUpdate()
        {
            var recipe = GetRecipe();
            if (recipe is null)
                return; //Reset recipe, time etc. pp.
            currentRecipe = recipe;

            requiredMillisecondsForRecipe = currentRecipe.Time ?? currentRecipe.Energy!.Value * 40;

            if (currentRecipe.MinTime is not null && requiredMillisecondsForRecipe < currentRecipe.MinTime)
                requiredMillisecondsForRecipe = currentRecipe.MinTime!.Value;

            if (requiredMillisecondsForRecipe < 0)
                return;
        }

        public bool HasRecipeFinished(TimeSpan elapsed, TimeSpan total)
        {
            return requiredMillisecondsForRecipe <= total.Milliseconds;
            //recipeEnd.TotalGameTime < currentGameTime.TotalGameTime
            //&& recipeEnd.TotalGameTime.Add(currentGameTime.ElapsedGameTime) > currentGameTime.TotalGameTime

            //Has just finished, Input => Output
            //Check if next recipe can start => OnInputSlotUpdating, after Input => Output
            //or go to idle
        }

        public void Update(GameTime currentGameTime)
        {
            stateMachine.Update(currentGameTime.ElapsedGameTime);


            //Idle to running
            //or Running to running Fuel Consumption

            if (requiredMillisecondsForRecipe <= 0)
            {
                //TODO Fragen die sich Maxi stellen tut:
                /*
                    - Wie also klar Zeitberechnung im Update? => GameTime when finished, etc.
                    - ✓ Auch die Energieberchnung? Theoretisch => Done
                    - Output vom Input aus Berechnen / generieren / ...?
                    - Betriebsmittel (Kleber, Rohöl, Schrauben, andere Öle, Brennmittel ...), was braucht die Maschine um ihre Arbeit zu verrichten?
                        - Seperate(r) Slot für Input
                        - Teil vom Block
                        - 
                    - Für den Rezeptservice:
                        - Wie wird das relevanteste Rezept selektiert? (Kriterien, Demokratie? Ja!)
                        - Priorisierung von kollidierenden Rezepten => Aktuell ungelöst, für den Anfang FirstOrDefault (Future: UI für Priorisierung / Sortierung, etc)
                        - Pay to Prioritize (Betterplace donations oder sowas), Abomodell
                
                    - ✓ Inventory überarbeiten, weil Slot hinzufügen / entfernen doof, wenn es fixe Slots geben sollte

                    - Placeholder für InputMaterial = Output Material
                    - Rezept als optionalen Input für Item => Dann können Rezepte von außen überschrieben werden vor der Verarbeitung des Inputs

                 */

                /*
                 Notes:
                    - Output Material = Input Material, when no explicit material is supplied
                 */

                //Input = null;
                //Outputs.AddSlot(currentRecipe!.Outputs);
            }
        }
    }

    static void Main()
    {
        var rs = new RecipeService();
        rs.Load(@"../../../Recipes");
        var dfm = new DefinitionManager(new Extension.ExtensionService());
        TypeContainer.Register<IDefinitionManager, DefinitionManager>(dfm);
        TypeContainer.Register<DefinitionManager, DefinitionManager>(dfm);


        var furnace = new Furnace(rs, dfm);
        furnace.Initialize();
        GameTime gameTime;
        var swTotal = new Stopwatch();
        swTotal.Start();
        var swLast = new Stopwatch();
        while (true)
        {
            gameTime = new GameTime(swTotal.Elapsed, swLast.Elapsed);
            furnace.Update(gameTime);
            swLast.Restart();
        }
        Console.ReadLine();
    }
}
