using OctoAwesome.Database;
using OctoAwesome.Graphs;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization;


/// <summary>
/// Database context for chunk columns using <see cref="IChunkColumn"/>.
/// </summary>
public sealed class GraphDbContext : DatabaseContext<IdTag<Pencil>, Pencil>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseContext{TTag,TObject}"/> class.
    /// </summary>
    /// <param name="database">The underlying database for this context.</param>
    /// <param name="planet">The planet the chunk columns are on.</param>
    public GraphDbContext(Database<IdTag<Pencil>> database) : base(database)
    {

    }

    /// <inheritdoc />
    public override void AddOrUpdate(Pencil value)
    {
        using (Database.Lock(Operation.Write))
            Database.AddOrUpdate(new IdTag<Pencil>(value.PlanetId), new Value(Serializer.Serialize(value)));
    }

    /// <summary>
    /// Gets a chunk column at a given location.
    /// </summary>
    /// <param name="key">The location to get the chunk column at.</param>
    /// <returns>The chunk column loaded from the database; or <c>null</c> if no matching column was found.</returns>
    /// <seealso cref="Get(IdTag{Pencil})"/>
    public Pencil Get(int key)
        => Get(new IdTag<Pencil>(key));

    /// <inheritdoc />
    public override Pencil Get(IdTag<Pencil> key)
    {
        if (!Database.ContainsKey(key))
            return new Pencil
            {
                PlanetId = key.Id
            };

        using var stream
            = Serializer
                .Manager
                .GetStream($"{nameof(GraphDbContext)}.{nameof(Get)}", Database.GetValue(key).Content.AsSpan());
        using var reader = new BinaryReader(stream);
        return Pencil.DeserializeAndCreate(reader);
    }

    /// <inheritdoc />
    public override void Remove(Pencil value)
    {
        using (Database.Lock(Operation.Write))
            Database.Remove(new IdTag<Pencil>(value.PlanetId));
    }
}
