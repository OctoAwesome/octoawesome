using OctoAwesome.Components;

namespace OctoAwesome.UI.Components;

public class UiKeyComponent : Component, IEntityComponent, IFunctionalBlockComponent
{
    public string PrimaryKey { get; set; }
}