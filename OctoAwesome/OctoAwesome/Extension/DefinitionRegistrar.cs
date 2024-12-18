using OctoAwesome.Caching;
using OctoAwesome.Definitions;
using OctoAwesome.Extension;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml;

namespace OctoAwesome.Extension
{
    /// <summary>
    /// Should contain the typename and definition that will be used to register the definition.
    /// </summary>
    /// <param name="TypeName">The generic name of the type.</param>
    /// <param name="Definition">The generic definition type.</param>
    public record struct TypeDefinitionRegistration(string TypeName, Type Definition);
    /// <summary>
    /// Should contain the id, element, and types that will be used to register the definition instance.
    /// </summary>
    /// <param name="Id">The unique id of the node.</param>
    /// <param name="Element">The deserialized node.</param>
    /// <param name="Types">All types that are in the json.</param>
    public record struct DefinitionInstanceRegistration(string Id, JsonNode Element, string[] Types);

    /// <summary>
    /// Should contain the id and definition that will be used to register the definition.
    /// </summary>
    /// <param name="Id">The unique id of the definition.</param>
    /// <param name="Definition">The actual instance of the definition.</param>
    public record struct DefinitionRegistration(string Id, IDefinition Definition);

    /// <summary>
    /// Registrar for managing type definitions, definition instances, and definitions.
    /// </summary>
    public class DefinitionRegistrar : IExtensionRegistrar<TypeDefinitionRegistration>, IExtensionRegistrar<DefinitionInstanceRegistration>, IExtensionRegistrar<DefinitionRegistration>
    {
        /// <inheritdoc/>
        public string ChannelName => ChannelNames.Definitions;

        /// <summary>
        /// Gets a list of all definitions with their associated unique key.
        /// </summary>
        public IReadOnlyDictionary<string, IDefinition> FlattenedDefinitions => flattenedDefinitions;

        /// <summary>
        /// Gets a list of all definition ids with their associated unique definition implementation.
        /// </summary>
        public IReadOnlyDictionary<IDefinition, string> FlattenedDefinitionIds => definitionIds;

        private Dictionary<string, IReadOnlyCollection<IDefinition>> readonlyDefinitions = [];
        private Dictionary<string, List<IDefinition>> definitions = [];
        private Dictionary<string, IDefinition> flattenedDefinitions = [];
        private Dictionary<IDefinition, string> definitionIds = [];
        private Dictionary<IDefinition, string> definitionVariations = [];
        private Dictionary<string, Type> definitionTypes = [];
        private Dictionary<IDefinition, ICollection<string>> typesPerDefinition = new();

        /// <summary>
        /// Registers a new type definition.
        /// </summary>
        /// <param name="value">The type definition registration to add.</param>
        public void Register(TypeDefinitionRegistration value)
        {
            if (!value.Definition.IsAssignableTo(typeof(IDefinition)))
                throw new ArgumentException(nameof(value.Definition));

            definitionTypes[value.TypeName] = value.Definition;
        }

        /// <summary>
        /// Registers a new definition instance.
        /// </summary>
        /// <param name="value">The definition instance registration to add.</param>
        public void Register(DefinitionInstanceRegistration value)
        {
            foreach (var type in value.Types)
            {
                if (definitionTypes.TryGetValue(type, out var definition))
                {
                    ref var entry = ref CollectionsMarshal.GetValueRefOrAddDefault(definitions, value.Id, out var exists);
                    if (!exists)
                        entry = new();

                    var def = (IDefinition)JsonSerializer.Deserialize(value.Element, definition)!;
                    var uniqueId = $"{value.Id}@{type}";
                    definitionVariations[def] = value.Id;
                    definitionIds[def] = uniqueId;
                    flattenedDefinitions[uniqueId] = def;
                    typesPerDefinition[def] = value.Types.ToImmutableArray();
                    entry!.Add(def);
                }
            }
        }

