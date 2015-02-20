using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    public class Chunk
    {
        public const int CHUNKSIZE_X = 50;
        public const int CHUNKSIZE_Y = 50;
        public const int CHUNKSIZE_Z = 50;

        public int Chunk_X { get; private set; }
        public int Chunk_Y { get; private set; }
        public int Chunk_Z { get; private set; }
        public Boolean Active { get; private set; }

        private IBlock[, ,] _Blocks;
        public IBlock[, ,] Blocks
        {
            get
            {
                Active = true;
                return _Blocks;
            }
            set
            {
                Active = true;
                _Blocks = value;
            }
        }
        private Boolean _Visible;

        public Boolean Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                Active = value;
                _Visible = value;
            }
        }

        public Chunk(int chunkX, int chunkY, int chunkZ)
        {

            this.Chunk_X = chunkX;
            this.Chunk_Y = chunkY;
            this.Chunk_Z = chunkZ;

            Active = true;

            _Blocks = new IBlock[CHUNKSIZE_X, CHUNKSIZE_Y, CHUNKSIZE_Z];
        }

    }
}
