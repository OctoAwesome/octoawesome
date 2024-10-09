
using engenious.Graphics;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Information;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Item placeholder for the hand(no item selected).
    /// </summary>
    public class Hand : Item
    {
        /// <summary>
        /// Gets the singleton hand instance
        /// </summary>
        public static readonly Hand Instance = new Hand(new HandDefinition());

        /// <summary>
        /// Initializes a new instance of the <see cref="Hand"/> class.
        /// </summary>
        /// <param name="handDefinition">The item definition for this item.</param>
        private Hand(HandDefinition handDefinition) : base(handDefinition, new MaterialDefinition())
        {

        }

        /// <inheritdoc />
        public override int Hit(IMaterialDefinition material, IBlockInteraction blockInfo, decimal volumeRemaining, int volumePerHit)
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
