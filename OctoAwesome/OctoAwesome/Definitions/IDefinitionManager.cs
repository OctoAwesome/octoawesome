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
        /// Loads all definitions and fills the arrays.
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
        /// Gets a definition using the definition's unique key.
        /// </summary>
        /// <param name="uniqueKey">The unique key of the definition.</param>
        /// <returns>The retrieved definition if a matching one was found; otherwise <c>null</c>.</returns>
        IDefinition? GetDefinitionByUniqueKey(string uniqueKey);

        /// <summary>
        /// Gets a definition using the definition's key and matches the type.
        /// </summary>
        /// <typeparam name="T">The type of the definition.</typeparam>
        /// <param name="key">The key of the definition.</param>
        /// <returns>The retrieved definition if a matching one was found; otherwise <c>null</c>.</returns>
        T? GetDefinitionByUniqueKey<T>(string key);

        /// <summary>
        /// Gets a unique key for the definition.
        /// </summary>
        /// <param name="definition">The definition to get the unique key for.</param>
        /// <returns>The retrieved unique key if a matching one was found; otherwise <c>null</c>.</returns>
        string? GetUniqueKeyByDefinition(IDefinition definition);

        /// <summary>
        /// Gets the index of a block definition.
        /// </summary>
        /// <param name="definition">The block definition to get the index of.</param>
        /// <returns>Index of the block definition.</returns>
        ushort GetDefinitionIndex(IDefinition definition);

        /// <summary>
        /// Registers a definition instance.
        /// </summary>
        /// <param name="key">The key of the definition.</param>
        /// <param name="o">The JSON object representing the definition.</param>
        /// <param name="jArr">The array of JSON strings representing the definition.</param>
        void RegisterDefinitionInstance(string key, JsonObject o, string[] jArr);

        /// <summary>
        /// Tries to get a definition by its ID.
        /// </summary>
        /// <typeparam name="T">The type of the definition.</typeparam>
        /// <param name="id">The ID of the definition.</param>
        /// <param name="definition">The retrieved definition if found.</param>
        /// <returns><c>true</c> if the definition was found; otherwise <c>false</c>.</returns>
        bool TryGet<T>(string id, out T? definition) where T : IDefinition;

        /// <summary>
        /// Loads the definitions based upon the sorted keys. If empty, it will load them in a predefined order, that could change based upon the definitions that exist.
        /// </summary>
        /// <param name="sortedDefinitionKeys">The sorted list of definition keys.</param>
        void LoadSaveGame(IReadOnlyList<string>? sortedDefinitionKeys);

        /// <summary>
        /// Gets the index of a definition by its key.
        /// </summary>
        /// <typeparam name="T">The type of the definition.</typeparam>
        /// <param name="key">The key of the definition.</param>
        /// <returns>The index of the definition.</returns>
        ushort GetDefinitionIndex<T>(string key) where T : IDefinition;

        /// <summary>
        /// Gets the ordered list of definition keys for the save game.
        /// </summary>
        /// <returns>THe ordered list of definition keys.</returns>
        IReadOnlyCollection<string> GetSaveGameData();

        /// <summary>
        /// Tries to get a variation of a definition.
        /// </summary>
        /// <typeparam name="T">The type of the variation.</typeparam>
        /// <param name="def">The definition to get the variation for.</param>
        /// <param name="variation">The retrieved variation if found.</param>
        /// <returns><c>true</c> if the variation was found; otherwise <c>false</c>.</returns>
        bool TryGetVariation<T>(IDefinition def, [NotNullWhen(true)] out T? variation);

        /// <summary>
        /// Gets the variations of a definition.
        /// </summary>
        /// <param name="def">The definition to get the variations for.</param>
        /// <returns>A read-only collection of variations.</returns>
        IReadOnlyCollection<IDefinition> GetVariations(IDefinition def);

        /// <summary>
        /// Gets the unique keys of a definition.
        /// </summary>
        /// <param name="def">The definition to get the unique keys for.</param>
        /// <returns>A read-only collection of unique keys.</returns>
        IReadOnlyCollection<string> GetUniqueKeys(IDefinition def);

        /// <summary>
        /// Event that is triggered when definitions are changed.
        /// </summary>
        event EventHandler DefinitionsChanged;
    }
}
