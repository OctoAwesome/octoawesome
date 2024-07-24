﻿using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

using System.Linq;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Pickaxe item definition.
    /// </summary>
    public class PickaxeDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string Icon => "pick_iron";

        /// <inheritdoc />
        public string DisplayName => "Pickaxe";

        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => material is ISolidMaterialDefinition;

        /// <inheritdoc />
        public Item? Create(IMaterialDefinition material)
        {
            if (material is IFoodMaterialDefinition)
                return null;
            return new Pickaxe(this, material);
        }
    }
}
