using System;
using System.Linq;

namespace OctoAwesome.Client;

struct ResolvedObject
{
    public ResolvedObject(IReflectionCache cache, object? value)
    {
        Cache = cache;
        Value = value;
        ResolveSuccess = true;
    }
    public bool ResolveSuccess { get; }
    public IReflectionCache? Cache { get; }
    public object? Value { get; }

    public ResolvedObject Resolve(string name)
    {
        if (Cache is not ReflectionCache rf)
            return default;
        var cache = rf.Resolve(name);
        if (cache is null)
            return default;
        if (cache is MethodCache mc)
            return new ResolvedObject(mc, Value);
        if (cache is not ReflectionCache rf2)
            return default;
        if (Value is null)
            throw new NullReferenceException();
        return new ResolvedObject(cache, rf2.GetValue(Value));
    }
}

internal class CommandVisitor : INodeVisitor<ResolvedObject>
{
    public CommandVisitor(object root)
        : this(new ResolvedObject(new ValueCache(root.GetType()), root))
    {
    }
    public CommandVisitor(ResolvedObject root)
    {
        Root = root;
    }

    public ResolvedObject Root { get; }
    public ResolvedObject Visit(Node node)
    {
        return node switch
        {
            MethodNode md => Visit(md),
            ReferenceNode rf => Visit(rf),
            Primitive p => Visit(p),
            _ => throw new NotSupportedException()
        };
    }

    private ResolvedObject Resolving(Node? parent, string identifier)
    {
        ResolvedObject resolved = Root;
        if (parent is not null)
        {
            resolved = Visit(parent);
        }
        if (!resolved.ResolveSuccess)
            throw new Exception();
        return resolved.Resolve(identifier);
    }

    public ResolvedObject Visit(MethodNode node)
    {
        var resolved = Resolving(node.Parent, node.Identifier);

        if (resolved.Cache is not MethodCache methodCache)
            return default;

        if (!resolved.ResolveSuccess)
            throw new Exception();
        var parameters = new MethodCache.TypeMatcher[node.Parameters.Elements.Length];
        for(int i=0;i<parameters.Length;i++)
        {
            var p = node.Parameters.Elements[i];
            var resolvedParam = p.Accept(this);
            if (resolvedParam.ResolveSuccess)
                parameters[i] = new MethodCache.TypeMatcher(resolvedParam.Value, true);
            else
                parameters[i] = new MethodCache.TypeMatcher(null, false);
        }

        if (!methodCache.TryGetValue(resolved.Value, parameters, out var resultType, out var result))
            throw new Exception();
        return new ResolvedObject(new ValueCache(resultType!), result);
    }

    public ResolvedObject Visit(ReferenceNode node)
    {
        var resolved = Resolving(node.Parent, node.Identifier);

        if (!resolved.ResolveSuccess)
            throw new Exception();
        return resolved;
    }

    public ResolvedObject Visit(Primitive node)
    {
        var value = node.GetValue();
        return new ResolvedObject(new ValueCache(node.ToSystemType()), value);
    }

    public ResolvedObject Visit(TupleTypeElement node)
    {
        throw new NotImplementedException();
    }

    public ResolvedObject Visit(ErrorNode node)
    {
        throw new NotImplementedException();
    }
}