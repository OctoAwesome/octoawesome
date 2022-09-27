using engenious;

using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Components;
using OctoAwesome.Crafting;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static OctoAwesome.StateMachine;

namespace OctoAwesome.Basics.EntityComponents;
internal class BurningComponent : InstanceComponent<ComponentContainer>, IFunctionalBlockComponent, IUpdateable
{

    internal InventoryComponent inventoryComponent;
    private RecipeService recipeService;
    private IDefinitionManager definitionManager;
    private IReadOnlyCollection<Recipe> recipes;
    private Recipe? currentRecipe;
    private StateMachine stateMachine;
    private int requiredMillisecondsForRecipe;
    internal OutputInventoryComponent outputComponent;
    internal ProductionResourcesInventoryComponent productionResourcesInventoryComponent;
    private int energieLeft = 0;



    public void Initialize(string typename)
    {
        recipeService = TypeContainer.Get<RecipeService>();
        definitionManager = TypeContainer.Get<IDefinitionManager>();
        recipes = recipeService.GetByType(typename);

        inventoryComponent = Instance.GetComponent<InventoryComponent>();
        outputComponent = Instance.GetComponent<OutputInventoryComponent>();
        productionResourcesInventoryComponent = Instance.GetComponent<ProductionResourcesInventoryComponent>();

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

    private bool HasRecipeFinished(TimeSpan elapsed, TimeSpan total)
    {
        var energySinceLastUpdate = (int)(elapsed.TotalMilliseconds * 2.5);

        if (energieLeft > 0)
            energieLeft -= energySinceLastUpdate;
        System.Diagnostics.Debug.WriteLine(energieLeft);
        if (energieLeft <= 0)
        {
            var firstProductionRessource = productionResourcesInventoryComponent.Inventory.First();

            if (firstProductionRessource is null
                || firstProductionRessource.Definition is not BlockDefinition bd
                || (bd.Material is not WoodMaterialDefinition
                    && bd.Material is not CottonMaterialDefinition
                    && bd.Material is not LeaveMaterialDefinition))
            {
                currentRecipe = null;
                return false;
            }
            else if (firstProductionRessource.Definition is BlockDefinition bd2
                && bd2.Material is ISolidMaterialDefinition smd)
            {
                energieLeft = smd.Density; //TODO real energy stuff calculations
                productionResourcesInventoryComponent.Remove(firstProductionRessource, 1);
            }
        }

        if (energieLeft <= 0)
            return false;

        requiredMillisecondsForRecipe -= (int)elapsed.TotalMilliseconds;

        return requiredMillisecondsForRecipe <= 0;
    }


    private bool GenerateOutput(TimeSpan elapsed, TimeSpan total)
    {
        var recipe = GetRecipe();
        if (recipe is null || currentRecipe is null || currentRecipe != recipe)
            return true;

        if (currentRecipe.Inputs.Length == 1)
        {
            var inputItem = currentRecipe.Inputs[0];
            var inputDef = definitionManager.Definitions.FirstOrDefault(x => x.DisplayName == inputItem.ItemName);

            var inputSlot = inventoryComponent.GetSlot(inputDef);

            foreach (var outputItem in currentRecipe.Outputs)
            {
                var outputDef = definitionManager.Definitions.FirstOrDefault(x => x.DisplayName == outputItem.ItemName);/*TODO Name not Displayname*/
                if (outputDef is IItemDefinition itemDef)
                {
                    IMaterialDefinition? mat;

                    if (string.IsNullOrWhiteSpace(outputItem.MaterialName))
                        mat = inputSlot.Item.Material;
                    else
                        mat = definitionManager.MaterialDefinitions.FirstOrDefault(x => x.DisplayName == outputItem.MaterialName);

                    if (mat is null)
                        return false;
                    outputComponent.Add(itemDef.Create(mat), outputItem.Count);
                }
                else if (outputDef is IBlockDefinition blockDefinition)
                {
                    outputComponent.Add(blockDefinition, outputItem.Count);
                }
            }
            inventoryComponent.Remove(inputSlot.Item, inputItem.Count);
        }
        else if (currentRecipe.Inputs.Length > 1)
        {
            foreach (var inputItem in currentRecipe.Inputs)
            {
                var inputDef = definitionManager.Definitions.FirstOrDefault(x => x.DisplayName == inputItem.ItemName);

                var inputSlot = inventoryComponent.GetSlot(inputDef);

                foreach (var outputItem in currentRecipe.Outputs)
                {

                    var outputDef = definitionManager.Definitions.FirstOrDefault(x => x.DisplayName == outputItem.ItemName);/*TODO Name not Displayname*/
                    if (outputDef is IItemDefinition itemDef)
                    {
                        IMaterialDefinition? mat;

                        if (string.IsNullOrWhiteSpace(outputItem.MaterialName))
                        {
                            if (outputItem.InputOutputMappingId != inputItem.InputOutputMappingId)
                                continue;
                            mat = inputSlot.Item.Material;
                        }
                        else
                            mat = definitionManager.MaterialDefinitions.FirstOrDefault(x => x.DisplayName == outputItem.MaterialName);

                        if (mat is null)
                            return false;
                        outputComponent.Add(itemDef.Create(mat), outputItem.Count);
                    }
                    else if (outputDef is IBlockDefinition blockDefinition)
                    {
                        outputComponent.Add(blockDefinition, outputItem.Count);
                    }
                }
                inventoryComponent.Remove(inputSlot.Item, inputItem.Count);
            }
        }
        else
        {
            foreach (var outputItem in currentRecipe.Outputs)
            {
                if (string.IsNullOrWhiteSpace(outputItem.MaterialName))
                    continue;
                var outputDef = definitionManager.Definitions.FirstOrDefault(x => x.DisplayName == outputItem.ItemName);/*TODO Name not Displayname*/
                if (outputDef is IItemDefinition itemDef)
                {
                    IMaterialDefinition? mat;

                    mat = definitionManager.MaterialDefinitions.FirstOrDefault(x => x.DisplayName == outputItem.MaterialName);

                    if (mat is null)
                        return false;
                    outputComponent.Add(itemDef.Create(mat), outputItem.Count);
                }
                else if (outputDef is IBlockDefinition blockDefinition)
                {
                    outputComponent.Add(blockDefinition, outputItem.Count);
                }
            }
        }

        OnInputSlotUpdate();

        return true; //We should never be two updates in this state! That would be mist
    }

    private bool OnInputSlotUpdate()
    {
        currentRecipe = GetRecipe();
        if (currentRecipe is null)
            return false; //Reset recipe, time etc. pp.

        Debug.Assert(currentRecipe.Time is not null || currentRecipe.Energy is not null,
            $"Either {nameof(Recipe.Energy)} or {nameof(Recipe.Time)} needs to be set.");
        requiredMillisecondsForRecipe = currentRecipe.Time ?? currentRecipe.Energy!.Value * 40;

        if (currentRecipe.MinTime is not null && requiredMillisecondsForRecipe < currentRecipe.MinTime)
            requiredMillisecondsForRecipe = currentRecipe.MinTime!.Value;

        if (requiredMillisecondsForRecipe < 0)
            return false;

        if (energieLeft > 0)
            return true;

        var firstProductionRessource = productionResourcesInventoryComponent.Inventory.First();

        if (firstProductionRessource is null
            || firstProductionRessource.Definition is not BlockDefinition bd
            || (bd.Material is not WoodMaterialDefinition
                && bd.Material is not CottonMaterialDefinition
                && bd.Material is not LeaveMaterialDefinition))
        {
            currentRecipe = null;
            return false;
        }
        else if (firstProductionRessource.Definition is BlockDefinition bd2
            && bd2.Material is ISolidMaterialDefinition smd)
        {
            energieLeft = smd.Density; //TODO real energy stuff calculations
            productionResourcesInventoryComponent.Remove(firstProductionRessource, 1);
        }

        return true;
    }

    private Recipe? GetRecipe()
    {
        if (inventoryComponent is null)
            return null; //Reset recipe, time etc. pp.
                         //var inputSlot = Input.Inventory.FirstOrDefault();
        var inputs = inventoryComponent.Inventory
            .Where(x => !string.IsNullOrWhiteSpace(x.Definition?.DisplayName))
            .GroupBy(x => x.Definition.DisplayName)
            .Select(x => new RecipeItem(x.Key, x.Sum(c => c.Amount), x.First().Item.Material.DisplayName, null /*TODO Name not Displayname*/))
            .ToArray();
        if (inputs.Length == 0)
            return null; //Reset recipe, time etc. pp.
        var recipe = RecipeService.GetByInputs(recipes, inputs);
        if (recipe is null)
            return null; //Reset recipe, time etc. pp.
        return recipe;
    }

    /// <inheritdoc/>
    public void Update(GameTime gameTime)
    {

        stateMachine.Update(gameTime.ElapsedGameTime);
    }
}
