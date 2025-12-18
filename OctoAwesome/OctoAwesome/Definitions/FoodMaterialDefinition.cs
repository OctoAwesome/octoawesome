namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Represents a food material definition.
    /// </summary>
    public class FoodMaterialDefinition : MaterialDefinition, IFoodMaterialDefinition
    {
        /// <inheritdoc />
        public ushort Joule { get; }
        /// <inheritdoc />
        public bool Edible { get; }

    }
}
