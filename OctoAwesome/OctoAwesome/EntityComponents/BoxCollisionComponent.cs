using engenious;
using OctoAwesome.Components;
using System;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Component for entities with box collision.
    /// </summary>
    public sealed class BoxCollisionComponent : CollisionComponent, IFunctionalBlockComponent
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

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxCollisionComponent"/> class.
        /// </summary>
        /// <param name="boundingBoxes">The collision bounding boxes of the entity.</param>
        public BoxCollisionComponent(BoundingBox[] boundingBoxes)
        {
            this.boundingBoxes = boundingBoxes;
        }
    }
}
