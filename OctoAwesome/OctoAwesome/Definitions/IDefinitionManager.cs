using System.Collections.Generic;

namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Interface for definition managers for managing definitions.
    /// </summary>
    public interface IDefinitionManager
    {
        /// <summary>
        /// Gets an array of definitions.
        /// </summary>
        IDefinition[] Definitions { get; }

        /// <summary>
        /// Gets an array of item definitions (includes Blocks, Resources, Tools)
        /// </summary>
        IItemDefinition[] ItemDefinitions { get; }

        /// <summary>
        /// Gets an array of block definitions.
        /// </summary>
        IBlockDefinition[] BlockDefinitions { get; }

        /// <summary>
        /// Gets an array of material definitions.
        /// </summary>
        IMaterialDefinition[] MaterialDefinitions { get; }
        IFoodMaterialDefinition[] FoodDefinitions { get; }

        /// <summary>
        /// Gets the block definition by a block index.
        /// </summary>
        /// <param name="index">The index of the block definition.</param>
        /// <returns>The <see cref="BlockDefinition"/>.</returns>
        IBlockDefinition? GetBlockDefinitionByIndex(ushort index);

        /// <summary>
        /// Gets a block definition using the block definition's type name
        /// </summary>
        /// <param name="typeName">The type name of the block definition to retrieve.</param>
        /// <typeparam name="T">The generic type of the block definition.</typeparam>
        /// <returns>The retrieved block definition if a matching one was found; otherwise <c>null</c>.</returns>
        T? GetDefinitionByTypeName<T>(string typeName) where T : IDefinition;

        /// <summary>
        /// Gets the index of a block definition.
        /// </summary>
        /// <param name="definition">The block definition to get the index of.</param>
        /// <returns>Index of the block definition.</returns>
        ushort GetDefinitionIndex(IDefinition definition);

        /// <summary>
        /// Gets the block definition index by a generic type.
        /// </summary>
        /// <typeparam name="T">The block definition type.</typeparam>
        /// <returns>The index of the block definition.</returns>
        ushort GetDefinitionIndex<T>() where T : IDefinition;

        /// <summary>
        /// Gets an enumeration of block definitions matching a given generic type.
        /// </summary>
        /// <typeparam name="T">Type of the block definitions to enumerate.</typeparam>
        /// <returns>The enumeration of the block definitions.</returns>
        IEnumerable<T> GetDefinitions<T>() where T : class, IDefinition;
    }
}
