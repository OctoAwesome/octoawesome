using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public class Planet
    {
        private Chunk[,,] chunks;

        public int SizeX { get; private set; }
        public int SizeY { get; private set; }
        public int SizeZ { get; private set; }

        public Planet(int sizeX, int sizeY, int sizeZ)
        {
            SizeX = sizeX;
            SizeY = sizeY;
            SizeZ = sizeZ;

            chunks = new Chunk[sizeX, sizeY, sizeZ];

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    for (int z = 0; z < sizeZ; z++)
                    {
                        chunks[x, y, z] = new Chunk();
                    }
                }
            }
        }

        public Chunk GetChunk(Index3 pos)
        {
            return GetChunk(pos.X, pos.Y, pos.Z);
        }

        public Chunk GetChunk(int x, int y, int z)
        {
            return chunks[x, y, z];
        }
    }
}
