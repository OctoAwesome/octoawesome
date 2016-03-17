using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public class OakTreeDefinition : ITreeDefinition
    {
        private ushort wood;
        private ushort leave;

        public void Init(IDefinitionManager definitionManager)
        {
            wood = definitionManager.GetBlockDefinitionIndex<WoodBlockDefinition>();
            leave = definitionManager.GetBlockDefinitionIndex<LeavesBlockDefinition>();
        }

        public int GetDensity(IPlanet planet, Index3 index)
        {
            return 4;
        }

        public void PlantTree(IDefinitionManager definitionManager, IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
            Random rand = new Random(seed);
            int height = rand.Next(6, 16);
            int radius = rand.Next(3, height - 2);

            builder.FillSphere(0, 0, height, radius, leave);

            for (int i = 0; i < height + 2; i++)
            {
                builder.SetBlock(0, 0, 0 + i, wood);
            }
        }
    }
}
