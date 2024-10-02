using OctoAwesome.Definitions;
using OctoAwesome.Extension;
using OctoAwesome.Definitions.Items;

using Json.Path;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace OctoAwesome.Runtime
{
    public class ItemDefinition : IItemDefinition
    {
        public string DisplayName { get; }
        public string Icon { get; }

        public ItemDefinition(string displayName, string icon)
        {
            DisplayName = displayName;
            Icon = icon;
        }

        public bool CanMineMaterial(IMaterialDefinition material)
        {
            return true;
        }

        public Item? Create(IMaterialDefinition material)
        {
            return null;
        }
    }

    /// <summary>
    /// Definition Manager which loads extensions.
    /// </summary>
    public class DefinitionManager : IDefinitionManager
    {
        private const string refStr = "\"@ref\"";

        private readonly ExtensionService extensionService;
        private DefinitionRegistrar registrar;

        public event EventHandler DefinitionsChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinitionManager"/> class.
        /// </summary>
        /// <param name="extensionService">The extension service to get definitions from extensions with.</param>
        public DefinitionManager(ExtensionService extensionService)
        {
            this.extensionService = extensionService;
        }

        /// <inheritdoc />
        public IDefinition[] Definitions { get; private set; }

        /// <inheritdoc />
        public IItemDefinition[] ItemDefinitions { get; private set; }

        /// <inheritdoc />
        public IBlockDefinition[] BlockDefinitions { get; private set; }

        /// <inheritdoc />
        public IMaterialDefinition[] MaterialDefinitions { get; private set; }
        /// <inheritdoc />
        public IFoodMaterialDefinition[] FoodDefinitions { get; private set; }

        /// <inheritdoc/>
        public void Initialize()
        {
            var json = GetDefinitionJson();

            var ro =
                JsonSerializer
                .Deserialize<Dictionary<string, Dictionary<string, JsonNode>>>(json)
                .SelectMany(x => x.Value)
                .ToDictionary(x => x.Key, x => x.Value);

            foreach (var item in extensionService.GetRegistrars(ChannelNames.Definitions))
            {
                if (item is DefinitionRegistrar registrar)
                {
                    this.registrar = registrar;
                    break;
                }
            }

            foreach (var item in ro)
            {
                if (item.Value is JsonObject o && o.ContainsKey("@types"))
                {
                    var jArr = o["@types"].Deserialize<string[]>();
                    RegisterDefinitionInstance(item.Key, o, jArr);
                }
            }
        }

        public void LoadSaveGame(IReadOnlyList<string>? definitionKeyIndices)
        {
            if (definitionKeyIndices is null) //New Game
            {
                Definitions = registrar.FlattenedDefinitions.Select(x => x.Value).ToArray();
            }
            else
            {
                Definitions = new IDefinition[definitionKeyIndices.Count];
                for (int i = 0; i < definitionKeyIndices.Count; i++)
                {
                    var key = definitionKeyIndices[i];

                    if (registrar.FlattenedDefinitions.TryGetValue(key, out var def))
                    {
                        Definitions[i] = def;
                    }
                    else
                    {
                        //TODO Old Type, Warn for maybe broken world when continue loading
                    }
                }
            }
            ItemDefinitions = Definitions.OfType<IItemDefinition>().ToArray();
            BlockDefinitions = Definitions.OfType<IBlockDefinition>().ToArray();
            MaterialDefinitions = Definitions.OfType<IMaterialDefinition>().ToArray();
            FoodDefinitions = Definitions.OfType<IFoodMaterialDefinition>().ToArray();
            DefinitionsChanged?.Invoke(this, EventArgs.Empty);
        }

        public IReadOnlyCollection<string> GetSaveGameData()
        {
            var keys = new string[Definitions.Length];
            for (int i = 0; i < Definitions.Length; i++)
            {
                IDefinition item = Definitions[i];
                keys[i] = registrar.FlattenedDefinitionIds[item];
            }
            return keys;
        }

        /// <inheritdoc />
        public IDefinition? GetDefinitionByIndex(ushort index)
        {
            if (index == 0)
                return null;

            return Definitions[index - 1];
        }
        /// <inheritdoc />
        public T? GetDefinitionByIndex<T>(ushort index) where T : IDefinition
        {
            if (index == 0)
                return default;
            var def = Definitions[index - 1];
            if (def is T t)
                return t;
            return default;
        }

        /// <inheritdoc />
        public ushort GetDefinitionIndex(IDefinition definition)
        {
            return (ushort)(Array.IndexOf(Definitions, definition) + 1);
        }

        /// <inheritdoc />
        public ushort GetDefinitionIndex<T>(string key) where T : IDefinition
        {
            var definition = registrar.Get<T>(key);
            if (definition is null)
                throw new ArgumentException(nameof(key));
            return GetDefinitionIndex(definition);

        }

        /// <inheritdoc />
        public IDefinition? GetDefinitionByUniqueKey(string uniqueKey)
        {
            if (registrar.FlattenedDefinitions.TryGetValue(uniqueKey, out var def))
            {
                return def;
            }

            return default;
        }
        /// <inheritdoc />
        public string? GetUniqueKeyByDefinition(IDefinition definition)
        {
            if (registrar.FlattenedDefinitionIds.TryGetValue(definition, out var key))
            {
                return key;
            }

            return default;
        }

        public T? GetDefinitionByUniqueKey<T>(string key)
        {
            return registrar.Get<T>(key);
        }

        public IReadOnlyCollection<IDefinition> GetVariations(IDefinition def)
            => registrar.GetVariations(def);

        public bool TryGetVariation<T>(IDefinition def, [MaybeNullWhen(false)] out T? variation)
        {
            var variants = GetVariations(def);
            foreach (var variant in variants)
            {
                if (variant is not T t)
                    continue;

                variation = t;
                return true;
            }

            variation = default;
            return false;
        }

        public bool TryGet<T>(string id, [MaybeNullWhen(false)] out T? definition) where T : IDefinition
        {
            definition = registrar.Get<T>(id);
            return definition is not null;
        }
        public void RegisterDefinitionInstance(string key, JsonObject o, string[] jArr)
        {
            registrar.Register(new DefinitionInstanceRegistration(key, o, jArr));
        }

        private JsonNode GetDefinitionJson()
        {

            StringBuilder sb = new();
            sb.Append("{");
            var curDir = Directory.GetCurrentDirectory();
            foreach (var path in Directory.GetFiles(curDir, "Definitions/*.json", SearchOption.AllDirectories))
            {
                if (path.Contains("Recipes"))
                    continue;
                sb.Append(
                    $$"""
                "{{Path.GetRelativePath(curDir, path)[12..].Replace('\\', '/')}}" : {{File.ReadAllText(path)}},
                """);
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            var allCombined = sb.ToString();
            var jo = JsonNode.Parse(allCombined);
            int currentIndex = 0;
            while (currentIndex < allCombined.Length)
            {
                var refIndex = allCombined.IndexOf(refStr, currentIndex);
                if (refIndex == -1)
                    break;
                int refFrom = 0;
                int refTo = 0;
                byte foundQuotations = 0;
                for (int i = refIndex + refStr.Length; i < allCombined.Length - 1; i++)
                {
                    if (allCombined[i] == '"' && allCombined[i - 1] != '\\')
                    {
                        foundQuotations++;
                        if (foundQuotations == 1)
                        {
                            refFrom = i;
                        }
                        else
                        {
                            refTo = i + 1;
                            break;
                        }
                    }
                }
                if (foundQuotations < 2)
                    break;
                refIndex = allCombined.LastIndexOf('{', refIndex);
                var removeTo = allCombined.IndexOf('}', refTo) + 1;
                var path = allCombined[(refFrom + 1)..(refTo - 1)];
                allCombined = allCombined.Remove(refIndex, removeTo - refIndex);

                var jPath = JsonPath.Parse(path);
                var res = jPath.Evaluate(jo);
                var match = res.Matches[0];

                var key = jPath.Segments.Last().Selectors.Last().ToString().Replace("'", "\"");
                allCombined = allCombined.Insert(refIndex, $"{{{key}:{match.Value.ToJsonString()}}}");
                currentIndex = refIndex;
            }
            return JsonNode.Parse(allCombined);
        }

    }
}
