namespace OctoAwesome.Definitions
{
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

        //TODO move to item
        ///// <summary>
        ///// Can this be eaten without beeing poisened or not
        ///// </summary>
        //bool Poisenous { get; }
    }
}
