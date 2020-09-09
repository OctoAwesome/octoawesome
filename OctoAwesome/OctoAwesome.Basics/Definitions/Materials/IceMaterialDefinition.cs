using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class IceMaterialDefinition : ISolidMaterialDefinition
    {
        public int Granularity => 1;

        public int FractureToughness => 20;

        public int Hardness => 15;

        public int Density => 934;

        public string Name => "Ice";

        public string Icon => "";
    }
}
