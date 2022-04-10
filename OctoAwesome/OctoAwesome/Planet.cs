using System;
using System.Diagnostics;
using System.IO;
using OctoAwesome.Notifications;

namespace OctoAwesome
{
    /// <summary>
    /// The default implementation for planets.
    /// </summary>
    public class Planet : IPlanet
    {
        /// <summary>
        /// Backing field for <see cref="ClimateMap"/>.
        /// </summary>
        protected IClimateMap? climateMap;
        /// <inheritdoc />
        public int Id { get; private set; }

        /// <inheritdoc />
        public Guid Universe { get; private set; }

        /// <inheritdoc />
        public IClimateMap ClimateMap
        {
            get
            {
                Debug.Assert(climateMap != null, nameof(climateMap) + " != null");
                return climateMap;
            }
        }

        /// <inheritdoc />
        public int Seed { get; private set; }

        /// <inheritdoc />
        public Index3 Size { get; private set; }

        /// <inheritdoc />
        public float Gravity { get; protected set; }

        /// <inheritdoc />
        public IMapGenerator Generator { get; set; }

        /// <inheritdoc />
        public IGlobalChunkCache GlobalChunkCache { get; }

        private bool disposed;

        private static int NextId => ++nextId;
        private static int nextId = 0;

        private int secretId = NextId;

        /// <summary>
        /// Initializes a new instance of the <see cref="Planet"/> class.
        /// </summary>
        /// <param name="id">The id of the planet.</param>
        /// <param name="universe">The <see cref="Guid"/> of the universe.</param>
        /// <param name="size">Size number of chunks in dualistic logarithmic scale.</param>
        /// <param name="seed">The seed to generate data with.</param>
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
        /// Initializes a new instance of the <see cref="Planet"/> class.
        /// </summary>
        public Planet()
        {
            GlobalChunkCache = new GlobalChunkCache(this, TypeContainer.Get<IResourceManager>(), TypeContainer.Get<IUpdateHub>(), TypeContainer.Get<SerializationIdTypeProvider>());
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public virtual void Deserialize(BinaryReader reader)
        {
            Id = reader.ReadInt32();
            Seed = reader.ReadInt32();
            Gravity = reader.ReadSingle();
            Size = new Index3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            Universe = new Guid(reader.ReadBytes(16));
            //var name = reader.ReadString();
        }

        /// <inheritdoc />
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
