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
    public class Chunk : IChunk
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

        protected IBlock[] blocks;

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
            blocks = new IBlock[CHUNKSIZE_X * CHUNKSIZE_Y * CHUNKSIZE_Z];
            Index = pos;
            Planet = planet;
            ChangeCounter = 0;
        }

        /// <summary>
        /// Liefet den Block an der angegebenen Koordinate zurück.
        /// </summary>
        /// <param name="index">Koordinate des Blocks innerhalb des Chunkgs</param>
        /// <returns>Block oder null, falls es dort keinen Block gibt.</returns>
        public IBlock GetBlock(Index3 index)
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
        public IBlock GetBlock(int x, int y, int z)
        {
            return blocks[GetFlatIndex(x, y, z)];
        }

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="index">Koordinate des Blocks innerhalb des Chunks</param>
        /// <param name="block">Der neue Block oder null, fall der Block geleert werden soll</param>
        public void SetBlock(Index3 index, IBlock block)
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
        public void SetBlock(int x, int y, int z, IBlock block)
        {
            blocks[GetFlatIndex(x, y, z)] = block;
            ChangeCounter++;
        }

        /// <summary>
        /// Liefert den Index des Blocks im abgeflachten Block-Array der angegebenen 3D-Koordinate zurück.
        /// </summary>
        /// <param name="x">X-Anteil der Koordinate</param>
        /// <param name="y">Y-Anteil der Koordinate</param>
        /// <param name="z">Z-Anteil der Koordinate</param>
        /// <returns>Index innerhalb des flachen Arrays</returns>
        protected int GetFlatIndex(int x, int y, int z)
        {
            return ((z & (CHUNKSIZE_Z - 1)) << (LimitX + LimitY))
                   | ((y & (CHUNKSIZE_Y - 1)) << LimitX)
                   | ((x & (CHUNKSIZE_X - 1)));
        }

        /// <summary>
        /// Liefert den Index des Blocks im abgeflachten Block-Array der angegebenen 3D-Koordinate zurück.
        /// </summary>
        /// <param name="index">3D-Koordinate</param>
        /// <returns>Index innerhalb des flachen Arrays</returns>
        protected int GetFlatIndex(Index3 index)
        {
            return GetFlatIndex(index.X, index.Y, index.Z);
        }

        /// <summary>
        /// Serialisiert den aktuellen Chunk in den angegebenen Stream.
        /// </summary>
        /// <param name="stream">Output Stream</param>
        public void Serialize(Stream stream)
        {
            using (BinaryWriter bw = new BinaryWriter(stream))
            {
                List<Type> types = new List<Type>();

                // Types sammeln
                for (int i = 0; i < blocks.Length; i++)
                {
                    if (blocks[i] != null)
                    {
                        Type t = blocks[i].GetType();
                        if (!types.Contains(t))
                            types.Add(t);
                    }
                }

                // Schreibe Phase 1
                bw.Write(types.Count);

                // Im Falle eines Luft-Chunks...
                if (types.Count == 0)
                    return;

                foreach (var t in types)
                {
                    bw.Write(t.FullName);
                }

                // Schreibe Phase 2
                for (int i = 0; i < blocks.Length; i++)
                {
                    if (blocks[i] == null)
                        bw.Write(0);
                    else
                    {
                        bw.Write(types.IndexOf(blocks[i].GetType()) + 1);
                        bw.Write((byte)blocks[i].Orientation);
                    }
                }
            }
        }

        /// <summary>
        /// Deserialisiert einen Chunk aus dem angegebenen Stream.
        /// </summary>
        /// <param name="stream">Input Stream</param>
        /// <param name="knownBlocks">Liste der bekannten Block-Typen</param>
        public void Deserialize(Stream stream, IEnumerable<IBlockDefinition> knownBlocks)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                List<Type> types = new List<Type>();
                int typecount = br.ReadInt32();

                // Im Falle eines Luftchunks
                if (typecount == 0)
                    return;

                for (int i = 0; i < typecount; i++) 
                {
                    string typeName = br.ReadString();
                    var blockDefinition = knownBlocks.First(d => d.GetBlockType().FullName == typeName);
                    types.Add(blockDefinition.GetBlockType());
                }

                for (int i = 0; i < blocks.Length; i++)
                {
                    int typeIndex = br.ReadInt32();
                    if (typeIndex > 0)
                    {
                        OrientationFlags orientation = (OrientationFlags)br.ReadByte();
                        Type t = types[typeIndex - 1];
                        blocks[i] = (IBlock)Activator.CreateInstance(t);
                        blocks[i].Orientation = orientation;
                    }
                }
            }
        }
    }
}
