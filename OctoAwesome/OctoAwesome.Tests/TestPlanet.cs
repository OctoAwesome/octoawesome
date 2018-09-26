using System;
using System.IO;

namespace OctoAwesome.Tests
{
    internal class TestPlanet : IPlanet
    {
        public TestPlanet(Guid universe, int id, Index3 size)
        {
            Universe = universe;
            Id = id;
            Size = size;
        }

        public IClimateMap ClimateMap => throw new NotImplementedException();

        public IMapGenerator Generator => throw new NotImplementedException();

        public int Id { get; private set; }

        public int Seed => throw new NotImplementedException();

        public Index3 Size { get; private set; }

        public Guid Universe { get; private set; }
        IMapGenerator IPlanet.Generator { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Deserialize(BinaryReader reader, IDefinitionManager definitionManager) => throw new NotImplementedException();
        public void Serialize(BinaryWriter writer, IDefinitionManager definitionManager) => throw new NotImplementedException();
    }
}
