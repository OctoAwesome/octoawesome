using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public static class ResourceDefinitionManager
    {
        private static List<IResourceDefinition> definitions;

        public static IEnumerable<IResourceDefinition> GetBlockDefinitions()
        {
            if (definitions == null)
            {
                definitions = new List<IResourceDefinition>();
                definitions.AddRange(ExtensionManager.GetInstances<IResourceDefinition>());
            }

            return definitions;
        }
    }
}
