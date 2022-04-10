using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Definition Manager which loads extensions.
    /// </summary>
    public class DefinitionManager : IDefinitionManager
    {
        private readonly IExtensionResolver extensionResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinitionManager"/> class.
        /// </summary>
        /// <param name="extensionResolver">The extension resolver to get definitions from extensions with.</param>
        public DefinitionManager(IExtensionResolver extensionResolver)
        {
            this.extensionResolver = extensionResolver;

            Definitions = extensionResolver.GetDefinitions<IDefinition>().ToArray();

            // collect items
            ItemDefinitions = Definitions.OfType<IItemDefinition>().ToArray();

            // collect blocks
            BlockDefinitions = Definitions.OfType<IBlockDefinition>().ToArray();

            // collect materials
            MaterialDefinitions = Definitions.OfType<IMaterialDefinition>().ToArray();
        }

        /// <inheritdoc />
        public IDefinition[] Definitions { get; }

        /// <inheritdoc />
        public IItemDefinition[] ItemDefinitions { get; }

        /// <inheritdoc />
        public IBlockDefinition[] BlockDefinitions { get; }

        /// <inheritdoc />
        public IMaterialDefinition[] MaterialDefinitions { get; }

        /// <inheritdoc />
        public IBlockDefinition? GetBlockDefinitionByIndex(ushort index)
        {
            if (index == 0)
                return null;

            return (IBlockDefinition)Definitions[(index & Blocks.TypeMask) - 1];
        }

        /// <inheritdoc />
        public ushort GetDefinitionIndex(IDefinition definition)
        {
            return (ushort)(Array.IndexOf(Definitions, definition) + 1);
        }

        /// <inheritdoc />
        public ushort GetDefinitionIndex<T>() where T : IDefinition
        {
            int i = 0;
            IDefinition? definition = default;
            foreach (var d in Definitions)
            {
                if (i > 0 && d.GetType() == typeof(T))
                {
                    throw new InvalidOperationException("Multiple Object where found that match the condition");
                }

                if (i == 0 && d.GetType() == typeof(T))
                {
                    definition = d;
                    ++i;
                }
            }
            return definition == null ? (ushort)0 : GetDefinitionIndex(definition);
        }

        /// <inheritdoc />
        public IEnumerable<T> GetDefinitions<T>() where T : class, IDefinition
        {
            // TODO: Caching (Generalized IDefinition-Interface for Dictionary (+1 from Maxi on 07.04.2021))
            return extensionResolver.GetDefinitions<T>();
        }

        /// <inheritdoc />
        public T? GetDefinitionByTypeName<T>(string typeName) where T : IDefinition
        {
            var searchedType = typeof(T);
            if (typeof(IBlockDefinition).IsAssignableFrom(searchedType))
            {
                return GetDefinitionFromArrayByTypeName<T>(typeName, BlockDefinitions);
            }

            if (typeof(IItemDefinition).IsAssignableFrom(searchedType))
            {
                return GetDefinitionFromArrayByTypeName<T>(typeName, ItemDefinitions);
            }

            if (typeof(IMaterialDefinition).IsAssignableFrom(searchedType))
            {
                return GetDefinitionFromArrayByTypeName<T>(typeName, MaterialDefinitions);
            }

            return default;
        }

        private static T GetDefinitionFromArrayByTypeName<T>(string typeName, IDefinition[] array) where T : IDefinition
        {
            foreach (var definition in array)
            {
                if (string.Equals(definition.GetType().FullName, typeName))
                    return (T)definition;
            }

            return default;
        }
    }
}
