using OctoAwesome.Definitions;
using OctoAwesome.Extension;

using Json.Path;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Nodes;
using OctoAwesome.Definitions.Items;
using Microsoft.Win32;
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
        private readonly Dictionary<string, List<IDefinition>> aliasDict = new();
        private DefinitionRegistrar registrar;

        /*
         3. Jsonisierung
         4. Übersetzungs keys in Definitions überarbeiten (alte lösung, evtl. weblate, community for the win)
         5. Profit?
         */

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinitionManager"/> class.
        /// </summary>
        /// <param name="extensionService">The extension servuce to get definitions from extensions with.</param>
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

            var definitions = new List<IDefinition>();

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

            definitions.AddRange(registrar.GetAll<IDefinition>()); //Replace IDefinition with BaseBlockDefinition, BaseItemDefinition etc. in multiple calls


            Definitions = definitions.ToArray();

            // collect items
            ItemDefinitions = Definitions.OfType<IItemDefinition>().ToArray();

            // collect blocks
            BlockDefinitions = Definitions.OfType<IBlockDefinition>().ToArray();

            // collect materials
            MaterialDefinitions = Definitions.OfType<IMaterialDefinition>().ToArray();

            // collect foods
            FoodDefinitions = Definitions.OfType<IFoodMaterialDefinition>().ToArray();
        }
        /// <inheritdoc />
        public IBlockDefinition? GetBlockDefinitionByIndex(ushort index)
        {
            if (index == 0)
                return null;

            return (IBlockDefinition)Definitions[(index & Blocks.TypeMask) - 1];
        }

        /// <inheritdoc />
        public ushort GetDefinitionIndex(IDefinition definition)
        {
            return (ushort)(Array.IndexOf(Definitions, definition) + 1);
        }

        /// <inheritdoc />
        public ushort GetDefinitionIndex<T>() where T : IDefinition
        {
            int i = 0;
            IDefinition? definition = default;
            foreach (var d in Definitions)
            {
                if (i > 0 && d.GetType() == typeof(T))
                {
                    throw new InvalidOperationException("Multiple Object where found that match the condition");
                }

                if (i == 0 && d.GetType() == typeof(T))
                {
                    definition = d;
                    ++i;
                }
            }
            return definition == null ? (ushort)0 : GetDefinitionIndex(definition);
        }

        /// <inheritdoc />
        public IEnumerable<T> GetDefinitions<T>() where T : class, IDefinition
        {
            // TODO: Caching (Generalized IDefinition-Interface for Dictionary (+1 from Maxi on 07.04.2021))
            return Definitions.OfType<T>();
        }

        /// <inheritdoc />
        public T? GetDefinitionByTypeName<T>(string typeName) where T : IDefinition
        {
            var searchedType = typeof(T);
            if (typeof(IBlockDefinition).IsAssignableFrom(searchedType))
            {
                return GetDefinitionFromArrayByTypeName<T>(typeName, BlockDefinitions);
            }

            if (typeof(IItemDefinition).IsAssignableFrom(searchedType))
            {
                return GetDefinitionFromArrayByTypeName<T>(typeName, ItemDefinitions);
            }

            if (typeof(IMaterialDefinition).IsAssignableFrom(searchedType))
            {
                return GetDefinitionFromArrayByTypeName<T>(typeName, MaterialDefinitions);
            }

            return default;
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

        private static T? GetDefinitionFromArrayByTypeName<T>(string typeName, IDefinition[] array)
            where T : IDefinition
        {
            foreach (var definition in array)
            {
                if (string.Equals(definition.GetType().FullName, typeName))
                    return (T)definition;
            }

            return default;
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
