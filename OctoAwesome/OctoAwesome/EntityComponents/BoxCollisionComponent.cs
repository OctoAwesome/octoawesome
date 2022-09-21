using engenious;
using OctoAwesome.Components;
using System;
using System.Linq;
using System.Collections.Generic;

namespace OctoAwesome.EntityComponents
{

    /// <summary>
    /// Component for entities with box collision.
    /// </summary>
    public sealed class BoxCollisionComponent : CollisionComponent, IEquatable<BoxCollisionComponent?>
    {
        /// <summary>
        /// Gets the collision bounding boxes of the entity.
        /// </summary>
        public ReadOnlySpan<BoundingBox> BoundingBoxes => new(boundingBoxes);

        private readonly BoundingBox[] boundingBoxes;

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
/// <inheritdoc/>

        public static bool operator ==(BoxCollisionComponent? left, BoxCollisionComponent? right)
        {
            return EqualityComparer<BoxCollisionComponent>.Default.Equals(left, right);
        }
/// <inheritdoc/>

        public static bool operator !=(BoxCollisionComponent? left, BoxCollisionComponent? right)
        {
            return !(left == right);
        }
    }
}
