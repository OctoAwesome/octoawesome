using System;
using System.Drawing;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GreenCottonBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.GreenCotton; }
        }

        public override string Icon
        {
            get { return "cotton_green"; }
        }


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "cotton_green"
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

      
    }
}
