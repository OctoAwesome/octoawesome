using OctoAwesome.Basics.Properties;
using OctoAwesome.Information;
using OctoAwesome.Definitions;
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


        public bool CanMineMaterial(IMaterialDefinition material)
        {
            if(material is ISolidMaterialDefinition solid)
            {
                return true;
            }

            return false;
        }

        public Pickaxe Create(IMaterialDefinition material)
        {
            return new Pickaxe(this, material);
        }
    }
}
