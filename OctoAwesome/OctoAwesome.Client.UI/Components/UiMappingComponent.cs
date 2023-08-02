using OctoAwesome.Components;
using OctoAwesome.Rx;

using System;

namespace OctoAwesome.UI.Components;

/// <summary>
/// Class for mapping a <see cref="Component"/> to an <see cref="IEntityComponent"/>
/// </summary>
[SerializationId(3, 2)]
public class UiMappingComponent : Component, IEntityComponent, IDisposable
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

    /// <inheritdoc/>
    public void Dispose()
    {
        Changed?.Dispose();
    }
}