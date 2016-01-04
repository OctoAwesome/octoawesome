using System.IO;

namespace OctoAwesome
{
    public interface IChunkSerializer
    {
        void Serialize(Stream stream, IChunk chunk);

        IChunk Deserialize(Stream stream, PlanetIndex3 position);
    }
}