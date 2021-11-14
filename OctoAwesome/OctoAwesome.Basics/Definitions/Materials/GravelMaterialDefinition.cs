using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class GravelMaterialDefinition : ISolidMaterialDefinition
    {
        public int Hardness => 60;

        public int Density => 1440;

        public int Granularity => 70;

        public int FractureToughness => 0;

        public string Name => "Gravel";

        public string Icon => string.Empty;
    }
}
