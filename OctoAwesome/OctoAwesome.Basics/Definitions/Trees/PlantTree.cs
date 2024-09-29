using OctoAwesome.Definitions;
using OctoAwesome.Information;
using OctoAwesome.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Definitions.Trees;
internal class PlantTree
{
    private readonly IDefinitionManager manager;
    private ushort water;
    private ushort cactus;
    private ushort woodBirch;
    private ushort wood;
    private ushort leave;
    private ushort leaveOrange;

    public PlantTree(IDefinitionManager manager)
    {
        manager.DefinitionsChanged += Init;
        this.manager = manager;
    }

    private void Init(object? sender, EventArgs e)
    {
        woodBirch = manager.GetDefinitionIndex<BlockDefinition>("base_block_wood_birch");
        wood = manager.GetDefinitionIndex<BlockDefinition>("base_block_wood");
        leave = manager.GetDefinitionIndex<BlockDefinition>("base_block_leaves");
        leaveOrange = manager.GetDefinitionIndex<BlockDefinition>("base_block_leaves_orange");
        water = manager.GetDefinitionIndex<BlockDefinition>("base_block_water");
        cactus = manager.GetDefinitionIndex<BlockDefinition>("base_block_cactus");
    }

    internal void Birch(IDefinition _, IPlanet __, Index3 ___, LocalBuilder builder, int seed) 
        => TreePlant(builder, seed, (3, 7, 3, 0), leave, woodBirch);

    internal void Spruce(IDefinition _, IPlanet __, Index3 ___, LocalBuilder builder, int seed) 
        => TreePlant(builder, seed, (3, 5, 3, 0), leaveOrange, wood);

    internal void Oak(IDefinition _, IPlanet __, Index3 ___, LocalBuilder builder, int seed) 
        => TreePlant(builder, seed, (6, 10, 3, -2), leave, wood);

    internal void Cactus(IDefinition _, IPlanet __, Index3 ___, LocalBuilder builder, int seed) 
        => TreePlant(builder, seed, (2, 4, 0, 0), 0, cactus);
    
    private void TreePlant(LocalBuilder builder, int seed, (int minHeight, int maxHeight, int minRadius, int maxRadiusOffset) randomParameters, ushort leaves, ushort wood)
    {
        ushort ground = builder.GetBlock(0, 0, -1);
        if (ground == water)
            return;
        Random rand = new Random(seed);
        int height = rand.Next(randomParameters.minHeight, randomParameters.maxHeight);
        if (randomParameters.minRadius > 0)
        {
            int radius = rand.Next(randomParameters.minRadius, height + randomParameters.maxRadiusOffset);

            builder.FillSphere(0, 0, height, radius, leave);
        }

        var infos = new BlockInfo[height + 2];
        for (int i = 0; i < height + 2; i++)
        {
            infos[i] = (0, 0, i, wood);
        }
        builder.SetBlocks(false, infos);
    }
}
