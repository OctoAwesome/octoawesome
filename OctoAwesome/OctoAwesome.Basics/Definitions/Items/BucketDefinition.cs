using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Items
{
    class BucketDefinition : IItemDefinition
    {
        public string Name { get; }
        public string Icon { get; }

        public BucketDefinition()
        {
            Name = "Bucket";
            Icon = "bucket";
        }

        public bool CanMineMaterial(IMaterialDefinition material)
        {
            if (material is IFluidMaterialDefinition fluid)
            {
                return true;
            }

            return false;
        }

        public Item Create(IMaterialDefinition material)
            => new Bucket(this, material);
    }
}
