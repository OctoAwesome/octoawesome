using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Tests
{
    internal class TestChunkColumn : IChunkColumn
    {
        private int planet;
        private Index2 index;

        public bool Populated
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int Planet
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Index2 Index
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int[,] Heights
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IChunk[] Chunks
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public TestChunkColumn(int planet, Index2 index)
        {
            this.planet = planet;
            this.index = index;
        }

        public ushort GetBlock(Index3 index)
        {
            throw new NotImplementedException();
        }

        public ushort GetBlock(int x, int y, int z)
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

        public int GetBlockMeta(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public void SetBlockMeta(int x, int y, int z, int meta)
        {
            throw new NotImplementedException();
        }

        public ushort[] GetBlockResources(int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public void SetBlockResources(int x, int y, int z, ushort[] resources)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream stream, IDefinitionManager definitionManager)
        {
            throw new NotImplementedException();
        }

        public void Deserialize(Stream stream, IDefinitionManager definitionManager, int planetId, Index2 columnIndex)
        {
            throw new NotImplementedException();
        }
    }
}
