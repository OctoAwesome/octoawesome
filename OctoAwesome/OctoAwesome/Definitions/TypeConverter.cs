using NLog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace OctoAwesome.Definitions;

public class TypesConverter<T> : JsonConverter<T> where T : IDefinition
{
    private static readonly IDefinitionManager definitionManager = TypeContainer.Get<IDefinitionManager>();

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsAssignableFrom(typeof(T));
    }

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var elem = JsonSerializer.Deserialize<Dictionary<string, JsonNode>>(ref reader);

        var key = elem.Keys.First();

        var exists = definitionManager.TryGet<T>(key, out var def);
        if (exists)
            return def;

        if (elem[key] is JsonObject o && o.ContainsKey("@types"))
        {
            var jArr = o["@types"].Deserialize<string[]>();
            definitionManager.RegisterDefinitionInstance(key, o, jArr!);
        }

        definitionManager.TryGet(key, out def);
        return def;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}
