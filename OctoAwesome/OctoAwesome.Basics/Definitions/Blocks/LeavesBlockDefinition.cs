﻿using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Materials;
using OctoAwesome.Graphs;

namespace OctoAwesome.Basics.Definitions.Blocks
{
    /// <summary>
    /// Block definition for leave blocks.
    /// </summary>
    public sealed class LeavesBlockDefinition : BlockDefinition
    {
        /// <inheritdoc />
        public override string DisplayName => Languages.OctoBasics.Leaves;

        /// <inheritdoc />
        public override string Icon => "leaves";

        /// <inheritdoc />
        public override string[] Textures { get; init; } = ["leaves"];

        /// <inheritdoc />
        public override IMaterialDefinition Material { get; init; }
        /// <summary>
        /// Initializes a new instance of the <see cref="LeavesBlockDefinition"/> class.
        /// </summary>
        /// <param name="material">The material definition for this leave block definition.</param>
        public LeavesBlockDefinition(LeaveMaterialDefinition material)
        {
            Material = material;
        }

    }
}
