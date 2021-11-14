using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Items
{
    class ShovelDefinition : IItemDefinition
    {
        public string Name { get; }
        public string Icon { get; }

        public ShovelDefinition()
        {
            Name = "Shovel";
            Icon = "shovel_iron";
        }

        public bool CanMineMaterial(IMaterialDefinition material)
        {
            if (material is ISolidMaterialDefinition solid)
            {
                return true;
            }

            return false;
        }

        public Item Create(IMaterialDefinition material)
            => new Shovel(this, material);
    }
}
