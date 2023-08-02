using System;

namespace OctoAwesome
{
    /// <summary>
    /// Attribute for associating serialization ids to types for generic serialization across multiple mods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class SerializationIdAttribute : Attribute
    {
        /// <summary>
        /// Gets the id for the mod.
        /// </summary>
        public uint ModId { get; }

        /// <summary>
        /// Gets the id for the type in the mod.
        /// </summary>
        public new uint TypeId { get; }

        /// <summary>
        /// Gets the id combined from <see cref="ModId"/> and <see cref="TypeId"/>.
        /// </summary>
        public ulong CombinedId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationIdAttribute"/> class.
        /// </summary>
        /// <param name="modId">The id for the mod.</param>
        /// <param name="typeId">The id for the type in the mod.</param>
        public SerializationIdAttribute(uint modId, uint typeId)
        {
            ModId = modId;
            TypeId = typeId;
            CombinedId = ((ulong)modId << (sizeof(uint) * 8)) | typeId;
        }
    }
}
