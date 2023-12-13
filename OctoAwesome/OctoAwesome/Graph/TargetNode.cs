using System;

namespace OctoAwesome.Graph;

//public abstract partial class TargetNode<T> : Node<T>
//{

//}
public interface ITargetNode<T>
{
    int Priority { get; }
    Index3 Position { get; }

    void Execute(TargetInfo<T> targetInfo, IChunkColumn? column);

   TargetInfo<T> GetRequired();

}