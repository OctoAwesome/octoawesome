using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class SandMaterialDefinition : ISolidMaterialDefinition
    {
        public int Hardness => 70;

        public int Density => 1600;

        public int Granularity => 90;

        public int FractureToughness => 0;

        public string Name => "Sand";

        public string Icon => string.Empty;
    }
}
