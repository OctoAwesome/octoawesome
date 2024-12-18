namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Represents a solid material definition.
    /// </summary>
    public class SolidMaterialDefinition : MaterialDefinition, ISolidMaterialDefinition
    {
        /// <summary>
        /// Gets the granularity of this material.
        /// </summary>
        public int Granularity { get; }
        /// <summary>
        /// Gets the fracture toughness of this material.
        /// </summary>
        public int FractureToughness { get; }
    }
}
