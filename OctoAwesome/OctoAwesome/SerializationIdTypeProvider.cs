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
        private readonly static Dictionary<ulong, Type> idToTypes = new();
        private readonly static Dictionary<Type, ulong> typesToId = new();

        /// <summary>
        /// Registers a type associated to a given serialization id(<see cref="SerializationIdAttribute.CombinedId"/>).
        /// </summary>
        /// <param name="serId">The serialization id to associated the <see cref="Type"/> to.</param>
        /// <param name="type">The type to register with the serialization id association.</param>
        public static void Register(ulong serId, Type type)
        {
            idToTypes.Add(serId, type);
            typesToId.Add(type, serId);
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
            return idToTypes[serId];
        }

        /// <summary>
        /// Gets the serialization id associated to a serialization type.
        /// </summary>
        /// <param name="type">The type to get the serialization id  for.</param>
        /// <returns>The type associated to the serialization id.</returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown when no matching serialization id was found for the given <see cref="Type"/>.
        /// </exception>
        public static ulong Get(Type type)
        {
            return typesToId[type];
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
            return idToTypes.TryGetValue(serId, out type);
        }
        /// <summary>
        /// Tries to get the serialization id associated to a the type
        /// (<see cref="SerializationIdAttribute.CombinedId"/>).
        /// </summary>
        /// <param name="type">The type associated to the serialization id.</param>
        /// <param name="serId">The serialization id to get the <see cref="Type"/> for.</param>
        /// <returns><c>true</c> when the type could be found; otherwise <c>false</c>.</returns>
        public static bool TryGet(Type type, [MaybeNullWhen(false)] out ulong serId)
        {
            return typesToId.TryGetValue(type, out serId);
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
