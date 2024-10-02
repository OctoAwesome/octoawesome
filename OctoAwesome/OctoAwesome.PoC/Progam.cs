using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Json.Path;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SixLabors.ImageSharp.Formats.Gif;
using OctoAwesome.Definitions;
using NonSucking.Framework.Extension.IoC;
using OctoAwesome.Location;
namespace OctoAwesome.PoC;



class TestSer {


    [JsonInclude]
    private string myField;

    public void SetMyField(string abc)
    {
        myField = abc;
    }
}

class TestSerA : TestSer
{

}
public class PocManager
{
    private Dictionary<string, List<IDefinition>> definitions = [];
    private Dictionary<string, Type> definitionTypes = new();



    public dynamic GetDefinition(int index) { return ""; }
    public dynamic GetDefinitionByTypeName(string typeName) { return ""; }

    public void RegisterDefinitionType(string typeName, Type definition)
    {
        if (!definition.IsAssignableTo(typeof(IDefinition)))
            throw new ArgumentException(nameof(definition));

        definitionTypes[typeName] = definition;
    }

    public void RegisterDefinitionInstance(string id, JsonNode element, string[] types)
    {
        foreach (var type in types)
        {
            if (definitionTypes.TryGetValue(type, out var definition))
            {
                ref var entry = ref CollectionsMarshal.GetValueRefOrAddDefault(definitions, id, out var exists);
                if (!exists)
                    entry = new();

                entry.Add((IDefinition)JsonSerializer.Deserialize(element, definition));
            }
        }
    }

    public void RegisterOnInteract(string definitionType, Action<IMaterialDefinition, object> onInteract)
    {

    }

    public T GetDefinition<T>(string id)
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
}
public class TypesConverter<T> : JsonConverter<T> where T : IDefinition
{
    private PocManager definitionManager;

    public TypesConverter()
    {
        definitionManager = TypeContainer.Get<PocManager>();
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsAssignableFrom(typeof(T));
    }

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var elem = JsonSerializer.Deserialize<Dictionary<string, JsonNode>>(ref reader);

        var key = elem.Keys.First();

        var def = definitionManager.GetDefinition<T>(key);
        if (def is not null)
            return def;

        if (elem[key] is JsonObject o && o.ContainsKey("@types"))
        {

            var jArr = o["@types"].Deserialize<string[]>();
            definitionManager.RegisterDefinitionInstance(key, o, jArr);
        }

        return definitionManager.GetDefinition<T>(key);
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

public class TestMaterialDefinition : IMaterialDefinition
{
    public int Hardness { get; set; }
    public int Density { get; set; }
    public string DisplayName { get; set; }
    public string Icon { get; set; }
}

public class BaseBlockDefinition : BlockDefinition
{
    [JsonConverter(typeof(TypesConverter<TestMaterialDefinition>)), JsonInclude, JsonPropertyName("Material")]
    public override IMaterialDefinition Material { get => base.Material; init => base.Material = value; }

    //[JsonConverter(typeof(TypesConverter<TestMaterialDefinition>)), JsonInclude, JsonPropertyName("Material")]
    //private IMaterialDefinition material = default!;

}

public class ModMagic : BaseBlockDefinition
{
    [JsonConverter(typeof(TypesConverter<IMaterialDefinition>))]
    public int MagicMana { get; set; } //ModB

}
public static class Program
{
    const string refStr = "\"@ref\"";
    static List<Delegate> delegates = [];

    static Dictionary<string, Dictionary<IDefinition, List<Delegate>>> abc = [];

    public static void Action<T, T2, T3, T4>(string actionName, T param1, T2 param2, T3 param3, T4 param4)
    {
        foreach (var item in abc[actionName])
        {
            if (item is Action<T, T2, T3, T4> act)
                act.Invoke(param1, param2, param3, param4);
        }
    }

    public static TRet? Function<TRet, T, T2, T3, T4>(string actionName, T param1, T2 param2, T3 param3, T4 param4)
    {
        TRet? lastResult = default;
        foreach (var item in abc[actionName])
        {
            if (item is Func<TRet?, T, T2, T3, T4, TRet> act)
                lastResult = act.Invoke(lastResult, param1, param2, param3, param4);
        }
        return lastResult;
    }

    public static void Main()
    {
        var abc = new TestSer();
        abc.SetMyField("Test");
        var serialized = JsonSerializer.Serialize(abc);
        var deserialized = JsonSerializer.Deserialize<TestSer>(serialized);

        var func = ()=> new TestSerA();

        if(func is Func<TestSer> func2)
        {
            var abc123 = func2();
        }


        int planet = 0;
        int x = 0, y = 0, z = 0;
        string builder = "";
        Random random = new Random();
        //abc["PlantTree"] = new List<Delegate>() { new Action<int, int>((int a, int b) => { Console.WriteLine(a + b); }) };
        //abc["PlantTree"] = new List<Delegate>() { new Action<int, Index3, string, int>((a, ind, str, b) => { Console.WriteLine(a + b); }) };


        Action("PlantTree", planet, new Index3(x, y, z), builder, random.Next(int.MaxValue));

        var action = delegates[0];
        if (action is Action<int, int> a)
        {
            a(12, 23);
        }


        var definitionManager = new PocManager();
        TypeContainer.Register(definitionManager);
        definitionManager.RegisterDefinitionType("core.block", typeof(BaseBlockDefinition));
        definitionManager.RegisterDefinitionType("core.testMaterial", typeof(TestMaterialDefinition));
        //definitionManager.RegisterTypeDefinition("modA.block", typeof(ModABD));

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
        jo = JsonNode.Parse(allCombined);

        var ro =
            JsonSerializer
            .Deserialize<Dictionary<string, Dictionary<string, JsonNode>>>(allCombined)
            .SelectMany(x => x.Value)
            .ToDictionary(x => x.Key, x => x.Value);

        foreach (var item in ro)
        {
            if (item.Value is JsonObject o && o.ContainsKey("@types"))
            {
                var jArr = o["@types"].Deserialize<string[]>();
                definitionManager.RegisterDefinitionInstance(item.Key, item.Value, jArr);
            }
        }

        var bw = definitionManager.GetDefinition<BaseBlockDefinition>("base_woodwood");


        TestJson(allCombined);

        Console.ReadLine();
    }


