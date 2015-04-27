using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public class ResourceManager
    {
        public static int CacheSize = 10000;

        private bool disablePersistence = false;

        private IMapGenerator mapGenerator = null;
        private IChunkPersistence chunkPersistence = null;

        /// <summary>
        /// Chunk Cache.
        /// </summary>
        private Cache<Index3, IChunk> l1Cache;

        /// <summary>
        /// Chunk Cache.
        /// </summary>
        private Cache<Index3, IChunk> l2Cache;

        private IUniverse universe;
        private IPlanet planet;


        #region Singleton

        private static ResourceManager instance = null;
        public static ResourceManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ResourceManager();
                return instance;
            }
        }

        #endregion

        private ResourceManager()
        {
            mapGenerator = MapGeneratorManager.GetMapGenerators().First();
            chunkPersistence = new ChunkDiskPersistence();

            l1Cache = new Cache<Index3, IChunk>(10, loadL1Chunk, null);
            l2Cache = new Cache<Index3, IChunk>(CacheSize, loadL2Chunk, saveChunk);

            universe = mapGenerator.GenerateUniverse("Milchstraße");
            planet = mapGenerator.GeneratePlanet(universe, 0);

            bool.TryParse(ConfigurationManager.AppSettings["DisablePersistence"], out disablePersistence); 
        }

        public IUniverse GetUniverse(int id)
        {
            return universe;
        }

        public IPlanet GetPlanet(IUniverse universe, int id)
        {
            return planet;
        }

        /// <summary>
        /// Liefert den Chunk an der angegebenen Chunk-Koordinate zurück.
        /// </summary>
        /// <param name="index">Chunk Index</param>
        /// <returns>Instanz des Chunks</returns>
        public IChunk GetChunk(Index3 index)
        {
            if (index.X < 0 || index.X >= planet.Size.X ||
                index.Y < 0 || index.Y >= planet.Size.Y ||
                index.Z < 0 || index.Z >= planet.Size.Z)
                return null;

            return l2Cache.Get(index);
        }

        /// <summary>
        /// Liefert den Block an der angegebenen Block-Koodinate zurück.
        /// </summary>
        /// <param name="index">Block Index</param>
        /// <returns>Block oder null, falls dort kein Block existiert</returns>
        public IBlock GetBlock(Index3 index)
        {
            index.NormalizeXY(new Index2(
                planet.Size.X * Chunk.CHUNKSIZE_X,
                planet.Size.Y * Chunk.CHUNKSIZE_Y));
            Coordinate coordinate = new Coordinate(0, index, Vector3.Zero);

            // Betroffener Chunk ermitteln
            Index3 chunkIndex = coordinate.ChunkIndex;
            if (chunkIndex.X < 0 || chunkIndex.X >= planet.Size.X ||
                chunkIndex.Y < 0 || chunkIndex.Y >= planet.Size.Y ||
                chunkIndex.Z < 0 || chunkIndex.Z >= planet.Size.Z)
                return null;
            IChunk chunk = l1Cache.Get(chunkIndex);
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
                planet.Size.X * Chunk.CHUNKSIZE_X,
                planet.Size.Y * Chunk.CHUNKSIZE_Y,
                planet.Size.Z * Chunk.CHUNKSIZE_Z));
            Coordinate coordinate = new Coordinate(0, index, Vector3.Zero);
            IChunk chunk = GetChunk(coordinate.ChunkIndex);
            chunk.SetBlock(coordinate.LocalBlockIndex, block);
        }

        private IChunk loadL1Chunk(Index3 index)
        {
            return l2Cache.Get(index);
        }

        private IChunk loadL2Chunk(Index3 index)
        {
            // Load from disk
            IChunk first = chunkPersistence.Load(planet, index);
            if (first != null)
                return first;

            IChunk[] result = mapGenerator.GenerateChunk(planet, new Index2(index.X, index.Y));
            if (result != null && result.Length > index.Z)
            {
                result[index.Z].ChangeCounter = 0;
                return result[index.Z];
            }

            return null;
        }

        private void saveChunk(Index3 index, IChunk value)
        {
            if (!disablePersistence && value.ChangeCounter > 0)
            {
                chunkPersistence.Save(value, value.Planet);
                value.ChangeCounter = 0;
            }
        }

        /// <summary>
        /// Persistiert den Planeten.
        /// </summary>
        public void Save()
        {
            l2Cache.Flush();
        }
    }
}
