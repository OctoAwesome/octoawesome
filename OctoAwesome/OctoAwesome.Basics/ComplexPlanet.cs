using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OctoAwesome.Basics.Biomes;


namespace OctoAwesome.Basics
{
    public class ComplexPlanet : Planet
    {
        public int HEIGHTMAPDETAILS = 8;

        public float[,] Heightmap { get; private set; }

        public float[, ,] CloudMap { get; private set; }

        public SurfaceBiomeGenerator BiomeGenerator { get; private set; }

        public ComplexPlanet(int id, int universe, Index3 size, IMapGenerator generator, int seed)
            : base(id, universe, size, seed)
        {

            BiomeGenerator = new SurfaceBiomeGenerator(this, 40);
            this.Heightmap = null;  
            ClimateMap = new Climate.ComplexClimateMap(this);
        }
    }
}
