using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
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
        /// Chunk Cache.
        /// </summary>
        private Cache<Index3, IChunk> chunks;

        /// <summary>
        /// Fortlaufender Counter für den Zugriff auf Chunks.
        /// </summary>
        private int accessCounter = 0;

        /// <summary>
        /// ID des Planeten.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Referenz auf das Parent Universe
        /// </summary>
        public IUniverse Universe { get; private set; }

        public IClimateMap ClimateMap { get; protected set; }

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
        public Planet(int id, IUniverse universe, Index3 size, IMapGenerator generator, int seed)
        {
            Id = id;
            Universe = universe;
            this.generator = generator;
            Size = size;
            Seed = seed;

            chunks = new Cache<Index3,IChunk>(CACHELIMIT, loadChunk, saveChunk);
        }

        private IChunk loadChunk(Index3 index)
        {
            // Load from disk
            IChunk first = ChunkPersistence.Load(this, index);
            if (first != null)
                return first;

            IChunk[] result = generator.GenerateChunk(this, new Index2(index.X, index.Y));
            if (result != null && result.Length > index.Z)
            {
                return result[index.Z];
            }

            return null;
        }

        private void saveChunk(Index3 index, IChunk value)
        {
            ChunkPersistence.Save(value, value.Planet);
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

            return chunks.Get(index);
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
            chunks.Flush();
        }
    }
}
