using OctoAwesome.Components;
using OctoAwesome.Rx;

namespace OctoAwesome.UI.Components;

/// <summary>
/// Class for mapping a <see cref="Component"/> to an <see cref="IEntityComponent"/> or <see cref="IFunctionalBlockComponent"/>
/// </summary>
public class UiMappingComponent : Component, IEntityComponent, IFunctionalBlockComponent
{
    /// <summary>
    /// Gets the relay that can be subscribed for receiving changed events
    /// </summary>
    public Relay<(ComponentContainer instance, string primaryKey, bool show)> Changed { get; }    

    /// <summary>
    /// Initializes a new instance of the<see cref="UiMappingComponent" /> class
    /// </summary>
    public UiMappingComponent()
    {
        Changed = new Relay<(ComponentContainer instance, string primaryKey, bool show)>();
    }
}