
using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class ClayMaterialDefinition : ISolidMaterialDefinition
    {
        public int Hardness => 3;

        public int Density => 2000;

        public int Granularity => throw new NotImplementedException();

        public int FractureToughness => throw new NotImplementedException();

        public string Name => "Clay";

        public string Icon => string.Empty;
    }
}
