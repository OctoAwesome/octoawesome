using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.PoC
{
    public class DependencyLeaf
    {
        public DependencyLeaf Parent { get; set; }
        public DependencyLeaf Child { get; set; }
        
        public List<DependencyItem> Items { get; set; }

    }
}
