using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{

    public class StoneMaterialDefinition : ISolidMaterialDefinition
    {

        public int Hardness => 60;
        public int Density => 2700;
        public int Granularity => 1;
        public int FractureToughness => 4;
        public string Name => "Stone";
        public string Icon => string.Empty;
    }
}
