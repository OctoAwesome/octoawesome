using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Repräsentiert einen Karten-Abschnitt innerhalb des Planeten.
    /// </summary>
    public sealed class Chunk : IChunk
    {
        public const int LimitX = 5;
        public const int LimitY = 5;
        public const int LimitZ = 5;

        /// <summary>
        /// Größe eines Chunks in Blocks in X-Richtung.
        /// </summary>
        public const int CHUNKSIZE_X = 1 << LimitX;

        /// <summary>
        /// Größe eines Chunks in Blocks in Y-Richtung.
        /// </summary>
        public const int CHUNKSIZE_Y = 1 << LimitY;

        /// <summary>
        /// Größe eines Chunks in Blocks in Z-Richtung.
        /// </summary>
        public const int CHUNKSIZE_Z = 1 << LimitZ;


        public static readonly Index3 CHUNKSIZE = new Index3(CHUNKSIZE_X, CHUNKSIZE_Y, CHUNKSIZE_Z);

        public ushort[] Blocks { get; private set; }

        public int[] MetaData { get; private set; }

        public ushort[][] Resources { get; private set; }

        public ICollection<Entity> Entities { get; private set; }


        /// <summary>
        /// Chunk Index innerhalb des Planeten.
        /// </summary>
        public Index3 Index { get; private set; }

        public int Planet { get; private set; }

        /// <summary>
        /// Ein Counter, der jede Veränderung durch SetBlock gemacht wird. Kann 
        /// dazu verwendet werden herauszufinden, ob es Änderungen gab.
        /// </summary>
        public int ChangeCounter { get; set; }

        public Chunk(Index3 pos, int planet)
        {
            Blocks = new ushort[CHUNKSIZE_X*CHUNKSIZE_Y*CHUNKSIZE_Z];
            MetaData = new int[CHUNKSIZE_X*CHUNKSIZE_Y*CHUNKSIZE_Z];
            Resources = new ushort[CHUNKSIZE_X*CHUNKSIZE_Y*CHUNKSIZE_Z][];

            Index = pos;
            Planet = planet;
            ChangeCounter = 0;

            Entities = new List<Entity>();
        }

        /// <summary>
        /// Liefet den Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="index">Koordinate des Blocks innerhalb des Chunkgs</param>
        /// <returns>Block oder null, falls es dort keinen Block gibt.</returns>
        public ushort GetBlock(Index3 index)
        {
            return GetBlock(index.X, index.Y, index.Z);
        }

        /// <summary>
        /// Liefet den Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Block oder null, falls es dort keinen Block gibt.</returns>
        public ushort GetBlock(int x, int y, int z)
        {
            return Blocks[GetFlatIndex(x, y, z)];
        }

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="index">Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="block">Der neue Block oder null, fall der Block geleert werden soll</param>
        public void SetBlock(Index3 index, ushort block, int meta = 0)
        {
            SetBlock(index.X, index.Y, index.Z, block);
        }


        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="block">Der neue Block oder null, fall der Block geleert werden soll</param>
        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
        {
            int index = GetFlatIndex(x, y, z);
            Blocks[index] = block;
            MetaData[index] = meta;

            //TODO: ChangeCounter überdenken, eventuell eine bool
            ChangeCounter++;
        }

        public int GetBlockMeta(int x, int y, int z)
        {
            return MetaData[GetFlatIndex(x, y, z)];
        }

        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            MetaData[GetFlatIndex(x, y, z)] = meta;

            //TODO: ChangeCounter überdenken, eventuell eine bool
            ChangeCounter++;
        }

        public ushort[] GetBlockResources(int x, int y, int z)
        {
            return Resources[GetFlatIndex(x, y, z)];
        }

        public void SetBlockResources(int x, int y, int z, ushort[] resources)
        {
            Resources[GetFlatIndex(x, y, z)] = resources;

            //TODO: ChangeCounter überdenken, eventuell eine bool
            ChangeCounter++;
        }

        /// <summary>
        /// Liefert den Index des Blocks im abgeflachten Block-Array der angegebenen 3D-Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate</param>
        /// <param name="y">Y-Anteil der Koordinate</param>
        /// <param name="z">Z-Anteil der Koordinate</param>
        /// <returns>Index innerhalb des flachen Arrays</returns>
        private int GetFlatIndex(int x, int y, int z)
        {
            return ((z & (CHUNKSIZE_Z - 1)) << (LimitX + LimitY))
                   | ((y & (CHUNKSIZE_Y - 1)) << LimitX)
                   | ((x & (CHUNKSIZE_X - 1)));
        }
    }
}