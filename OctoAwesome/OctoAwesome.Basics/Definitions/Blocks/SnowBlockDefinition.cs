using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Chunking;
using OctoAwesome.Definitions;

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

        public override string[] Textures { get; } = new[] {"snow","dirt","dirt_snow",};

        public override IMaterialDefinition Material { get; }

        public SnowBlockDefinition(SnowMaterialDefinition material)
        {
            Material = material;
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
