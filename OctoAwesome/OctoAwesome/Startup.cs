using NLog;
using NLog.Config;
using NLog.Targets;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Services;
using System;

namespace OctoAwesome
{
    /// <summary>
    /// Helper class for startup initialization.
    /// </summary>
    public static class Startup
    {
        /// <summary>
        /// Register all basic types needed for startup.
        /// </summary>
        /// <param name="typeContainer">The type container to register the types in.</param>
        public static void Register(ITypeContainer typeContainer)
        {
            typeContainer.Register<GlobalChunkCache, GlobalChunkCache>();
            typeContainer.Register<IGlobalChunkCache, GlobalChunkCache>();

            typeContainer.Register<Logging.NullLogger, Logging.NullLogger>();
            typeContainer.Register<Logging.Logger, Logging.Logger>();
            typeContainer.Register<Logging.ILogger, Logging.Logger>();

            typeContainer.Register<IPool<Awaiter>, Pool<Awaiter>>(InstanceBehavior.Singleton);
            typeContainer.Register<Pool<Awaiter>, Pool<Awaiter>>(InstanceBehavior.Singleton);

            typeContainer.Register<IPool<BlockChangedNotification>, Pool<BlockChangedNotification>>(InstanceBehavior.Singleton);
            typeContainer.Register<Pool<BlockChangedNotification>, Pool<BlockChangedNotification>>(InstanceBehavior.Singleton);
            typeContainer.Register<IPool<BlocksChangedNotification>, Pool<BlocksChangedNotification>>(InstanceBehavior.Singleton);
            typeContainer.Register<Pool<BlocksChangedNotification>, Pool<BlocksChangedNotification>>(InstanceBehavior.Singleton);
            typeContainer.Register<IPool<EntityNotification>, Pool<EntityNotification>>(InstanceBehavior.Singleton);
            typeContainer.Register<Pool<EntityNotification>, Pool<EntityNotification>>(InstanceBehavior.Singleton);
            typeContainer.Register<IPool<PropertyChangedNotification>, Pool<PropertyChangedNotification>>(InstanceBehavior.Singleton);
            typeContainer.Register<Pool<PropertyChangedNotification>, Pool<PropertyChangedNotification>>(InstanceBehavior.Singleton);
            typeContainer.Register<IPool<Chunk>, ChunkPool>(InstanceBehavior.Singleton);
            typeContainer.Register<ChunkPool, ChunkPool>(InstanceBehavior.Singleton);
            typeContainer.Register<IPool<BlockVolumeState>, Pool<BlockVolumeState>>(InstanceBehavior.Singleton);
            typeContainer.Register<BlockCollectionService>(InstanceBehavior.Singleton);
        }

        /// <summary>
        /// Configures the logger for logging to the correct locations and log levels.
        /// </summary>
        /// <param name="clientType">The application client type.</param>
        public static void ConfigureLogger(ClientType clientType)
        {
            var config = new LoggingConfiguration();

            switch (clientType)
            {
                case ClientType.DesktopClient:
                    config.AddRule(LogLevel.Trace, LogLevel.Fatal, new FileTarget("octoawesome.logfile")
                    {
                        FileName = $"./logs/octoClient-{DateTime.Now:ddMMyy_hhmmss}.log"
                    });
                    break;
                case ClientType.GameServer:
                    config.AddRule(LogLevel.Trace, LogLevel.Fatal, new ColoredConsoleTarget("octoawesome.logconsole"));
                    config.AddRule(LogLevel.Debug, LogLevel.Fatal, new FileTarget("octoawesome.logfile")
                    {
                        FileName = $"./logs/server-{DateTime.Now:ddMMyy_hhmmss}.log"
                    });
                    break;
                default:
                    config.AddRule(LogLevel.Trace, LogLevel.Fatal, new FileTarget("octoawesome.logfile")
                    {
                        FileName = $"./logs/generic-{DateTime.Now:ddMMyy_hhmmss}.log"
                    });
                    break;
            }

            LogManager.Configuration = config;
        }
    }
}
