using OctoAwesome.Basics.Biomes;
using System;
using System.Diagnostics;
using System.IO;

namespace OctoAwesome.Basics
{
    public class ComplexPlanet : Planet
    {
        // Die Gravitationskonstante ist absichtlich so "groß", vgl. Issue #220
        private const double GravitationalConstant = 6.67e-7;

        private SurfaceBiomeGenerator? biomeGenerator;
        public SurfaceBiomeGenerator BiomeGenerator
        {
            get
            {
                Debug.Assert(biomeGenerator != null, nameof(biomeGenerator) + " != null");
                return biomeGenerator;
            }
        }

        /// <summary>
        /// Konstruktor des komplexen Map-Generators
        /// </summary>
        /// <param name="id">ID des Planeten</param>
        /// <param name="universe">ID des Universums</param>
        /// <param name="size">Größe des Planeten in Zweierpotenzen Chunks</param>
        /// <param name="generator">Instanz des Map-Generators</param>
        /// <param name="seed">Seed des Zufallsgenerators</param>
        /// <param name="averageDensity">Durchschnittliche Dichte des Planeten zur Berechnung der Gravitation in kg/m³. Erd- und Standardwert: 5510</param>
        public ComplexPlanet(int id, Guid universe, Index3 size, IMapGenerator generator, int seed, int averageDensity = 5510)
            : base(id, universe, size, seed)
        {
            Generator = generator;

            // Berechnung der Gravitation auf Basis des Newton'schen Grundgesetzes und
            // der Annahme einer Kugel mit gleicher Oberfläche wie der rechteckige Planet.
            var radius = Math.Sqrt((Size.X * Size.Y) / (16 * Math.PI));
            Gravity = (float)((4f / 3f) * Math.PI * GravitationalConstant * averageDensity * radius);
            Initialize();
        }
        public ComplexPlanet()
        {
            //Initalize();
        }
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            Initialize();
        }

        private void Initialize()
        {
            biomeGenerator = new SurfaceBiomeGenerator(this, 40);
            climateMap = new Climate.ComplexClimateMap(this);
        }
    }
}
