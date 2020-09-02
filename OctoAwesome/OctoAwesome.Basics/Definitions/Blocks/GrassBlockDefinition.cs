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

        public override MaterialDefinition GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new MaterialDefinition()
            {
                Density = 2f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }

      

        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            if (wall == Wall.Top)
            {
                return 0;
            } else if (wall == Wall.Bottom)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }
}
