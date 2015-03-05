using Microsoft.Xna.Framework;
using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public class Planet
    {
        private Chunk[, ,] chunks;

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
                        chunks[x, y, z] = new Chunk(new Index3(x, y, z));
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
            if (chunks[x, y, z] == null)
            {
                // TODO: Load from disk
            }

            return chunks[x, y, z];
        }

        public IBlock GetBlock(Index3 pos)
        {
            Coordinate coordinate = new Coordinate(0, pos, Vector3.Zero);
            Chunk chunk = GetChunk(coordinate.AsChunk());
            return chunk.GetBlock(coordinate.AsLocalBlock());
        }

        public void SetBlock(Index3 pos, IBlock block, TimeSpan time)
        {
            Coordinate coordinate = new Coordinate(0, pos, Vector3.Zero);
            Chunk chunk = GetChunk(coordinate.AsChunk());
            chunk.SetBlock(coordinate.AsLocalBlock(), block, time);
        }
    }
}
