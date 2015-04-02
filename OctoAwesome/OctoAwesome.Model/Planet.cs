using Microsoft.Xna.Framework;
using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    /// <summary>
    /// Standard-Implementierung des Planeten.
    /// </summary>
    public class Planet : IPlanet
    {
        /// <summary>
        /// Größe des Chunk-Caches. (Anzahl im speicher gehaltener Chunks)
        /// </summary>
        private readonly int CACHELIMIT = 10000;

        /// <summary>
        /// Referenz auf den verwendeten Map-Generator.
        /// </summary>
        private IMapGenerator generator;

        /// <summary>
        /// Chunk Array.
        /// </summary>
        private IChunk[, ,] chunks;

        /// <summary>
        /// Speicher für den letzten Zugriff auf Chunks (Caching)
        /// </summary>
        private Dictionary<Index3, int> lastAccess = new Dictionary<Index3, int>();

        /// <summary>
        /// Fortlaufender Counter für den Zugriff auf Chunks.
        /// </summary>
        private int accessCounter = 0;

        /// <summary>
        /// ID des Planeten.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Seed des Zufallsgenerators dieses Planeten.
        /// </summary>
        public int Seed { get; private set; }

        /// <summary>
        /// Die Größe des Planeten in Blocks.
        /// </summary>
        public Index3 Size { get; private set; }

        /// <summary>
        /// Instanz der Persistierungseinheit.
        /// </summary>
        public IChunkPersistence ChunkPersistence { get; set; }

        /// <summary>
        /// Initialisierung des Planeten
        /// </summary>
        /// <param name="size">Größe des Planeten in Blocks</param>
        /// <param name="generator">Instanz des Map-Generators</param>
        /// <param name="seed">Seed des Zufallsgenerators</param>
        public Planet(Index3 size, IMapGenerator generator, int seed)
        {
            this.Id = 0;
            this.generator = generator;
            Size = size;
            Seed = seed;

            chunks = new Chunk[Size.X, Size.Y, Size.Z];
        }

        /// <summary>
        /// Liefert den Chunk an der angegebenen Chunk-Koordinate zurück.
        /// </summary>
        /// <param name="index">Chunk Index</param>
        /// <returns>Instanz des Chunks</returns>
        public IChunk GetChunk(Index3 index)
        {
            if (index.X < 0 || index.X >= Size.X || 
                index.Y < 0 || index.Y >= Size.Y || 
                index.Z < 0 || index.Z >= Size.Z)
                return null;

            if (chunks[index.X, index.Y, index.Z] == null)
            {
                // Load from disk
                IChunk first = ChunkPersistence.Load(this, index);
                if (first != null)
                {
                    for (int z = 0; z < this.Size.Z; z++)
                    {
                        chunks[index.X, index.Y, z] = ChunkPersistence.Load(
                            this, new Index3(index.X, index.Y, z));
                        lastAccess.Add(new Index3(index.X, index.Y, z), accessCounter++);
                    }
                }
                else
                {
                    IChunk[] result = generator.GenerateChunk(this, new Index2(index.X, index.Y));
                    for (int layer = 0; layer < this.Size.Z; layer++)
                    {
                        chunks[index.X, index.Y, layer] = result[layer];
                        lastAccess.Add(new Index3(index.X, index.Y, layer), accessCounter++);
                    }
                }

                // Cache regulieren
                while (lastAccess.Count > CACHELIMIT)
                {
                    Index3 oldest = lastAccess.OrderBy(a => a.Value).Select(a => a.Key).First();
                    var chunk = chunks[oldest.X, oldest.Y, oldest.Z];
                    ChunkPersistence.Save(chunk, this);
                    chunks[oldest.X, oldest.Y, oldest.Z] = null; // TODO: Pooling
                    lastAccess.Remove(oldest);
                }
            }
            else
            {
                lastAccess[index] = accessCounter++;
            }

            return chunks[index.X, index.Y, index.Z];
        }

        /// <summary>
        /// Liefert den Block an der angegebenen Block-Koodinate zurück.
        /// </summary>
        /// <param name="index">Block Index</param>
        /// <returns>Block oder null, falls dort kein Block existiert</returns>
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

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="index">Block-Koordinate</param>
        /// <param name="block">Neuer Block oder null, falls der alte Bock gelöscht werden soll.</param>
        public void SetBlock(Index3 index, IBlock block)
        {
            index.NormalizeXYZ(new Index3(
                Size.X * Chunk.CHUNKSIZE_X,
                Size.Y * Chunk.CHUNKSIZE_Y,
                Size.Z * Chunk.CHUNKSIZE_Z));
            Coordinate coordinate = new Coordinate(0, index, Vector3.Zero);
            IChunk chunk = GetChunk(coordinate.ChunkIndex);
            chunk.SetBlock(coordinate.LocalBlockIndex, block);
        }

        /// <summary>
        /// Persistiert den Planeten.
        /// </summary>
        public void Save()
        {
            for (int z = 0; z < Size.Z; z++)
            {
                for (int y = 0; y < Size.Y; y++)
                {
                    for (int x = 0; x < Size.X; x++)
                    {
                        if (chunks[x, y, z] != null)
                            ChunkPersistence.Save(chunks[x, y, z], this);
                    }
                }
            }
        }
    }
}
