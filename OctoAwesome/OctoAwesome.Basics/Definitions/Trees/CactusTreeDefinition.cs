using OctoAwesome.Basics.Definitions.Blocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Definitions.Trees
{
    public class CactusTreeDefinition : TreeDefinition
    {
        private ushort cactus, water;

        public override float MaxTemperature
        {
            get { return 45; }
        }

        public override float MinTemperature
        {
            get { return 32; }
        }

        public override int Order
        {
            get { return 20; }
        }

        public override int GetDensity(IPlanet planet, Index3 index)
        {
            return 2;
        }

        public override void Init(IDefinitionManager definitionManager)
        {
            cactus = definitionManager.GetBlockDefinitionIndex<CactusBlockDefinition>();
            water = definitionManager.GetBlockDefinitionIndex<WaterBlockDefinition>();
        }

        public override void PlantTree(IDefinitionManager definitionManager, IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
            ushort ground = builder.GetBlock(0, 0, -1);
            if (ground == water) return;

            Random rand = new Random(seed);
            int height = rand.Next(2, 4);

            for (int i = 0; i < height; i++)
                builder.SetBlock(0, 0, i, cactus);
        }
    }
}
