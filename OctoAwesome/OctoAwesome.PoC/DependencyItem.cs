using System;
using System.Collections.Generic;

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
