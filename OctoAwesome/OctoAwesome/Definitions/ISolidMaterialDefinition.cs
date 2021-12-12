namespace OctoAwesome.Definitions
{
    public interface ISolidMaterialDefinition : IMaterialDefinition
    {
        /// <summary>
        /// Granularity, efficiency of "materials" bucket for high values, pickaxe for low
        /// </summary>
        int Granularity { get; }

        /// <summary>
        /// Fracture toughness, How quickly does something break? Durability.
        /// </summary>
        int FractureToughness { get; }
    }
}
