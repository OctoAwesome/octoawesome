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
using OctoAwesome.Extension;
using static OctoAwesome.StateMachine;

namespace OctoAwesome.Basics.EntityComponents;
internal class BurningComponent : InstanceComponent<ComponentContainer>, IFunctionalBlockComponent, IUpdateable
{
    private readonly StateMachine stateMachine;
    private readonly RecipeService recipeService;
    private readonly IDefinitionManager definitionManager;
    private Recipe? currentRecipe;
    private int requiredMillisecondsForRecipe;
    private int energyLeft = 0;
    private InventoryComponent? inventoryComponent;
    private OutputInventoryComponent? outputComponent;
    private ProductionResourcesInventoryComponent? productionResourcesInventoryComponent;
    private IReadOnlyCollection<Recipe>? recipes;

    private IReadOnlyCollection<Recipe> Recipes
    {
        get => NullabilityHelper.NotNullAssert(recipes, $"{nameof(Recipes)} was not initialized!");
        set => recipes = NullabilityHelper.NotNullAssert(value, $"{nameof(Recipes)} cannot be initialized with null!");
    }

    internal InventoryComponent InventoryComponent
    {
        get => NullabilityHelper.NotNullAssert(inventoryComponent, $"{nameof(InventoryComponent)} was not initialized!");
        set => inventoryComponent = NullabilityHelper.NotNullAssert(value, $"{nameof(InventoryComponent)} cannot be initialized with null!");
    }

    internal OutputInventoryComponent OutputComponent
    {
        get => NullabilityHelper.NotNullAssert(outputComponent, $"{nameof(OutputComponent)} was not initialized!");
        set => outputComponent = NullabilityHelper.NotNullAssert(value, $"{nameof(OutputComponent)} cannot be initialized with null!");
    }

    internal ProductionResourcesInventoryComponent ProductionResourcesInventoryComponent
    {
        get => NullabilityHelper.NotNullAssert(productionResourcesInventoryComponent, $"{nameof(ProductionResourcesInventoryComponent)} was not initialized!");
        set => productionResourcesInventoryComponent = NullabilityHelper.NotNullAssert(value, $"{nameof(ProductionResourcesInventoryComponent)} cannot be initialized with null!");
    }

    public BurningComponent()
    {
        stateMachine = CreateStateMachine();
        recipeService = TypeContainer.Get<RecipeService>();
        definitionManager = TypeContainer.Get<IDefinitionManager>();
    }

    private StateMachine CreateStateMachine()
    {
        //Idle => Recipe => Running => Output => Recipe / Idle

        var idleState = new GenericNode("idle", (elapsed, total) => OnInputSlotUpdate());
        var running = new GenericNode("running", HasRecipeFinished);
        var recipe = new GenericNode("recipe", (elapsed, total) => true);
        var output = new GenericNode("output", GenerateOutput);

        var stateMachine = new StateMachine(idleState);
        stateMachine.AddNodes(running, recipe, output);

        stateMachine.AddTransition(idleState, recipe, () => currentRecipe is not null);
        stateMachine.AddTransition(recipe, running, () => true);
        stateMachine.AddTransition(running, output, () => true);
        stateMachine.AddTransition(output, recipe, () => currentRecipe is not null);
        stateMachine.AddTransition(output, idleState, () => currentRecipe is null);
        return stateMachine;
    }


    public void Initialize(string typename)
    {
        Recipes = recipeService.GetByType(typename);

        var invComp = Instance.GetComponent<InventoryComponent>();
        var outComp = Instance.GetComponent<OutputInventoryComponent>();
        var prodResComp = Instance.GetComponent<ProductionResourcesInventoryComponent>();
        Debug.Assert(invComp != null, nameof(invComp) + " != null");
        Debug.Assert(outComp != null, nameof(outComp) + " != null");
        Debug.Assert(prodResComp != null, nameof(prodResComp) + " != null");
        InventoryComponent = invComp;
        OutputComponent = outComp;
        ProductionResourcesInventoryComponent = prodResComp;
    }

