using OctoAwesome.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public static class BlockDefinitionManager
    {
        private static List<IBlockDefinition> definitions;

        public static IEnumerable<IBlockDefinition> GetBlockDefinitions()
        {
            if (definitions == null)
            {
                definitions = new List<IBlockDefinition>();
                definitions.Add(new GrassBlockDefinition());
                definitions.Add(new GroundBlockDefinition());
                definitions.Add(new SandBlockDefinition());
                definitions.Add(new StoneBlockDefinition());
                definitions.Add(new WaterBlockDefinition());
            }

            return definitions;
        }
    }
}
