using NLog;
using NLog.Config;
using NLog.Targets;

using NonSucking.Framework.Extension.IoC;

using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
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

            typeContainer.Register<IPool<Awaiter>, Pool<Awaiter>>(InstanceBehaviour.Singleton);
            typeContainer.Register<Pool<Awaiter>, Pool<Awaiter>>(InstanceBehaviour.Singleton);

            typeContainer.Register<IPool<BlockChangedNotification>, Pool<BlockChangedNotification>>(InstanceBehaviour.Singleton);
            typeContainer.Register<Pool<BlockChangedNotification>, Pool<BlockChangedNotification>>(InstanceBehaviour.Singleton);
            typeContainer.Register<IPool<BlocksChangedNotification>, Pool<BlocksChangedNotification>>(InstanceBehaviour.Singleton);
            typeContainer.Register<Pool<BlocksChangedNotification>, Pool<BlocksChangedNotification>>(InstanceBehaviour.Singleton);
            typeContainer.Register<IPool<EntityNotification>, Pool<EntityNotification>>(InstanceBehaviour.Singleton);
            typeContainer.Register<Pool<EntityNotification>, Pool<EntityNotification>>(InstanceBehaviour.Singleton);
            typeContainer.Register<IPool<PropertyChangedNotification>, Pool<PropertyChangedNotification>>(InstanceBehaviour.Singleton);
            typeContainer.Register<Pool<PropertyChangedNotification>, Pool<PropertyChangedNotification>>(InstanceBehaviour.Singleton);
            typeContainer.Register<IPool<ChatNotification>, Pool<ChatNotification>>(InstanceBehaviour.Singleton);
            typeContainer.Register<Pool<ChatNotification>, Pool<ChatNotification>>(InstanceBehaviour.Singleton);
            typeContainer.Register<IPool<Chunk>, ChunkPool>(InstanceBehaviour.Singleton);
            typeContainer.Register<ChunkPool, ChunkPool>(InstanceBehaviour.Singleton);
            typeContainer.Register<IPool<BlockVolumeState>, Pool<BlockVolumeState>>(InstanceBehaviour.Singleton);
            typeContainer.Register<BlockCollectionService>(InstanceBehaviour.Singleton);
            typeContainer.Register<ComponentChangedNotificationHandler>(InstanceBehaviour.Singleton);

            typeContainer.Register<InteractService>(InstanceBehaviour.Singleton);
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
                        FileName = $"./logs/octoClient-{DateTime.Now:dd_MM_yyyy}.log"
                    });
                    break;
                case ClientType.GameServer:
                    config.AddRule(LogLevel.Debug, LogLevel.Fatal, new ColoredConsoleTarget("octoawesome.logconsole"));
                    config.AddRule(LogLevel.Trace, LogLevel.Fatal, new FileTarget("octoawesome.logfile")
                    {
                        FileName = $"./logs/server-{DateTime.Now:dd_MM_yyyy}.log"
                    });
                    break;
                default:
                    config.AddRule(LogLevel.Trace, LogLevel.Fatal, new FileTarget("octoawesome.logfile")
                    {
                        FileName = $"./logs/generic-{DateTime.Now:dd_MM_yyyy}.log"
                    });
                    break;
            }

            LogManager.Configuration = config;
        }
    }
}
