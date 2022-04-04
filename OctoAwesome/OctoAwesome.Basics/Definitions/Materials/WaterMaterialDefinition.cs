using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{

    public class WaterMaterialDefinition : IFluidMaterialDefinition
    {

        public int Hardness => 0;
        public int Density => 997;
        public string Name => "Water";
        public string Icon => string.Empty;
        public int Viscosity => 1008;
    }
}
