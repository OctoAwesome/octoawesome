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

        public int Id { get; private set; }

        public int Seed { get; private set; }

        public Index3 Size { get; private set; }

        public IChunkPersistence ChunkPersistence { get; set; }

        public Planet(Index3 size, IMapGenerator generator, int seed)
        {
            this.Id = 0;
            this.generator = generator;
            Size = size;
            Seed = seed;

            chunks = new Chunk[Size.X, Size.Y, Size.Z];
        }

        public IChunk GetChunk(Index3 index)
        {
            if (index.X < 0 || index.X >= Size.X || 
                index.Y < 0 || index.Y >= Size.Y || 
                index.Z < 0 || index.Z >= Size.Z)
                return null;

            if (chunks[index.X, index.Y, index.Z] == null)
            {
                // Load from disk
                IChunk first = ChunkPersistence.Load(Id, index);
                if (first != null)
                {
                    for (int z = 0; z < this.Size.Z; z++)
                    {
                        chunks[index.X, index.Y, z] = ChunkPersistence.Load(
                            Id, new Index3(index.X, index.Y, z));
                    }
                }
                else
                {
                    IChunk[] result = generator.GenerateChunk(this, new Index2(index.X, index.Y));
                    for (int layer = 0; layer < this.Size.Z; layer++)
                    {
                        chunks[index.X, index.Y, layer] = result[layer];
                    }
                }
            }

            return chunks[index.X, index.Y, index.Z];
        }

        public IBlock GetBlock(Index3 index)
        {
            index.NormalizeXY(new Index2(
                Size.X * Chunk.CHUNKSIZE_X, 
                Size.Y * Chunk.CHUNKSIZE_Y));
            Coordinate coordinate = new Coordinate(0, index, Vector3.Zero);
            
            // Betroffener Chunk ermitteln
            IChunk chunk = GetChunk(coordinate.ChunkIndex);
            if (chunk == null)
                return null;

            return chunk.GetBlock(coordinate.LocalBlockIndex);
        }

        public void SetBlock(Index3 index, IBlock block, TimeSpan time)
        {
            index.NormalizeXYZ(new Index3(
                Size.X * Chunk.CHUNKSIZE_X,
                Size.Y * Chunk.CHUNKSIZE_Y,
                Size.Z * Chunk.CHUNKSIZE_Z));
            Coordinate coordinate = new Coordinate(0, index, Vector3.Zero);
            IChunk chunk = GetChunk(coordinate.ChunkIndex);
            chunk.SetBlock(coordinate.LocalBlockIndex, block);
        }

        public void Save()
        {
            for (int z = 0; z < Size.Z; z++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    for (int x = 0; x < Size.X; x++)
                    {
                        if (chunks[x, y, z] != null)
                            ChunkPersistence.Save(chunks[x, y, z], Id);
                    }
                }
            }
        }
    }
}
