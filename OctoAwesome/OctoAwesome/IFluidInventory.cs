using OctoAwesome.Definitions;

namespace OctoAwesome
{

    public interface IFluidInventory
    {
        int MaxQuantity { get; }
        int Quantity { get; }
        IBlockDefinition? FluidBlock { get; }

        void AddFluid(int quantity, IBlockDefinition fluid);
    }
}
