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

        public int Granularity => throw new NotImplementedException();

        public int FractureToughness => throw new NotImplementedException();

        public string Name => "Sand";

        public string Icon => string.Empty;
    }
}
