using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public struct Coordinate
    {
        public int Planet;

        public Index3 Block;

        public Vector3 Position;

        public Coordinate(int planet, Index3 block, Vector3 position)
        {
            Planet = planet;
            Block = block;
            Position = position;
            this.Normalize();
        }

        public Vector3 AsVector3()
        {
            return new Vector3(
                Block.X + Position.X, 
                Block.Y + Position.Y, 
                Block.Z + Position.Z);
        }

        public Index3 AsChunk()
        {
            return new Index3(
                (int)(Block.X / Chunk.CHUNKSIZE_X), 
                (int)(Block.Y / Chunk.CHUNKSIZE_Y), 
                (int)(Block.Z / Chunk.CHUNKSIZE_Z));
        }

        public Index3 AsLocalBlock()
        {
            return new Index3(
                Block.X % Chunk.CHUNKSIZE_X, 
                Block.Y % Chunk.CHUNKSIZE_Y, 
                Block.Z % Chunk.CHUNKSIZE_Z);
        }

        public Vector3 AsLocalPosition()
        {
            return new Vector3(
                Block.X % Chunk.CHUNKSIZE_X + Position.X,
                Block.Y % Chunk.CHUNKSIZE_Y + Position.Y,
                Block.Z % Chunk.CHUNKSIZE_Z + Position.Z);
        }

        public void Normalize()
        {
            Block.X += (int)Math.Floor(Position.X);
            Position.X = (Position.X >= 0) ? (Position.X = Position.X % 1) : (1f + (Position.X % 1));

            Block.Y += (int)Math.Floor(Position.Y);
            Position.Y = (Position.Y >= 0) ? (Position.Y = Position.Y % 1) : (1f + (Position.Y % 1));

            Block.Z += (int)Math.Floor(Position.Z);
            Position.Z = (Position.Z >= 0) ? (Position.Z = Position.Z % 1) : (1f + (Position.Z % 1));
        }

        public static Coordinate operator +(Coordinate i1, Coordinate i2)
        {
            Vector3 position = i1.Position + i2.Position;
            Index3 block = i1.Block + i2.Block;

            if (i1.Planet != i2.Planet)
                throw new NotSupportedException();

            Coordinate result = new Coordinate(i1.Planet, block, position);
            result.Normalize();
            return result;
        }

        public static Coordinate operator +(Coordinate i1, Vector3 i2)
        {
            Vector3 position = i1.Position + i2;
            Index3 block = i1.Block;

            Coordinate result = new Coordinate(i1.Planet, block, position);
            result.Normalize();
            return result;
        }

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
