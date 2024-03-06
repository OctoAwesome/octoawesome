
using Castle.Components.DictionaryAdapter;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Json.Path;
using Json.More;
using OctoAwesome.Caching;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace OctoAwesome.PoC;



public static class Program
{


    class BaseJsonType
    {
        [JsonExtensionData]
        public JsonObject JObject { get; set; }

        public IDictionary<string, JsonNode?> Dict => GenericCaster<JsonObject, IDictionary<string, JsonNode?>>.Cast(JObject);

    }



    public static void Main()
    {
        //TODOS
        //1. Merge Jsons from multiple  files into this object
        //2. Schauen wie die Komfortfunktionen für Paths und Refs aussehen könnten (Json Options müssen gesetzt sein!)
        //3. ???

        StringBuilder sb = new();
        sb.Append("{");
        var curDir = Directory.GetCurrentDirectory();
        foreach (var path in Directory.GetFiles(curDir, "Definitions/*.json", SearchOption.AllDirectories))
        {
            sb.Append(
                $$"""
                "{{Path.GetRelativePath(curDir, path)[12..].Replace('\\', '/')}}" : {{File.ReadAllText(path)}},
                """);
        }
        sb.Remove(sb.Length - 1, 1);
        sb.Append("}");
        var allCombined = sb.ToString();
        TestJson(allCombined);

        Console.ReadLine();
    }

    public static void MyAwesomeModFunc(ushort blockIndex)
    {
        var pocManager = new PocManager();
        var definition = pocManager.GetDefinition(blockIndex);

        if (!definition.TryGetProperty("fluid", out JsonNode fluidObj))
            return;
        var visc = fluidObj["viscosity"];



    }

    public class PocManager
    {

        public dynamic GetDefinition(int index) { return ""; }
        public dynamic GetDefinitionByTypeName(string typeName) { return ""; }
    }

    private static void TestJson([StringSyntax("JSON")] string json)
    {

        var deserialized =JsonSerializer.Deserialize<JsonObject>(json);
        JsonSerializerOptions options = new()
        {
            ReferenceHandler = new JsonRefHandler(deserialized),
            WriteIndented = true

        };

        options.Converters.Add(new TestABC());

        //var node = deserialized[1];
        //JsonObject obj = default;
        //var objDIct = (IDictionary<string, JsonNode?>)obj;


        var path = JsonPath.Parse("$['TestB.json'].base_woodwood");
        var res = path.Evaluate(deserialized);
        var res2 = res.Matches[0];
        var ro = res2.Value.Deserialize<Rootobject>(options);
        //JsonElement e;
        //e.TryGetProperty;

        //deserialized[0].Dict.

        //path.Evaluate(deserialized);

        //var js = deserialized.ToString();
        //var js2 = deserialized.ToJsonString();

        //var asd = JsonSerializer.Deserialize<JsonElement>("true").

        ;
    }


    public class Rootobject
    {
        public bool test { get; set; }
        public Material Material { get; set; }
        public Fluid fluid { get; set; }
        public Etherbehaviour etherBehaviour { get; set; }
    }

    public class Material
    {
    }

    public class Fluid
    {
        public int viscosity { get; set; }
    }

    public class Etherbehaviour
    {
        public Fluid1 fluid { get; set; }
    }

    public class Fluid1
    {
        public int viscosity { get; set; }
    }


    public class TestABC : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            
            return false;
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {

            return new ExampleConverter();
        }


        private class ExampleConverter : JsonConverter<object>
        {
            public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return JsonDocument.ParseValue(ref reader);
            }

            public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }

    }

    public class JsonRefHandler : ReferenceHandler
    {
        private readonly JsonObject completeRef;
        public JsonRefHandler(JsonObject completeRef)
        {
            this.completeRef = completeRef;
        }

        public override ReferenceResolver CreateResolver()
        {
            return new JsonPathResolver(completeRef);
        }
    }

    public class JsonPathResolver : ReferenceResolver
    {
        private static Dictionary<string, object> resolvedRefs = new();
        private readonly JsonObject completeRef;
        public JsonPathResolver(JsonObject completeRef)
        {
            this.completeRef = completeRef;
        }


        public override object ResolveReference(string referenceId)
        {

            //var path = JsonPath.Parse(referenceId);
            //var res = path.Evaluate(completeRef);
            //var res2 = res.Matches[0].Value.Deserialize(typeForDeserialize);
            //return resolved = res2;
            return "";

        }

        public override string GetReference(object? value, out bool alreadyExists)
        {
            //if (!(alreadyExists = _people.ContainsKey(person.Id)))
            //{
            //    _people[person.Id] = person;
            //}

            //return person.Id.ToString()!;
            alreadyExists = false;
            return "";
        }

        public override void AddReference(string reference, object value)
        {
            //person.Id = id;
            //_people[reference] = value;

        }

        //public override T ResolveReference<T>(string referenceId)
        //{
        //    ref var resolved = ref CollectionsMarshal.GetValueRefOrAddDefault(resolvedRefs, referenceId, out var exists);
        //    if (exists)
        //        return (T)resolved;

        //    var st = new StackTrace();
        //    var caller = st.GetFrame(2);
        //    var method = caller.GetMethod();
        //    var method2 = MethodBase.GetMethodFromHandle(method.MethodHandle);
        //    var cur = MethodBase.GetCurrentMethod();



        //    var typeForDeserialize = caller.GetMethod().GetGenericArguments()[0];
        //    return (T)(object)null;
        //}
    }
}
