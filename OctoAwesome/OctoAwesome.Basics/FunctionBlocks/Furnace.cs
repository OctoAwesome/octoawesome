using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Crafting;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.jvbslContribution;
using OctoAwesome.Serialization;
using OctoAwesome.UI.Components;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static OctoAwesome.jvbslContribution.StateMachine;

namespace OctoAwesome.Basics.FunctionBlocks;


[SerializationId(1, 4)]
public class Furnace : FunctionalBlock
{
    internal InventoryComponent inventoryComponent;
    internal AnimationComponent animationComponent;

#region PutIntoComponent
    private readonly RecipeService recipeService;
    private readonly IDefinitionManager definitionManager;
    private IReadOnlyCollection<Recipe> recipes;
    private Recipe? currentRecipe;
    private StateMachine stateMachine;
    private int requiredMillisecondsForRecipe;
    internal OutputInventoryComponent outputComponent;
#endregion

    public Furnace()
    {

        recipeService = TypeContainer.Get<RecipeService>();
        definitionManager = TypeContainer.Get<IDefinitionManager>();
        Initialize();
    }

    private void Initialize()
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

    public bool HasRecipeFinished(TimeSpan elapsed, TimeSpan total)
    {
        return requiredMillisecondsForRecipe <= total.TotalMilliseconds;
    }


    public override void Deserialize(BinaryReader reader) => base.Deserialize(reader);//Doesnt get called

    public Furnace(Coordinate position) : this()
    {
        Components.AddComponent(new PositionComponent()
        {
            Position = position
        });
    }

    protected override void OnInteract(GameTime gameTime, Entity entity)
    {
        if (TryGetComponent<UiKeyComponent>(out var ownUiKeyComponent)
           && entity.TryGetComponent<TransferComponent>(out var transferComponent)
           && entity.TryGetComponent<UiMappingComponent>(out var uiMappingComponent))
        {
            transferComponent.Targets.Clear();
            transferComponent.Targets.Add(inventoryComponent);
            transferComponent.Targets.Add(outputComponent);
            uiMappingComponent.Changed.OnNext((entity, ownUiKeyComponent.PrimaryKey, true));

            animationComponent.CurrentTime = 0f;
            animationComponent.AnimationSpeed = 60f;
        }
    }

    public override void Update(GameTime gameTime)
    {
        stateMachine.Update(gameTime.ElapsedGameTime);
    }

    private bool GenerateOutput(TimeSpan elapsed, TimeSpan total)
    {
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
                            if (outputItem.MaterialId != inputItem.MaterialId)
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

    private Recipe? GetRecipe()
    {
        if (inventoryComponent is null)
            return null; //Reset recipe, time etc. pp.
                         //var inputSlot = Input.Inventory.FirstOrDefault();
        var inputs = inventoryComponent.Inventory
            .Where(x=>!string.IsNullOrWhiteSpace(x.Definition?.DisplayName))
            .GroupBy(x => x.Definition.DisplayName)
            .Select(x => new RecipeItem(x.Key, x.Sum(c => c.Amount), x.First().Item.Material.DisplayName /*TODO Name not Displayname*/))
            .ToArray();
        if (inputs.Length == 0)
            return null; //Reset recipe, time etc. pp.
        var recipe = recipeService.GetByInputs(inputs, recipes);
        if (recipe is null)
            return null; //Reset recipe, time etc. pp.
        return recipe;
    }
}
