using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.PoC
{
    public class DependencyTree
    {
        private readonly Dictionary<Type, DependencyLeaf> leaves;

        public DependencyTree(Dictionary<Type, DependencyLeaf> leaves)
        {
            this.leaves = leaves;
        }

        //False and True
        public bool IsValid()
        {
            var leaves = this.leaves.Values.ToList();

            foreach (var leave in leaves)
            {
                foreach (var child in leave.Children)
                {
                    if (child.Position <= leave.Position)
                        return false;
                }
                foreach (var parent in leave.Parents)
                {
                    if (parent.Position >= leave.Position)
                        return false;
                }
            }

            return true;
        }
    }
}
