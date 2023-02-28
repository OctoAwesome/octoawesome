using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OctoAwesome.Client;

struct Suggestions
{
    public Suggestions(IReflectionCache cache, string? name = null)
        : this(new ResolvedObject(cache, null), name)
    {
    }

    public Suggestions(IEnumerable<string> suggestions)
    {
        PossibleCompletions.AddRange(suggestions);

    }
    public Suggestions(ResolvedObject resolvedObject, string? name)
    {
        if (name is not null)
        {
            TypeCache? type = null;
            if (resolvedObject.Cache is FieldCache fc)
                type = fc.Type;
            else if (resolvedObject.Cache is PropertyCache pc)
                type = pc.Type;
            else if (resolvedObject.Cache is ValueCache vc)
                type = vc.Type;
            if (type != null)
            {
                PossibleCompletions.AddRange(type.Methods.Where(x => x.Name.StartsWith(name)).SelectMany(cache => cache.Methods).Select(GetSignature));
                PossibleCompletions.AddRange(type.Fields.Where(x => x.Name.StartsWith(name)).Select(x => $"{x.Name}"));
                PossibleCompletions.AddRange(type.Properties.Where(x => x.Name.StartsWith(name)).Select(x => $"{x.Name}"));
            }
        }
        else
        {
            ResolvedObject = resolvedObject;
        }
    }

    public List<string>? PossibleCompletions { get; } = new();
    public ResolvedObject ResolvedObject { get; }

    public bool ResolveSuccess => ResolvedObject.ResolveSuccess;

    public Suggestions Resolve(string name, bool fullResolve)
    {
        var resolved = fullResolve ? ResolvedObject.Resolve(name) : default;
        return new Suggestions(resolved.ResolveSuccess ? resolved : ResolvedObject, resolved.ResolveSuccess ? null : name);
    }

    public static string GetSignature(MethodInfo m)
    {
        return $"{m.ReturnType} {m.Name}({string.Join(", ",
            m.GetParameters()
                .Select(y => $"{y.ParameterType} {y.Name}"))})";
    }
}

internal class AutoCompleteVisitor : INodeVisitor<Suggestions>
{
    public AutoCompleteVisitor(object root)
        : this(new Suggestions(new ValueCache(root.GetType())))
    {
    }
    public AutoCompleteVisitor(Suggestions root)
    {
        Root = root;
    }

    public Suggestions Root { get; }
    public Suggestions Visit(Node node)
    {
        return node switch
        {
            ErrorNode e => Visit(e),
            MethodNode md => Visit(md),
            ReferenceNode rf => Visit(rf),
            Primitive p => Visit(p),
            _ => throw new NotSupportedException()
        };
    }
    private Suggestions Resolving(Node? parent, string identifier, bool fullResolve)
    {
        Suggestions resolved = Root;
        if (parent is not null)
        {
            resolved = Visit(parent);
        }
        if (!resolved.ResolveSuccess)
            throw new Exception();
        return resolved.Resolve(identifier, fullResolve);
    }
    public Suggestions Visit(MethodNode node)
    {
        Suggestions resolved = Resolving(node.Parent, node.Identifier, true);

        if (resolved.ResolvedObject.Cache is not MethodCache methodCache)
            return default;

        if (!resolved.ResolveSuccess)
            throw new Exception();
        var parameters = new List<MethodCache.TypeMatcher>(node.Parameters.Elements.Length);
        bool hasClosingBracket = true;
        foreach (var p in node.Parameters.Elements)
        {
            if (p is ErrorNode errorNode)
            {
                if (errorNode.Identifier == ")")
                {
                    hasClosingBracket = false;
                    continue;
                }
                else
                {
                    parameters.Add(new MethodCache.TypeMatcher(null, false));
                }
            }
            else
            {
                var resolvedParam = p.Accept(this);
                parameters.Add(resolvedParam.ResolveSuccess
                    ? new MethodCache.TypeMatcher(resolvedParam.ResolvedObject.Value, true)
                    : new MethodCache.TypeMatcher(null, false));
            }

        }

        int matchCount = 0;

        MethodInfo? match = null;

        var matchingMethods = methodCache.Filter(parameters.ToArray()).ToArray();
        foreach (var m in matchingMethods)
        {
            match = m;
            matchCount++;
        }

        if (match is null)
        {
            if (hasClosingBracket)
                throw new Exception();
        }
        else
        {
            if (!hasClosingBracket || matchCount > 1)
            {
                return new Suggestions(matchingMethods.Select(Suggestions.GetSignature));
            }

            return new Suggestions(new ValueCache(match.ReturnType));
        }
        if (!hasClosingBracket)
            return new Suggestions(methodCache, node.Identifier);
        
        throw new Exception();
    }

    public Suggestions Visit(ReferenceNode node)
    {
        return Resolving(node.Parent, node.Identifier, false);
    }

    public Suggestions Visit(Primitive node)
    {
        return new Suggestions(new ValueCache(node.ToSystemType()));
    }

    public Suggestions Visit(TupleTypeElement node)
    {
        throw new NotImplementedException();
    }

    public Suggestions Visit(ErrorNode node)
    {
        return Visit(new ReferenceNode(node.Parent, ""));
    }
}