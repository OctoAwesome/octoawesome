﻿using OctoAwesome.Definitions;

using System;
using System.Collections.Generic;

namespace OctoAwesome
{
    public class DefinitionRegister : BaseRegistrar<Type, IDefinition>
    {
        private readonly StandaloneTypeContainer definitionTypeContainer;
        private readonly Dictionary<Type, List<Type>> definitionsLookup;

        public DefinitionRegister(ISettings settings) : base(settings)
        {
            definitionTypeContainer = new StandaloneTypeContainer();
            definitionsLookup = new Dictionary<Type, List<Type>>();
        }

        /// <summary>
        /// Registers a new Definition.
        /// </summary>
        /// <param name="type">Type of the Definition</param>
        public override void Register(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var interfaceTypes = type.GetInterfaces();

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
        /// Removes an existing Definition Type.
        /// </summary>
        /// <typeparam name="T">Definition Type</typeparam>
        public override void Unregister(IDefinition definition) 
        {
            throw new NotSupportedException("Currently not supported by TypeContainer");
        }

        /// <summary>
        /// Return a List of Definitions
        /// </summary>
        /// <typeparam name="T">Definitiontype</typeparam>
        /// <returns>List</returns>
        public IEnumerable<T> GetDefinitions<T>() where T : class, IDefinition
        {
            if (definitionsLookup.TryGetValue(typeof(T), out var definitionTypes))
            {
                foreach (var type in definitionTypes)
                    yield return (T)definitionTypeContainer.Get(type);
            }
        }
    }
}
