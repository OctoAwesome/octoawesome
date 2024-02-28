using OctoAwesome.Chunking;
using OctoAwesome.Database;
using OctoAwesome.Location;

using System;
using System.IO;
using System.IO.Compression;

namespace OctoAwesome.Serialization
{
    /// <summary>
    /// Database context for chunk columns using <see cref="IChunkColumn"/>.
    /// </summary>
    public sealed class ChunkColumnDbContext : DatabaseContext<Index2Tag, IChunkColumn>
    {
        private readonly IPlanet currentPlanet;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContext{TTag,TObject}"/> class.
        /// </summary>
        /// <param name="database">The underlying database for this context.</param>
        /// <param name="planet">The planet the chunk columns are on.</param>
        public ChunkColumnDbContext(Database<Index2Tag> database, IPlanet planet) : base(database) => currentPlanet = planet;

        /// <inheritdoc />
        public override void AddOrUpdate(IChunkColumn value)
        {
            using (Database.Lock(Operation.Write))
                Database.AddOrUpdate(new Index2Tag(value.Index), new Value(Serializer.SerializeCompressed(value, 2048)));
        }

        /// <summary>
        /// Gets a chunk column at a given location.
        /// </summary>
        /// <param name="key">The location to get the chunk column at.</param>
        /// <returns>The chunk column loaded from the database; or <c>null</c> if no matching column was found.</returns>
        /// <seealso cref="Get(Index2Tag)"/>
        public IChunkColumn? Get(Index2 key)
            => Get(new Index2Tag(key));

        /// <inheritdoc />
        public override IChunkColumn? Get(Index2Tag key)
        {
            if (!Database.ContainsKey(key))
                return null;

            var chunkColumn = new ChunkColumn(currentPlanet.Id);
            using var stream 
                = Serializer
                    .Manager
                    .GetStream($"{nameof(ChunkColumnDbContext)}.{nameof(Get)}", Database.GetValue(key).Content.AsSpan());
            using var zip = new GZipStream(stream, CompressionMode.Decompress);
            using var buffered = new BufferedStream(zip);
            using var reader = new BinaryReader(buffered);
            chunkColumn.Deserialize(reader);
            return chunkColumn;
        }

        /// <inheritdoc />
        public override void Remove(IChunkColumn value)
        {
            using (Database.Lock(Operation.Write))
                Database.Remove(new Index2Tag(value.Index));
        }
    }
}
