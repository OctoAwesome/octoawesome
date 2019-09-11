using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Notifications
{
    public class PropertyChangedNotification : SerializableNotification
    {
        public string Issuer { get; set; }
        public string Property { get; set; }

        public byte[] Value { get; set; }

        public override void Deserialize(BinaryReader reader)
        {
            Issuer = reader.ReadString();
            Property = reader.ReadString();
            var count = reader.ReadInt32();
            Value = reader.ReadBytes(count);
        }

        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Issuer);
            writer.Write(Property);
            writer.Write(Value.Length);
            writer.Write(Value);
        }

        protected override void OnRelease()
        {
            Issuer = default;
            Property = default;
            Value = default;

            base.OnRelease();
        }
    }
}
