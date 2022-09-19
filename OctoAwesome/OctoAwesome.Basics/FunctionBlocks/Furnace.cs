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


namespace OctoAwesome.Basics.FunctionBlocks;

/// <summary>
/// Represents the furnace object in the world
/// </summary>
[SerializationId(1, 4)]
public class Furnace : FunctionalBlock
{
    internal InventoryComponent inventoryComponent;
    internal AnimationComponent animationComponent;

    internal OutputInventoryComponent outputComponent;
    internal ProductionResourcesInventoryComponent productionResourcesInventoryComponent;
    internal BurningComponent burningComponent;

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
            transferComponent.Targets.Add(inventoryComponent);
            transferComponent.Targets.Add(outputComponent);
            transferComponent.Targets.Add(productionResourcesInventoryComponent);
            uiMappingComponent.Changed.OnNext((entity, ownUiKeyComponent.PrimaryKey, true));

            animationComponent.CurrentTime = 0f;
            animationComponent.AnimationSpeed = 60f;
        }
    }
}
