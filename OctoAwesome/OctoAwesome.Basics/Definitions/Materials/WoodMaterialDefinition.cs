using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class WoodMaterialDefinition : ISolidMaterialDefinition
    {
        public int Hardness => 35;

        public int Density => 680;

        public int Granularity => 1;

        public int FractureToughness => 200;

        public string Name => "Wood";

        public string Icon => string.Empty;
    }
}
