using OctoAwesome.Caching;
using OctoAwesome.Definitions;
using OctoAwesome.Extension;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace OctoAwesome.Extension
{
    public record struct TypeDefinitionRegistration(string TypeName, Type Definition);
    public record struct DefinitionInstanceRegistration(string Id, JsonNode Element, string[] Types);

    //TODO Do we want to move the registrars in another library away from core?
    public class DefinitionRegistrar : IExtensionRegistrar<TypeDefinitionRegistration>, IExtensionRegistrar<DefinitionInstanceRegistration>
    {
        /// <inheritdoc/>
        public string ChannelName => ChannelNames.Definitions;

        private Dictionary<string, List<IDefinition>> definitions = [];
        private Dictionary<IDefinition, string> definitionIds = [];
        private Dictionary<string, Type> definitionTypes = [];

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
                    definitionIds[def] = value.Id;
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
            return definitionIds.TryGetValue(def, out var id)
                && definitions.TryGetValue(id, out var list)
                ? list
                : Array.Empty<IDefinition>();
        }
    }
}
