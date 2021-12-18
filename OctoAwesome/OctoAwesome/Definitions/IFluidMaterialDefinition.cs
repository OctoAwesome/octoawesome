namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Represents the physical properties of fluid a block/item/...
    /// </summary>
    public interface IFluidMaterialDefinition : IMaterialDefinition
    {
        /// <summary>
        /// Gets the viscosity describing the tenacity of liquids.
        /// This value is in µPa·s.
        /// </summary>
        int Viscosity { get; }
    }
}
