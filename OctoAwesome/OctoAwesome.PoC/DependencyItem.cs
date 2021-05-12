using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.PoC
{
    public class DependencyItem
    {
        public Type Type { get; set; }
        public string Name { get; set; }
        public List<string> AfterDependencyItems { get; set; }
        public List<string> BeforeDependencyItems { get; set; }
        public DependencyItem()
        {
        }

        public DependencyItem(Type type, string name, List<string> afterDependencyItems, List<string> beforeDependencyItems)
        {
            Type = type;
            Name = name;
            AfterDependencyItems = afterDependencyItems;
            BeforeDependencyItems = beforeDependencyItems;
        }
    }
}
