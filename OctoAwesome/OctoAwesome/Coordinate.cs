using System;
using System.Xml.Serialization;
using engenious;

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
        public Index3 Block;

        /// <summary>
        /// Position innerhalb des Blocks (0...1).
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// Erzeugt eine neue Instanz der Coordinate-Struktur.
        /// </summary>
        /// <param name="planet">Index des Planeten</param>
        /// <param name="block">Blockindex innerhalb des Planeten</param>
        /// <param name="position">Position innerhalb des Blockes</param>
        public Coordinate(int planet, Index3 block, Vector3 position)
        {
            Planet = planet;
            Block = block;
            Position = position;
            Normalize();
        }

        /// <summary>
        /// Gibt den Index des Chunks zurück oder legt diesen fest.
        /// </summary>
        [XmlIgnore]
        public Index3 ChunkIndex
        {
            get
            {
                return new Index3(Block.X >> Chunk.LimitX, Block.Y >> Chunk.LimitY,
                    Block.Z >> Chunk.LimitZ);
            }
            set
            {
                Index3 localBlockIndex = LocalBlockIndex;
                Block = new Index3(
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
            get { return Block; }
            set { Block = value; }
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
                    Block.X - (chunk.X * Chunk.CHUNKSIZE_X),
                    Block.Y - (chunk.Y * Chunk.CHUNKSIZE_Y),
                    Block.Z - (chunk.Z * Chunk.CHUNKSIZE_Z));
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
                    Block.X + Position.X,
                    Block.Y + Position.Y,
                    Block.Z + Position.Z);
            }
            set
            {
                Block = Index3.Zero;
                Position = value;
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
                    blockIndex.X + Position.X,
                    blockIndex.Y + Position.Y,
                    blockIndex.Z + Position.Z);
            }
            set
            {
                Index3 chunkIndex = ChunkIndex;
                Block = new Index3(
                    chunkIndex.X * Chunk.CHUNKSIZE_X,
                    chunkIndex.Y * Chunk.CHUNKSIZE_Y,
                    chunkIndex.Z * Chunk.CHUNKSIZE_Z);
                Position = value;
                Normalize();
            }
        }

        /// <summary>
        /// Gibt die Position innerhalb des aktuellen Blockes zurück oder legt diese fest.
        /// </summary>
        public Vector3 BlockPosition
        {
            get { return Position; }
            set
            {
                Position = value;
                Normalize();
            }
        }

        /// <summary>
        /// Normalisiert die vorhandenen Parameter auf den Position-Wertebereich von [0...1] und die damit verbundene Verschiebung im Block.
        /// </summary>
        private void Normalize()
        {
            Index3 shift = new Index3(
                (int)Math.Floor(Position.X),
                (int)Math.Floor(Position.Y),
                (int)Math.Floor(Position.Z));

            Block += shift;
            Position = Position - shift;
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

        /// <summary>
        /// Addiert die zwei gegebenen <see cref="Coordinate"/>s.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <exception cref="NotSupportedException">Wenn die beiden Coordinates nicht auf den selben Planeten verweisen</exception>
        /// <returns>Das Ergebnis der Addition</returns>
        public static Coordinate operator +(Coordinate i1, Coordinate i2)
        {
            if (i1.Planet != i2.Planet)
                throw new NotSupportedException();

            return new Coordinate(i1.Planet, i1.Block + i2.Block, i1.Position + i2.Position);
        }

        /// <summary>
        /// Addiert den gegebenen Vector3 auf die <see cref="BlockPosition"/> der Coordinate.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns>Das Ergebnis der Addition</returns>
        public static Coordinate operator +(Coordinate i1, Vector3 i2)
        {
            return new Coordinate(i1.Planet, i1.Block, i1.Position + i2);
        }

        /// <summary>
        /// Stellt die Coordinate-Instanz als string dar.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                "(" + Planet + "/" +
                (Block.X + Position.X).ToString("0.00") + "/" +
                (Block.Y + Position.Y).ToString("0.00") + "/" +
                (Block.Z + Position.Z).ToString("0.00") + ")";
        }
    }
}
