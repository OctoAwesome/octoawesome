using OctoAwesome.Components;

using System;
using System.Collections.Generic;

namespace OctoAwesome.UI.Components;

/// <summary>
/// Component to identify which UIComponent to use for an entity interaction.
/// </summary>
public class UiKeyComponent : Component, IEntityComponent, IEquatable<UiKeyComponent>
{
    /// <summary>
    /// Gets the primary key.
    /// </summary>
    public string PrimaryKey { get; }

    /// <param name="primaryKey">The primary key.</param>
    public UiKeyComponent(string primaryKey)
    {
        PrimaryKey = primaryKey;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return Equals(obj as UiKeyComponent);
    }

    /// <inheritdoc/>
    public bool Equals(UiKeyComponent? other)
    {
        return other is not null &&
               PrimaryKey == other.PrimaryKey;
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(PrimaryKey);
    }
/// <inheritdoc/>

    public static bool operator ==(UiKeyComponent left, UiKeyComponent right)
    {
        return EqualityComparer<UiKeyComponent>.Default.Equals(left, right);
    }
/// <inheritdoc/>

    public static bool operator !=(UiKeyComponent left, UiKeyComponent right)
    {
        return !(left == right);
    }
}