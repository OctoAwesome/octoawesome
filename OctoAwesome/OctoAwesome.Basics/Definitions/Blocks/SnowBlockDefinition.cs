using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for snow blocks.
    /// </summary>
    public class SnowBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string Name => Languages.OctoBasics.Snow;

        /// <inheritdoc />
        public override string Icon => "snow";

        /// <inheritdoc />
        public override string[] Textures { get; } = { "snow", "dirt", "dirt_snow", };

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnowBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this snow block definition.</param>
        public SnowBlockDefinition(SnowMaterialDefinition material)
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
