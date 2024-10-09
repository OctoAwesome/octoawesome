using engenious.Graphics;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Information;
using OctoAwesome.Services;

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
        public Bucket(IDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {
            MaxQuantity = 1250;
        }

        /// <inheritdoc />
        public void AddFluid(int quantity, IBlockDefinition fluidBlock)
        {
            if (!DefinitionActionService.Function("CanMineMaterial", Definition, false, fluidBlock.Material))
                return;

            if (Quantity < 125)
                Quantity += quantity;

            FluidBlock = fluidBlock;
        }

        /// <inheritdoc />
        public override int Hit(IMaterialDefinition material, IBlockInteraction hitInfo, decimal volumeRemaining, int volumePerHit)
        {
            if (!DefinitionActionService.Function("CanMineMaterial", Definition, false, material))
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

        /// <inheritdoc />
        public override int Interact(IMaterialDefinition material, IBlockInteraction hitInfo, decimal volumeRemaining)
        {
            if (Quantity > 125 && FluidBlock is not null)
            {
                BlockInteractionService.CalculatePositionAndRotation(hitInfo, out var facingDirection, out _);
            }

            return base.Interact(material, hitInfo, volumeRemaining);
        }
    }
}
