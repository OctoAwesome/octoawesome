using NonSucking.Framework.Serialization;

using OctoAwesome.Components;

using System;
using System.Collections.Generic;

namespace OctoAwesome.UI.Components;

/// <summary>
/// Component to identify which UIComponent to use for an entity interaction.
/// </summary>
[Nooson]
public partial class UiKeyComponent : Component, IEntityComponent, IEquatable<UiKeyComponent>
{
    /// <summary>
    /// Gets the primary key.
    /// </summary>
    public string PrimaryKey { get; private set; }

    public UiKeyComponent() : base()
    {
        Sendable = true;
        PrimaryKey = "";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UiKeyComponent"/> class.
    /// </summary>
    /// <param name="primaryKey">The primary key.</param>
    public UiKeyComponent(string primaryKey) : this()
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

    /// <summary>Compare two ui key components for equality.</summary>
    /// <param name="left">The first <see cref="UiKeyComponent" /> to compare.</param>
    /// <param name="right">The second <see cref="UiKeyComponent" /> to compare.</param>
    /// <returns><see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> are considered equal; otherwise, <see langword="false" />.</returns>
    /// <seealso cref="Equals(UiKeyComponent)" />
    public static bool operator ==(UiKeyComponent left, UiKeyComponent right)
    {
        return EqualityComparer<UiKeyComponent>.Default.Equals(left, right);
    }

    /// <summary>Compare two ui key components for inequality.</summary>
    /// <param name="left">The first <see cref="UiKeyComponent" /> to compare.</param>
    /// <param name="right">The second <see cref="UiKeyComponent" /> to compare.</param>
    /// <returns><see langword="true" /> if <paramref name="left"/> and <paramref name="right"/> are not considered equal; otherwise, <see langword="false" />.</returns>
    /// <seealso cref="Equals(UiKeyComponent)" />
    public static bool operator !=(UiKeyComponent left, UiKeyComponent right)
    {
        return !(left == right);
    }
}