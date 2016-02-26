using System.Collections.Generic;

namespace OctoAwesome
{
    /// <summary>
    /// Basisinterface für einen DefinitionManager, der z.B. Erweiterungen behandelt
    /// </summary>
    public interface IDefinitionManager
    {
        /// <summary>
        /// Liefert eine Liste aller bekannten Item Definitions (inkl. Blocks, Resources, Tools)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IItemDefinition> GetItemDefinitions();

        /// <summary>
        /// Liefert eine Liste der bekannten Ressourcen.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IResourceDefinition> GetResourceDefinitions();

        /// <summary>
        /// Liefert eine Liste der bekannten Blocktypen.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IBlockDefinition> GetBlockDefinitions();

        /// <summary>
        /// Liefert die BlockDefinition zum angegebenen Index.
        /// </summary>
        /// <param name="index">Index der BlockDefinition</param>
        /// <returns>BlockDefinition</returns>
        IBlockDefinition GetBlockDefinitionByIndex(ushort index);

        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <param name="definition">BlockDefinition</param>
        /// <returns>Index der Block Definition</returns>
        ushort GetBlockDefinitionIndex(IBlockDefinition definition);
    }
}
