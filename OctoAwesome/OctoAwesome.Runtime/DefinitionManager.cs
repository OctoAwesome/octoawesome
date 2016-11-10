using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Definition Manager, der Typen aus Erweiterungen nachlädt.
    /// </summary>
    public class DefinitionManager : IDefinitionManager
    {
        private List<IItemDefinition> itemDefinitions;

        private List<IResourceDefinition> resourceDefinitions;

        private IBlockDefinition[] blockDefinitions;

        private IExtensionResolver extensionResolver;

        public DefinitionManager(IExtensionResolver extensionResolver)
        {
            this.extensionResolver = extensionResolver;

            // Items sammeln
            itemDefinitions = new List<IItemDefinition>();
            itemDefinitions.AddRange(extensionResolver.GetDefinitions<IItemDefinition>());

            // Ressourcen sammeln
            resourceDefinitions = new List<IResourceDefinition>();
            resourceDefinitions.AddRange(itemDefinitions.OfType<IResourceDefinition>());

            // Blöcke sammeln
            blockDefinitions = itemDefinitions.OfType<IBlockDefinition>().ToArray();
        }

        /// <summary>
        /// Liefert eine Liste aller bekannten Item Definitions (inkl. Blocks, Resources, Tools)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IItemDefinition> GetItemDefinitions()
        {
            return itemDefinitions;
        }

        /// <summary>
        /// Liefert eine Liste der bekannten Ressourcen.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IResourceDefinition> GetResourceDefinitions()
        {
            return resourceDefinitions;
        }

        /// <summary>
        /// Liefert eine Liste der bekannten Blocktypen.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IBlockDefinition> GetBlockDefinitions()
        {
            return blockDefinitions;
        }

        /// <summary>
        /// Liefert die BlockDefinition zum angegebenen Index.
        /// </summary>
        /// <param name="index">Index der BlockDefinition</param>
        /// <returns>BlockDefinition</returns>
        public IBlockDefinition GetBlockDefinitionByIndex(ushort index)
        {
            if (index == 0)
                return null;

            return blockDefinitions[(index & Blocks.TypeMask) - 1];
        }

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <param name="definition">BlockDefinition</param>
        /// <returns>Index der Block Definition</returns>
        public ushort GetBlockDefinitionIndex(IBlockDefinition definition)
        {
            return (ushort)(Array.IndexOf(blockDefinitions, definition) + 1);
        }

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <typeparam name="T">BlockDefinition Type</typeparam>
        /// <returns>Index der Block Definition</returns>
        public ushort GetBlockDefinitionIndex<T>() where T : IBlockDefinition
        {
            IBlockDefinition definition = blockDefinitions.SingleOrDefault(d => d.GetType() == typeof(T));
            return GetBlockDefinitionIndex(definition);
        }

        /// <summary>
        /// Gibt die Liste von Instanzen des angegebenen Definition Interfaces zurück.
        /// </summary>
        /// <typeparam name="T">Typ der Definition</typeparam>
        /// <returns>Auflistung von Instanzen</returns>
        public IEnumerable<T> GetDefinitions<T>() where T : IDefinition
        {
            // TODO: Caching (Generalisiertes IDefinition-Interface für Dictionary)
            return extensionResolver.GetDefinitions<T>();
        }
    }
}
