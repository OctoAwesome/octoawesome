using engenious;

using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Crafting;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome;
using OctoAwesome.Serialization;
using OctoAwesome.UI.Components;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OctoAwesome.Extension;


namespace OctoAwesome.Basics.FunctionBlocks;

/// <summary>
/// Represents the furnace object in the world
/// </summary>
[SerializationId(1, 4)]
public class Furnace : FunctionalBlock
{
    private InventoryComponent? inventoryComponent;
    private AnimationComponent? animationComponent;
    private OutputInventoryComponent? outputComponent;
    private ProductionResourcesInventoryComponent? productionResourcesInventoryComponent;

    internal InventoryComponent InventoryComponent
    {
        get => NullabilityHelper.NotNullAssert(inventoryComponent, $"{nameof(InventoryComponent)} was not initialized!");
        set => inventoryComponent = NullabilityHelper.NotNullAssert(value, $"{nameof(InventoryComponent)} cannot be initialized with null!");
    }

    internal AnimationComponent AnimationComponent
    {
        get => NullabilityHelper.NotNullAssert(animationComponent, $"{nameof(AnimationComponent)} was not initialized!");
        set => animationComponent = NullabilityHelper.NotNullAssert(value, $"{nameof(AnimationComponent)} cannot be initialized with null!");
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
    // internal BurningComponent BurningComponent;

    /// <summary>
    /// Initializes a new instance of the<see cref="Furnace" /> class
    /// </summary>
    public Furnace()
    {
    }

    /// <inheritdoc />
    protected override void OnInitialize(IResourceManager manager)
    {
        base.OnInitialize(manager);
        GetComponent<BurningComponent>()?.Initialize("furnace");
    }

    /// <summary>
    /// Initializes a new instance of the<see cref="Furnace" /> class
    /// </summary>
    public Furnace(Coordinate position) : this()
    {
        Components.AddComponent(new PositionComponent()
        {
            Position = position
        });
        
    }

    /// <inheritdoc/>
    public override void Deserialize(BinaryReader reader) => base.Deserialize(reader);//Doesnt get called

    /// <inheritdoc/>
    protected override void OnInteract(GameTime gameTime, Entity entity)
    {
        if (TryGetComponent<UiKeyComponent>(out var ownUiKeyComponent)
           && entity.TryGetComponent<TransferComponent>(out var transferComponent)
           && entity.TryGetComponent<UiMappingComponent>(out var uiMappingComponent))
        {
            transferComponent.Targets.Clear();
            transferComponent.Targets.Add(InventoryComponent);
            transferComponent.Targets.Add(OutputComponent);
            transferComponent.Targets.Add(ProductionResourcesInventoryComponent);
            uiMappingComponent.Changed.OnNext((entity, ownUiKeyComponent.PrimaryKey, true));

            AnimationComponent.CurrentTime = 0f;
            AnimationComponent.AnimationSpeed = 60f;
        }
    }
}
