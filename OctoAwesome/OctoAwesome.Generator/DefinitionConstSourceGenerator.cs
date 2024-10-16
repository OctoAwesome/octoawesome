using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OctoAwesome.Generator
{
    [Generator(LanguageNames.CSharp)]
    public class DefinitionConstSourceGenerator : IIncrementalGenerator
    {
        private class JsonContent
        {
            [JsonExtensionData]
            public IDictionary<string, JsonElement> Definitions { get; set; }
        }
        private class OutContent
        {
            public IDictionary<string, JsonClass> Definitions { get; set; }
        }


        private class JsonClass
        {
            [JsonPropertyName("@types")]
            public ImmutableArray<string> Types { get; set; }
        }

        private class InBetweenOutput
        {
            public ImmutableArray< string > Types { get; set; }
            public Dictionary<string, ImmutableArray<string>> IdWithUniqueIds { get; set; }
        }

        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            

            string PascalizeName(string name)
            {
                StringBuilder builder = new StringBuilder();
                bool capatalizeNext = true;
                foreach (var item in name)
                {
                    if (char.IsLetter(item))
                    {
                        if (capatalizeNext)
                            builder.Append(char.ToUpper(item));
                        else
                            builder.Append(item);
                        capatalizeNext = false;
                    }
                    else
                    {
                        capatalizeNext = true;
                    }
                }
                return builder.ToString();
            }

            IncrementalValuesProvider<AdditionalText> jsonFiles = initContext.AdditionalTextsProvider.Where(static file => file.Path.EndsWith("Definitions.json"));

            // read their contents and save their name
            IncrementalValuesProvider<(string name, string content)> namesAndContents = jsonFiles.Select((text, cancellationToken) =>
            {
                try
                {

                    var ret = (name: Path.GetFileNameWithoutExtension(text.Path), content: text.GetText(cancellationToken)?.ToString() ?? "");
                    return ret;
                }
                catch (Exception)
                {

                    throw;
                }
            });

            var collected = namesAndContents
                .Select((input, _) =>
                {
                    try
                    {
                        var step1 = JsonSerializer.Deserialize<JsonContent>(input.content);
                        var ret = new OutContent { Definitions = step1.Definitions.ToDictionary(x=>x.Key, x=>x.Value.Deserialize<JsonClass>()) };
                        return ret;
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                })
                .Collect();

            var output = collected.Select((x, _) =>
                new InBetweenOutput()
                {
                    Types = x.SelectMany(x => x.Definitions.Values).SelectMany(x => x.Types).Distinct().ToImmutableArray(),
                    IdWithUniqueIds = x
                .SelectMany(x => x.Definitions)
                .SelectMany(x => x.Value.Types
                    .Select(t => (x.Key, $"{x.Key}@{t}"))
                    .GroupBy(x => x.Key)
                    )
                .ToDictionary(v => v.Key, v => v.Select(x => x.Item2)
                .ToImmutableArray())
                }
            );

            // generate a class that contains their values as const strings
            initContext.RegisterSourceOutput(output, (spc, output) =>
            {
                StringBuilder builder = new StringBuilder("public static partial class ConstStrings\r\n{\r\n");

                void AppendLine(string name)
                {

                    builder.AppendLine($"\tpublic const string {PascalizeName(name)} = \"{name}\";");
                }

                foreach (var type in output.Types)
                {
                    AppendLine(type);
                }
                foreach (var type in output.IdWithUniqueIds)
                {
                    AppendLine(type.Key);
                    foreach (var item in type.Value)
                    {
                        AppendLine(item);
                    }
                }
                builder.AppendLine("}");

                spc.AddSource($"DefinitionsConstStrings.g.cs", builder.ToString());
            });
        }
    }
}
