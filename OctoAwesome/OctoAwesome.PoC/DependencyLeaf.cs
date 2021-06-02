using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.PoC
{
    public class DependencyLeaf 
    {
        public DependencyItem Item { get; }
        /// <summary>
        /// This loads before these items
        /// </summary>
        public List<DependencyLeaf> Children { get; }

        /// <summary>
        /// This loads after these items
        /// </summary>
        public List<DependencyLeaf> Parents { get; }

        public int Position { get; set; }

        public DependencyLeaf(DependencyItem item, List<DependencyLeaf> children, List<DependencyLeaf> parents, int position)
        {
            Item = item;
            Children = children;
            Parents = parents;
            Position = position;
        }       

        public override string ToString()
            => $"{Item.Name} loads Before: {string.Join(", ", Children.Select(x => x.Item.Name))} and loads After: {string.Join(", ", Parents.Select(x => x.Item.Name))} (BeforeCount: {Children.Count}, AfterCount: {Parents.Count})";

    }
}
