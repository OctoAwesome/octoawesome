using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model
{
    [Flags]
    public enum Axis
    {
        None = 0,
        X = 1,
        Y = 2,
        Z = 4
    }
}
