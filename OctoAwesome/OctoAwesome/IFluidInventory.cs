using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public interface IFluidInventory
    {
        int MaxQuantity { get; }
        int Quantity { get; }
        IBlockDefinition FluidBlock { get; }

        void AddFluid(int quantity, IBlockDefinition fluid);
    }
}
