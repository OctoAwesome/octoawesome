using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.PoC
{
    public class DependencyTree
    {
        private readonly List<DependencyLeaf> roots;

        public DependencyTree()
        {
            roots = new();
        }

        public void Add(DependencyLeaf leaf)
        {

        }

        public DependencyLeaf Get(string name)
        {
            return default;
        }

        public void Remove(DependencyLeaf leaf)
        {

        }

    }
}
