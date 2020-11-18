using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class CottonMaterialDefinition : ISolidMaterialDefinition
    {
        public int Hardness => 4;

        public int Density => 132;

        public int Granularity => 10;

        public int FractureToughness => 600;

        public string Name => "Cotton";

        public string Icon => string.Empty;
    }
}
