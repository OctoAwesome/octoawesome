using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Items;
internal class CanMineMaterial
{
    public bool CanMineMaterialSolid(bool lastResult, IDefinition def, IMaterialDefinition materialDefinition)
        => materialDefinition is ISolidMaterialDefinition;
    public bool CanMineMaterialFluid(bool lastResult, IDefinition def, IMaterialDefinition materialDefinition)
        => materialDefinition is IFluidMaterialDefinition;

    public bool CanMineEverything(bool lastResult, IDefinition def, IMaterialDefinition materialDefinition)
        => true;

    public bool CanMineNothing(bool lastResult, IDefinition def, IMaterialDefinition materialDefinition)
        => false;


}
