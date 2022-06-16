﻿using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.Definitions.Materials
{
    public class PlayerMeatMaterialDefinition : IFoodMaterialDefinition
    {
        public string DisplayName => "Player Meat";

        public string Icon => string.Empty;

        public ushort Joule { get; } = 10960;
        public bool Edible { get; } = true;
        public int Hardness { get; } = 1;
        public int Density { get; } = 1040;
    }
}