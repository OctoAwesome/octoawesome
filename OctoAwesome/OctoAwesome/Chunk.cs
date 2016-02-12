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
        /// <summary>
        /// Zweierpotenz der Chunkgrösse. Ausserdem gibt es die Anzahl Bits an,
        /// die die X-Koordinate im Array <see cref="Blocks"/> verwendet.
        /// </summary>
        public const int LimitX = 5;
        /// <summary>
        /// Zweierpotenz der Chunkgrösse. Ausserdem gibt es die Anzahl Bits an,
        /// die die Y-Koordinate im Array <see cref="Blocks"/> verwendet.
        /// </summary>
        public const int LimitY = 5;
        /// <summary>
        /// Zweierpotenz der Chunkgrösse. Ausserdem gibt es die Anzahl Bits an,
        /// die die Z-Koordinate im Array <see cref="Blocks"/> verwendet.
        /// </summary>
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


        /// <summary>
        /// Grösse eines Chunk als <see cref="Index3"/>
        /// </summary>
        public static readonly Index3 CHUNKSIZE = new Index3(CHUNKSIZE_X, CHUNKSIZE_Y, CHUNKSIZE_Z);

        /// <summary>
        /// Array, das alle Blöcke eines Chunks enthält. Jeder eintrag entspricht einer Block-ID.
        /// Der Index ist derselbe wie bei <see cref="MetaData"/> und <see cref="Resources"/>.
        /// </summary>
        public ushort[] Blocks { get; private set; }

        /// <summary>
        /// Array, das die Metadaten zu den Blöcken eines Chunks enthält.
        /// Der Index ist derselbe wie bei <see cref="Blocks"/> und <see cref="Resources"/>.
        /// </summary>
        public int[] MetaData { get; private set; }

        /// <summary>
        /// Verzweigtes Array, das die Ressourcen zu den Blöcken eines Chunks enthält.
        /// Der Index der ersten Dimension ist derselbe wie bei <see cref="Blocks"/> und <see cref="Resources"/>.
        /// </summary>
        public ushort[][] Resources { get; private set; }

        /// <summary>
        /// Sammlung aller <see cref="Entity"/> im aktuellen Chunk. Diese verden weitergereicht, wenn sie den Chunk verlassen.
        /// </summary>
        public ICollection<Entity> Entities { get; private set; }

        /// <summary>
        /// Chunk Index innerhalb des Planeten.
        /// </summary>
        public Index3 Index { get; private set; }

        /// <summary>
        /// Referenz auf den Planeten.
        /// </summary>
        public int Planet { get; private set; }

        /// <summary>
        /// Ein Counter, der jede Veränderung durch SetBlock gemacht wird. Kann 
        /// dazu verwendet werden herauszufinden, ob es Änderungen gab.<para/>
        /// TODO: ChangeCounter überdenken, eventuell eine bool
        /// </summary>
        public int ChangeCounter { get; set; }

        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse Chunk
        /// </summary>
        /// <param name="pos">Position des Chunks</param>
        /// <param name="planet">Index des Planeten</param>
        public Chunk(Index3 pos, int planet)
        {
            Blocks = new ushort[CHUNKSIZE_X * CHUNKSIZE_Y * CHUNKSIZE_Z];
            MetaData = new int[CHUNKSIZE_X * CHUNKSIZE_Y * CHUNKSIZE_Z];
            Resources = new ushort[CHUNKSIZE_X * CHUNKSIZE_Y * CHUNKSIZE_Z][];

            Index = pos;
            Planet = planet;
            ChangeCounter = 0;

            Entities = new List<Entity>();
        }

        /// <summary>
        /// Liefet den Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="index">Koordinate des Blocks innerhalb des Chunkgs</param>
        /// <returns>Die Block-ID an der angegebenen Koordinate</returns>
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
        /// <returns>Block-ID der angegebenen Koordinate</returns>
        public ushort GetBlock(int x, int y, int z)
        {
            return Blocks[GetFlatIndex(x, y, z)];
        }

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="index">Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="block">Die neue Block-ID.</param>
        /// <param name="meta">(Optional) Metainformationen für den Block</param>
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
        /// <param name="block">Die neue Block-ID</param>
        /// <param name="meta">(Optional) Die Metadaten des Blocks</param>
        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
        {
            int index = GetFlatIndex(x, y, z);
            Blocks[index] = block;
            MetaData[index] = meta;

            //TODO: ChangeCounter überdenken, eventuell eine bool
            ChangeCounter++;
        }

        /// <summary>
        /// Gibt die Metadaten des Blocks an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <returns>Die Metadaten des angegebenen Blocks</returns>
        public int GetBlockMeta(int x, int y, int z)
        {
            return MetaData[GetFlatIndex(x, y, z)];
        }

        /// <summary>
        /// Ändert die Metadaten des Blockes an der angegebenen Koordinate. 
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="meta">Die neuen Metadaten</param>
        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            MetaData[GetFlatIndex(x, y, z)] = meta;

            //TODO: ChangeCounter überdenken, eventuell eine bool
            ChangeCounter++;
        }

        /// <summary>
        /// Liefert alle Ressourcen im Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <returns>Ein Array aller Ressourcen des Blocks</returns>
        public ushort[] GetBlockResources(int x, int y, int z)
        {
            return Resources[GetFlatIndex(x, y, z)];
        }

        /// <summary>
        /// Ändert die Ressourcen des Blocks an der angegebenen Koordinate
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="resources">Ein <see cref="ushort"/>-Array, das alle Ressourcen enthält</param>
        public void SetBlockResources(int x, int y, int z, ushort[] resources)
        {
            Resources[GetFlatIndex(x, y, z)] = resources;

            //TODO: ChangeCounter überdenken, eventuell eine bool
            ChangeCounter++;
        }

        /// <summary>
        /// Liefert den Index des Blocks im abgeflachten Block-Array der angegebenen 3D-Koordinate zurück. Sollte die Koordinate ausserhalb
        /// der Chunkgrösse liegen, wird dies gewrapt.
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