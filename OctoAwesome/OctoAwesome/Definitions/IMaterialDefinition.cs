namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Represents the physical properties of a block/item/...
    /// </summary>
    public interface IMaterialDefinition : IDefinition
    {
        /// <summary>
        /// Hardness, which materials can be mined
        /// </summary>
        int Hardness { get; }

        /// <summary>
        /// Density in kg/m^3, How much needed (volume calculation) for crafting or hit result etc...
        /// </summary>
        int Density { get; }

    }
}
