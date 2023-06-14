using engenious;
using OctoAwesome.Components;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using OctoAwesome.Database;

namespace OctoAwesome.EntityComponents
{

    /// <summary>
    /// Component for entities with box collision.
    /// </summary>
    [Nooson]
    public sealed partial class BoxCollisionComponent : CollisionComponent, IEquatable<BoxCollisionComponent?>
    {
        /// <summary>
        /// Gets the collision bounding boxes of the entity.
        /// </summary>
        [NoosonIgnore]
        public ReadOnlySpan<BoundingBox> BoundingBoxes => new(boundingBoxes);

        [NoosonInclude]
        private BoundingBox[] boundingBoxes;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxCollisionComponent"/> class.
        /// </summary>
        public BoxCollisionComponent()
        {
            boundingBoxes = Array.Empty<BoundingBox>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxCollisionComponent"/> class.
        /// </summary>
        /// <param name="boundingBoxes">The collision bounding boxes of the entity.</param>
        public BoxCollisionComponent(BoundingBox[] boundingBoxes)
        {
            this.boundingBoxes = boundingBoxes;
            Sendable = true;
        }
       

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as BoxCollisionComponent);
        }

        /// <inheritdoc/>
        public bool Equals(BoxCollisionComponent? other)
        {
            return other is not null &&
                   boundingBoxes.SequenceEqual(other.boundingBoxes);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(boundingBoxes);
        }
        /// <summary>Compare two box collision components for equality.</summary>
        /// <param name="left">The first <see cref="BoxCollisionComponent" /> to compare.</param>
        /// <param name="right">The second <see cref="BoxCollisionComponent" /> to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left"/> and <paramref name="right"/> are considered equal; otherwise, <see langword="false" />.</returns>
        /// <seealso cref="Equals(BoxCollisionComponent)" />
        public static bool operator ==(BoxCollisionComponent? left, BoxCollisionComponent? right)
        {
            return EqualityComparer<BoxCollisionComponent>.Default.Equals(left, right);
        }

        /// <summary>Compare two box collision components for equality.</summary>
        /// <param name="left">The first <see cref="BoxCollisionComponent" /> to compare.</param>
        /// <param name="right">The second <see cref="BoxCollisionComponent" /> to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left"/> and <paramref name="right"/> are considered equal; otherwise, <see langword="false" />.</returns>
        /// <seealso cref="Equals(BoxCollisionComponent)" />
        public static bool operator !=(BoxCollisionComponent? left, BoxCollisionComponent? right)
        {
            return !(left == right);
        }
    }
}
