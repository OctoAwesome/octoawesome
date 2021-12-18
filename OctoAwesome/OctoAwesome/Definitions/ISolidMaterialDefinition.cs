namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Represents the physical properties of solid a block/item/...
    /// </summary>
    public interface ISolidMaterialDefinition : IMaterialDefinition
    {
        /// <summary>
        /// Gets the granularity, efficiency of "materials" bucket for high values, pickaxe for low
        /// </summary>
        int Granularity { get; }

        /// <summary>
        /// Gets the fracture toughness, How quickly does something break? Durability.
        /// </summary>
        int FractureToughness { get; }
    }
}
