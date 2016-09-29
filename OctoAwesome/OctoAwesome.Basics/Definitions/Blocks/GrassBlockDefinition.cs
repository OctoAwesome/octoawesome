using System;
using System.Drawing;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GrassBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Grass; }
        }

        public override string Icon
        {
            get { return "grass_top"; }
        }

        public override string[] Textures
        {
            get
            {
                

                return new[] {
                    "grass_top",
                    "dirt",
                    "dirt_grass",
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
