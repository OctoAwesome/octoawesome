namespace OctoAwesome.Graph;

//public abstract class SourceNode<T> : Node<T>
//{

//}

public interface ISourceNode<T>
{
    int Priority { get; }
    Index3 Position { get; }

    SourceInfo<T> GetCapacity();

    void Use(SourceInfo<T> targetInfo, IChunkColumn? column);
}
