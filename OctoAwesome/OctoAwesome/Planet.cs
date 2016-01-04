﻿using System;

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
        /// Die Größe des Planeten in Chunks.
        /// </summary>
        public Index3 Size { get; private set; }

        /// <summary>
        /// Initialisierung des Planeten
        /// </summary>
        /// <param name="id">ID des Planeten</param>
        /// <param name="universe">ID des Universums</param>
        /// <param name="size">Größe des Planeten in Zweierpotenzen Chunks</param>
        /// <param name="generator">Instanz des Map-Generators</param>
        /// <param name="seed">Seed des Zufallsgenerators</param>
        public Planet(int id, int universe, Index3 size, int seed)
        {
            Id = id;
            Universe = universe;
            Size = new Index3(
                (int) Math.Pow(2, size.X),
                (int) Math.Pow(2, size.Y),
                (int) Math.Pow(2, size.Z));
            Seed = seed;
        }
    }
}