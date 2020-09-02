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

        public int Granularity => throw new NotImplementedException();

        public int FractureToughness => throw new NotImplementedException();

        public string Name => "Wood";

        public string Icon => string.Empty;
    }
}
