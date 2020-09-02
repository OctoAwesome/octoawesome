using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class CactusMaterialDefinition : ISolidMaterialDefinition
    {
        public int Hardness => 25;

        public int Density => 850;

        public int Granularity => throw new NotImplementedException();

        public int FractureToughness => throw new NotImplementedException();

        public string Name => "Cactus";

        public string Icon => string.Empty;
    }
}
