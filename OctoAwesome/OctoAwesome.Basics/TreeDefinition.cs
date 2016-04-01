using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public abstract class TreeDefinition : ITreeDefinition
    {
        public abstract int Order { get; }

        public abstract float MaxTemperature { get; }

        public abstract float MinTemperature { get; }

        public abstract int GetDensity(IPlanet planet, Index3 index);

        public abstract void Init(IDefinitionManager definitionManager);

        public abstract void PlantTree(IDefinitionManager definitionManager, IPlanet planet, Index3 index, LocalBuilder builder, int seed);
    }
}
