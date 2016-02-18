using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Runtime
{
    public class DefinitionManager : IDefinitionManager
    {
        #region Singleton

        private static DefinitionManager instance;

        public static IDefinitionManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new DefinitionManager();
                return instance;
            }
        }

        private DefinitionManager()
        {

        }

        #endregion

        private List<IItemDefinition> itemDefinitions;

        private List<IResourceDefinition> resourceDefinitions;

        private IBlockDefinition[] blockDefinitions;

        private void EnsureLoaded()
        {
            if (itemDefinitions == null)
            {
                // Items sammeln
                itemDefinitions = new List<IItemDefinition>();
                itemDefinitions.AddRange(ExtensionManager.GetInstances<IItemDefinition>());

                // Ressourcen sammeln
                resourceDefinitions = new List<IResourceDefinition>();
                resourceDefinitions.AddRange(itemDefinitions.OfType<IResourceDefinition>());

                // Blöcke sammeln
                blockDefinitions = itemDefinitions.OfType<IBlockDefinition>().ToArray();
            }
        }

        /// <summary>
        /// Liefert eine Liste aller bekannten Item Definitions (inkl. Blocks, Resources, Tools)
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IItemDefinition> GetItemDefinitions()
        {
            EnsureLoaded();
            return itemDefinitions;
        }

        /// <summary>
        /// Liefert eine Liste der bekannten Ressourcen.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IResourceDefinition> GetResourceDefinitions()
        {
            EnsureLoaded();
            return resourceDefinitions;
        }

        /// <summary>
        /// Liefert eine Liste der bekannten Blocktypen.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IBlockDefinition> GetBlockDefinitions()
        {
            EnsureLoaded();
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

            EnsureLoaded();
            return blockDefinitions[(index & Blocks.TypeMask) - 1];
        }

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <param name="definition">BlockDefinition</param>
        /// <returns>Index of Block Definition</returns>
        public ushort GetBlockDefinitionIndex(IBlockDefinition definition)
        {
            EnsureLoaded();
            return (ushort)(Array.IndexOf(blockDefinitions, definition) + 1);
        }
    }
}
