using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Information;
using OctoAwesome.Services;

namespace OctoAwesome.Basics.Definitions.Items
{
    class Bucket : Item, IFluidInventory
    {
        public int Quantity { get; private set; }
        public IBlockDefinition FluidBlock { get; private set; }
        public int MaxQuantity { get; }

        public Bucket(BucketDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {
            MaxQuantity = 1250;
        }

        public void AddFluid(int quantity, IBlockDefinition fluidBlock)
        {
            if (!Definition.CanMineMaterial(fluidBlock.Material))
                return;

            if (Quantity < 125)
                Quantity += quantity;

            FluidBlock = fluidBlock;
        }

        public override int Hit(IMaterialDefinition material, IBlockInteraction hitInfo, decimal volumeRemaining, int volumePerHit)
        {
            if (!Definition.CanMineMaterial(material))
                return 0;

            if (material is IFluidMaterialDefinition fluid)
            {
                if (!(FluidBlock is null) && fluid != FluidBlock.Material)
                    return 0;

                if (Quantity + volumePerHit >= MaxQuantity)
                    return MaxQuantity - Quantity;

                return volumePerHit;
            }


            return base.Hit(material, hitInfo, volumeRemaining, volumePerHit);
        }

        public override int Apply(IMaterialDefinition material, IBlockInteraction hitInfo, decimal volumeRemaining)
        {
            if (Quantity > 125 && FluidBlock is not null)
            {
                BlockInteractionService.CalculatePositionAndRotation(hitInfo, out var index3, out _);
            }

            return base.Apply(material, hitInfo, volumeRemaining);
        }
    }
}
