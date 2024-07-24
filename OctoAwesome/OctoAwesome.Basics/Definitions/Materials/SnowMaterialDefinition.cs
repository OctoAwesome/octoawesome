﻿using OctoAwesome.Definitions;

using System.Linq;

namespace OctoAwesome.Basics.Definitions.Materials
{
    /// <summary>
    /// Material definition for snow.
    /// </summary>
    public class SnowMaterialDefinition : ISolidMaterialDefinition
    {
        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();
        /// <inheritdoc />
        public int Hardness => 1;

        /// <inheritdoc />
        public int Density => 250;

        /// <inheritdoc />
        public int Granularity => 50;

        /// <inheritdoc />
        public int FractureToughness => 5;

        /// <inheritdoc />
        public string DisplayName => "Snow";

        /// <inheritdoc />
        public string Icon => string.Empty;
    }
}
