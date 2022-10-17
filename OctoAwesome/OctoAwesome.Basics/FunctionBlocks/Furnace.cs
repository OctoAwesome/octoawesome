using engenious;

using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Crafting;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.Location;
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
public class Furnace : Entity
{
    internal ProductionInventoriesComponent ProductionInventoriesComponent
    {
        get => NullabilityHelper.NotNullAssert(productionInventoriesComponent, $"{nameof(ProductionInventoriesComponent)} was not initialized!");
        set => productionInventoriesComponent = NullabilityHelper.NotNullAssert(value, $"{nameof(ProductionInventoriesComponent)} cannot be initialized with null!");
    }
    internal AnimationComponent AnimationComponent
    {
        get => NullabilityHelper.NotNullAssert(animationComponent, $"{nameof(AnimationComponent)} was not initialized!");
        set => animationComponent = NullabilityHelper.NotNullAssert(value, $"{nameof(AnimationComponent)} cannot be initialized with null!");
    }
    internal BurningComponent BurningComponent
    {
        get => NullabilityHelper.NotNullAssert(burningComponent, $"{nameof(BurningComponent)} was not initialized!");
        set => burningComponent = NullabilityHelper.NotNullAssert(value, $"{nameof(BurningComponent)} cannot be initialized with null!");
    }

    private ProductionInventoriesComponent? productionInventoriesComponent;
    private AnimationComponent? animationComponent;
    private BurningComponent? burningComponent;

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
    /// <param name="position">The position the furnace is at.</param>
    /// <param name="direction">The direction the chest is facing to.</param>
    public Furnace(Coordinate position, float direction) : this()
    {
        Components.AddIfTypeNotExists(new PositionComponent()
        {
            Position = position,
            Direction = direction
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
            transferComponent.Targets.Add(ProductionInventoriesComponent.InputInventory);
            transferComponent.Targets.Add(ProductionInventoriesComponent.OutputInventory);
            transferComponent.Targets.Add(ProductionInventoriesComponent.ProductionInventory);
            uiMappingComponent.Changed.OnNext((entity, ownUiKeyComponent.PrimaryKey, true));

            AnimationComponent.CurrentTime = 0f;
            AnimationComponent.AnimationSpeed = 60f;
        }
    }
}
