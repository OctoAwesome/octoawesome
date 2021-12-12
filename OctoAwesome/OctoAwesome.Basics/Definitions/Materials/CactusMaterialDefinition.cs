using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{

    public class CactusMaterialDefinition : ISolidMaterialDefinition
    {

        public int Hardness => 25;
        public int Density => 850;
        public int Granularity => 1;
        public int FractureToughness => 300;
        public string Name => "Cactus";
        public string Icon => string.Empty;
    }
}
