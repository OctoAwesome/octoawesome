using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class DirtMaterialDefinition : ISolidMaterialDefinition
    {
        public int Hardness => 10;

        public int Density => 1400;

        public int Granularity => 1;

        public int FractureToughness => 50;

        public string Name => "Dirt";

        public string Icon => string.Empty;
    }
}
