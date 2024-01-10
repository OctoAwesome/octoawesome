namespace OctoAwesome.Graphs;

//public abstract class SourceNode<T> : Node<T>
//{

//}

public interface ISourceNode<T>
{
    int Priority { get; }
    Index3 Position { get; }

    SourceInfo<T> GetCapacity(Simulation simulation);

    void Use(SourceInfo<T> targetInfo, IChunkColumn? column);
}
