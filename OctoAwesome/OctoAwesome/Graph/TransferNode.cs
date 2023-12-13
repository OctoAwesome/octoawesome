using engenious.Content.Serialization;

using System.IO;

namespace OctoAwesome.Graph;

public abstract class EmptyTransferNode<T> : Node<T>, ITransferNode<T>
{
    
}

public interface ITransferNode<T>
{

}
