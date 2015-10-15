using System;
using System.Drawing;

namespace OctoAwesome.Basics
{
    public sealed class GrassBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return "Grass"; }
        }

        public override Bitmap Icon
        {
            get { return (Bitmap)Bitmap.FromFile("./Assets/OctoAwesome.Basics/grass_top.png"); }
        }

        public override Bitmap[] Textures
        {
            get
            {
                

                return new[] {
                    (Bitmap)Bitmap.FromFile("./Assets/OctoAwesome.Basics/grass_top.png"),
                    (Bitmap)Bitmap.FromFile("./Assets/OctoAwesome.Basics/dirt.png"),
                    (Bitmap)Bitmap.FromFile("./Assets/OctoAwesome.Basics/dirt_grass.png"),
                };
            }
        }

        public override PhysicalProperties GetProperties(IPlanetResourceManager manager, int x, int y, int z)
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

        public override int GetBottomTextureIndex(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 1;
        }

        public override int GetNorthTextureIndex(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 2;
        }

        public override int GetSouthTextureIndex(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 2;
        }

        public override int GetWestTextureIndex(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 2;
        }

        public override int GetEastTextureIndex(IPlanetResourceManager manager, int x, int y, int z)
        {
            return 2;
        }

        
    }
}
