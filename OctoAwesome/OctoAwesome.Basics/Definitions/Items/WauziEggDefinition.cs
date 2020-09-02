using OctoAwesome.Information;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Items
{
    public sealed class WauziEggDefinition : IItemDefinition
    {
        public string Icon
        {
            get
            {
                return "wauziegg";
            }
        }

        public string Name
        {
            get
            {
                return "Wauzi Egg";
            }
        }

        public int StackLimit
        {
            get
            {
                return 1000;
            }
        }

        public float VolumePerUnit
        {
            get
            {
                return 1;
            }
        }

        int IInventoryableDefinition.VolumePerUnit => 1;

        public void Hit(IItem item, IBlockDefinition blockDefinition, BlockHitInformation blockHit) => throw new System.NotImplementedException();
    }
}
