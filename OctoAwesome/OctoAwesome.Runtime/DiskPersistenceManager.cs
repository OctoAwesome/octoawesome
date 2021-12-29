using OctoAwesome.Components;
using OctoAwesome.Database;
using OctoAwesome.Logging;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;
using OctoAwesome.Serialization.Entities;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Persists game data to disk drives.
    /// </summary>
    public class DiskPersistenceManager : IPersistenceManager, IDisposable
    {
        private const string UniverseFilename = "universe.info";

        private const string PlanetGeneratorInfo = "generator.info";

        private const string PlanetFilename = "planet.info";

        private DirectoryInfo? root;
        private IUniverse currentUniverse;
        private readonly ISettings settings;
        private readonly IPool<Awaiter> awaiterPool;
        private readonly IPool<BlockChangedNotification> blockChangedNotificationPool;
        private readonly IDisposable chunkSubscription;
        private readonly Extension.ExtensionService extensionService;
        private readonly DatabaseProvider databaseProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskPersistenceManager"/> class.
        /// </summary>
        /// <param name="extensionResolver">The extension resolver.</param>
        /// <param name="settings">The game settings.</param>
        /// <param name="updateHub">The update hub.</param>
        public DiskPersistenceManager(Extension.ExtensionService extensionService, ISettings Settings, IUpdateHub updateHub)
        {
            this.extensionService = extensionService;
            settings = Settings;
            databaseProvider = new DatabaseProvider(GetRoot(), TypeContainer.Get<ILogger>());
            awaiterPool = TypeContainer.Get<IPool<Awaiter>>();
            blockChangedNotificationPool = TypeContainer.Get<IPool<BlockChangedNotification>>();
            chunkSubscription = updateHub.ListenOn(DefaultChannels.Chunk).Subscribe(OnNext);
        }

        private string GetRoot()
        {
            if (root != null)
                return root.FullName;

            string appConfig = settings.Get<string>("ChunkRoot");
            if (!string.IsNullOrEmpty(appConfig))
            {
                root = new DirectoryInfo(appConfig);
                if (!root.Exists) root.Create();
                return root.FullName;
            }
            else
            {
                var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                root = new DirectoryInfo(exePath + Path.DirectorySeparatorChar + "OctoMap");
                if (!root.Exists) root.Create();
                return root.FullName;
            }
        }

        /// <inheritdoc />
        public void SaveUniverse(IUniverse universe)
        {
            string path = Path.Combine(GetRoot(), universe.Id.ToString());
            Directory.CreateDirectory(path);
            currentUniverse = universe;
            string file = Path.Combine(path, UniverseFilename);
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            using (GZipStream zip = new GZipStream(stream, CompressionMode.Compress))
            using (var writer = new BinaryWriter(zip))
            {
                universe.Serialize(writer);
            }
        }

        /// <inheritdoc />
        public void DeleteUniverse(Guid universeGuid)
        {
            string path = Path.Combine(GetRoot(), universeGuid.ToString());
            Directory.Delete(path, true);
        }

        /// <inheritdoc />
        public void SavePlanet(Guid universeGuid, IPlanet planet)
        {
            string path = Path.Combine(GetRoot(), universeGuid.ToString(), planet.Id.ToString());
            Directory.CreateDirectory(path);

            string generatorInfo = Path.Combine(path, PlanetGeneratorInfo);
            using (Stream stream = File.Open(generatorInfo, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(planet.Generator.GetType().FullName!);
                }
            }

            string file = Path.Combine(path, PlanetFilename);
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            using (GZipStream zip = new GZipStream(stream, CompressionMode.Compress))
            using (BinaryWriter writer = new BinaryWriter(zip))
                planet.Serialize(writer);
        }

        /// <inheritdoc />
        public void SaveColumn(Guid universeGuid, IPlanet planet, IChunkColumn column)
        {
            var chunkColumContext = new ChunkColumnDbContext(databaseProvider.GetDatabase<Index2Tag>(universeGuid, planet.Id, false), planet);
            chunkColumContext.AddOrUpdate(column);
        }

        /// <inheritdoc />
        public void SavePlayer(Guid universeGuid, Player player)
        {
            string path = Path.Combine(GetRoot(), universeGuid.ToString());
            Directory.CreateDirectory(path);

            // TODO: consider player name
            string file = Path.Combine(path, "player.info");
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    player.Serialize(writer);
                }
            }
        }

        /// <inheritdoc />
        public void Save<TContainer, TComponent>(TContainer container, Guid universe)
           where TContainer : ComponentContainer<TComponent>
           where TComponent : IComponent
        {
            var context
               = new ComponentContainerDbContext<TContainer, TComponent>(databaseProvider, universe);
            context.AddOrUpdate(container);
        }

        /// <inheritdoc />
        public Awaiter Load(out SerializableCollection<IUniverse> universes)
        {
            string root = GetRoot();
            var awaiter = awaiterPool.Rent();
            universes = new SerializableCollection<IUniverse>();
            awaiter.Result = universes;
            foreach (var folder in Directory.GetDirectories(root))
            {
                string id = Path.GetFileNameWithoutExtension(folder);//folder.Replace(root + "\\", "");
                if (Guid.TryParse(id, out Guid guid))
                {
                    Load(out var universe, guid).WaitOnAndRelease();
                    universes.Add(universe);
                }
            }
            awaiter.SetResult(universes);

            return awaiter;
        }

        /// <inheritdoc />
        public Awaiter? Load(out IUniverse universe, Guid universeGuid)
        {
            string file = Path.Combine(GetRoot(), universeGuid.ToString(), UniverseFilename);
            universe = new Universe();
            currentUniverse = universe;
            if (!File.Exists(file))
                return null;

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            using (GZipStream zip = new GZipStream(stream, CompressionMode.Decompress))
            using (var reader = new BinaryReader(zip))
            {
                var awaiter = awaiterPool.Rent();
                universe.Deserialize(reader);
                awaiter.SetResult(universe);
                return awaiter;
            }

        }

        /// <inheritdoc />
        public Awaiter? Load(out IPlanet planet, Guid universeGuid, int planetId)
        {
            string file = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString(), PlanetFilename);
            string generatorInfo = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString(), PlanetGeneratorInfo);
            planet = new Planet();
            if (!File.Exists(generatorInfo) || !File.Exists(file))
                return null;

            IMapGenerator? generator = null;
            using (Stream stream = File.Open(generatorInfo, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader bw = new BinaryReader(stream))
                {
                    string generatorName = bw.ReadString();
                    generator = extensionService.GetFromRegistrar<IMapGenerator>().FirstOrDefault(g => g.GetType().FullName!.Equals(generatorName));
                }
            }

            if (generator == null)
                throw new Exception("Unknown Generator");


            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                using (GZipStream zip = new GZipStream(stream, CompressionMode.Decompress))
                {
                    var awaiter = awaiterPool.Rent();
                    planet = generator.GeneratePlanet(zip);
                    awaiter.SetResult(planet);
                    return awaiter;
                }
            }
        }

        /// <inheritdoc />
        public Awaiter? Load(out IChunkColumn? column, Guid universeGuid, IPlanet planet, Index2 columnIndex)
        {
            var chunkColumContext = new ChunkColumnDbContext(databaseProvider.GetDatabase<Index2Tag>(universeGuid, planet.Id, false), planet);

            column = chunkColumContext.Get(columnIndex);

            if (column == null)
                return null;
            //var localColumn = column;

            ApplyChunkDiff(column, universeGuid, planet);

            var awaiter = awaiterPool.Rent();
            awaiter.SetResult(column);
            return awaiter;
        }

        /// <inheritdoc />
        public Awaiter Load(out Entity entity, Guid universeGuid, Guid entityId)
        {
            var entityContext = new ComponentContainerDbContext<Entity, IEntityComponent>(databaseProvider, universeGuid);
            entity = entityContext.Get(new GuidTag<Entity>(entityId));

            var awaiter = awaiterPool.Rent();
            awaiter.SetResult(entity);
            return awaiter;
        }

        /// <inheritdoc />
        public Awaiter Load<TContainer, TComponent>(out TContainer componentContainer, Guid universeGuid, Guid id)
            where TContainer : ComponentContainer<TComponent>
            where TComponent : IComponent
        {
            var entityContext = new ComponentContainerDbContext<TContainer, TComponent>(databaseProvider, universeGuid);
            componentContainer = entityContext.Get(new GuidTag<TContainer>(id));

            var awaiter = awaiterPool.Rent();
            awaiter.SetResult(componentContainer);
            return awaiter;
        }

        /// <inheritdoc />
        public Awaiter? Load(out Player player, Guid universeGuid, string playerName)
        {
            //TODO: Replace with player name later on.
            string file = Path.Combine(GetRoot(), universeGuid.ToString(), "player.info");
            player = new Player();
            if (!File.Exists(file))
                return null;

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    try
                    {
                        var awaiter = awaiterPool.Rent();
                        awaiter.Result = player;
                        player.Deserialize(reader);
                        awaiter.SetResult(player);
                        return awaiter;
                    }
                    catch (Exception)
                    {
                        // File.Delete(file);
                    }
                }
            }

            return null;
        }

        /// <inheritdoc />
        public IEnumerable<Guid> GetEntityIds(Guid universeGuid)
            => new ComponentContainerDbContext<Entity, IEntityComponent>(databaseProvider, universeGuid).GetAllKeys().Select(i => i.Id);

        /// <inheritdoc />
        public IEnumerable<(Guid Id, T Component)> GetEntityComponents<T>(Guid universeGuid, Guid[] entityIds) where T : IEntityComponent, new()
        {
            foreach (var entityId in entityIds)
                yield return (entityId, new ComponentContainerComponentDbContext<T>(databaseProvider, universeGuid).Get<T>(entityId));
        }

        /// <inheritdoc />
        public IEnumerable<(Guid Id, T Component)> GetAllComponents<T>(Guid universeGuid) where T : IComponent, new()
        {
            var context = new ComponentContainerComponentDbContext<T>(databaseProvider, universeGuid);

            var keys = context.GetAllKeys<T>();

            foreach (var key in keys)
            {
                yield return (key.Id, context.Get<T>(key.Id));
            }
        }

        /// <inheritdoc />
        public T GetComponent<T>(Guid universeGuid, Guid id) where T : IComponent, new()
        {
            var context
                = new ComponentContainerComponentDbContext<T>(databaseProvider, universeGuid);

            return context.Get<T>(id);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            databaseProvider.Dispose();
            chunkSubscription.Dispose();
        }

        /// <summary>
        /// Gets called when a new notification is received.
        /// </summary>
        /// <param name="notification">The received notification.</param>
        public void OnNext(Notification notification)
        {
            if (notification is BlockChangedNotification blockChanged)
                SaveChunk(blockChanged);
            else if (notification is BlocksChangedNotification blocksChanged)
                SaveChunk(blocksChanged);
        }

        private void SaveChunk(BlockChangedNotification chunkNotification)
        {
            var database = databaseProvider.GetDatabase<ChunkDiffTag>(currentUniverse.Id, chunkNotification.Planet, true);
            var databaseContext = new ChunkDiffDbContext(database, blockChangedNotificationPool);
            databaseContext.AddOrUpdate(chunkNotification);
        }

        private void SaveChunk(BlocksChangedNotification chunkNotification)
        {
            var database = databaseProvider.GetDatabase<ChunkDiffTag>(currentUniverse.Id, chunkNotification.Planet, true);
            var databaseContext = new ChunkDiffDbContext(database, blockChangedNotificationPool);
            databaseContext.AddOrUpdate(chunkNotification);
        }

        private void ApplyChunkDiff(IChunkColumn column, Guid universeGuid, IPlanet planet)
        {
            var database = databaseProvider.GetDatabase<ChunkDiffTag>(universeGuid, planet.Id, true);
            var databaseContext = new ChunkDiffDbContext(database, blockChangedNotificationPool);
            var keys = databaseContext.GetAllKeys();

            for (int i = 0; i < keys.Count; i++)
            {
                ChunkDiffTag key = keys[i];
                if (key.ChunkPositon.X == column.Index.X && key.ChunkPositon.Y == column.Index.Y)
                {
                    var block = databaseContext.Get(key);
                    column.Chunks[key.ChunkPositon.Z].Blocks[key.FlatIndex] = block.BlockInfo.Block;
                    column.Chunks[key.ChunkPositon.Z].MetaData[key.FlatIndex] = block.BlockInfo.Meta;
                }
            }

            if (keys.Count > 1000)
            {
                SaveColumn(universeGuid, planet, column);
                databaseContext.Remove(keys);
            }
        }
    }
}
