using System;

namespace OctoAwesome.Serialization
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class SerializationIdAttribute : Attribute
    {

        public uint ModId { get; }
        public new uint TypeId { get; }

        public ulong CombinedId { get; }
        public SerializationIdAttribute(uint modId, uint typeId)
        {
            ModId = modId;
            TypeId = typeId;
            CombinedId = ((ulong)modId << (sizeof(uint) * 8)) | typeId;
        }
    }
}
