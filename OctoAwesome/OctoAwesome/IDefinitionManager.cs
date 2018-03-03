using OctoAwesome.Entities;
using System.Collections.Generic;

namespace OctoAwesome
{
    /// <summary>
    /// Basisinterface für einen DefinitionManager, der z.B. Erweiterungen behandelt
    /// </summary>
    public interface IDefinitionManager
    {
        /// <summary>
        /// Liefert eine Liste von Definitions.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDefinition> GetDefinitions();
        /// <summary>
        /// Liefert eine Liste aller bekannten Item Definitions (inkl. Blocks, Resources, Tools)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IItemDefinition> GetItemDefinitions();
        /// <summary>
        /// Liefert eine Liste der bekannten Blocktypen.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IBlockDefinition> GetBlockDefinitions();
        /// <summary>
        /// Returns all <see cref="IResourceDefinition"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<IResourceDefinition> GetResourceDefinitions();
        /// <summary>
        /// Returns all <see cref="IEntityDefinition"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEntityDefinition> GetEntityDefinitions();
        /// <summary>
        /// Return als <see cref="IInventoryableDefinition"/>
        /// </summary>
        /// <returns></returns>
        IEnumerable<IInventoryableDefinition> GetInventoryableDefinitions();
        /// <summary>
        /// Liefert eine <see cref="IDefinition"/> zum angegebenen Index.
        /// </summary>
        /// <typeparam name="T">Type of Definition</typeparam>
        /// <param name="index">Index der BlockDefinition</param>
        /// <returns></returns>
        T GetDefinitionByIndex<T>(ushort index) where T : class, IDefinition;
        /// <summary>
        /// Return an <see cref="IDefinition"/> for the given name;
        /// </summary>
        /// <typeparam name="T">Type of <see cref="IDefinition"/></typeparam>
        /// <param name="name">Name of <see cref="IDefinition"/></param>
        /// <returns></returns>
        T GetDefinitionByName<T>(string name) where T : class, IDefinition;
        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <param name="definition">BlockDefinition</param>
        /// <returns>Index der Block Definition</returns>
        ushort GetDefinitionIndex(IDefinition definition);
        /// <summary>
        /// Returns an index of an <see cref="IDefinition"/> by givenname
        /// </summary>
        /// <param name="name">Name of <see cref="IDefinition"/></param>
        /// <returns></returns>
        ushort GetDefinitionIndexByName(string name);
        /// <summary>
        /// Liefert den Index der angegebenen BlockDefinition.
        /// </summary>
        /// <typeparam name="T">BlockDefinition Type</typeparam>
        /// <returns>Index der Block Definition</returns>
        IList<ushort> GetDefinitionIndex<T>() where T : IDefinition;
        /// <summary>
        /// Gibt die Liste von Instanzen des angegebenen Definition Interfaces zurück.
        /// </summary>
        /// <typeparam name="T">Typ der Definition</typeparam>
        /// <returns>Auflistung von Instanzen</returns>
        IEnumerable<T> GetDefinitions<T>() where T : IDefinition;
    }
}
