using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Definition Manager, der Typen aus Erweiterungen nachlädt.
    /// </summary>
    public class DefinitionManager : IDefinitionManager
    {
        private readonly IExtensionResolver extensionResolver;

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

        /// <summary>
        /// Liefert eine Liste von Defintions.
        /// </summary>
        /// <returns></returns>
        public IDefinition[] Definitions { get; }

        /// <summary>
        /// Liefert eine Liste aller bekannten Item Definitions (inkl. Blocks, Resources, Tools)
        /// </summary>
        /// <returns></returns>
        public IItemDefinition[] ItemDefinitions { get; }

        /// <summary>
        /// Liefert eine Liste der bekannten Blocktypen.
        /// </summary>
        /// <returns></returns>
        public IBlockDefinition[] BlockDefinitions { get; }

        public IMaterialDefinition[] MaterialDefinitions { get; }

        /// <summary>
        /// Liefert die BlockDefinition zum angegebenen Index.
        /// </summary>
        /// <param name="index">Index der BlockDefinition</param>
        /// <returns>BlockDefinition</returns>
        public IBlockDefinition GetBlockDefinitionByIndex(ushort index)
        {
            if (index == 0)
                return null;

            return (IBlockDefinition)Definitions[(index & Blocks.TypeMask) - 1];
        }

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <param name="definition">BlockDefinition</param>
        /// <returns>Index der Block Definition</returns>
        public ushort GetDefinitionIndex(IDefinition definition)
        {
            return (ushort)(Array.IndexOf(Definitions, definition) + 1);
        }

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <typeparam name="T">BlockDefinition Type</typeparam>
        /// <returns>Index der Block Definition</returns>
        public ushort GetDefinitionIndex<T>() where T : IDefinition
        {
            int i = 0;
            IDefinition definition = default;
            foreach (var  d in Definitions)
            {
                if (i > 0 && d.GetType() == typeof(T))
                {
                    throw new InvalidOperationException("Multiple Object where found that match the condition");
                }
                else if (i == 0 && d.GetType() == typeof(T))
                {
                    definition = d;
                    ++i;
                }
            }
            return GetDefinitionIndex(definition);
        }

        /// <summary>
        /// Gibt die Liste von Instanzen des angegebenen Definition Interfaces zurück.
        /// </summary>
        /// <typeparam name="T">Typ der Definition</typeparam>
        /// <returns>Auflistung von Instanzen</returns>
        public IEnumerable<T> GetDefinitions<T>() where T : class, IDefinition
        {
            // TODO: Caching (Generalisiertes IDefinition-Interface für Dictionary (+1 von Maxi am 07.04.2021))
            return extensionResolver.GetDefinitions<T>();
        }

        public T GetDefinitionByTypeName<T>(string typeName) where T : IDefinition
        {
            var searchedType = typeof(T);
            if (typeof(IBlockDefinition).IsAssignableFrom(searchedType))
            {
                return GetDefinitionFromArrayByTypeName<T>(typeName, BlockDefinitions);
            }
            else if (typeof(IItemDefinition).IsAssignableFrom(searchedType))
            {
                return GetDefinitionFromArrayByTypeName<T>(typeName, ItemDefinitions);
            }
            else if (typeof(IMaterialDefinition).IsAssignableFrom(searchedType))
            {
                return GetDefinitionFromArrayByTypeName<T>(typeName, MaterialDefinitions);
            }
            else
            {
                return default;
            }
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
