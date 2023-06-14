using System;
using OctoAwesome.Database;
using OctoAwesome.Location;
using OctoAwesome.Serialization;
using System.IO;

namespace OctoAwesome.Client.Cache
{
    internal class ChunkRendererDbContext : DatabaseContext<Index3Tag, VerticesForChunk>
    {
        public ChunkRendererDbContext(Database<Index3Tag> database) : base(database)
        {
        }

        public override void AddOrUpdate(VerticesForChunk? value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            using (Database.Lock(Operation.Write))
                Database.AddOrUpdate(new Index3Tag(value.ChunkPosition), new Value(Serializer.Serialize(value)));
        }

        public VerticesForChunk? Get(Index3 key)
            => Get(new Index3Tag(key));

        public override VerticesForChunk? Get(Index3Tag key)
        {
            if (!Database.ContainsKey(key))
                return null;

            var verticesForChunk = new VerticesForChunk();
            using var stream = new MemoryStream(Database.GetValue(key).Content);
            using var buffered = new BufferedStream(stream);
            using var reader = new BinaryReader(buffered);

            verticesForChunk.Deserialize(reader);
            return verticesForChunk;
        }

        public override void Remove(VerticesForChunk? value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            using (Database.Lock(Operation.Write))
                Database.Remove(new Index3Tag(value.ChunkPosition));
        }
    }
}
