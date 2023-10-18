using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Definitions;
using OctoAwesome.Graph;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for light on blocks.
    /// </summary>
    public class LightOffBlockDefinition : BlockDefinition, INetworkBlock
    {
        /// <inheritdoc />
        public override string Icon => "light_off";

        /// <inheritdoc />
        public override string DisplayName => "Licht aus";

        /// <inheritdoc />
        public override string[] Textures { get; } = { "light_off" };

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }
        public NetworkBlockType BlockType => NetworkBlockType.Target;
        public string TransferType => "Signal";

        /// <summary>
        /// Initializes a new instance of the <see cref="CactusBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this cactus block definition.</param>
        public LightOffBlockDefinition(CactusMaterialDefinition material)
        {
            Material = material;
        }

    }
}
