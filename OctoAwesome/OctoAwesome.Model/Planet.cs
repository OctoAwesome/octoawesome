using Microsoft.Xna.Framework;
using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public class Planet : IPlanet
    {
        private IMapGenerator generator;

        private IChunk[, ,] chunks;

        public int Seed { get; private set; }

        public Index3 Size { get; private set; }

        public Planet(Index3 size, IMapGenerator generator, int seed)
        {
            this.generator = generator;
            Size = size;
            Seed = seed;

            chunks = new Chunk[Size.X, Size.Y, Size.Z];

            for (int x = 0; x < Size.X; x++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    IChunk[] result = generator.GenerateChunk(this, new Index2(x, y));
                    for (int layer = 0; layer < this.Size.Z; layer++)
                    {
                        chunks[x, y, layer] = result[layer];
                    }
                }
            }
        }

        public IChunk GetChunk(Index3 index)
        {
            if (index.X < 0 || index.X >= Size.X || 
                index.Y < 0 || index.Y >= Size.Y || 
                index.Z < 0 || index.Z >= Size.Z)
                return null;

            if (chunks[index.X, index.Y, index.Z] == null)
            {
                // TODO: Load from disk
            }

            return chunks[index.X, index.Y, index.Z];
        }

        public IBlock GetBlock(Index3 index)
        {
            index.Normalize(new Index3(
                Size.X * Chunk.CHUNKSIZE_X, 
                Size.Y * Chunk.CHUNKSIZE_Y, 
                Size.Z * Chunk.CHUNKSIZE_Z));
            Coordinate coordinate = new Coordinate(0, index, Vector3.Zero);
            IChunk chunk = GetChunk(coordinate.ChunkIndex);
            return chunk.GetBlock(coordinate.LocalBlockIndex);
        }

        public void SetBlock(Index3 index, IBlock block, TimeSpan time)
        {
            index.Normalize(new Index3(
                Size.X * Chunk.CHUNKSIZE_X,
                Size.Y * Chunk.CHUNKSIZE_Y,
                Size.Z * Chunk.CHUNKSIZE_Z));
            Coordinate coordinate = new Coordinate(0, index, Vector3.Zero);
            IChunk chunk = GetChunk(coordinate.ChunkIndex);
            chunk.SetBlock(coordinate.LocalBlockIndex, block);
        }
    }
}
