using OctoAwesome.Database;
using OctoAwesome.Logging;
using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Provider for databases.
    /// </summary>
    public sealed class DatabaseProvider : IDisposable, IDatabaseProvider
    {
        private readonly string rootPath;
        private readonly ILogger logger;
        private readonly LockSemaphore planetSemaphore;
        private readonly LockSemaphore universeSemaphore;
        private readonly LockSemaphore globalSemaphore;
        private readonly Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database> planetDatabaseRegister;
        private readonly Dictionary<(Type Type, Guid Universe), Database.Database> universeDatabaseRegister;
        private readonly Dictionary<Type, Database.Database> globalDatabaseRegister;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseProvider"/> class.
        /// </summary>
        /// <param name="rootPath">The root path to load databases from.</param>
        /// <param name="logger">The logger to log messages to.</param>
        public DatabaseProvider(string rootPath, ILogger? logger)
        {
            this.rootPath = rootPath;
            this.logger = (logger ?? NullLogger.Default).As(nameof(DatabaseProvider));
            planetSemaphore = new LockSemaphore(1, 1);
            universeSemaphore = new LockSemaphore(1, 1);
            globalSemaphore = new LockSemaphore(1, 1);
            planetDatabaseRegister = new Dictionary<(Type Type, Guid Universe, int PlanetId), Database.Database>();
            universeDatabaseRegister = new Dictionary<(Type Type, Guid Universe), Database.Database>();
            globalDatabaseRegister = new Dictionary<Type, Database.Database>();
        }

        /// <inheritdoc />
        public Database<T> GetDatabase<T>(bool fixedValueSize) where T : ITag, new()
        {
            Type key = typeof(T);
            using (globalSemaphore.Wait())
            {
                if (globalDatabaseRegister.TryGetValue(key, out var database))
                {
                    return (Database<T>)database;
                }
                else
                {
                    Database<T> tmpDatabase = CreateDatabase<T>(rootPath, fixedValueSize);

                    try
                    {
                        tmpDatabase.Open();
                    }
                    catch (Exception ex)
                    {
                        tmpDatabase.Dispose();
                        logger.Error($"Can not Open Database for global, {typeof(T).Name}", ex);
                        throw;
                    }
                    globalDatabaseRegister.Add(key, tmpDatabase);
                    return tmpDatabase;
                }
            }
        }

        /// <inheritdoc />
        public Database<T> GetDatabase<T>(Guid universeGuid, bool fixedValueSize) where T : ITag, new()
        {
            (Type, Guid universeGuid) key = (typeof(T), universeGuid);
            using (universeSemaphore.Wait())
            {
                if (universeDatabaseRegister.TryGetValue(key, out var database))
                {
                    return (Database<T>)database;
                }
                else
                {
                    Database<T> tmpDatabase = CreateDatabase<T>(Path.Combine(rootPath, universeGuid.ToString()), fixedValueSize);

                    try
                    {
                        tmpDatabase.Open();
                    }
                    catch (Exception ex)
                    {
                        tmpDatabase.Dispose();
                        logger.Error($"Can not Open Database for [{universeGuid}], {typeof(T).Name}", ex);
                        throw;
                    }
                    universeDatabaseRegister.Add(key, tmpDatabase);
                    return tmpDatabase;
                }
            }
        }

        /// <inheritdoc />
        public Database<T> GetDatabase<T>(Guid universeGuid, int planetId, bool fixedValueSize) where T : ITag, new()
        {
            (Type, Guid universeGuid, int planetId) key = (typeof(T), universeGuid, planetId);
            using (planetSemaphore.Wait())
            {
                if (planetDatabaseRegister.TryGetValue(key, out var database))
                {
                    return (Database<T>)database;
                }
                else
                {
                    Database<T> tmpDatabase = CreateDatabase<T>(Path.Combine(rootPath, universeGuid.ToString(), planetId.ToString()), fixedValueSize);
                    try
                    {
                        tmpDatabase.Open();
                    }
                    catch (Exception ex)
                    {
                        tmpDatabase.Dispose();
                        logger.Error($"Can not Open Database for [{universeGuid}]{planetId}, {typeof(T).Name}", ex);
                        throw;
                    }
                    planetDatabaseRegister.Add(key, tmpDatabase);
                    return tmpDatabase;
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            foreach (KeyValuePair<(Type Type, Guid Universe, int PlanetId), Database.Database> database in planetDatabaseRegister)
                database.Value.Dispose();

            foreach (KeyValuePair<(Type Type, Guid Universe), Database.Database> database in universeDatabaseRegister)
                database.Value.Dispose();

            foreach (KeyValuePair<Type, Database.Database> database in globalDatabaseRegister)
                database.Value.Dispose();

            planetDatabaseRegister.Clear();
            universeDatabaseRegister.Clear();
            globalDatabaseRegister.Clear();

            planetSemaphore.Dispose();
            universeSemaphore.Dispose();
            globalSemaphore.Dispose();
        }

        private Database<T> CreateDatabase<T>(string path, bool fixedValueSize, string? typeName = null) where T : ITag, new()
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            Type type = typeof(T);
            if (typeName == null)
                typeName = type.Name;

            string name = "";

            foreach (var c in Path.GetInvalidFileNameChars())
            {
                typeName = typeName.Replace(c, '\0');
            }

            if (type.IsGenericType)
            {

                do
                {
                    path = Path.Combine(path, typeName);
                    type = type.GenericTypeArguments.First();

                    if (type.GenericTypeArguments.Length == 0)
                    {
                        name = type.Name;
                        break;
                    }

                    typeName = type.Name;

                } while (true);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

            }
            else
            {
                name = typeName;
            }

            var keyFile = Path.Combine(path, $"{name}.keys");
            var valueFile = Path.Combine(path, $"{name}.db");
            return new Database<T>(new FileInfo(keyFile), new FileInfo(valueFile), fixedValueSize);
        }
    }
}
