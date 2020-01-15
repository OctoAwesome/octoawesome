using OctoAwesome.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Runtime
{
    public sealed class DatabaseProvider : IDisposable, IDatabaseProvider
    {
        private readonly string rootPath;
        private readonly Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database> planetDatabaseRegister;
        private readonly Dictionary<(Type Type, Guid Universe), Database.Database> universeDatabaseRegister;
        private readonly Dictionary<Type, Database.Database> globalDatabaseRegister;

        public DatabaseProvider(string rootPath)
        {
            this.rootPath = rootPath;
            planetDatabaseRegister = new Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database>();
            universeDatabaseRegister = new Dictionary<(Type Type, Guid Universe), Database.Database>();
            globalDatabaseRegister = new Dictionary<Type, Database.Database>();
        }

        public Database<T> GetDatabase<T>() where T : ITag, new()
        {
            var key = typeof(T);
            if (globalDatabaseRegister.TryGetValue(key, out var database))
            {
                return database as Database<T>;
            }
            else
            {
                Database<T> tmpDatabase = CreateDatabase<T>(rootPath);
                globalDatabaseRegister.Add(key, tmpDatabase);
                return tmpDatabase;
            }
        }

        public Database<T> GetDatabase<T>(Guid universeGuid) where T : ITag, new()
        {
            var key = (typeof(T), universeGuid);
            if (universeDatabaseRegister.TryGetValue(key, out var database))
            {
                return database as Database<T>;
            }
            else
            {
                Database<T> tmpDatabase = CreateDatabase<T>(Path.Combine(rootPath, universeGuid.ToString()));
                universeDatabaseRegister.Add(key, tmpDatabase);
                return tmpDatabase;
            }
        }

        public Database<T> GetDatabase<T>(Guid universeGuid, int planetId) where T : ITag, new()
        {
            var key = (typeof(T), universeGuid, planetId);
            if (planetDatabaseRegister.TryGetValue(key, out var database))
            {
                return database as Database<T>;
            }
            else
            {
                Database<T> tmpDatabase = CreateDatabase<T>(Path.Combine(rootPath, universeGuid.ToString(), planetId.ToString()));
                planetDatabaseRegister.Add(key, tmpDatabase);
                return tmpDatabase;
            }
        }

        public void Dispose()
        {
            foreach (var database in planetDatabaseRegister)
                database.Value.Dispose();

            foreach (var database in universeDatabaseRegister)
                database.Value.Dispose();

            foreach (var database in globalDatabaseRegister)
                database.Value.Dispose();

            planetDatabaseRegister.Clear();
            universeDatabaseRegister.Clear();
            globalDatabaseRegister.Clear();
        }

        private Database<T> CreateDatabase<T>(string path) where T : ITag, new()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var name = typeof(T).Name.Substring(0, typeof(T).Name.Length - 3);
            string keyFile = Path.Combine(path, $"{name}.keys");
            string valueFile = Path.Combine(path, $"{name}.db");
            return new Database<T>(new FileInfo(keyFile), new FileInfo(valueFile));
        }
    }
}
