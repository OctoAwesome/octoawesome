using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    class SnowBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get
            {
                return "Snow";
            }
        }

        public override Bitmap Icon
        {
            get
            {
                return (Bitmap)Bitmap.FromFile("./Assets/OctoAwesome.Basics/snow.png"); 
            }
        }

        public override Bitmap[] Textures
        {
            get
            {
                return new[] {
                    (Bitmap)Bitmap.FromFile("./Assets/OctoAwesome.Basics/snow.png"),
                    (Bitmap)Bitmap.FromFile("./Assets/OctoAwesome.Basics/dirt.png"),
                    (Bitmap)Bitmap.FromFile("./Assets/OctoAwesome.Basics/dirt_snow.png"),
                };
            }
        }

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 2f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
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
