using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Caching;
using OctoAwesome.Definitions;
using OctoAwesome.Graph;

using System.Data.Common;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for light on blocks.
    /// </summary>
    public class LightBlockDefinition : BlockDefinition, INetworkBlock<int>
    {
        /// <inheritdoc />
        public override string Icon => "light_off";

        /// <inheritdoc />
        public override string DisplayName => "Licht";

        /// <inheritdoc />
        public override string[] Textures { get; } = { "light_off", "light_on" };

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }
        public string TransferType => "Energy";

        /// <summary>
        /// Initializes a new instance of the <see cref="CactusBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this cactus block definition.</param>
        public LightBlockDefinition(CactusMaterialDefinition material)
        {
            Material = material;
        }

        /// <inheritdoc/>
        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            var meta = manager.GetBlockMeta(x, y, z);
            if ((meta & 1) == 1)
                return 1;
            return 0;
        }

        public Node<int> CreateNode()
        {
            return new LightNode();
        }
    }


    internal partial class LightNode : Node<int>, ITargetNode<int>
    {
        public int Priority { get; } = 1;


        public void Execute(TargetInfo<int> targetInfo, IChunkColumn? chunk)
        {
            chunk.SetBlockMeta(Position, targetInfo.RepeatedTimes > 0 ? 1 : 0);
        }

        public TargetInfo<int> GetRequired()
        {
            return new TargetInfo<int>(this, 50, 1, 0);
        }
    }
}
