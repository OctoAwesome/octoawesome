using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OctoAwesome.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Type provider for serialization ids.
    /// </summary>
    public static class SerializationIdTypeProvider
    {
        private readonly static Dictionary<ulong, Type> types = new();

        /// <summary>
        /// Registers a type associated to a given serialization id(<see cref="SerializationIdAttribute.CombinedId"/>).
        /// </summary>
        /// <param name="serId">The serialization id to associated the <see cref="Type"/> to.</param>
        /// <param name="type">The type to register with the serialization id association.</param>
        public static void Register(ulong serId, Type type)
        {
            types.Add(serId, type);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> associated to a serialization id(<see cref="SerializationIdAttribute.CombinedId"/>).
        /// </summary>
        /// <param name="serId">The serialization id to get the <see cref="Type"/> for.</param>
        /// <returns>The type associated to the serialization id.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no matching <see cref="Type"/> was found for the given serialization id.
        /// </exception>
        public static Type Get(ulong serId)
        {
            return types[serId];
        }

        private static ulong SerIdFromModTypeId(uint modId, uint typeId) => ((ulong)modId << (sizeof(uint) * 8)) | typeId;

        /// <summary>
        /// Gets the <see cref="Type"/> associated to a mod and type id
        /// (<see cref="SerializationIdAttribute.ModId"/> and <see cref="SerializationIdAttribute.TypeId"/>).
        /// </summary>
        /// <param name="modId">The mod id to get the <see cref="Type"/> for.</param>
        /// <param name="typeId">The type id to get the <see cref="Type"/> for.</param>
        /// <returns>The type associated to the serialization id.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no matching <see cref="Type"/> was found for the given mod and type id.
        /// </exception>
        public static Type Get(uint modId, uint typeId)
        {
            return Get(SerIdFromModTypeId(modId, typeId));
        }

        /// <summary>
        /// Tries to get the <see cref="Type"/> associated to a serialization id
        /// (<see cref="SerializationIdAttribute.CombinedId"/>).
        /// </summary>
        /// <param name="serId">The serialization id to get the <see cref="Type"/> for.</param>
        /// <param name="type">The type associated to the serialization id.</param>
        /// <returns><c>true</c> when the type could be found; otherwise <c>false</c>.</returns>
        public static bool TryGet(ulong serId, [MaybeNullWhen(false)] out Type type)
        {
            return types.TryGetValue(serId, out type);
        }

        /// <summary>
        /// Tries to get the <see cref="Type"/> associated to a serialization id
        /// (<see cref="SerializationIdAttribute.CombinedId"/>).
        /// </summary>
        /// <param name="modId">The mod id to get the <see cref="Type"/> for.</param>
        /// <param name="typeId">The type id to get the <see cref="Type"/> for.</param>
        /// <param name="type">The type associated to the serialization id.</param>
        /// <returns><c>true</c> when the type could be found; otherwise <c>false</c>.</returns>
        public static bool TryGet(uint modId, uint typeId, [MaybeNullWhen(false)] out Type type)
        {
            return TryGet(SerIdFromModTypeId(modId, typeId), out type);
        }
    }
}
