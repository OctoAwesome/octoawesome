using OctoAwesome.Extension;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;

namespace OctoAwesome
{
    public class SerializationRegistrar : BaseRegistrar<Type>
    {
        public override string ChannelName => ChannelNames.Serialization;

        private readonly SerializationIdTypeProvider serializationIdTypeProvider;

        public SerializationRegistrar(SerializationIdTypeProvider serializationIdTypeProvider)
        {
            this.serializationIdTypeProvider = serializationIdTypeProvider;
        }

        /// <summary>
        /// Registers a Type with the required SerializationId attribute.
        /// </summary>
        /// <typeparam name="type">Type with serialization id attribute</typeparam>
        public override void Register(Type type)
        {
            var serId = type.SerializationId();

            if (serId == 0)
                throw new ArgumentException($"Missing {nameof(SerializationIdAttribute)} on type {type.Name}, so it cant be registered.");

            serializationIdTypeProvider.Register(serId, type);
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
