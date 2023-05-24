using System.IO;
using OctoAwesome.Extension;
using OctoAwesome.Serialization;

namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Notification for a property change.
    /// </summary>
    [Nooson]
    public partial class PropertyChangedNotification : SerializableNotification, IConstructionSerializable<PropertyChangedNotification>
    {
        private string? issuer;
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
        /// Gets or sets the raw data of the new property value.
        /// </summary>
        public byte[] Value
        {
            get => NullabilityHelper.NotNullAssert(value, $"{nameof(Value)} was not initialized!");
            set => this.value = NullabilityHelper.NotNullAssert(value, $"{nameof(Value)} cannot be initialized with null!");
        }


        /// <inheritdoc />
        protected override void OnRelease()
        {
            issuer = default;
            value = default;

            base.OnRelease();
        }
    }
}
