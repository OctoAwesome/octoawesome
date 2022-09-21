using OctoAwesome.Components;
using System.IO;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Component describing the body properties of an entity.
    /// </summary>
    public sealed class BodyComponent : Component, IEntityComponent
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

        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Mass);
            writer.Write(Radius);
            writer.Write(Height);
        }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            Mass = reader.ReadSingle();
            Radius = reader.ReadSingle();
            Height = reader.ReadSingle();
        }
    }
}
