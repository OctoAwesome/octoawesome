using System.IO;

namespace OctoAwesome.Serialization
{

    public interface ISerializable
    {
        void Serialize(BinaryWriter writer);

        void Deserialize(BinaryReader reader);
    }
}