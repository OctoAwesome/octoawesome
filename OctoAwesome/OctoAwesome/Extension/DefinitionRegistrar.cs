using OctoAwesome.Definitions;
using OctoAwesome.Extension;

using System;
using System.Collections.Generic;

namespace OctoAwesome
{
    /// <summary>
    /// The registrat to extend definitions
    /// </summary>
    public class DefinitionRegistrar : BaseRegistrar<Type>
    {
        /// <inheritdoc/>
        public override string ChannelName => ChannelNames.Definitions;

        private readonly StandaloneTypeContainer definitionTypeContainer;
        private readonly Dictionary<Type, List<Type>> definitionsLookup;        

        /// <summary>
        /// Initializes a new instance of the<see cref="DefinitionRegistrar" /> class
        /// </summary>
        public DefinitionRegistrar()
        {
            definitionTypeContainer = new StandaloneTypeContainer();
            definitionsLookup = new Dictionary<Type, List<Type>>();
        }

        /// <inheritdoc/>
        public override void Register(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var interfaceTypes = type.GetInterfaces();

            //TODO: Check type for idefinition, otherwise throw exception

            foreach (var interfaceType in interfaceTypes)
            {
                if (definitionsLookup.TryGetValue(interfaceType, out var typeList))
                {
                    typeList.Add(type);
                }
                else
                {
                    definitionsLookup.Add(interfaceType, new List<Type> { type });
                }
            }

            definitionTypeContainer.Register(type, type, InstanceBehaviour.Singleton);
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public override void Unregister(Type definition) 
        {
            throw new NotSupportedException("Currently not supported by TypeContainer");
        }
        /// <summary>
        /// Not supported, use <see cref="Get{T}"/> instead
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public override IReadOnlyCollection<Type> Get() => throw new NotSupportedException();


        /// <summary>
        /// Return a List of Definitions
        /// </summary>
        /// <typeparam name="T">Definitiontype</typeparam>
        /// <returns>List</returns>
        public IEnumerable<T> Get<T>() where T : class, IDefinition
        {
            if (definitionsLookup.TryGetValue(typeof(T), out var definitionTypes))
            {
                foreach (var type in definitionTypes)
                    yield return (T)definitionTypeContainer.Get(type);
            }
        }
    }
}
