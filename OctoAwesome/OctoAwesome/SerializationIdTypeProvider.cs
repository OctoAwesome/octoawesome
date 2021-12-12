using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OctoAwesome.Serialization;

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
        
        private static ulong SerIdFromModTypeId(uint modId, uint typeId) => ((ulong)modId << (sizeof(uint) * 8)) | typeId;

        public Type Get(uint modId, uint typeId)
        {
            return Get(SerIdFromModTypeId(modId, typeId));
        }
        public bool TryGet(ulong serId, [MaybeNullWhen(false)] out Type type)
        {
            return types.TryGetValue(serId, out type);
        }

        public bool TryGet(uint modId, uint typeId, [MaybeNullWhen(false)] out Type type)
        {
            return TryGet(SerIdFromModTypeId(modId, typeId), out type);
        }
    }
}
