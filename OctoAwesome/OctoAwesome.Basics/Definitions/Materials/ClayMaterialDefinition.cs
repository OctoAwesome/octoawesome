
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{

    public class ClayMaterialDefinition : ISolidMaterialDefinition
    {

        public int Hardness => 3;
        public int Density => 2000;
        public int Granularity => 25;
        public int FractureToughness => 60;
        public string Name => "Clay";
        public string Icon => string.Empty;
    }
}
