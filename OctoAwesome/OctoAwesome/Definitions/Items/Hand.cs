namespace OctoAwesome.Definitions.Items
{
    /// <summary>
    /// Item placeholder for the hand(no item selected).
    /// </summary>
    public class Hand : Item
    {
        public static readonly Hand Instance = new Hand(new HandDefinition());

        /// <summary>
        /// Initializes a new instance of the <see cref="Hand"/> class.
        /// </summary>
        /// <param name="handDefinition">The item definition for this item.</param>
        public Hand(HandDefinition handDefinition) : base(handDefinition, null)
        {

        }

        /// <inheritdoc />
        public override int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining, int volumePerHit)
        {
            if (material is ISolidMaterialDefinition solidMaterial)
            {
                if (solidMaterial.Granularity > 1)
                    return volumePerHit / 3;
            }
            if (material is IGasMaterialDefinition || material is IFluidMaterialDefinition)
                return 0;

            return volumePerHit - material.Hardness / 2;
        }
    }
}
