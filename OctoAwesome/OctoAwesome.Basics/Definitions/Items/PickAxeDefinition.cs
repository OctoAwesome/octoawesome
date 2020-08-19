using OctoAwesome.Basics.Properties;
using OctoAwesome.Information;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class PickaxeDefinition : IItemDefinition
    {
        public string Icon
        {
            get
            {
                return "pick_iron";
            }
        }

        public string Name
        {
            get
            {
                return "Pickaxe";
            }
        }

        public int StackLimit
        {
            get
            {
                return 1;
            }
        }

        public float VolumePerUnit
        {
            get
            {
                return 10;
            }
        }

        int IInventoryableDefinition.VolumePerUnit => 1;

        public PhysicalProperties GetProperties(IItem item)
        {
            return new PhysicalProperties()
            {
                Density = 1f,
                FractureToughness = 1f,
                Granularity = 1f,
                Hardness = 1f
            };
        }

        public void Hit(IItem item, IBlockDefinition blockDefinition, BlockHitInformation blockHit)
        {
            // item.Condition--;
        }
    }
}
