using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Standard-Implementierung des Planeten.
    /// </summary>
    public class Planet : IPlanet
    {
        /// <summary>
        /// ID des Planeten.
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// Referenz auf das Parent Universe
        /// </summary>
        public int Universe { get; private set; }

        public IClimateMap ClimateMap { get; protected set; }

        /// <summary>
        /// Seed des Zufallsgenerators dieses Planeten.
        /// </summary>
        public int Seed { get; private set; }

        /// <summary>
        /// Die Größe des Planeten in Blocks.
        /// </summary>
        public Index3 Size { get; private set; }

        /// <summary>
        /// Initialisierung des Planeten
        /// </summary>
        /// <param name="size">Größe des Planeten in Chunks</param>
        /// <param name="generator">Instanz des Map-Generators</param>
        /// <param name="seed">Seed des Zufallsgenerators</param>
        public Planet(int id, int universe, Index3 size, int seed)
        {
            Id = id;
            Universe = universe;
            Size = size;
            Seed = seed;
        }
    }
}
