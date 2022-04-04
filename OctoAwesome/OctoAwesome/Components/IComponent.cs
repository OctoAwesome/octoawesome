using OctoAwesome.Serialization;

namespace OctoAwesome.Components
{

    public interface IComponent : ISerializable
    {

        bool Sendable { get; set; }
        bool Enabled { get; set; }
    }
}
