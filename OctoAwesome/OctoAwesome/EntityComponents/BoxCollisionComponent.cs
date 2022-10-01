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
    public sealed class BoxCollisionComponent : CollisionComponent, IEquatable<BoxCollisionComponent?>
    {
        /// <summary>
        /// Gets the collision bounding boxes of the entity.
        /// </summary>
        public ReadOnlySpan<BoundingBox> BoundingBoxes => new(boundingBoxes);

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
        }
        /// <inheritdoc/>
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);
            writer.Write(boundingBoxes.Length);
            foreach (BoundingBox box in boundingBoxes)
            {
                writer.Write(box.Min.X);
                writer.Write(box.Min.Y);
                writer.Write(box.Min.Z);
                writer.Write(box.Max.X);
                writer.Write(box.Max.Y);
                writer.Write(box.Max.Z);
            }
        }

        /// <inheritdoc/>
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            var len = reader.ReadInt32();
            var boxes = new BoundingBox[len];
            for (int i = 0; i < len; i++)
            {
                boxes[i] = new BoundingBox(
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle()
                );

            }

            this.boundingBoxes = boxes;
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
