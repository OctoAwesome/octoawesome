using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IItem
    {
        List<IResource> Resources { get; }

        Coordinate? Position { get; set; }

        int Condition { get; set; }
    }
}
