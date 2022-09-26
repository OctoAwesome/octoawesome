using System.IO;
using OctoAwesome.Extension;

namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Notification for a property change.
    /// </summary>
    public class PropertyChangedNotification : SerializableNotification
    {
        private string? issuer, property;
        private byte[]? value;

        /// <summary>
        /// Gets or sets the name of the issuer that caused the property change.
        /// </summary>
        public string Issuer
        {
            get => NullabilityHelper.NotNullAssert(issuer, $"{nameof(Issuer)} was not initialized!");
            set => issuer = NullabilityHelper.NotNullAssert(value, $"{nameof(Issuer)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets or sets the name of the property that was changed.
        /// </summary>
        public string Property
        {
            get => NullabilityHelper.NotNullAssert(property, $"{nameof(Property)} was not initialized!");
            set => property = NullabilityHelper.NotNullAssert(value, $"{nameof(Property)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets or sets the raw data of the new property value.
        /// </summary>
        public byte[] Value
        {
            get => NullabilityHelper.NotNullAssert(value, $"{nameof(Value)} was not initialized!");
            set => this.value = NullabilityHelper.NotNullAssert(value, $"{nameof(Value)} cannot be initialized with null!");
        }

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
            issuer = default;
            property = default;
            value = default;

            base.OnRelease();
        }
    }
}
