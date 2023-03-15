using OctoAwesome.Components;

using System;
using System.Collections.Generic;
using System.IO;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Component describing the body properties of an entity.
    /// </summary>
    [Nooson]
    public sealed partial class BodyComponent : Component, IEntityComponent, IEquatable<BodyComponent?>
    {
        /// <summary>
        /// Gets or sets the body entity mass.
        /// </summary>
        public float Mass { get; set; }

        /// <summary>
        /// Gets or sets the entity body radius(in block units).
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Gets or sets the height of the entity body.
        /// </summary>
        public float Height { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyComponent"/> class.
        /// </summary>
        public BodyComponent()
        {
            Mass = 1; //1kg
            Radius = 1;
            Height = 1;
        }


        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as BodyComponent);
        }

        /// <inheritdoc/>
        public bool Equals(BodyComponent? other)
        {
            return other is not null &&
                   Mass == other.Mass &&
                   Radius == other.Radius &&
                   Height == other.Height;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Mass, Radius, Height);
        }

        /// <summary>Compare two body components for equality.</summary>
        /// <param name="left">The first <see cref="BodyComponent" /> to compare.</param>
        /// <param name="right">The second <see cref="BodyComponent" /> to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left"/> and <paramref name="right"/> are considered equal; otherwise, <see langword="false" />.</returns>
        /// <seealso cref="Equals(BodyComponent)" />
        public static bool operator ==(BodyComponent? left, BodyComponent? right)
        {
            return EqualityComparer<BodyComponent>.Default.Equals(left, right);
        }

        /// <summary>Compare two body components for inequality.</summary>
        /// <param name="left">The first <see cref="BodyComponent" /> to compare.</param>
        /// <param name="right">The second <see cref="BodyComponent" /> to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left"/> and <paramref name="right"/> are not considered equal; otherwise, <see langword="false" />.</returns>
        /// <seealso cref="Equals(BodyComponent)" />
        public static bool operator !=(BodyComponent? left, BodyComponent? right)
        {
            return !(left == right);
        }
    }
}
