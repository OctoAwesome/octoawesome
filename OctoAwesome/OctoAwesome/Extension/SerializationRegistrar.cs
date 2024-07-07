﻿using OctoAwesome.Extension;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;

namespace OctoAwesome.Extension
{
    /// <summary>
    /// Registrar class for handling types annotated with the <see cref="SerializationIdAttribute"/>.
    /// </summary>
    public class SerializationRegistrar : IExtensionRegistrar<BaseSerializationIdAttribute>
    {
        /// <inheritdoc />
        public string ChannelName => ChannelNames.Serialization;

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
        public void Register(BaseSerializationIdAttribute type)
        {
            var serId = type.CombinedId;

            if (serId == 0)
                throw new ArgumentException($"Missing {nameof(SerializationIdAttribute)} on type {type.Type.Name}, so it cant be registered.");

            SerializationIdTypeProvider.Register(serId, type.Type);
        }

        /// <summary>
        /// Registers a Type without the <see cref="SerializationIdAttribute"/>.
        /// </summary>
        /// <param name="type">Type without <see cref="SerializationIdAttribute"/></param>
        /// <param name="serializationId">The serialization id which normally would be given via <see cref="SerializationIdAttribute"/></param>
        //public void Register(Type type, ulong serializationId)
        //{

        //    if (serializationId == 0)
        //        throw new ArgumentException($"0 is not allowed for a serialization id, because it indicates a missing attribute of {nameof(SerializationIdAttribute)}.");

        //    SerializationIdTypeProvider.Register(serializationId, type);
        //}

        /// <summary>
        /// Not supported
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public void Unregister(BaseSerializationIdAttribute value) => throw new NotSupportedException();

        /// <summary>
        /// Not supported, use <see cref="SerializationIdTypeProvider"/> instead
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public IReadOnlyCollection<BaseSerializationIdAttribute> Get() => throw new NotSupportedException($"Please use {nameof(SerializationIdTypeProvider)} instead");
    }
}
