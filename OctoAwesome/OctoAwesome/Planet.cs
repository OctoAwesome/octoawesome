using System;
using System.IO;
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
        /// Gravitation des Planeten.
        /// </summary>
        public float Gravity { get; private set; }

        /// <summary>
        /// Der Generator des Planeten.
        /// </summary>
        public IMapGenerator Generator { get; set; }

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
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                writer.Write(Id);
                writer.Write(Seed);
                writer.Write(Gravity);
                writer.Write(Size.X);
                writer.Write(Size.Y);
                writer.Write(Size.Z);
                writer.Write(Universe.ToByteArray());
                //writer.Write(Generator.GetType().FullName);
            }
        }

        /// <summary>
        /// Deserialisiert den Planeten aus dem angegebenen Stream.
        /// </summary>
        /// <param name="stream">Quellstream</param>
        public virtual void Deserialize(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
            {
                Id = reader.ReadInt32();
                Seed = reader.ReadInt32();
                Gravity = reader.ReadSingle();
                Size = new Index3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
                Universe = new Guid(reader.ReadBytes(16));
                //var name = reader.ReadString();
            }
        }
    }
}
