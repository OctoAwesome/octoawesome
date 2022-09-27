using OctoAwesome.Components;

namespace OctoAwesome.UI.Components;

/// <summary>
/// Component to identify which UIComponent to use for an entity/functional block interaction.
/// </summary>
public class UiKeyComponent : Component, IEntityComponent, IFunctionalBlockComponent
{
    /// <summary>
    /// Gets or sets the primary key.
    /// </summary>
    public string PrimaryKey { get; set; }
}