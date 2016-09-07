using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public class SnowBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get
            {
                return Languages.OctoBasics.Snow;
            }
        }

        public override string Icon
        {
            get
            {
                return "snow"; 
            }
        }

        public override string[] Textures
        {
            get
            {
                return new[] {
                    "snow",
                    "dirt",
                    "dirt_snow",
                };
            }
        }

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 1.5f,
                FractureToughness = 0.2f,
                Granularity = 0.9f,
                Hardness = 0.05f
            };
        }

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties)
        {
            throw new NotImplementedException();
        }

        public override int GetBottomTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 1;
        }

        public override int GetNorthTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 2;
        }

        public override int GetSouthTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 2;
        }

        public override int GetWestTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 2;
        }

        public override int GetEastTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 2;
        }
    }
}
