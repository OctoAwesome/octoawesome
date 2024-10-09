using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

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
        /// <summary>
        /// Gets an array of food definitions.
        /// </summary>
        IFoodMaterialDefinition[] FoodDefinitions { get; }

        /// <summary>
        /// Loads all definitions and fills the arrays
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gets the definition by an index.
        /// </summary>
        /// <param name="index">The index of the definition.</param>
        /// <returns>The <see cref="IDefinition"/>.</returns>
        IDefinition? GetDefinitionByIndex(ushort index);

        /// <summary>
        /// Gets the definition by an index.
        /// </summary>
        /// <param name="index">The index of the definition.</param>
        /// <returns>The <see cref="IDefinition"/>.</returns>
        T? GetDefinitionByIndex<T>(ushort index) where T : IDefinition;

        /// <summary>
        /// Gets a definition using the definition's unique key
        /// </summary>
        /// <returns>The retrieved definition if a matching one was found; otherwise <c>null</c>.</returns>
        IDefinition? GetDefinitionByUniqueKey(string uniqueKey);

        /// <summary>
        /// Gets a definition using the definition's key and matches the t
        /// </summary>
        /// <returns>The retrieved definition if a matching one was found; otherwise <c>null</c>.</returns>
        T? GetDefinitionByUniqueKey<T>(string key);

        /// <summary>
        /// Gets a unique key for the definition
        /// </summary>
        /// <returns>The retrieved Unique key if a matching one was found; otherwise <c>null</c>.</returns>
        string? GetUniqueKeyByDefinition(IDefinition definition);

        /// <summary>
        /// Gets the index of a block definition.
        /// </summary>
        /// <param name="definition">The block definition to get the index of.</param>
        /// <returns>Index of the block definition.</returns>
        ushort GetDefinitionIndex(IDefinition definition);


        void RegisterDefinitionInstance(string key, JsonObject o, string[] jArr);
        bool TryGet<T>(string id, out T? definition) where T : IDefinition;
        void LoadSaveGame(IReadOnlyList<string>? sortedDefinitionKeys);
        ushort GetDefinitionIndex<T>(string key) where T : IDefinition;
        IReadOnlyCollection<string> GetSaveGameData();
        bool TryGetVariation<T>(IDefinition def, [NotNullWhen(true)] out T? variation);
        IReadOnlyCollection<IDefinition> GetVariations(IDefinition def);

        event EventHandler DefinitionsChanged;
    }
}
