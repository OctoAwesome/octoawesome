using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.Location;
using OctoAwesome;
using OctoAwesome.Serialization;
using OctoAwesome.UI.Components;
using OctoAwesome.Extension;


namespace OctoAwesome.Basics.FunctionBlocks;

/// <summary>
/// Represents the furnace object in the world
/// </summary>
[SerializationId()]
[Nooson]
public partial class Furnace : Entity, IConstructionSerializable<Furnace>
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

    [NoosonIgnore]
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
    protected override void OnInitialize()
    {
        base.OnInitialize();
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


}
