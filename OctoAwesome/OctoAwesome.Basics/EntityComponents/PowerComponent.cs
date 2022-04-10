using engenious;
using System.IO;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.EntityComponents
{
    /// <summary>
    /// Base component to apply power to an entity.
    /// </summary>
    public abstract class PowerComponent : Component, IEntityComponent
    {
        /// <summary>
        /// Gets or sets a value indicating the amount of power to apply onto the entity.
        /// </summary>
        public float Power { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the direction to apply the power onto the entity.
        /// </summary>
        public Vector3 Direction { get; set; }


        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Power);
        }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            Power = reader.ReadSingle();
        }
    }
}
