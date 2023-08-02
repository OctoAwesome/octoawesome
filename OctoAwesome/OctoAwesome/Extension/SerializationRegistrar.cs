using OctoAwesome.Extension;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;

namespace OctoAwesome
{
    /// <summary>
    /// Registrar class for handling types annotated with the <see cref="SerializationIdAttribute"/>.
    /// </summary>
    public class SerializationRegistrar : BaseRegistrar<Type>
    {
        /// <inheritdoc />
        public override string ChannelName => ChannelNames.Serialization;


        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationRegistrar"/> class.
        /// </summary>
        /// <param name="serializationIdTypeProvider">
        /// The <see cref="SerializationIdTypeProvider"/> used for registering serialization types.</param>
        public SerializationRegistrar()
        {
        }

        /// <summary>
        /// Registers a Type with the required <see cref="SerializationIdAttribute"/>.
        /// </summary>
        /// <param name="type">Type with <see cref="SerializationIdAttribute"/></param>
        public override void Register(Type type)
        {
            var serId = type.SerializationId();

            if (serId == 0)
                throw new ArgumentException($"Missing {nameof(SerializationIdAttribute)} on type {type.Name}, so it cant be registered.");

            SerializationIdTypeProvider.Register(serId, type);
        }

        /// <summary>
        /// Registers a Type without the <see cref="SerializationIdAttribute"/>.
        /// </summary>
        /// <param name="type">Type without <see cref="SerializationIdAttribute"/></param>
        /// <param name="serializationId">The serialization id which normally would be given via <see cref="SerializationIdAttribute"/></param>
        public void Register(Type type, ulong serializationId)
        {

            if (serializationId == 0)
                throw new ArgumentException($"0 is not allowed for a serialization id, because it indicates a missing attribute of {nameof(SerializationIdAttribute)}.");

            SerializationIdTypeProvider.Register(serializationId, type);
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public override void Unregister(Type value) => throw new NotSupportedException();

        /// <summary>
        /// Not supported, use <see cref="SerializationIdTypeProvider"/> instead
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public override IReadOnlyCollection<Type> Get() => throw new NotSupportedException($"Please use {nameof(SerializationIdTypeProvider)} instead");
    }
}
