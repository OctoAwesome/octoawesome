using System.IO;

namespace OctoAwesome.Basics.EntityComponents
{
    /// <summary>
    /// Component to apply power to the body of an entity.
    /// </summary>
    public sealed class BodyPowerComponent : PowerComponent
    {
        /// <summary>
        /// Gets or sets a value indicating the time a jump should take.
        /// </summary>
        public int JumpTime { get; set; }

        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(JumpTime);
        }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            JumpTime = reader.ReadInt32();
        }
    }
}
