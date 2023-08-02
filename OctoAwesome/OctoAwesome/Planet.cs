using System;
using System.Diagnostics;
using System.IO;

using OctoAwesome.Caching;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// The default implementation for planets.
    /// </summary>
    [Nooson, SerializationId(1,5)]
    public partial class Planet : IPlanet, IConstructionSerializable<Planet>
    {
        /// <summary>
        /// Backing field for <see cref="ClimateMap"/>.
        /// </summary>
        protected IClimateMap? climateMap;
        /// <inheritdoc />
        public int Id { get; protected set; }

        /// <inheritdoc />
        public Guid Universe { get; protected set; }

        /// <inheritdoc />
        [NoosonIgnore]
        public IClimateMap ClimateMap
        {
            get
            {
                Debug.Assert(climateMap != null, nameof(climateMap) + " != null");
                return climateMap;
            }
        }

        /// <inheritdoc />
        public int Seed { get; protected set; }

        /// <inheritdoc />
        public Index3 Size { get; protected set; }

        /// <inheritdoc />
        public float Gravity { get; protected set; }

        /// <inheritdoc />
        [NoosonCustom(DeserializeMethodName = nameof(DeserializeMapGenerator), SerializeMethodName = nameof(SerializeMapGenerator))]
        public IMapGenerator Generator { get; }

        /// <inheritdoc />
        [NoosonIgnore]
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
        /// <param name="generator">The map generator to use for generating the planet.</param>
        /// <param name="seed">The seed to generate data with.</param>
        public Planet(int id, Guid universe, Index3 size, IMapGenerator generator, int seed) : this(generator)
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
        /// <param name="generator">The map generator to use for generating the planet.</param>
        public Planet(IMapGenerator generator)
        {
            Generator = generator;
            var tc = TypeContainer.Get<ITypeContainer>();
            GlobalChunkCache = new GlobalChunkCache(this, tc.Get<IResourceManager>(), tc.Get<IUpdateHub>());
        }

        private void SerializeMapGenerator(BinaryWriter bw)
        {
            bw.Write(Generator.GetType().AssemblyQualifiedName!);
        }
        private static IMapGenerator DeserializeMapGenerator(BinaryReader br)
        {
            return GenericCaster<object, IMapGenerator>.Cast(Activator.CreateInstance(Type.GetType(br.ReadString())!)!);
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
