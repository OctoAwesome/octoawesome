namespace OctoAwesome.Definitions
{
    public class FoodMaterialDefinition : MaterialDefinition, IFoodMaterialDefinition
    {
        /// <inheritdoc />
        public ushort Joule { get; }
        /// <inheritdoc />
        public bool Edible { get; }

    }
}
