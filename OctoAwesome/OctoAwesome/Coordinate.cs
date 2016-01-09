using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Datenstruktur zur genauen Position von Spiel-Elementen innerhalb der OctoAwesome Welt.
    /// </summary>
    public struct Coordinate
    {
        /// <summary>
        /// Index des Planeten im Universum.
        /// </summary>
        public int Planet;

        /// <summary>
        /// Index des betroffenen Blocks.
        /// </summary>
        private Index3 block;

        /// <summary>
        /// Position innerhalb des Blocks (0...1).
        /// </summary>
        private Vector3 position;

        public Coordinate(int planet, Index3 block, Vector3 position)
        {
            Planet = planet;
            this.block = block;
            this.position = position;
            this.Normalize();
        }

        /// <summary>
        /// Gibt den Index des Chunks zurück oder legt diesen fest.
        /// </summary>
        [XmlIgnore]
        public Index3 ChunkIndex
        {
            get
            {
                return new Index3(block.X >> Chunk.LimitX, block.Y >> Chunk.LimitY,
                    block.Z >> Chunk.LimitZ);
            }
            set
            {
                Index3 localBlockIndex = LocalBlockIndex;
                block = new Index3(
                    (value.X * Chunk.CHUNKSIZE_X) + localBlockIndex.X,
                    (value.Y * Chunk.CHUNKSIZE_Y) + localBlockIndex.Y,
                    (value.Z * Chunk.CHUNKSIZE_Z) + localBlockIndex.Z);
            }
        }

        /// <summary>
        /// Gibt den globalen Index (Planet-Koordinaten) des Blockes zurück oder legt diesen fest.
        /// </summary>
        public Index3 GlobalBlockIndex
        {
            get { return block; }
            set { block = value; }
        }

        /// <summary>
        /// Gibt den lokalen Index des Blocks (Chunk-Koordinaten) zurück oder legt diesen fest.
        /// </summary>
        [XmlIgnore]
        public Index3 LocalBlockIndex
        {
            get
            {
                Index3 chunk = ChunkIndex;
                return new Index3(
                    block.X - (chunk.X * Chunk.CHUNKSIZE_X),
                    block.Y - (chunk.Y * Chunk.CHUNKSIZE_Y),
                    block.Z - (chunk.Z * Chunk.CHUNKSIZE_Z));
            }
            set
            {
                Index3 chunk = ChunkIndex;
                GlobalBlockIndex = new Index3(
                    (chunk.X * Chunk.CHUNKSIZE_X) + value.X,
                    (chunk.Y * Chunk.CHUNKSIZE_Y) + value.Y,
                    (chunk.Z * Chunk.CHUNKSIZE_Z) + value.Z);
                Normalize();
            }
        }

        /// <summary>
        /// Gibt die globale Position (Planet-Koordinaten) als Vektor zurück oder legt diesen fest.
        /// </summary>
        [XmlIgnore]
        public Vector3 GlobalPosition
        {
            get
            {
                return new Vector3(
                    block.X + position.X,
                    block.Y + position.Y,
                    block.Z + position.Z);
            }
            set
            {
                block = Index3.Zero;
                position = value;
                Normalize();
            }
        }

        /// <summary>
        /// Gibt die lokale Position (Chunk-Koordinaten) als Vektor zurück oder legt diese fest.
        /// </summary>
        [XmlIgnore]
        public Vector3 LocalPosition
        {
            get
            {
                Index3 blockIndex = LocalBlockIndex;
                return new Vector3(
                    blockIndex.X + position.X,
                    blockIndex.Y + position.Y,
                    blockIndex.Z + position.Z);
            }
            set
            {
                Index3 chunkIndex = ChunkIndex;
                block = new Index3(
                    chunkIndex.X * Chunk.CHUNKSIZE_X,
                    chunkIndex.Y * Chunk.CHUNKSIZE_Y,
                    chunkIndex.Z * Chunk.CHUNKSIZE_Z);
                position = value;
                Normalize();
            }
        }

        /// <summary>
        /// Gibt die Position innerhalb des aktuellen Blockes zurück oder legt diese fest.
        /// </summary>
        public Vector3 BlockPosition
        {
            get { return position; }
            set
            {
                position = value;
                Normalize();
            }
        }

        /// <summary>
        /// Normalisiert die vorhandenen Parameter auf den Position-Wertebereich von [0...1] und die damit verbundene Verschiebung im Block.
        /// </summary>
        private void Normalize()
        {
            Index3 shift = new Index3(
                (int)Math.Floor(position.X),
                (int)Math.Floor(position.Y),
                (int)Math.Floor(position.Z));

            block += shift;
            position = position - shift;
        }

        /// <summary>
        /// Normalisiert den ChunkIndex auf die gegebenen Limits.
        /// </summary>
        /// <param name="limit"></param>
        public void NormalizeChunkIndexXY(Index3 limit)
        {
            Index3 index = ChunkIndex;
            index.NormalizeXY(limit);
            ChunkIndex = index;
        }

        public static Coordinate operator +(Coordinate i1, Coordinate i2)
        {
            if (i1.Planet != i2.Planet)
                throw new NotSupportedException();

            return new Coordinate(i1.Planet, i1.block + i2.block, i1.position + i2.position);
        }

        public static Coordinate operator +(Coordinate i1, Vector3 i2)
        {
            return new Coordinate(i1.Planet, i1.block, i1.position + i2);
        }

        public override string ToString()
        {
            return
                "(" + Planet + "/" +
                (block.X + position.X).ToString("0.00") + "/" +
                (block.Y + position.Y).ToString("0.00") + "/" +
                (block.Z + position.Z).ToString("0.00") + ")";
        }
    }
}