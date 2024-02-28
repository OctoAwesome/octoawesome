using OctoAwesome.Basics.Biomes;
using OctoAwesome.Location;

using System;
using System.Diagnostics;
using System.IO;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// A complex planet implementation with complex features.
    /// </summary>
    public class ComplexPlanet : Planet
    {
        // The gravitational constant was chosen on purpose to be that "big", see Issue #220
        private const double GravitationalConstant = 6.67e-7;

        private SurfaceBiomeGenerator? biomeGenerator;

        /// <summary>
        /// Gets the biome generator used for generating biomes on the planet.
        /// </summary>
        public SurfaceBiomeGenerator BiomeGenerator
        {
            get
            {
                Debug.Assert(biomeGenerator != null, nameof(biomeGenerator) + " != null");
                return biomeGenerator;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexPlanet"/> class.
        /// </summary>
        /// <param name="id">The planet id.</param>
        /// <param name="universe">The id of the universe the planet resides in.</param>
        /// <param name="size">The planet size in dualistic logarithmic scale.</param>
        /// <param name="generator">The map generator to use for generating the planet.</param>
        /// <param name="seed">Seeding value for generating a unique planet.</param>
        /// <param name="averageDensity">
        /// Average planet density in kg/m³ for calculating planets gravity. Defaults to the value matching earth : 5510
        /// </param>
        public ComplexPlanet(int id, Guid universe, Index3 size, IMapGenerator generator, int seed, int averageDensity = 5510)
            : base(id, universe, size, generator, seed)
        {
            // Calculation of gravity based on newtonian laws and the assumption of a sphere with same surface as the planet.
            var radius = Math.Sqrt((Size.X * Size.Y) / (16 * Math.PI));
            Gravity = (float)((4f / 3f) * Math.PI * GravitationalConstant * averageDensity * radius);
            Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComplexPlanet"/> class.
        /// </summary>
        /// <param name="generator">The map generator to use for generating the planet.</param>
        public ComplexPlanet(IMapGenerator generator)
            : base(generator)
        {
            //Initalize();
        }

        /// <inheritdoc />
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
