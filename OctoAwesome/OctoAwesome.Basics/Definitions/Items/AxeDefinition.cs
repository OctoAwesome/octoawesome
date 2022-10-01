﻿using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Axe item definition.
    /// </summary>
    public class AxeDefinition : IItemDefinition
    {
        /// <inheritdoc />
        public string DisplayName => "Axe";

        /// <inheritdoc />
        public string Icon => "axe_iron";

        /// <inheritdoc />
        public bool CanMineMaterial(IMaterialDefinition material)
            => material is ISolidMaterialDefinition;

        /// <inheritdoc />
        public Item? Create(IMaterialDefinition material)
        {
            if (material is IFoodMaterialDefinition)
                return null;
            return new Axe(this, material);
        }
    }
}
