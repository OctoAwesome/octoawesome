using OctoAwesome.Information;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Definitions.Items
{
    public class HandDefinition : IItemDefinition
    {
        public int VolumePerUnit => 0;

        public int StackLimit => 0;

        public string Name => nameof(Hand);

        public string Icon => "";

        public bool CanMineMaterial(IMaterialDefinition material) =>true;

        public void Hit(IItem item, IBlockDefinition blockDefinition, BlockHitInformation blockHit) { }
    }
}
