using engenious;

using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Components;
using OctoAwesome.Crafting;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OctoAwesome.Extension;
using static OctoAwesome.StateMachine;
using OctoAwesome.Serialization;
using OctoAwesome.Caching;
using OpenTK.Windowing.Common.Input;

namespace OctoAwesome.Basics.EntityComponents;

[Nooson]
[SerializationId()]
internal partial class ProductionInventoriesComponent : Component, IEntityComponent, IConstructionSerializable<ProductionInventoriesComponent>
{
    public InventoryComponent InputInventory { get; set; }
    public InventoryComponent OutputInventory { get; set; }
    public InventoryComponent ProductionInventory { get; set; }

    public ProductionInventoriesComponent()
    {
        InputInventory = new();
        OutputInventory = new();
        ProductionInventory = new();
        Sendable = true;
    }

    public ProductionInventoriesComponent(bool fixedSlot, int slotCountProduction) : this()
    {
        InputInventory = new();
        OutputInventory = new();
        ProductionInventory = new(fixedSlot, slotCountProduction);
        Sendable = true;
    }

    protected override void OnParentSetting(IComponentContainer newParent)
    {
        if (newParent.Simulation is null)
            return;
        InputInventory.Parent = newParent;
        OutputInventory.Parent = newParent; 
        ProductionInventory.Parent = newParent;
        newParent.Simulation.GlobalComponentList.Add(InputInventory);
        newParent.Simulation.GlobalComponentList.Add(OutputInventory);
        newParent.Simulation.GlobalComponentList.Add(ProductionInventory);
    }
}

[SerializationId()]
internal partial class BurningComponent : Component, IEntityComponent, IUpdateable
{
    private StateMachine stateMachine;
    private RecipeService recipeService;
    private IDefinitionManager definitionManager;

    internal ProductionInventoriesComponent? inventoryComponent;

    private Recipe? currentRecipe;
    private int requiredMillisecondsForRecipe;
    private int energyLeft;
    private IReadOnlyCollection<Recipe>? recipes;

    [NoosonIgnore]
    private IReadOnlyCollection<Recipe> Recipes
    {
        get => NullabilityHelper.NotNullAssert(recipes, $"{nameof(Recipes)} was not initialized!");
        set => recipes = NullabilityHelper.NotNullAssert(value, $"{nameof(Recipes)} cannot be initialized with null!");
    }
    [NoosonIgnore]
    internal ProductionInventoriesComponent InventoryComponent
    {
        get => NullabilityHelper.NotNullAssert(inventoryComponent, $"{nameof(InventoryComponent)} was not initialized!");
        set => inventoryComponent = NullabilityHelper.NotNullAssert(value, $"{nameof(InventoryComponent)} cannot be initialized with null!");
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
        recipeService = TypeContainer.Get<RecipeService>();
        definitionManager = TypeContainer.Get<IDefinitionManager>();
        recipes = recipeService.GetByType(typename);
        var ivComponent = Parent.GetComponent<ProductionInventoriesComponent>();
        Debug.Assert(ivComponent is not null,
            $"Entity for Burning component needs to have a not null {nameof(ProductionInventoriesComponent)}.");
        inventoryComponent = ivComponent;
        Debug.Assert(inventoryComponent != null, nameof(inventoryComponent) + " != null");
    }


    private bool HasRecipeFinished(TimeSpan elapsed, TimeSpan total)
    {
        var energySinceLastUpdate = (int)(elapsed.TotalMilliseconds * 2.5);

        if (energyLeft > 0)
            energyLeft -= energySinceLastUpdate;
        Debug.WriteLine(energyLeft);
        if (energyLeft <= 0)
        {
            var firstProductionResource = InventoryComponent.ProductionInventory.Inventory.First();

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
                InventoryComponent.ProductionInventory.Remove(firstProductionResource, 1);
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

            var inputSlot = InventoryComponent.InputInventory.GetSlot(inputDef);

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
                        mat = definitionManager.GetDefinitionByUniqueKey<IMaterialDefinition>(outputItem.MaterialName);

                    if (mat is null)
                        return false;

                    var createdItem = itemDef.Create(mat);

                    if (createdItem is null)
                        return false;

                    InventoryComponent.OutputInventory.Add(createdItem, outputItem.Count);

                }
                else if (outputDef is IBlockDefinition blockDefinition)
                {
                    InventoryComponent.OutputInventory.Add(blockDefinition, outputItem.Count);
                }
            }
            InventoryComponent.InputInventory.Remove(inputSlot.Item, inputItem.Count);
        }
        else if (currentRecipe.Inputs.Length > 1)
        {
            foreach (var inputItem in currentRecipe.Inputs)
            {
                var inputDef = definitionManager.Definitions.FirstOrDefault(x => x.DisplayName == inputItem.ItemName);

                var inputSlot = InventoryComponent.InputInventory.GetSlot(inputDef);

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
                            mat = definitionManager.GetDefinitionByUniqueKey<IMaterialDefinition>(outputItem.MaterialName);

                        if (mat is null)
                            return false;

                        var createdItem = itemDef.Create(mat);

                        if (createdItem is null)
                            return false;
                        InventoryComponent.OutputInventory.Add(createdItem, outputItem.Count);

                    }
                    else if (outputDef is IBlockDefinition blockDefinition)
                    {
                        InventoryComponent.OutputInventory.Add(blockDefinition, outputItem.Count);
                    }
                }
                InventoryComponent.InputInventory.Remove(inputSlot.Item, inputItem.Count);
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
                    var mat = definitionManager.GetDefinitionByUniqueKey<IMaterialDefinition>(outputItem.MaterialName);

                    if (mat is null)
                        return false;

                    var createdItem = itemDef.Create(mat);

                    if (createdItem is null)
                        return false;
                    InventoryComponent.OutputInventory.Add(createdItem, outputItem.Count);
                }
                else if (outputDef is IBlockDefinition blockDefinition)
                {
                    InventoryComponent.OutputInventory.Add(blockDefinition, outputItem.Count);
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

        var firstProductionResource = InventoryComponent.ProductionInventory.Inventory.First();

        if (firstProductionResource.Definition is not BlockDefinition bd
            || (bd.Material is not WoodMaterialDefinition
                && bd.Material is not CottonMaterialDefinition
                && bd.Material is not LeaveMaterialDefinition))
        {
            currentRecipe = null;
            return false;
        }
        else if (firstProductionResource.Definition is BlockDefinition bd2
            && bd2.Material is ISolidMaterialDefinition smd)
        {
            energyLeft = smd.Density; //TODO real energy stuff calculations
            InventoryComponent.ProductionInventory.Remove(firstProductionResource, 1);
        }

        return true;
    }

    private Recipe? GetRecipe()
    {
        var inputs = InventoryComponent.InputInventory.Inventory
            .Where(x => !string.IsNullOrWhiteSpace(x.Definition?.DisplayName))
            .GroupBy(x => x.Definition!.DisplayName)
            .Select(x => new RecipeItem(x.Key, x.Sum(c => c.Amount), x.First().Item!.Material.DisplayName, null /*TODO Name not Displayname*/)) //TODO Implement the alias stuff in the ofen
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
