﻿using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public class PickaxeDefinition : IItemDefinition
    {
        public Bitmap Icon
        {
            get
            {
                return (Bitmap)Bitmap.FromFile("./Assets/OctoAwesome.Basics/Items/pick_iron.png");
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

        public void Hit(IItem item, PhysicalProperties itemProperties)
        {
            // item.Condition--;
        }
    }
}
