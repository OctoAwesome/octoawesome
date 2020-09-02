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

        public override MaterialDefinition GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new MaterialDefinition()
            {
                Density = 1.5f,
                FractureToughness = 0.2f,
                Granularity = 0.9f,
                Hardness = 0.05f
            };
        }
             
        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            if (wall == Wall.Top)
            {
                return 0;
            }
            else if (wall == Wall.Bottom)
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
