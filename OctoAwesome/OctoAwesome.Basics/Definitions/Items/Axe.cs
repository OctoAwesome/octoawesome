using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Information;
using OctoAwesome.OctoMath;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Axe item for inventories.
    /// </summary>
    public class Axe : Item
    {

        private static readonly Polynomial polynomial;

        static Axe()
        {
            polynomial = new Polynomial(0, 3f / 8f, 1f / 800f, -1f / 320000f);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Axe"/> class.
        /// </summary>
        /// <remarks>This is only to be used for deserialization.</remarks>
        public Axe()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Axe"/> class.
        /// </summary>
        /// <param name="definition">The axe item definition.</param>
        /// <param name="materialDefinition">The material definition the axe is made out of.</param>
        public Axe(AxeDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {

        }

        /// <inheritdoc />
        public override int Hit(IMaterialDefinition material, IBlockInteraction hitInfo, decimal volumeRemaining, int volumePerHit)
        {
            //⁅𝑥^2/800+3𝑥/8+(−𝑥^3)/320000⁆
            var baseEfficiency = base.Hit(material, hitInfo, volumeRemaining, volumePerHit);
            //typeof(Item).GUID
            if (material is ISolidMaterialDefinition solid && baseEfficiency > 0)
            {
                var fractureEfficiency = polynomial.Evaluate(solid.FractureToughness);
                return (int)(baseEfficiency * (fractureEfficiency) / 100);
            }

            return baseEfficiency;
        }
    }
}
