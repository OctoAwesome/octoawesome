using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Definitions
{
    public interface ISolidMaterialDefinition : IMaterialDefinition
    {
        /// <summary>
        /// Granularität, Effiktivität von "Materialien" Schaufel für hohe Werte, Pickaxe für niedrige
        /// </summary>
        int Granularity { get; }
    }
}
