using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class LeaveMaterialDefinition : ISolidMaterialDefinition
    {
        public int Hardness => 1;

        public int Density => 200;

        public int Granularity => 40;

        public int FractureToughness => 0;

        public string Name => "Leave";

        public string Icon => string.Empty;
    }
}
