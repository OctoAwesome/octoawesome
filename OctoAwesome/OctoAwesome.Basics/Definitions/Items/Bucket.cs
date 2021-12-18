using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Bucket item for inventories.
    /// </summary>
    public class Bucket : Item, IFluidInventory
    {
        /// <inheritdoc />
        public int Quantity { get; private set; }

        /// <inheritdoc />
        public IBlockDefinition? FluidBlock { get; private set; }

        /// <inheritdoc />
        public int MaxQuantity { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Bucket"/> class.
        /// </summary>
        /// <param name="definition">The bucket item definition.</param>
        /// <param name="materialDefinition">The material definition the bucket is made out of.</param>
        public Bucket(BucketDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {
            MaxQuantity = 125;
        }

        /// <inheritdoc />
        public void AddFluid(int quantity, IBlockDefinition fluidBlock)
        {
            if (!Definition.CanMineMaterial(fluidBlock.Material))
                return;

            Quantity += quantity;
            FluidBlock = fluidBlock;
        }

        /// <inheritdoc />
        public override int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining, int volumePerHit)
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


            return base.Hit(material, blockInfo, volumeRemaining, volumePerHit);
        }
    }
}
