using OctoAwesome.Common;
using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class WaterMaterialDefinition : IFluidMaterialDefinition
    {
        public int Hardness => 0;

        public int Density => 997;

        public string Name => "Water";

        public string Icon => string.Empty;

        public int Viscosity => 1008;
    }
}
