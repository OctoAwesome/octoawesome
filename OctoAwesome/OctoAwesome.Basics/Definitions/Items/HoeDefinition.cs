using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Items
{
    class HoeDefinition : IItemDefinition
    {
        public string Name { get; }
        public string Icon { get; }

        public HoeDefinition()
        {
            Name = "Hoe";
            Icon = "hoe_iron";
        }

        public bool CanMineMaterial(IMaterialDefinition material)
        {
            return false;
        }

        public Item Create(IMaterialDefinition material)
            => new Hoe(this, material);
    }
}
