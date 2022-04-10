using System.IO;

namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Notification for a property change.
    /// </summary>
    public class PropertyChangedNotification : SerializableNotification
    {
        /// <summary>
        /// Gets or sets the name of the issuer that caused the property change.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the name of the property that was changed.
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// Gets or sets the raw data of the new property value.
        /// </summary>
        public byte[] Value { get; set; }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            Issuer = reader.ReadString();
            Property = reader.ReadString();
            var count = reader.ReadInt32();
            Value = reader.ReadBytes(count);
        }

        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Issuer);
            writer.Write(Property);
            writer.Write(Value.Length);
            writer.Write(Value);
        }

        /// <inheritdoc />
        protected override void OnRelease()
        {
            Issuer = default;
            Property = default;
            Value = default;

            base.OnRelease();
        }
    }
}
