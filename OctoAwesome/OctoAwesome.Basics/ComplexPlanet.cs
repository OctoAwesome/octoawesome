using OctoAwesome.Basics.Biomes;
using System;
using System.IO;

namespace OctoAwesome.Basics
{
    public class ComplexPlanet : Planet
    {
        // Die Gravitationskonstante ist absichtlich so "groß", vgl. Issue #220
        private const double GravitationalConstant = 6.67e-7;

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
        /// <param name="averageDensity">Durchschnittliche Dichte des Planeten zur Berechnung der Gravitation in kg/m³. Erd- und Standardwert: 5510</param>
        public ComplexPlanet(int id, Guid universe, Index3 size, int seed, int averageDensity = 5510)
            : base(id, universe, size, seed)
        {
            // Berechnung der Gravitation auf Basis des Newton'schen Grundgesetzes und
            // der Annahme einer Kugel mit gleicher Oberfläche wie der rechteckige Planet.
            var radius = Math.Sqrt((Size.X * Size.Y) / (16 * Math.PI));
            Gravity = (float)((4f / 3f) * Math.PI * GravitationalConstant * averageDensity * radius);

            BiomeGenerator = new SurfaceBiomeGenerator(this, 40);
            ClimateMap = new Climate.ComplexClimateMap(this);
        }

        public ComplexPlanet() : base()
        {
            BiomeGenerator = new SurfaceBiomeGenerator(this, 40);
            ClimateMap = new Climate.ComplexClimateMap(this);
        }

        public override void Deserialize(Stream stream)
        {
            base.Deserialize(stream);
        }
    }
}
