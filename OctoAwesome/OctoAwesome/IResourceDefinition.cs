using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    interface IResourceDefinition
    {
        string Name { get; }

        Bitmap Icon { get; }

        IResource GetInstance();

        Type GetResourceType();
    }
}
