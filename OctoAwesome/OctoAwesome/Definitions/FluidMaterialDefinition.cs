namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Represents a fluid material definition.
    /// </summary>
    public class FluidMaterialDefinition : MaterialDefinition, IFluidMaterialDefinition
    {
        /// <summary>
        /// Gets the visosity of the fluid.
        /// </summary>
        public int Viscosity { get; }
    }
}
