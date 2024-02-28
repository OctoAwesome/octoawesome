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
        private byte[]? value;
        private int componentId;

        /// <summary>
        /// Gets or sets the name of the issuer that caused the property change.
        /// </summary>
        public ulong Issuer { get; set; }


        /// <summary>
        /// Gets or sets the raw data of the new property value.
        /// </summary>
        public byte[] Value
        {
            get => NullabilityHelper.NotNullAssert(value, $"{nameof(Value)} was not initialized!");
            set => this.value = NullabilityHelper.NotNullAssert(value, $"{nameof(Value)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets or sets the raw data of the new property value.
        /// </summary>
        public int ComponentId
        {
            get => componentId;
            set => this.componentId = value;
        }


        /// <inheritdoc />
        protected override void OnRelease()
        {
            Issuer = 0;
            value = default;
            componentId = default;

            base.OnRelease();
        }
    }
}
