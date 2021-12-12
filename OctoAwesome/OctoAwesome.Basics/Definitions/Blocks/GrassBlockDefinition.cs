using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    public sealed class GrassBlockDefinition : BlockDefinition
    {
        public override string Name => Languages.OctoBasics.Grass;
        
        public override string Icon => "grass_top";
        
        public override string[] Textures { get; } = { "grass_top",
                                                         "dirt",
                                                         "dirt_grass" };
        
        public override IMaterialDefinition Material { get; }

        public GrassBlockDefinition(DirtMaterialDefinition material)
        {
            Material = material;
        }
        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            return wall switch
            {
                Wall.Top => 0,
                Wall.Bottom => 1,
                _ => 2
            };
        }
    }
}
