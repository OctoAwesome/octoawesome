using OctoAwesome.Information;

namespace OctoAwesome.Definitions.Items
{
    public class Hand : Item
    {
        public Hand(HandDefinition handDefinition) : base(handDefinition, null)
        {

        }

        public override int Hit(IMaterialDefinition material, IBlockInteraction hitInfo, decimal volumeRemaining, int volumePerHit)
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
