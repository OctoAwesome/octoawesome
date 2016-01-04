using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Tests
{
    internal class TestChunk : IChunk
    {
        private int planet;
        private Index3 index;

        public TestChunk(PlanetIndex3 index)
        {
            this.planet = index.Planet;
            this.index = index.ChunkIndex;
        }

        public ushort[] Blocks
        {
            get { throw new NotImplementedException(); }
        }

        public int ChangeCounter
        {
            get { throw new NotImplementedException(); }

            set { throw new NotImplementedException(); }
        }

        public Index3 Index
        {
            get { return index; }
        }

        public int[] MetaData
        {
            get { throw new NotImplementedException(); }
        }

        public int Planet
        {
            get { return planet; }
        }

        public ushort[][] Resources
        {
            get { throw new NotImplementedException(); }
        }

        public ICollection<Entity> Entities
        {
            get { throw new NotImplementedException(); }
        }

        public ushort GetBlock(Index3 index)
        {
            throw new NotImplementedException();
        }

        public ushort GetBlock(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public int GetBlockMeta(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public ushort[] GetBlockResources(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public void SetBlock(Index3 index, ushort block, int meta = 0)
        {
            throw new NotImplementedException();
        }

        public void SetBlock(int x, int y, int z, ushort block, int meta = 0)
        {
            throw new NotImplementedException();
        }

        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            throw new NotImplementedException();
        }

        public void SetBlockResources(int x, int y, int z, ushort[] resources)
        {
            throw new NotImplementedException();
        }
    }
}