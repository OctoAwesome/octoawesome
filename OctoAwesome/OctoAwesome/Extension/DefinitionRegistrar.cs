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
    public record struct TypeDefinitionRegistration(string TypeName, Type Definition);
    public record struct DefinitionInstanceRegistration(string Id, JsonNode Element, string[] Types);

    public record struct DefinitionRegistration(string Id, IDefinition Definition);

    //TODO Do we want to move the registrars in another library away from core?
    public class DefinitionRegistrar : IExtensionRegistrar<TypeDefinitionRegistration>, IExtensionRegistrar<DefinitionInstanceRegistration>, IExtensionRegistrar<DefinitionRegistration>
    {
        /// <inheritdoc/>
        public string ChannelName => ChannelNames.Definitions;

        /// <summary>
        /// Get a list of all definitions with their associated unique key
        /// </summary>
        public IReadOnlyDictionary<string, IDefinition> FlattenedDefinitions => flattenedDefinitions;
        /// <summary>
        /// Get a list of all definition ids with their associated unique definition implementation
        /// </summary>
        public IReadOnlyDictionary<IDefinition, string> FlattenedDefinitionIds => definitionIds;

        private Dictionary<string, IReadOnlyCollection<IDefinition>> readonlyDefinitions = [];
        private Dictionary<string, List<IDefinition>> definitions = [];
        private Dictionary<string, IDefinition> flattenedDefinitions = [];
        private Dictionary<IDefinition, string> definitionIds = [];
        private Dictionary<IDefinition, string> definitionVariations = [];
        private Dictionary<string, Type> definitionTypes = [];
        private Dictionary<IDefinition, ICollection<string>> typesPerDefinition = new ();

        public void Register(TypeDefinitionRegistration value)
        {
            if (!value.Definition.IsAssignableTo(typeof(IDefinition)))
                throw new ArgumentException(nameof(value.Definition));

            definitionTypes[value.TypeName] = value.Definition;
        }

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

        public void Unregister(TypeDefinitionRegistration value)
        {
            definitionTypes.Remove(value.TypeName);
        }

        public void Unregister(DefinitionInstanceRegistration value)
        {
            throw new NotSupportedException();
        }

        IReadOnlyCollection<DefinitionInstanceRegistration> IExtensionRegistrar<DefinitionInstanceRegistration>.Get()
        {
            throw new NotSupportedException();
        }
        public IReadOnlyCollection<TypeDefinitionRegistration> Get()
        {
            throw new NotSupportedException();
        }

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
        public IReadOnlyCollection<string> GetUniqueKeys(IDefinition def)
        {
            if (typesPerDefinition.TryGetValue(def, out var types))
            {
                return (IReadOnlyCollection<string>)(types) ?? [];
            }
            return [];
        }

        public void Register(DefinitionRegistration value)
        {
            definitionVariations[value.Definition] = value.Id;
            definitionIds[value.Definition] = value.Id;
            flattenedDefinitions[value.Id] = value.Definition;
        }

        public void Unregister(DefinitionRegistration value)
        {
            definitionVariations.Remove(value.Definition);
            definitionIds.Remove(value.Definition);
            flattenedDefinitions.Remove(value.Id);
        }

        IReadOnlyCollection<DefinitionRegistration> IExtensionRegistrar<DefinitionRegistration>.Get()
        {
            throw new NotImplementedException();
        }
    }
}
