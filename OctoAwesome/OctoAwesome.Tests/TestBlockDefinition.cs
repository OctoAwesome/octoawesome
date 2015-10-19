using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Tests
{
    public sealed class TestBlockDefinition : BlockDefinition
    {
        public override Bitmap Icon
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string Name
        {
            get
            {
                return "Testblock";
            }
        }

        public override Bitmap[] Textures
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override PhysicalProperties GetProperties(IPlanetResourceManager manager, int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public override void Hit(IBlockDefinition block, PhysicalProperties itemProperties)
        {
            throw new NotImplementedException();
        }
    }
}
