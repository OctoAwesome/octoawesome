using System;

namespace OctoAwesome
{
    /// <summary>
    /// Attribute for telling the non existent source code generator, that a type id for this type should be generated.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class SerializationIdAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationIdAttribute{T}"/> class.
        /// </summary>
        public SerializationIdAttribute()
        {
        }
    }

    /// <summary>
    /// Base class for the <see cref="SerializationIdAttribute{T}"/>
    /// </summary>
    public abstract class BaseSerializationIdAttribute : Attribute
    {
        /// <summary>
        /// Gets the serialization id
        /// </summary>
        public abstract ulong CombinedId { get; }

        /// <summary>
        /// Gets the type for which this serialzation id attribute should be used
        /// </summary>
        public abstract Type Type { get; }
    }

    /// <summary>
    /// Attribute for associating serialization ids to types for generic serialization across multiple mods.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class SerializationIdAttribute<T> : BaseSerializationIdAttribute
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
        public override ulong CombinedId { get; }

        /// <summary>
        /// Gets the type for which this serialzation id attribute should be used
        /// </summary>
        public override Type Type => typeof(T);

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationIdAttribute{T}"/> class.
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
