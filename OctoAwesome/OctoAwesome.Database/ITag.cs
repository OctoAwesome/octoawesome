using System;

namespace OctoAwesome.Database
{
    public interface ITag
    {
        int Length { get; }

        byte[] GetBytes(); // TODO: code deduplication: Use WriteBytes to create array
        void FromBytes(byte[] array, int startIndex); // TODO: use span

        void WriteBytes(Span<byte> span);
    }
}
