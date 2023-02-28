using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OctoAwesome.Client;

interface IReflectionCache
{
    string Name { get; }
}

class TypeCache
{
    private static Dictionary<Type, TypeCache> cachedTypes = new();
    public static TypeCache ResolveType(Type type)
    {
        if (cachedTypes.TryGetValue(type, out var resolved))
            return resolved;
        resolved = new TypeCache(type);
        cachedTypes.Add(type, resolved);
        return resolved;
    }
    private Dictionary<string, IReflectionCache> lookup = new();
    public Type Type { get; }
    public MethodCache[] Methods { get; }
    public FieldCache[] Fields { get; }
    public PropertyCache[] Properties { get; }

    public TypeCache(Type type)
    {
        Type = type;

        var lst = new List<MethodCache>();
        foreach (var g in Type.GetMethods(BindingFlags.Public | BindingFlags.Instance).GroupBy(x => x.Name))
        {
            var mc = new MethodCache(g.Key, g.ToArray(), this);
            lookup.Add(g.Key, mc);
            lst.Add(mc);
        }
        Methods = lst.ToArray();

        var fieldInfos = Type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        Fields = new FieldCache[fieldInfos.Length];
        for (int i = 0; i < Fields.Length; i++)
        {
            var fc = new FieldCache(fieldInfos[i], this);
            lookup.Add(fc.Name, fc);
            Fields[i] = fc;
        }
        
        var propInfos = Type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        Properties = new PropertyCache[propInfos.Length];
        for (int i = 0; i < Properties.Length; i++)
        {
            var pc = new PropertyCache(propInfos[i], this);
            lookup.Add(pc.Name, pc);
            Properties[i] = pc;
        }
    }
    public IReflectionCache? Resolve(string name)
    {
        return lookup.TryGetValue(name, out var resolved) ? resolved : null;
    }
}
abstract class ReflectionCache : IReflectionCache
{
    public TypeCache Type { get; }
    public TypeCache? ParentType { get; }

    public abstract object? GetValue(object value);

    public ReflectionCache(TypeCache type, TypeCache? parentType)
    {
        Type = type;
        ParentType = parentType;
    }

    public abstract string Name { get; }

    public IReflectionCache? Resolve(string name)
    {
        return Type.Resolve(name);
    }
}

class ValueCache : ReflectionCache
{
    public ValueCache(Type type) : base(TypeCache.ResolveType(type), null)
    {
    }

    public override object? GetValue(object value)
    {
        throw new NotSupportedException();
    }

    public override string Name => "[VALUE]";
}
class FieldCache : ReflectionCache
{
    public FieldInfo FieldInfo { get; }

    public FieldCache(FieldInfo fieldInfo, TypeCache parent) : base(TypeCache.ResolveType(fieldInfo.FieldType), parent)
    {
        FieldInfo = fieldInfo;
    }

    public override string Name => FieldInfo.Name;


    public override object? GetValue(object value)
    {
        return FieldInfo.GetValue(value);
    }
}
class PropertyCache : ReflectionCache
{
    public PropertyInfo PropertyInfo { get; }

    public PropertyCache(PropertyInfo propertyInfo, TypeCache parent) : base(TypeCache.ResolveType(propertyInfo.PropertyType), parent)
    {
        PropertyInfo = propertyInfo;
    }

    public override string Name => PropertyInfo.Name;

    public override object? GetValue(object value)
    {
        return PropertyInfo.GetValue(value);
    }
}
class MethodCache : IReflectionCache
{
    public TypeCache Parent { get; }
    public string Name { get; }
    public MethodInfo[] Methods { get; }

    public MethodCache(string name, MethodInfo[] methods, TypeCache parent)
    {
        Parent = parent;
        Name = name;
        Methods = methods;
    }

    public struct TypeMatcher
    {
        public TypeMatcher(object? value, bool hasType)
        {
            Value = value;
            HasType = hasType;
        }

        public bool HasType { get; }
        public Type? Type => HasType ? Value?.GetType() : null;
        public object? Value { get; }
    }
    public IEnumerable<MethodInfo> Filter(TypeMatcher[] parameters)
    {
        foreach (var m in Methods)
        {
            var cacheParameters = m.GetParameters();
            if (cacheParameters.Length < parameters.Length)
                continue;
            bool isMatch = true;
            for (int i = 0; i < Math.Min(parameters.Length, cacheParameters.Length); i++)
            {
                var paramTypeMatcher = parameters[i];
                var destType = cacheParameters[i].ParameterType;
                if (!paramTypeMatcher.HasType)
                    continue;
                var paramValue = paramTypeMatcher.Type;

                if (paramValue is null)
                {
                    isMatch = !destType.IsValueType ||
                              destType.IsValueType && destType.IsGenericType && typeof(Nullable<>).IsAssignableFrom(destType);
                }
                else if(!paramValue.IsAssignableTo(destType))
                {
                    isMatch = false;
                }

                if (!isMatch)
                    break;
            }

            if (isMatch)
            {
                yield return m;
            }
        }
    }
    public bool TryGetValue(object parent, TypeMatcher[] parameters, out Type? resultType, out object? result)
    {
        foreach (var m in Filter(parameters))
        {
            if (m.GetParameters().Length != parameters.Length)
                continue;
            resultType = m.ReturnType;
            result = m.Invoke(parent, parameters.Select(x => x.Value).ToArray());
            return true;
        }

        resultType = null;
        result = null;
        return false;
    }
    
}