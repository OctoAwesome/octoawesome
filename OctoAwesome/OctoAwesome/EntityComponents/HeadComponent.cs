using engenious;
using System.IO;
using OctoAwesome.Components;
using OctoAwesome.Serialization;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Component describing the head properties of an entity.
    /// </summary>
    [Nooson, SerializationId()]
    public sealed partial class HeadComponent : Component, IEntityComponent
    {
        /// <summary>
        /// Gets or sets the offset the head is located at relative to the entity position.
        /// </summary>
        public Vector3 Offset { get; set; }

        /// <summary>
        /// Gets or sets the tilt of the head(in radians).
        /// </summary>
        public float Tilt { get; set; }

        /// <summary>
        /// Gets or sets the angle of the head(in radians).
        /// </summary>
        public float Angle { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeadComponent"/> class.
        /// </summary>
        public HeadComponent()
        {

        }


    }
}
