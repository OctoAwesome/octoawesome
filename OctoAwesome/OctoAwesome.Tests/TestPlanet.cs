using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Tests
{
    class TestPlanet : IPlanet
    {
        public TestPlanet(int universe, int id, Index3 size)
        {
            Universe = universe;
            Id = id;
            Size = size;
        }

        public IClimateMap ClimateMap
        {
            get { throw new NotImplementedException(); }
        }

        public int Id { get; private set; }

        public int Seed
        {
            get { throw new NotImplementedException(); }
        }

        public Index3 Size { get; private set; }

        public int Universe { get; private set; }
    }
}