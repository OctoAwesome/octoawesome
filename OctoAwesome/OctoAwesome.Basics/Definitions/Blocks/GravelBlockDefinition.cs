using System;
using System.Drawing;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GravelBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Gravel; }
        }

        public override string Icon
        {
            get { return "gravel"; }
        }


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "gravel",
                };
            }
        }

        public override MaterialDefinition GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new MaterialDefinition()
            {
                Density = 2.5f,
                FractureToughness = 0.1f,
                Granularity = 0.1f,
                Hardness = 0.9f
            };
        }

        
    }
}
