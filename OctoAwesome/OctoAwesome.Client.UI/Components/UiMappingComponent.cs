using OctoAwesome.Components;
using OctoAwesome.Rx;

namespace OctoAwesome.UI.Components;

public class UiMappingComponent : Component, IEntityComponent, IFunctionalBlockComponent
{
    public Relay<(ComponentContainer instance, string primaryKey, bool show)> Changed { get; }

    public UiMappingComponent()
    {
        Changed = new Relay<(ComponentContainer instance, string primaryKey, bool show)>();
    }
}