using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.PoC
{
    public class DependencyTree
    {
        private readonly List<DependencyLeaf> leaves;

        public DependencyTree(List<DependencyLeaf> leaves)
        {
            this.leaves = leaves;
        }

        //False and True
        public bool IsValid()
        {
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

            foreach (var leave in leaves)
            {
                foreach (var child in leave.Children)
                {
                    if (leaves.IndexOf(child) < leaves.IndexOf(leave))
                        return false;
                }
                foreach (var parent in leave.Parents)
                {
                    if (leaves.IndexOf(parent) > leaves.IndexOf(leave))
                        return false;
                }
            }

            return true;
        }
    }
}
