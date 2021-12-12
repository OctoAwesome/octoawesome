using OctoAwesome.Serialization;
using System;
using System.IO;

namespace OctoAwesome.Notifications
{

    public sealed class FunctionalBlockNotification : SerializableNotification
    {

        public ActionType Type { get; set; }
        public Guid BlockId { get; set; }
        public FunctionalBlock Block
        {
            get => block; set
            {
                block = value;
                BlockId = value?.Id ?? default;
            }
        }

        private FunctionalBlock block;
        public FunctionalBlockNotification()
        {
        }

        public FunctionalBlockNotification(Guid id) : this()
        {
            BlockId = id;
        }
        public override void Deserialize(BinaryReader reader)
        {
            Type = (ActionType)reader.ReadInt32();
            if (Type == ActionType.Add) { }
            //Block = Serializer.Deserialize()
            else
                BlockId = new Guid(reader.ReadBytes(16));

        }
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
        protected override void OnRelease()
        {
            Type = default;
            Block = default;

            base.OnRelease();
        }
        public enum ActionType
        {

            None,
            Add,
            Remove,
            Update,
            

            Request
        }
    }
}
