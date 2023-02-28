namespace OctoAwesome.Client;

internal interface INodeVisitor<out TRes>
{
    TRes Visit(Node node);
    TRes Visit(MethodNode node);
    TRes Visit(ReferenceNode node);
    TRes Visit(Primitive node);
    TRes Visit(TupleTypeElement node);
    TRes Visit(ErrorNode node);
}