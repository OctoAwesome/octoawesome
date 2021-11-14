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
        public int VolumePerUnit { get; }

        public int StackLimit { get; }

        public string Name { get; }

        public string Icon { get; }

        private readonly Hand hand;

        public HandDefinition()
        {
            VolumePerUnit = 0;
            StackLimit = 0;
            Name = nameof(Hand);
            Icon = "";
            hand = new Hand(this);
        }

        public bool CanMineMaterial(IMaterialDefinition material) 
            => true;

        public Item Create(IMaterialDefinition material)
            => hand;

        public void Hit(IItem item, IBlockDefinition blockDefinition, BlockHitInformation blockHit) { }
    }
}
