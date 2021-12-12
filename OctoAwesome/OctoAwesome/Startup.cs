using NLog;
using NLog.Config;
using NLog.Targets;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Services;
using System;

namespace OctoAwesome
{

    public static class Startup
    {
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
