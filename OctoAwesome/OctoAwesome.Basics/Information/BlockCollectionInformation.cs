using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Information
{
    public sealed class BlockCollectionInformation : BlockInteractionInformation
    {
        public decimal VolumeRemaining { get; set; }

        public void SetBlock(BlockInfo info, IBlockDefinition definition)
        {
            VolumeRemaining = definition.VolumePerUnit;
        }
    }
}