        /// <summary>
        /// Unregisters an existing type definition.
        /// </summary>
        /// <param name="value">The type definition registration to remove.</param>
        public void Unregister(TypeDefinitionRegistration value)
        {
            definitionTypes.Remove(value.TypeName);
        }

        /// <summary>
        /// Unregisters an existing definition instance.
        /// </summary>
        /// <param name="value">The definition instance registration to remove.</param>
        public void Unregister(DefinitionInstanceRegistration value)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a list of all definition instance registrations.
        /// </summary>
        /// <returns>A list of all definition instance registrations.</returns>
        IReadOnlyCollection<DefinitionInstanceRegistration> IExtensionRegistrar<DefinitionInstanceRegistration>.Get()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a list of all type definition registrations.
        /// </summary>
        /// <returns>A list of all type definition registrations.</returns>
        public IReadOnlyCollection<TypeDefinitionRegistration> Get()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets a specific definition by its id.
        /// </summary>
        /// <typeparam name="T">The type of the definition.</typeparam>
        /// <param name="id">The id of the definition.</param>
        /// <returns>The definition if found; otherwise, default.</returns>
        public T? Get<T>(string id)
        {
            if (definitions.TryGetValue(id, out var defs))
            {
                foreach (var def in defs)
                {
                    if (def is T t)
                        return t;
                }
            }

            return default;
        }

        /// <summary>
        /// Gets all definitions of a specific type.
        /// </summary>
        /// <typeparam name="T">The type of the definitions.</typeparam>
        /// <returns>An enumerable of all definitions of the specified type.</returns>
        public IEnumerable<T> GetAll<T>()
        {
            foreach (var defs in definitions.Values)
            {
                foreach (var def in defs)
                {
                    if (def is T t)
                        yield return t;
                }
            }
        }

        /// <summary>
        /// Gets all variations of a specific definition.
        /// </summary>
        /// <param name="def">The definition to get variations for.</param>
        /// <returns>A read-only collection of all variations of the specified definition.</returns>
        public IReadOnlyCollection<IDefinition> GetVariations(IDefinition def)
        {
            if (definitionVariations.TryGetValue(def, out var id))
            {
                ref var list = ref CollectionsMarshal.GetValueRefOrAddDefault(readonlyDefinitions, id, out var exists);

                if (exists)
                    return list!;

                return list = definitions.TryGetValue(id, out var mutableList)
                    ? new ReadOnlyCollection<IDefinition>(mutableList)
                    : [];
            }
            return [];
        }

        /// <summary>
        /// Gets the unique keys associated with a specific definition.
        /// </summary>
        /// <param name="def">The definition to get unique keys for.</param>
        /// <returns>A read-only collection of unique keys associated with the specified definition.</returns>
        public IReadOnlyCollection<string> GetUniqueKeys(IDefinition def)
        {
            if (typesPerDefinition.TryGetValue(def, out var types))
            {
                return (IReadOnlyCollection<string>)(types) ?? [];
            }
            return [];
        }

        /// <summary>
        /// Registers a new definition.
        /// </summary>
        /// <param name="value">The definition registration to add.</param>
        public void Register(DefinitionRegistration value)
        {
            definitionVariations[value.Definition] = value.Id;
            definitionIds[value.Definition] = value.Id;
            flattenedDefinitions[value.Id] = value.Definition;
        }

        /// <summary>
        /// Unregisters an existing definition.
        /// </summary>
        /// <param name="value">The definition registration to remove.</param>
        public void Unregister(DefinitionRegistration value)
        {
            definitionVariations.Remove(value.Definition);
            definitionIds.Remove(value.Definition);
            flattenedDefinitions.Remove(value.Id);
        }

        /// <summary>
        /// Gets a list of all definition registrations.
        /// </summary>
        /// <returns>A list of all definition registrations.</returns>
        IReadOnlyCollection<DefinitionRegistration> IExtensionRegistrar<DefinitionRegistration>.Get()
        {
            throw new NotImplementedException();
        }
    }
}
