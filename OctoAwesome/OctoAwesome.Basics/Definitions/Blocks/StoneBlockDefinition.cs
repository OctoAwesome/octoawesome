using OctoAwesome.Information;
using System;
using System.Drawing;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class StoneBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.Stone; }
        }

        public override string Icon
        {
            get { return "stone"; }
        }


        public override string[] Textures
        {
            get
            {
                return new[] {
                    "stone",
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
