using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OctoAwesome.Runtime
{
    public static class BlockDefinitionManager
    {
        private static BlockDefinition[] _definitions;

        public static IEnumerable<BlockDefinition> GetBlockDefinitions()
        {
            if (_definitions == null)
            {
                _definitions = ExtensionManager.GetInstances<BlockDefinition>().ToArray();
            }

            return _definitions;
        }

        public static BlockDefinition GetForType(ushort type)
        {
            return _definitions[type & Blocks.TypeMask];
        }
    }
}
