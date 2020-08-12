using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Tests
{
    public sealed class TestBlockDefinition : BlockDefinition
    {
        public override string Icon
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

        public override string[] Textures
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z)
        {
            throw new NotImplementedException();
        }

        public override BlockInteractionInformation Hit(BlockInteractionInformation interactionInformation, IItem item)
        {
            throw new NotImplementedException();
        }
    }
}
