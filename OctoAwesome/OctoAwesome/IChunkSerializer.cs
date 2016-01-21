using System.IO;

namespace OctoAwesome
{
    /// <summary>
    /// Interface, für einen Chunk-Serializer
    /// </summary>
    public interface IChunkSerializer
    {
        /// <summary>
        /// Serialisiert den angegebenen Chunk in den Angegebenen Stream
        /// </summary>
        /// <param name="stream">Der Stream in den der Chunk serialisiert werden soll</param>
        /// <param name="chunk">Der Chunk der serialisiert werden soll</param>
        void Serialize(Stream stream, IChunk chunk);

        /// <summary>
        /// Deserialisert einen Chunk aus dem angegebenen Stream für die angegebene Position
        /// </summary>
        /// <param name="stream">Der Stream, der die serialisierten Chunkdaten enthält</param>
        /// <param name="position">Die Position, an der der Chunk sein soll</param>
        /// <returns></returns>
        IChunk Deserialize(Stream stream, PlanetIndex3 position);
    }
}