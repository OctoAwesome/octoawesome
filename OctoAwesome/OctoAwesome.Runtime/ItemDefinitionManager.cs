using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public static class ItemDefinitionManager
    {
        private static List<IItemDefinition> definitions;

        public static IEnumerable<IItemDefinition> GetBlockDefinitions()
        {
            if (definitions == null)
            {
                definitions = new List<IItemDefinition>();
                definitions.AddRange(ExtensionManager.GetInstances<IItemDefinition>());
            }

            return definitions;
        }
    }
}
