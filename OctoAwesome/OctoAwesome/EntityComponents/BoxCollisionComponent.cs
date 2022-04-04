using engenious;
using OctoAwesome.Components;
using System;

namespace OctoAwesome.EntityComponents
{

    public sealed class BoxCollisionComponent : CollisionComponent, IFunctionalBlockComponent
    {

        public ReadOnlySpan<BoundingBox> BoundingBoxes => new(boundingBoxes);

        private readonly BoundingBox[] boundingBoxes;
        public BoxCollisionComponent()
        {

        }

        public BoxCollisionComponent(BoundingBox[] boundingBoxes)
        {
            this.boundingBoxes = boundingBoxes;
        }
    }
}
