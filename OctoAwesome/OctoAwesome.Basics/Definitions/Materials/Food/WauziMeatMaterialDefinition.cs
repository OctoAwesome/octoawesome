﻿using OctoAwesome.Definitions;

using System.Linq;

namespace OctoAwesome.Basics.Definitions.Materials.Food
{

    /// <inheritdoc/>
    public class WauziMeatMaterialDefinition : IFoodMaterialDefinition
    {
        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();
        /// <inheritdoc/>
        public string DisplayName => "Wauzi Meat";

        /// <inheritdoc/>
        public string Icon => string.Empty;
        /// <inheritdoc/>
        public ushort Joule { get; } = 10960;
        /// <inheritdoc/>
        public bool Edible { get; } = true;
        /// <inheritdoc/>
        public int Hardness { get; } = 1;
        /// <inheritdoc/>
        public int Density { get; } = 1040;
    }
}