    private bool HasRecipeFinished(TimeSpan elapsed, TimeSpan total)
    {
        var energySinceLastUpdate = (int)(elapsed.TotalMilliseconds * 2.5);

        if (energyLeft > 0)
            energyLeft -= energySinceLastUpdate;
        Debug.WriteLine(energyLeft);
        if (energyLeft <= 0)
        {
            var firstProductionResource = ProductionResourcesInventoryComponent.Inventory.FirstOrDefault();

            if (firstProductionResource?.Definition is not BlockDefinition bd 
                || (bd.Material is not WoodMaterialDefinition
                    && bd.Material is not CottonMaterialDefinition
                    && bd.Material is not LeaveMaterialDefinition))
            {
                currentRecipe = null;
                return false;
            }
            if (firstProductionResource.Definition is BlockDefinition { Material: ISolidMaterialDefinition smd })
            {
                energyLeft = smd.Density; //TODO real energy stuff calculations
                ProductionResourcesInventoryComponent.Remove(firstProductionResource, 1);
            }
        }

        if (energyLeft <= 0)
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

            var inputSlot = InventoryComponent.GetSlot(inputDef);

            if (inputSlot?.Item is null)
                return false;
            foreach (var outputItem in currentRecipe.Outputs)
            {
                var outputDef = definitionManager.Definitions.FirstOrDefault(x => x.DisplayName == outputItem.ItemName);/*TODO Name not Displayname*/
                if (outputDef is IItemDefinition itemDef)
                {
                    IMaterialDefinition? mat;

                    if (string.IsNullOrWhiteSpace(outputItem.MaterialName))
                    {
                        mat = inputSlot.Item.Material;
                    }
                    else
                        mat = definitionManager.MaterialDefinitions.FirstOrDefault(x => x.DisplayName == outputItem.MaterialName);

                    if (mat is null)
                        return false;

                    var createdItem = itemDef.Create(mat);

                    if (createdItem is null)
                        return false;
                    
                    OutputComponent.Add(createdItem, outputItem.Count);
                }
                else if (outputDef is IBlockDefinition blockDefinition)
                {
                    OutputComponent.Add(blockDefinition, outputItem.Count);
                }
            }
            InventoryComponent.Remove(inputSlot.Item, inputItem.Count);
        }
        else if (currentRecipe.Inputs.Length > 1)
        {
            foreach (var inputItem in currentRecipe.Inputs)
            {
                var inputDef = definitionManager.Definitions.FirstOrDefault(x => x.DisplayName == inputItem.ItemName);

                var inputSlot = InventoryComponent.GetSlot(inputDef);
                if (inputSlot?.Item is null)
                    return false;
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

                        var createdItem = itemDef.Create(mat);

                        if (createdItem is null)
                            return false;

                        OutputComponent.Add(createdItem, outputItem.Count);
                    }
                    else if (outputDef is IBlockDefinition blockDefinition)
                    {
                        OutputComponent.Add(blockDefinition, outputItem.Count);
                    }
                }
                InventoryComponent.Remove(inputSlot.Item, inputItem.Count);
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
                    var mat = definitionManager.MaterialDefinitions.FirstOrDefault(x => x.DisplayName == outputItem.MaterialName);

                    if (mat is null)
                        return false;
                    var createdItem = itemDef.Create(mat);

                    if (createdItem is null)
                        return false;
                    OutputComponent.Add(createdItem, outputItem.Count);
                }
                else if (outputDef is IBlockDefinition blockDefinition)
                {
                    OutputComponent.Add(blockDefinition, outputItem.Count);
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

        if (energyLeft > 0)
            return true;

        var firstProductionRessource = ProductionResourcesInventoryComponent.Inventory.First();

        if (firstProductionRessource.Definition is not BlockDefinition bd
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
            energyLeft = smd.Density; //TODO real energy stuff calculations
            ProductionResourcesInventoryComponent.Remove(firstProductionRessource, 1);
        }

        return true;
    }

    private Recipe? GetRecipe()
    {
        if (inventoryComponent is null)
            return null; //Reset recipe, time etc. pp.
                         //var inputSlot = Input.Inventory.FirstOrDefault();
        var inputs = InventoryComponent.Inventory
            .Where(x => !string.IsNullOrWhiteSpace(x.Definition?.DisplayName) && x.Item is not null)
            .GroupBy(x => x.Definition!.DisplayName)
            .Select(x => new RecipeItem(x.Key, x.Sum(c => c.Amount), x.First().Item!.Material.DisplayName, null /*TODO Name not Displayname*/))
            .ToArray();
        if (inputs.Length == 0)
            return null; //Reset recipe, time etc. pp.
        var recipe = RecipeService.GetByInputs(Recipes, inputs);
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
