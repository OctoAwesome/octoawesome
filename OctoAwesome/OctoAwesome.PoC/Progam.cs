using engenious;

using OctoAwesome.Basics;
using OctoAwesome.Basics.Definitions.Items.Food;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.EntityComponents;
using OctoAwesome.Runtime;

using OpenTK.Windowing.Common.Input;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

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
        private readonly IDefinitionManager manager;
        private IReadOnlyCollection<Recipe> recipes;
        private Recipe? currentRecipe;
        private int requiredMillisecondsForRecipe;
        private int whEnergyUsage = 2000;

        private GameTime recipeEnd;
        private StateMachine stateMachine;

        public InventoryComponent Input { get; set; } = new();
        public InventoryComponent Outputs { get; } = new();

        public Furnace(RecipeService recipeService, IDefinitionManager manager)
        {
            this.recipeService = recipeService;
            this.manager = manager;
        }

        public void Initialize()
        {
            recipes = recipeService.GetByType("furnace");

            //Idle => Recipe => Running => Output => Recipe / Idle

            var idleState = new GenericNode("idle", (elapsed, total) => OnInputSlotUpdate());
            var running = new GenericNode("running", HasRecipeFinished);
            var recipe = new GenericNode("recipe", (elapsed, total) => true);
            var output = new GenericNode("output", GenerateOutput);

            stateMachine = new StateMachine(idleState);
            stateMachine.AddNodes(running, recipe, output);

            stateMachine.AddTransition(idleState, recipe, () => currentRecipe is not null);
            stateMachine.AddTransition(recipe, running, () => true);
            stateMachine.AddTransition(running, output, () => true);
            stateMachine.AddTransition(output, recipe, () => currentRecipe is not null);
            stateMachine.AddTransition(output, idleState, () => currentRecipe is null);
        }

        private bool GenerateOutput(TimeSpan elapsed, TimeSpan total)
        {
            if (currentRecipe.Inputs.Length == 1) 
            {
                var inputItem = currentRecipe.Inputs[0];
                var inputDef = manager.Definitions.FirstOrDefault(x => x.DisplayName == inputItem.ItemName);

                var inputSlot = Input.GetSlot(inputDef);

                foreach (var outputItem in currentRecipe.Outputs)
                {
                    var outputDef = manager.Definitions.FirstOrDefault(x => x.DisplayName == outputItem.ItemName);/*TODO Name not Displayname*/
                    if (outputDef is IItemDefinition itemDef)
                    {
                        IMaterialDefinition? mat;

                        if (string.IsNullOrWhiteSpace(outputItem.MaterialName))
                            mat = inputSlot.Item.Material;
                        else
                            mat = manager.MaterialDefinitions.FirstOrDefault(x => x.DisplayName == outputItem.MaterialName);

                        if (mat is null)
                            return false;
                        Outputs.Add(itemDef.Create(mat), outputItem.Count);
                    }
                    else if (outputDef is IBlockDefinition blockDefinition)
                    {
                        Outputs.Add(blockDefinition, outputItem.Count);
                    }
                }
                inputSlot.Remove(currentRecipe.Inputs[0].Count);
            }
            else if (currentRecipe.Inputs.Length > 1)
            {
                foreach (var inputItem in currentRecipe.Inputs)
                {
                    var inputDef = manager.Definitions.FirstOrDefault(x => x.DisplayName == inputItem.ItemName);

                    var inputSlot = Input.GetSlot(inputDef);

                    foreach (var outputItem in currentRecipe.Outputs)
                    {

                        var outputDef = manager.Definitions.FirstOrDefault(x => x.DisplayName == outputItem.ItemName);/*TODO Name not Displayname*/
                        if (outputDef is IItemDefinition itemDef)
                        {
                            IMaterialDefinition? mat;

                            if (string.IsNullOrWhiteSpace(outputItem.MaterialName))
                            {
                                if (outputItem.MaterialId != inputItem.MaterialId)
                                    continue;
                                mat = inputSlot.Item.Material;
                            }
                            else
                                mat = manager.MaterialDefinitions.FirstOrDefault(x => x.DisplayName == outputItem.MaterialName);

                            if (mat is null)
                                return false;
                            Outputs.Add(itemDef.Create(mat), outputItem.Count);
                        }
                        else if (outputDef is IBlockDefinition blockDefinition)
                        {
                            Outputs.Add(blockDefinition, outputItem.Count);
                        }
                    }
                    inputSlot.Remove(currentRecipe.Inputs[0].Count);
                }
            }
            else
            {
                foreach (var outputItem in currentRecipe.Outputs)
                {
                    if (string.IsNullOrWhiteSpace(outputItem.MaterialName))
                        continue;
                    var outputDef = manager.Definitions.FirstOrDefault(x => x.DisplayName == outputItem.ItemName);/*TODO Name not Displayname*/
                    if (outputDef is IItemDefinition itemDef)
                    {
                        IMaterialDefinition? mat;

                        mat = manager.MaterialDefinitions.FirstOrDefault(x => x.DisplayName == outputItem.MaterialName);

                        if (mat is null)
                            return false;
                        Outputs.Add(itemDef.Create(mat), outputItem.Count);
                    }
                    else if (outputDef is IBlockDefinition blockDefinition)
                    {
                        Outputs.Add(blockDefinition, outputItem.Count);
                    }
                }
            }

            OnInputSlotUpdate();

            return true; //We should never be two updates in this state! That would be mist
        }

        private Recipe? GetRecipe()
        {
            if (Input is null)
                return null; //Reset recipe, time etc. pp.
            //var inputSlot = Input.Inventory.FirstOrDefault();
            var inputs = Input.Inventory.GroupBy(x => x.Definition.DisplayName).Select(x => new RecipeItem(x.Key, x.Sum(c => c.Amount), x.First().Item.Material.DisplayName /*TODO Name not Displayname*/)).ToArray();
            if (inputs.Length == 0)
                return null; //Reset recipe, time etc. pp.
            var recipe = recipeService.GetByInputs(inputs, recipes);
            if (recipe is null)
                return null; //Reset recipe, time etc. pp.
            return recipe;
        }

        public bool OnInputSlotUpdate()
        {
            currentRecipe = GetRecipe();
            if (currentRecipe is null)
                return false; //Reset recipe, time etc. pp.

            requiredMillisecondsForRecipe = currentRecipe.Time ?? currentRecipe.Energy!.Value * 40;

            if (currentRecipe.MinTime is not null && requiredMillisecondsForRecipe < currentRecipe.MinTime)
                requiredMillisecondsForRecipe = currentRecipe.MinTime!.Value;

            if (requiredMillisecondsForRecipe < 0)
                return false;

            return true;
        }

        public bool HasRecipeFinished(TimeSpan elapsed, TimeSpan total)
        {
            return requiredMillisecondsForRecipe <= total.TotalMilliseconds;
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

    public static void Main()
    {
        var rs = new RecipeService();
        rs.Load(@"C:\Users\susch\source\repos\OctoAwesome\octoawesome\OctoAwesome\OctoAwesome.PoC\Recipes");
        //var dfm = new DefinitionManager(new Extension.ExtensionService());
        //TypeContainer.Register<IDefinitionManager, DefinitionManager>(dfm);
        //TypeContainer.Register<DefinitionManager, DefinitionManager>(dfm);
        var dfm = TypeContainer.Get<IDefinitionManager>();
        var furnace = new Furnace(rs, dfm);
        furnace.Initialize();
        GameTime gameTime;
        var swTotal = new Stopwatch();
        swTotal.Start();
        var swLast = new Stopwatch();
        int i = 0;
        while (true)
        {
            gameTime = new GameTime(swTotal.Elapsed, swLast.Elapsed);
            furnace.Update(gameTime);
            swLast.Restart();
            Thread.Sleep(10);
            i++;
            if (i % 50 == 0)
            {
                var meatDefinition = dfm.ItemDefinitions.First(x => x.DisplayName == "MeatRawDefinition");
                var foodMaterial = dfm.FoodDefinitions.FirstOrDefault(x => x.DisplayName == "Wauzi Meat");
                var fooditem = meatDefinition.Create(foodMaterial);
                if (fooditem is not null)
                    furnace.Input.Add(fooditem, fooditem.VolumePerUnit);

            }
            if (i % 100 == 0)
            {
                var meatDefinition = dfm.ItemDefinitions.First(x => x.DisplayName == "MeatCookedDefinition");
                var foodMaterial = dfm.FoodDefinitions.FirstOrDefault(x=>x.DisplayName == "Player Meat");
                var fooditem = meatDefinition.Create(foodMaterial);
                if (fooditem is not null)
                    furnace.Input.Add(fooditem, fooditem.VolumePerUnit);
            }
        }
        Console.ReadLine();
    }
}
