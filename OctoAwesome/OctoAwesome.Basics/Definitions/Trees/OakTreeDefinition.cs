using OctoAwesome.Basics.Definitions.Blocks;
using OctoAwesome.Definitions;
using System;

namespace OctoAwesome.Basics.Definitions.Trees
{
    /// <summary>
    /// Tree definition for planting oak trees.
    /// </summary>
    public class OakTreeDefinition : TreeDefinition
    {
        private ushort wood;
        private ushort leave;
        private ushort water;

        /// <inheritdoc />
        public override int Order => 10;

        /// <inheritdoc />
        public override float MaxTemperature => 27;

        /// <inheritdoc />
        public override float MinTemperature => -5;

        /// <inheritdoc />
        public override void Init(IDefinitionManager definitionManager)
        {
            wood = definitionManager.GetDefinitionIndex<WoodBlockDefinition>();
            leave = definitionManager.GetDefinitionIndex<LeavesBlockDefinition>();
            water = definitionManager.GetDefinitionIndex<WaterBlockDefinition>();
        }

        /// <inheritdoc />
        public override int GetDensity(IPlanet planet, Index3 index)
        {
            return 4;
        }

        /// <inheritdoc />
        public override void PlantTree(IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
            ushort ground = builder.GetBlock(0, 0, -1);
            if (ground == water) return;

            Random rand = new Random(seed);
            int height = rand.Next(6, 10);
            int radius = rand.Next(3, height - 2);

            builder.FillSphere(0, 0, height, radius, leave);

            var infos = new BlockInfo[height + 2];
            for (int i = 0; i < height + 2; i++)
            {
                infos[i] = (0, 0, i, wood);
            }
            builder.SetBlocks(false, infos);
        }
    }
}
