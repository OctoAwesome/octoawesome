using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Definitions
{
    public interface IFluidMaterialDefinition : IMaterialDefinition
    {
        /// <summary>
        /// Viscosity describes the tenacity of liquids
        /// This value is in µPa·s
        /// </summary>
        int Viscosity { get; }
    }
}
