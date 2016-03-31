using Microsoft.Xna.Framework;
using System;
using System.IO;

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
        public Guid Universe { get; private set; }

        /// <summary>
        /// Die Klimakarte des Planeten
        /// </summary>
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
        /// Der Generator des Planeten.
        /// </summary>
        public IMapGenerator Generator { get; set; }

        /// <summary>
        /// Die Gravitation auf dem Planeten
        /// </summary>
        public Vector3 Gravity { get; private set; }

        /// <summary>
        /// Initialisierung des Planeten
        /// </summary>
        /// <param name="id">ID des Planeten</param>
        /// <param name="universe">ID des Universums</param>
        /// <param name="size">Größe des Planeten in Zweierpotenzen Chunks</param>
        /// <param name="seed">Seed des Zufallsgenerators</param>
        public Planet(int id, Guid universe, Index3 size, int seed)
        {
            Id = id;
            Universe = universe;
            Size = new Index3(
                (int)Math.Pow(2, size.X), 
                (int)Math.Pow(2, size.Y),
                (int)Math.Pow(2, size.Z));
            Gravity = new Vector3(0, 0, -20f);
            Seed = seed;
        }

        /// <summary>
        /// Erzeugt eine neue Instanz eines Planeten.
        /// </summary>
        public Planet()
        {

        }

        /// <summary>
        /// Serialisiert den Planeten in den angegebenen Stream.
        /// </summary>
        /// <param name="stream">Zielstream</param>
        public virtual void Serialize(Stream stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserialisiert den Planeten aus dem angegebenen Stream.
        /// </summary>
        /// <param name="stream">Quellstream</param>
        public virtual void Deserialize(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
