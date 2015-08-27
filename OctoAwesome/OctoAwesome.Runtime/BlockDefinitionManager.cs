using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OctoAwesome.Runtime
{
    public static class BlockDefinitionManager
    {
        private static IBlockDefinition[] _definitions;

        public static IEnumerable<IBlockDefinition> GetBlockDefinitions()
        {
            if (_definitions == null)
            {
                _definitions = ExtensionManager.GetInstances<IBlockDefinition>().ToArray();
            }

            return _definitions;
        }

        public static IBlockDefinition GetForType(ushort type)
        {
            if (type == 0)
                return null;

            return _definitions[(type & Blocks.TypeMask) - 1];
        }
    }
}
