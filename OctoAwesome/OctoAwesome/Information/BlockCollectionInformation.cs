using OctoAwesome.Information;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Information
{
    public sealed class BlockCollectionInformation : BlockInteractionInformation
    {
        public decimal VolumeRemaining { get; set; }

        public override void Initialize(BlockInfo info, IBlockDefinition blockDefinition)
        {
            VolumeRemaining = blockDefinition.VolumePerUnit;
            base.Initialize(info, blockDefinition);
        }
    }
}
