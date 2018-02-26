using OctoAwesome.Basics.Biomes;
using System;
using System.IO;

namespace OctoAwesome.Basics
{
    public class ComplexPlanet : Planet
    {
        public int HEIGHTMAPDETAILS = 8;

        public float[,] Heightmap { get; private set; }

        public float[,,] CloudMap { get; private set; }

        public SurfaceBiomeGenerator BiomeGenerator { get; private set; }

        /// <summary>
        /// Konstruktor des komplexen Map-Generators
        /// </summary>
        /// <param name="id">ID des Planeten</param>
        /// <param name="universe">ID des Universums</param>
        /// <param name="size">Größe des Planeten in Zweierpotenzen Chunks</param>
        /// <param name="seed">Seed des Zufallsgenerators</param>
        public ComplexPlanet(int id, Guid universe, Index3 size, int seed) : base(id, universe, size, seed)
        {
            BiomeGenerator = new SurfaceBiomeGenerator(this, 40);
            ClimateMap = new Climate.ComplexClimateMap(this);
        }

        public ComplexPlanet() : base()
        {
            //Initalize();
        }

        public override void Deserialize(Stream stream)
        {
            base.Deserialize(stream);
            BiomeGenerator = new SurfaceBiomeGenerator(this, 40);
            ClimateMap = new Climate.ComplexClimateMap(this);
        }
    }
}
