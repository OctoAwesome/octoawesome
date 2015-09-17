using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IItemDefinition
    {
        string Name { get; }

        Bitmap Icon { get; }

        int StackLimit { get; }


        //PhysicalProperties GetProperties(IItem item);

        //void Hit(IItem item, PhysicalProperties itemProperties);
    }
}
