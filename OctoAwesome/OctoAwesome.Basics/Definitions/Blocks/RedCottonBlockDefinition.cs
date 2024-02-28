using System;
using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Chunking;
using OctoAwesome.Definitions;
using OctoAwesome.Location;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for red cotton blocks.
    /// </summary>
    public sealed class RedCottonBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.RedCotton;

        /// <inheritdoc />
        public override string Icon => "cotton_red";

        /// <inheritdoc />
        public override string[] Textures =>
            new[]{
                "cotton_red_0",
                "cotton_red_1",
                "cotton_red_2",
                "cotton_red_3",
                "cotton_red_4",
                "cotton_red_5",
                "cotton_red_6",
                "cotton_red_7",
                "cotton_red_8",
                "cotton_red_9",
                "cotton_red_10",
                "cotton_red_11",
                "cotton_red_12",
                "cotton_red_13",
                "cotton_red_14",
                "cotton_red_15",
            };

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RedCottonBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this cotton block definition.</param>
        public RedCottonBlockDefinition(CottonMaterialDefinition material)
        {
            Material = material;
        }

        /// <inheritdoc />
        public override int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z)
        {
            int fullBlockTextureIndex = 15;
            var centerBlock = new Index3(x, y, z);
            var baseBlockIndex = manager.GetBlock(centerBlock);

            var (firstAxis, secondAxis) = wall switch
            {
                Wall.Top => (Index3.UnitX, Index3.UnitY),
                Wall.Bottom => (-Index3.UnitX, Index3.UnitY),
                Wall.Left => (Index3.UnitY, -Index3.UnitZ),
                Wall.Right => (-Index3.UnitY, -Index3.UnitZ),
                Wall.Front => (Index3.UnitX, -Index3.UnitZ),
                Wall.Back => (-Index3.UnitX, -Index3.UnitZ),
                _ => throw new ArgumentOutOfRangeException(nameof(wall), wall, null)
            };

            fullBlockTextureIndex = RemoveEdgesOnAxis(manager, centerBlock, firstAxis, 0, baseBlockIndex, fullBlockTextureIndex);
            fullBlockTextureIndex = RemoveEdgesOnAxis(manager, centerBlock, secondAxis, 1, baseBlockIndex, fullBlockTextureIndex);
            
            return fullBlockTextureIndex;
        }

        private static int RemoveEdgesOnAxis(ILocalChunkCache manager, Index3 centerBlock, Index3 axis, int sideOffset, ushort baseBlockIndex,
            int fullBlockTextureIndex)
        {
            for (int i = -1; i <= 1; i+=2)
            {
                var otherIndex = manager.GetBlock(centerBlock + axis * i);

                if (otherIndex == baseBlockIndex)
                {
                    var bitOffset = (i + 1) + sideOffset;
                    fullBlockTextureIndex &= ~(1 << bitOffset);
                }
            }

            return fullBlockTextureIndex;
        }
    }
}
