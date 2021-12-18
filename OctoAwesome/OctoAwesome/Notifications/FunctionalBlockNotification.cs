using OctoAwesome.Serialization;
using System;
using System.IO;

namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Notifications caused by functional blocks.
    /// </summary>
    public sealed class FunctionalBlockNotification : SerializableNotification
    {
        /// <summary>
        /// Gets or sets the action type that caused the notification.
        /// </summary>
        public ActionType Type { get; set; }

        /// <summary>
        /// Gets or sets the id of the functional block that caused the notification.
        /// </summary>
        public Guid BlockId { get; set; }

        /// <summary>
        /// Gets or sets the functional block that caused the notification.
        /// </summary>
        public FunctionalBlock Block
        {
            get => block; set
            {
                block = value;
                BlockId = value?.Id ?? default;
            }
        }

        private FunctionalBlock block;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalBlockNotification"/> class.
        /// </summary>
        public FunctionalBlockNotification()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalBlockNotification"/> class.
        /// </summary>
        /// <param name="id">The id of the functional block that caused the notification.</param>
        public FunctionalBlockNotification(Guid id) : this()
        {
            BlockId = id;
        }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            Type = (ActionType)reader.ReadInt32();


            if (Type == ActionType.Add) { }
            //Block = Serializer.Deserialize()
            else
                BlockId = new Guid(reader.ReadBytes(16));

        }

        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write((int)Type);

            if (Type == ActionType.Add)
            {
                var bytes = Serializer.Serialize(Block);
                writer.Write(bytes.Length);
                writer.Write(bytes);
            }
            else
            {
                writer.Write(BlockId.ToByteArray());
            }

        }

        /// <inheritdoc />
        protected override void OnRelease()
        {
            Type = default;
            Block = default;

            base.OnRelease();
        }

        /// <summary>
        /// Enumeration o the function block action types.
        /// </summary>
        public enum ActionType
        {
            /// <summary>
            /// No specific action.
            /// </summary>
            None,

            /// <summary>
            /// Add the functional block.
            /// </summary>
            Add,

            /// <summary>
            /// Remove the functional block.
            /// </summary>
            Remove,

            /// <summary>
            /// Update the functional block.
            /// </summary>
            Update,

            /// <summary>
            /// Request the state of a functional block.
            /// </summary>
            Request
        }
    }
}
