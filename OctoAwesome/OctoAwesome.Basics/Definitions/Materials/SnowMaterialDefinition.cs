using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class SnowMaterialDefinition : ISolidMaterialDefinition
    {
        public int Hardness => 1;

        public int Density => 250;

        public int Granularity => 50;

        public int FractureToughness => 5;

        public string Name => "Snow";

        public string Icon => string.Empty;
    }
}
