using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noises
{
    public class ChunkGenerator
    {

        public UInt32 Chunksize_X { get; private set; }
        public UInt32 Chunksize_Y { get; private set; }
        public UInt32 Chunksize_Z { get; private set; }
        public INoise Generator { get; private set;}

        public ChunkGenerator(INoise generator , UInt32 chunksize_X, UInt32 chunksize_Y, UInt32 chunksize_Z)
        {

            this.Generator = generator;
            this.Chunksize_X = chunksize_X;
            this.Chunksize_Y = chunksize_Y;
            this.Chunksize_Z = chunksize_Z;

        }

        public IBlock[,,] CreateChunk(int chunkX, int chunkY, int chunkZ)
        {

            IBlock[,,] blocks = new IBlock[Chunksize_X, Chunksize_Y, Chunksize_Z];

            float[, ,] noise = Generator.GetNoise3(chunkX, chunkY, chunkZ, (int)Chunksize_X, (int)Chunksize_Y, (int)Chunksize_Z);

            for (int x = 0; x < Chunksize_X; x++)
            {
                for (int z = 0; z < Chunksize_Z; z++)
                {
                    for (int y = 0; y < Chunksize_Y; y++)
                    {
                        if (noise[x, y, z] < 0f)
                                blocks[x, y, z] = new SandBlock();
                        else if( noise[x,y,z] < 0.2f)
                                blocks[x, y, z] = new GrassBlock();
                    }
                }
            }


            return blocks;
        }



    }
}
