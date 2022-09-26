using System;
using System.IO;
using OctoAwesome.Notifications;

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

        public float Gravity => throw new NotImplementedException();

        public IGlobalChunkCache GlobalChunkCache { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IUpdateHub UpdateHub { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        IMapGenerator IPlanet.Generator => throw new NotImplementedException();

        public void Deserialize(BinaryReader reader) => throw new NotImplementedException();
        public void Dispose() => throw new NotImplementedException();
        public void Serialize(BinaryWriter writer) => throw new NotImplementedException();
    }
}
