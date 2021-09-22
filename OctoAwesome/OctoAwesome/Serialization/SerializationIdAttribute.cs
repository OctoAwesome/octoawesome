using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class SerializationIdAttribute : Attribute
    {
        public uint ModId { get; }
        public uint TypeId { get; }

        public ulong CombinedId { get; }

        public SerializationIdAttribute(uint modId, uint typeId)
        {
            ModId = modId;
            TypeId = typeId;
            CombinedId = ((ulong)modId << sizeof(uint)) | typeId;
        }
    }
}
