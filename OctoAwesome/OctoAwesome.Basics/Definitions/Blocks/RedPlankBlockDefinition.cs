using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class RedPlankBlockDefinition : BlockDefinition
    {
        public override string Name
        {
            get { return Languages.OctoBasics.RedPlank; }
        }

        public override string Icon
        {
            get { return "planks"; }
        }

        public override bool HasMetaData { get { return true; } }

        public override Bitmap[] Textures
        {
            get
            {
                return new[] {
                (Bitmap)Bitmap.FromFile("./Assets/OctoAwesome.Basics/Blocks/planks.png")};
            }
        }

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            return new PhysicalProperties()
            {
                Density = 0.87f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties)
        {
            throw new NotImplementedException();
        }

    }
}
