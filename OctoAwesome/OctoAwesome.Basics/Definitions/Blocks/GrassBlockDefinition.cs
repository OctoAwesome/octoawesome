using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Chunking;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for grass blocks.
    /// </summary>
    public sealed class GrassBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.Grass;

        /// <inheritdoc />
        public override string Icon => "grass_top";

        /// <inheritdoc />
        public override string[] Textures { get; } = { "grass_top",
                                                         "dirt",
                                                         "dirt_grass" };

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GrassBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this grass block definition.</param>
        public GrassBlockDefinition(DirtMaterialDefinition material)
        {
            Material = material;
        }

        /// <inheritdoc />
        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
            => wall switch
            {
                Wall.Top => 0,
                Wall.Bottom => 1,
                _ => 2
            };
    }
}
