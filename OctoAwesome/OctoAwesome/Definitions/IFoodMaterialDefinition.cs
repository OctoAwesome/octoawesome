namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Declares an item as eadible with according properties
    /// </summary>
    public interface IFoodMaterialDefinition : IMaterialDefinition
    {
        /// <summary>
        /// Joule per Gramm of this food
        /// </summary>
        ushort Joule { get; }

        /// <summary>
        /// Determines if this food can be eaten, independent of the flavor
        /// </summary>
        bool Edible { get; }
    }
}
