using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    internal class CellCache
    {
        public CellType CellType { get; set; }

        public float VelocityFactor { get; set; }

        public bool CanGoto { get; set; }
    }
}
