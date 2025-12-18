namespace OctoAwesome.Definitions
{
    public class SolidMaterialDefinition : MaterialDefinition, ISolidMaterialDefinition
    {
        public int Granularity { get; }
        public int FractureToughness { get; }
    }
}
