using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization
{
    public sealed class ChunkColumnDbContext : DatabaseContext<Index2Tag, ChunkColumn>
    {
        private readonly IPlanet currentPlanet;

        public ChunkColumnDbContext(Database<Index2Tag> database, IPlanet planet) : base(database)
        {
            currentPlanet = planet;
        }

        public override void AddOrUpdate(ChunkColumn value)
            => Database.AddOrUpdate(new Index2Tag(value.Index), new Value(Serializer.Serialize(value)));

        public ChunkColumn Get(Index2 key)
            => Get(new Index2Tag(key));
        public override ChunkColumn Get(Index2Tag key)
        {
            var chunkColumn = new ChunkColumn(currentPlanet);

            using (var stream = new MemoryStream(Database.GetValue(key).Content))
            using (var reader = new BinaryReader(stream))
            {
                chunkColumn.Deserialize(reader);
                return chunkColumn;
            }
        }

        public override void Remove(ChunkColumn value)
           => Database.Remove(new Index2Tag(value.Index));
    }
}
