using OctoAwesome.Components;

namespace OctoAwesome.UI.Components;

/// <summary>
/// Component to identify which UIComponent to use for an entity interaction.
/// </summary>
public class UiKeyComponent : Component, IEntityComponent
{
    /// <summary>
    /// Gets the primary key.
    /// </summary>
    public string PrimaryKey { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UiKeyComponent"/> class.
    /// </summary>
    /// <param name="primaryKey">The primary key.</param>
    public UiKeyComponent(string primaryKey)
    {
        PrimaryKey = primaryKey;
    }
}