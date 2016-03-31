using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public class OakTreeDefinition : TreeDefinition
    {
        private ushort wood;
        private ushort leave;
        private ushort water;

        public override int Order
        {
            get
            {
                return 10;
            }
        }

        public override void Init(IDefinitionManager definitionManager)
        {
            wood = definitionManager.GetBlockDefinitionIndex<WoodBlockDefinition>();
            leave = definitionManager.GetBlockDefinitionIndex<LeavesBlockDefinition>();
            water = definitionManager.GetBlockDefinitionIndex<WaterBlockDefinition>();
        }

        public override int GetDensity(IPlanet planet, Index3 index)
        {
            return 4;
        }

        public override void PlantTree(IDefinitionManager definitionManager, IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
            ushort ground = builder.GetBlock(0, 0, -1);
            if (ground == water) return;

            Random rand = new Random(seed);
            int height = rand.Next(6, 10);
            int radius = rand.Next(3, height - 2);

            builder.FillSphere(0, 0, height, radius, leave);

            for (int i = 0; i < height + 2; i++)
            {
                builder.SetBlock(0, 0, 0 + i, wood);
            }
        }
    }
}
