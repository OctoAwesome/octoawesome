using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Tests
{
    class TestPlanet : IPlanet
    {
        public TestPlanet(Guid universe, int id, Index3 size)
        {
            Universe = universe;
            Id = id;
            Size = size;
        }

        public IClimateMap ClimateMap
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IMapGenerator Generator
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Id { get; private set; }

        public int Seed
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Index3 Size { get; private set; }

        public Guid Universe { get; private set; }

        public void Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
