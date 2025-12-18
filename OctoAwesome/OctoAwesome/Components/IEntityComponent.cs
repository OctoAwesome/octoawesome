using OctoAwesome.Serialization;

namespace OctoAwesome.Components
{
    /// <summary>
    /// Interface for entity components.
    /// </summary>
    public interface IEntityComponent : IComponent, ISerializable
    {
    }
}
