using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class AxeDefinition : IItemDefinition
    {    

        public string Name { get; }
        public string Icon { get; }

        public AxeDefinition()
        {
            Name = "Axe";
            Icon = "axe_iron";
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
            => new Axe(this, material);
    }
}
