using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OctoAwesome.Notifications;

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
        public float Gravity { get; protected set; }

        /// <summary>
        /// Der Generator des Planeten.
        /// </summary>
        public IMapGenerator Generator { get; set; }

        public IGlobalChunkCache GlobalChunkCache { get; }

        private bool disposed;

        /// <summary>
        /// Initialisierung des Planeten.
        /// </summary>
        /// <param name="id">ID des Planeten.</param>
        /// <param name="universe">ID des Universums.</param>
        /// <param name="size">Größe des Planeten in Zweierpotenzen Chunks.</param>
        /// <param name="seed">Seed des Zufallsgenerators.</param>
        public Planet(int id, Guid universe, Index3 size, int seed) : this()
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
            GlobalChunkCache = new GlobalChunkCache(this, TypeContainer.Get<IResourceManager>(), TypeContainer.Get<IUpdateHub>());
        }

        /// <summary>
        /// Serialisiert den Planeten in den angegebenen Stream.
        /// </summary>
        /// <param name="stream">Zielstream</param>
        public virtual void Serialize(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Seed);
            writer.Write(Gravity);
            writer.Write(Size.X);
            writer.Write(Size.Y);
            writer.Write(Size.Z);
            writer.Write(Universe.ToByteArray());
        }

        /// <summary>
        /// Deserialisiert den Planeten aus dem angegebenen Stream.
        /// </summary>
        /// <param name="stream">Quellstream</param>
        public virtual void Deserialize(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            Seed = reader.ReadInt32();
            Gravity = reader.ReadSingle();
            Size = new Index3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            Universe = new Guid(reader.ReadBytes(16));
            //var name = reader.ReadString();
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;

            if (GlobalChunkCache is IDisposable disposable)
                disposable.Dispose();

        }
    }
}
