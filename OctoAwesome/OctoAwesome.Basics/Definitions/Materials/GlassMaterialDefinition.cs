using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{

    public class GlassMaterialDefinition : ISolidMaterialDefinition
    {

        public int Hardness => 55;
        public int Density => 2500;
        public int Granularity => 1;
        public int FractureToughness => 50;
        public string Name => "Glass";
        public string Icon => string.Empty;
    }
}
