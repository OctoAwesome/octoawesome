using OctoAwesome.Serialization;

using System;

namespace OctoAwesome
{
    public class SerializationRegistrar : BaseRegistrar<Type>
    {
        private readonly SerializationIdTypeProvider serializationIdTypeProvider;

        public SerializationRegistrar(ISettings settings, SerializationIdTypeProvider serializationIdTypeProvider) : base(settings)
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

        public override void Unregister(Type value) => throw new NotSupportedException();
    }
}
