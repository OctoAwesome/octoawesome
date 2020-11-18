﻿using OctoAwesome.Basics.Definitions.Blocks;
using OctoAwesome.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Definitions.Trees
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

        public override float MaxTemperature
        {
            get
            {
                return 27; 
            }
        }

        public override float MinTemperature
        {
            get
            {
                return -5;
            }
        }

        public override void Init(IDefinitionManager definitionManager)
        {
            wood = definitionManager.GetDefinitionIndex<WoodBlockDefinition>();
            leave = definitionManager.GetDefinitionIndex<LeavesBlockDefinition>();
            water = definitionManager.GetDefinitionIndex<WaterBlockDefinition>();
        }

        public override int GetDensity(IPlanet planet, Index3 index)
        {
            return 4;
        }

        public override void PlantTree(IPlanet planet, Index3 index, LocalBuilder builder, int seed)
        {
            ushort ground = builder.GetBlock(0, 0, -1);
            if (ground == water) return;

            Random rand = new Random(seed);
            int height = rand.Next(6, 10);
            int radius = rand.Next(3, height - 2);

            builder.FillSphere(0, 0, height, radius, leave);

            var infos = new BlockInfo[height +2];
            for (int i = 0; i < height + 2; i++)
            {
                infos[i] = (0, 0, i, wood);
            }
            builder.SetBlocks(false, infos);
        }
    }
}
