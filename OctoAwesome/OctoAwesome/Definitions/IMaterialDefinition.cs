namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Represents the physical properties of a block/item/...
    /// </summary>
    public interface IMaterialDefinition : IDefinition
    {
        /// <summary>
        /// Gets the material hardness, depicting which materials can be mined.
        /// </summary>
        int Hardness { get; }

        /// <summary>
        /// Gets the density in g/dm^3, How much needed (volume calculation) for crafting or hit result etc...
        /// </summary>
        int Density { get; }

    }
}
