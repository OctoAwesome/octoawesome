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
        private IDefinition[] definitions;

        private IItemDefinition[] itemDefinitions;

        private IResourceDefinition[] resourceDefinitions;

        private IBlockDefinition[] blockDefinitions;

        private IExtensionResolver extensionResolver;

        public DefinitionManager(IExtensionResolver extensionResolver)
        {
            this.extensionResolver = extensionResolver;

            definitions = extensionResolver.GetDefinitions<IDefinition>().ToArray();

            // Items sammeln
            itemDefinitions = definitions.OfType<IItemDefinition>().ToArray();

            // Ressourcen sammeln
            resourceDefinitions = definitions.OfType<IResourceDefinition>().ToArray();

            // Blöcke sammeln
            blockDefinitions = definitions.OfType<IBlockDefinition>().ToArray();
        }

        /// <summary>
        /// Liefert eine Liste von Defintions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDefinition> GetDefinitions()
        {
            return definitions;
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
        public IDefinition GetDefinitionByIndex(ushort index)
        {
            if (index == 0)
                return null;

            return definitions[(index & Blocks.TypeMask) - 1];
        }

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <param name="definition">BlockDefinition</param>
        /// <returns>Index der Block Definition</returns>
        public ushort GetDefinitionIndex(IDefinition definition)
        {
            return (ushort)(Array.IndexOf(definitions, definition) + 1);
        }

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <typeparam name="T">BlockDefinition Type</typeparam>
        /// <returns>Index der Block Definition</returns>
        public ushort GetDefinitionIndex<T>() where T : IDefinition
        {
            IDefinition definition = definitions.SingleOrDefault(d => d.GetType() == typeof(T));
            return GetDefinitionIndex(definition);
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
