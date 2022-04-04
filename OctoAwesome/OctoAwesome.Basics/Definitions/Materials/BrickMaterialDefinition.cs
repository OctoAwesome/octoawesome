using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{

    public class BrickMaterialDefinition : ISolidMaterialDefinition
    {

        public int Hardness => 45;
        public int Density => 1800;
        public int Granularity => 1;
        public int FractureToughness => 2;
        public string Name => "Brick";
        public string Icon => string.Empty;
    }
}
