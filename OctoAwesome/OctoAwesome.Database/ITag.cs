using System;

namespace OctoAwesome.Database
{
    /// <summary>
    /// Database tag interface.
    /// </summary>
    public interface ITag
    {
        /// <summary>
        /// Gets the tag length in bytes.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Gets the raw bytes for this tag.
        /// </summary>
        /// <returns>The raw bytes created from this tag.</returns>
        byte[] GetBytes(); // TODO: code deduplication: Use WriteBytes to create array

        /// <summary>
        /// Deserializes this tag from raw bytes.
        /// </summary>
        /// <param name="array">The raw byte data to deserialize the tag from.</param>
        /// <param name="startIndex">The starting index to start the deserialization at.</param>
        void FromBytes(byte[] array, int startIndex); // TODO: use span

        /// <summary>
        /// Serializes this tag to raw bytes.
        /// </summary>
        /// <param name="span">The span to write the serialized tag to.</param>
        void WriteBytes(Span<byte> span);
    }
}
