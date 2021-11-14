using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public sealed class SerializationIdTypeProvider
    {
        private readonly Dictionary<ulong, Type> types = new();

        public void Register(ulong serId, Type type)
        {
            types.Add(serId, type);
        }

        public Type Get(ulong serId)
        {
            return types[serId];
        }

        public Type Get(uint modId, uint typeId)
        {
            ulong serId = ((ulong)modId << sizeof(uint)) | typeId;
            return Get(serId);
        }

        public bool TryGet(ulong serId, out Type type)
        {
            return types.TryGetValue(serId, out type);
        }

        public bool TryGet(uint modId, uint typeId, out Type type)
        {
            ulong serId = ((ulong)modId << sizeof(uint)) | typeId;
            return TryGet(serId, out type);
        }
    }
}