    public class BaseDefinition
    {
        [JsonPropertyName("@types")]
        public string[] Types { get; set; }
    }



    private static void TestJson([StringSyntax("JSON")] string json)
    {
        //var settings = new JsonSerializerSettings
        //{
        //    ReferenceResolverProvider = () => new PathResolver()
        //};
        //var deserialized = JsonConvert.DeserializeObject<JObject>(json, settings);
        //JsonSerializerOptions options = new()
        //{
        //    ReferenceHandler = new JsonRefHandler(deserialized),
        //    WriteIndented = true

        //};

        //options.Converters.Add(new TestABC());

        //var node = deserialized[1];
        //JsonObject obj = default;
        //var objDIct = (IDictionary<string, JsonNode?>)obj;


        //var path = JsonPath.Parse("$['TestB.json'].base_woodwood");
        //var res = path.Evaluate(deserialized);
        //var res2 = res.Matches[0];
        //var jsoasdn = deserialized["TestB.json"]["base_woodwood"].ToString();
        //var ro = JsonConvert.DeserializeObject<Rootobject>(jsoasdn, settings);
        //JsonElement e;
        //e.TryGetProperty;

        //deserialized[0].Dict.

        //path.Evaluate(deserialized);

        //var js = deserialized.ToString();
        //var js2 = deserialized.ToJsonString();

        //var asd = JsonSerializer.Deserialize<JsonElement>("true").

        ;
    }




    //public class TestABC : JsonConverterFactory
    //{
    //    public override bool CanConvert(Type typeToConvert)
    //    {

    //        return false;
    //    }

    //    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    //    {

    //        return new ExampleConverter();
    //    }


    //    private class ExampleConverter : JsonConverter<object>
    //    {
    //        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    //        {
    //            return JsonDocument.ParseValue(ref reader);
    //        }

    //        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }

    //}

    //public class JsonRefHandler : ReferenceHandler
    //{
    //    private readonly JsonObject completeRef;
    //    public JsonRefHandler(JsonObject completeRef)
    //    {
    //        this.completeRef = completeRef;
    //    }

    //    public override ReferenceResolver CreateResolver()
    //    {
    //        return new JsonPathResolver(completeRef);
    //    }
    //}

    //public class JsonPathResolver : ReferenceResolver
    //{
    //    private static Dictionary<string, object> resolvedRefs = new();
    //    private readonly JsonObject completeRef;
    //    public JsonPathResolver(JsonObject completeRef)
    //    {
    //        this.completeRef = completeRef;
    //    }


    //    public override object ResolveReference(string referenceId)
    //    {
    //        var st = new StackTrace();
    //        var caller = st.GetFrame(2);
    //        var method = caller.GetMethod();
    //        var cur = MethodBase.GetCurrentMethod();
    //        Thread.Sleep(100000);
    //        //var path = JsonPath.Parse(referenceId);
    //        //var res = path.Evaluate(completeRef);
    //        //var res2 = res.Matches[0].Value.Deserialize(typeForDeserialize);
    //        //return resolved = res2;
    //        return "";

    //    }

    //    public override string GetReference(object? value, out bool alreadyExists)
    //    {
    //        //if (!(alreadyExists = _people.ContainsKey(person.Id)))
    //        //{
    //        //    _people[person.Id] = person;
    //        //}

    //        //return person.Id.ToString()!;
    //        alreadyExists = false;
    //        return "";
    //    }

    //    public override void AddReference(string reference, object value)
    //    {
    //        //person.Id = id;
    //        //_people[reference] = value;

    //    }

    //    //public override T ResolveReference<T>(string referenceId)
    //    //{
    //    //    Thread.Sleep(100000);
    //    //    ref var resolved = ref CollectionsMarshal.GetValueRefOrAddDefault(resolvedRefs, referenceId, out var exists);
    //    //    if (exists)
    //    //        return (T)resolved;

    //    //    var st = new StackTrace();
    //    //    var caller = st.GetFrame(2);
    //    //    var method = caller.GetMethod();
    //    //    var method2 = MethodBase.GetMethodFromHandle(method.MethodHandle);
    //    //    var cur = MethodBase.GetCurrentMethod();



    //    //    var typeForDeserialize = caller.GetMethod().GetGenericArguments()[0];
    //    //    return (T)(object)null;
    //    //}
    //}
}
