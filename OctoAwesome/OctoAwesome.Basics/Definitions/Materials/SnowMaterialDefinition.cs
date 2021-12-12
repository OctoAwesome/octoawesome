using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{

    public class SnowMaterialDefinition : ISolidMaterialDefinition
    {

        public int Hardness => 1;
        public int Density => 250;
        public int Granularity => 50;
        public int FractureToughness => 5;
        public string Name => "Snow";
        public string Icon => string.Empty;
    }
}
