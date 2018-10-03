using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Persistiert Chunks auf die Festplatte.
    /// </summary>
    public class DiskPersistenceManager : IPersistenceManager
    {
        private const string UniverseFilename = "universe.info";

        private const string PlanetGeneratorInfo = "generator.info";

        private const string PlanetFilename = "planet.info";

        private const string ColumnFilename = "column_{0}_{1}.dat";

        private DirectoryInfo root;
        private ISettings Settings;

        private readonly IDefinitionManager definitionManager;
        private readonly IExtensionResolver extensionResolver;

        public DiskPersistenceManager(IExtensionResolver extensionResolver, IDefinitionManager definitionManager, ISettings Settings)
        {
            this.extensionResolver = extensionResolver;
            this.definitionManager = definitionManager;
            this.Settings = Settings;
        }

        private string GetRoot()
        {
            if (root != null)
                return root.FullName;

            string appconfig = Settings.Get<string>("ChunkRoot");
            if (!string.IsNullOrEmpty(appconfig))
            {
                root = new DirectoryInfo(appconfig);
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

        /// <summary>
        /// Speichert das Universum.
        /// </summary>
        /// <param name="universe">Das zu speichernde Universum</param>
        public void SaveUniverse(IUniverse universe)
        {
            string path = Path.Combine(GetRoot(), universe.Id.ToString());
            Directory.CreateDirectory(path);

            string file = Path.Combine(path, UniverseFilename);
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            using (GZipStream zip = new GZipStream(stream, CompressionMode.Compress))
            using (var writer = new BinaryWriter(zip))
            {
                universe.Serialize(writer, null);
            }
        }

        /// <summary>
        /// Löscht ein Universum.
        /// </summary>
        /// <param name="universeGuid">Die Guid des Universums.</param>
        public void DeleteUniverse(Guid universeGuid)
        {
            string path = Path.Combine(GetRoot(), universeGuid.ToString());
            Directory.Delete(path, true);
        }

        /// <summary>
        /// Speichert einen Planeten.
        /// </summary>
        /// <param name="universeGuid">Guid des Universums</param>
        /// <param name="planet">Zu speichernder Planet</param>
        public void SavePlanet(Guid universeGuid, IPlanet planet)
        {
            string path = Path.Combine(GetRoot(), universeGuid.ToString(), planet.Id.ToString());
            Directory.CreateDirectory(path);

            string generatorInfo = Path.Combine(path, PlanetGeneratorInfo);
            using (Stream stream = File.Open(generatorInfo, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(planet.Generator.GetType().FullName);
                }
            }

            string file = Path.Combine(path, PlanetFilename);
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            using (GZipStream zip = new GZipStream(stream, CompressionMode.Compress))
            using (BinaryWriter writer = new BinaryWriter(zip))
                planet.Serialize(writer, null);
        }

        /// <summary>
        /// Speichert eine <see cref="IChunkColumn"/>.
        /// </summary>
        /// <param name="universeGuid">GUID des Universums.</param>
        /// <param name="planetId">Index des Planeten.</param>
        /// <param name="column">Zu serialisierende ChunkColumn.</param>
        public void SaveColumn(Guid universeGuid, int planetId, IChunkColumn column)
        {
            string path = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString());
            Directory.CreateDirectory(path);

            string file = path = Path.Combine(path, string.Format(ColumnFilename, column.Index.X, column.Index.Y));
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            using (GZipStream zip = new GZipStream(stream, CompressionMode.Compress))
            using (BinaryWriter writer = new BinaryWriter(zip))
                column.Serialize(writer, definitionManager);
        }

        /// <summary>
        /// Gibt alle Universen zurück, die geladen werden können.
        /// </summary>
        /// <returns>Die Liste der Universen.</returns>
        public Awaiter Load(out SerializableCollection<IUniverse> universes)
        {
            string root = GetRoot();
            var awaiter = new Awaiter();
            universes = new SerializableCollection<IUniverse>();
            awaiter.Serializable = universes;
            foreach (var folder in Directory.GetDirectories(root))
            {
                string id = Path.GetFileNameWithoutExtension(folder);//folder.Replace(root + "\\", "");
                if (Guid.TryParse(id, out Guid guid))
                    universes.Add((IUniverse)Load(out var universe, guid).WaitOn());
            }
            awaiter.SetResult(universes);

            return awaiter;
        }

        /// <summary>
        /// Lädt das Universum mit der angegebenen Guid.
        /// </summary>
        /// <param name="universeGuid">Die Guid des Universums.</param>
        /// <returns>Das geladene Universum.</returns>
        public Awaiter Load(out IUniverse universe, Guid universeGuid)
        {
            string file = Path.Combine(GetRoot(), universeGuid.ToString(), UniverseFilename);
            universe = new Universe();
            if (!File.Exists(file))
                return null;
            var tcs = new CustomAwaiter();

            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            using (GZipStream zip = new GZipStream(stream, CompressionMode.Decompress))
            using (var reader = new BinaryReader(zip))
            {
                var awaiter = new Awaiter();
                awaiter.Serializable = universe;
                universe.Deserialize(reader, null);
                awaiter.SetResult(universe);
                return awaiter;
            }

        }

        /// <summary>
        /// Lädt einen Planeten.
        /// </summary>
        /// <param name="universeGuid">Guid des Universums</param>
        /// <param name="planetId">Index des Planeten</param>
        /// <returns></returns>
        public Awaiter Load(out IPlanet planet, Guid universeGuid, int planetId)
        {
            string file = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString(), PlanetFilename);
            string generatorInfo = Path.Combine(GetRoot(), universeGuid.ToString(), planetId.ToString(), PlanetGeneratorInfo);
            planet = new Planet();
            if (!File.Exists(generatorInfo) || !File.Exists(file))
                return null;

            IMapGenerator generator = null;
            using (Stream stream = File.Open(generatorInfo, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader bw = new BinaryReader(stream))
                {
                    string generatorName = bw.ReadString();
                    generator = extensionResolver.GetMapGenerator().FirstOrDefault(g => g.GetType().FullName.Equals(generatorName));
                }
            }

            if (generator == null)
                throw new Exception("Unknown Generator");


            using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
            {
                using (GZipStream zip = new GZipStream(stream, CompressionMode.Decompress))
                {
                    var awaiter = new Awaiter();
                    awaiter.Serializable = planet;
                    awaiter.SetResult(generator.GeneratePlanet(zip));
                    return awaiter;
                }
            }
        }

        /// <summary>
        /// Lädt eine <see cref="IChunkColumn"/>.
        /// </summary>
        /// <param name="universeGuid">GUID des Universums.</param>
        /// <param name="planet">Index des Planeten.</param>
        /// <param name="columnIndex">Zu serialisierende ChunkColumn.</param>
        /// <returns>Die neu geladene ChunkColumn.</returns>
        public Awaiter Load(out IChunkColumn column, Guid universeGuid, IPlanet planet, Index2 columnIndex)
        {
            string file = Path.Combine(GetRoot(), universeGuid.ToString(), planet.Id.ToString(), string.Format(ColumnFilename, columnIndex.X, columnIndex.Y));
            column = new ChunkColumn();
            if (!File.Exists(file))
                return null;

            try
            {
                using (Stream stream = File.Open(file, FileMode.Open, FileAccess.Read))
                {
                    using (GZipStream zip = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        var awaiter = new Awaiter();
                        awaiter.Serializable = column;
                        awaiter.SetResult(planet.Generator.GenerateColumn(zip, definitionManager, planet.Id, columnIndex));
                        return awaiter;
                    }
                }
            }
            catch (IOException)
            {
                try
                {
                    File.Delete(file);
                }
                catch (IOException) { }
                return null;
            }
        }

        /// <summary>
        /// Lädt einen Player.
        /// </summary>
        /// <param name="universeGuid">Die Guid des Universums.</param>
        /// <param name="playername">Der Name des Spielers.</param>
        /// <returns></returns>
        public Awaiter Load(out Player player, Guid universeGuid, string playername)
        {
            //TODO: Später durch Playername ersetzen
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
                        var awaiter = new Awaiter();
                        awaiter.Serializable = player;
                        player.Deserialize(reader, definitionManager);
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

        /// <summary>
        /// Speichert einen Player
        /// </summary>
        /// <param name="universeGuid">Die Guid des Universums.</param>
        /// <param name="player">Der Player.</param>
        public void SavePlayer(Guid universeGuid, Player player)
        {
            string path = Path.Combine(GetRoot(), universeGuid.ToString());
            Directory.CreateDirectory(path);

            // TODO: Player Name berücksichtigen
            string file = Path.Combine(path, "player.info");
            using (Stream stream = File.Open(file, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    player.Serialize(writer, definitionManager);
                }
            }
        }
    }
}
