using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class GlassMaterialDefinition : ISolidMaterialDefinition
    {
        public int Hardness => 55;

        public int Density => 2500;

        public int Granularity => throw new NotImplementedException();

        public int FractureToughness => throw new NotImplementedException();

        public string Name => "Glass";

        public string Icon => string.Empty;
    }
}
