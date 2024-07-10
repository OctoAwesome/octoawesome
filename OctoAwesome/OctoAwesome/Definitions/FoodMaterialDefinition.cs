namespace OctoAwesome.Definitions
{
    public class FoodMaterialDefinition : MaterialDefinition, IFoodMaterialDefinition
    {
        public ushort Joule { get; }
        public bool Edible { get; }
    }
}
