using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization
{
    public struct ChunkDiffTag : ITag
    {
        public int Length => sizeof(int) * 4;

        public Index3 ChunkPositon { get; set; }
        public int FlatIndex { get; set; }

        public ChunkDiffTag(Index3 chunkPosition, int flatIndex)
        {
            ChunkPositon = chunkPosition;
            FlatIndex = flatIndex;
        }

        public void FromBytes(byte[] array, int startIndex)
        {
            var x = BitConverter.ToInt32(array, startIndex);
            var y = BitConverter.ToInt32(array, startIndex + sizeof(int));
            var z = BitConverter.ToInt32(array, startIndex + sizeof(int) * 2);
            FlatIndex = BitConverter.ToInt32(array, startIndex + sizeof(int) * 3);
            ChunkPositon = new Index3(x, y, z);
        }

        public byte[] GetBytes()
        {
            var array = new byte[Length];

            Buffer.BlockCopy(BitConverter.GetBytes(ChunkPositon.X), 0, array, 0, sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(ChunkPositon.Y), 0, array, sizeof(int), sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(ChunkPositon.Z), 0, array, sizeof(int) * 2, sizeof(int));
            Buffer.BlockCopy(BitConverter.GetBytes(FlatIndex), 0, array, sizeof(int) * 3, sizeof(int));

            return array;
        }
    }
}
