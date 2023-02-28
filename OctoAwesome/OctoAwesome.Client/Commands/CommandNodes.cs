using System;
using System.Linq;
using Sprache;

namespace OctoAwesome.Client;

internal class Node : IPositionAware<Node>
{
    public int Length { get; protected set; }
    public Position? StartPos { get; protected set; }
    public Node SetPos(Position startPos, int length)
    {
        Length = length;
        StartPos = startPos;
        return this;
    }

    public virtual TRes Accept<TRes>(INodeVisitor<TRes> visitor)
    {
        return visitor.Visit(this);
    }
}
internal abstract class Node<T> : Node, IPositionAware<T>
    where T : Node
{
    public new T SetPos(Position startPos, int length)
        => (T)base.SetPos(startPos, length);
}

internal class TupleNode : Node<TupleNode>
{
    public Node[] Elements { get; }

    public TupleNode(Node[] elements)
    {
        Elements = elements;
    }

    public override string ToString()
    {
        return $"({string.Join(", ", Elements.Select(x => x.ToString()))})";
    }
    public override TRes Accept<TRes>(INodeVisitor<TRes> visitor)
    {
        return visitor.Visit(this);
    }
}

internal class TupleTypeElement : Node
{
    public TupleTypeElement(Node type, string? name)
    {
        Type = type;
        Name = name;
    }

    public Node Type { get; }
    public string? Name { get; }
    
    public override string ToString()
    {
        return $"{Type}{(string.IsNullOrEmpty(Name) ? "" : $" {Name}")}";
    }
    public override TRes Accept<TRes>(INodeVisitor<TRes> visitor)
    {
        return visitor.Visit(this);
    }
}

internal interface IReferenceNode
{
    Node? Parent { get; }
    string Identifier { get; }
}

internal class MethodNode : Node<MethodNode>, IReferenceNode
{
    public MethodNode(Node? parent, string identifier, TupleNode parameters)
    {
        Parent = parent;
        Identifier = identifier;
        Parameters = parameters;
    }

    public Node? Parent { get; }
    public string Identifier { get; }
    public TupleNode Parameters { get; }

    public override string ToString()
    {
        return $"call {Parent}{(Parent is null ? "" : ".")}{Identifier} with parameters {Parameters}";
    }
    public override TRes Accept<TRes>(INodeVisitor<TRes> visitor)
    {
        return visitor.Visit(this);
    }
}
internal class ReferenceNode : Node<ReferenceNode>, IReferenceNode
{
    public ReferenceNode(Node? parent, string identifier)
    {
        Parent = parent;
        Identifier = identifier;
    }

    public Node? Parent { get; }
    public string Identifier { get; }

    public override string ToString()
    {
        return $"Ref<{Parent}{(Parent is null ? "" : ".")}{Identifier}>";
    }
    public override TRes Accept<TRes>(INodeVisitor<TRes> visitor)
    {
        return visitor.Visit(this);
    }
}

internal class ErrorNode : Node<ErrorNode>, IReferenceNode
{
    public Node? Parent { get; }
    public string Identifier { get; }
    public ErrorNode(Node? parent, string identifier)
    {
        Parent = parent;
        Identifier = identifier;
    }
    public override TRes Accept<TRes>(INodeVisitor<TRes> visitor)
    {
        return visitor.Visit(this);
    }
}
internal class Primitive : Node<Primitive>
{
    public enum PrimitiveType
    {
        Float,
        Double,
        Int,
        UInt,
        String
    }

    public PrimitiveType Type { get; }
    
    public uint UInt { get; }
    public int Int { get; }
    public float Float { get; }
    public double Double { get; }
    public string String { get; }

    private Primitive(PrimitiveType type)
    {
        Type = type;
    }

    public Primitive(uint value)
        : this(PrimitiveType.UInt)
    {
        UInt = value;
    }

    public Primitive(int value)
        : this(PrimitiveType.Int)
    {
        Int = value;
    }
    

    public Primitive(float value)
        : this(PrimitiveType.Float)
    {
        Float = value;
    }

    public Primitive(double value)
        : this(PrimitiveType.Double)
    {
        Double = value;
    }

    public Primitive(string value)
        : this(PrimitiveType.String)
    {
        String = value;
    }

    public object GetValue()
    {
        return Type switch
        {
            PrimitiveType.Float => Float,
            PrimitiveType.Double => Double,
            PrimitiveType.Int => Int,
            PrimitiveType.UInt => UInt,
            PrimitiveType.String => String,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    public Type ToSystemType()
    {
        return Type switch
        {
            PrimitiveType.Float => typeof(float),
            PrimitiveType.Double => typeof(double),
            PrimitiveType.Int => typeof(int),
            PrimitiveType.UInt => typeof(uint),
            PrimitiveType.String => typeof(string),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public override string? ToString()
    {
        return GetValue().ToString();
    }
    public override TRes Accept<TRes>(INodeVisitor<TRes> visitor)
    {
        return visitor.Visit(this);
    }
}