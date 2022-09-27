using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.OctoMath;

using System;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Pickaxe item for inventories.
    /// </summary>
    public class Pickaxe : Item
    {
        private static readonly Polynomial polynomial;

        static Pickaxe()
        {
            polynomial = new Polynomial(150, 0, -1f / 400f);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Pickaxe"/> class.
        /// </summary>
        /// <param name="pickaxeDefinition">The pickaxe item definition.</param>
        /// <param name="materialDefinition">The material definition the pickaxe is made out of.</param>
        public Pickaxe(PickaxeDefinition pickaxeDefinition, IMaterialDefinition materialDefinition)
            : base(pickaxeDefinition, materialDefinition)
        {

        }

        /// <inheritdoc />
        public override int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining, int volumePerHit)
        {
            //⁅((−𝑥^2)/400)+150⁆
            var baseEfficiency = base.Hit(material, blockInfo, volumeRemaining, volumePerHit);

            if (material is ISolidMaterialDefinition solid && baseEfficiency > 0)
            {
                var fractureEfficiency = polynomial.Evaluate(solid.FractureToughness);

                return (int)(baseEfficiency * (fractureEfficiency) / 100);
            }

            return baseEfficiency;
        }
    }
}
