using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public static class Startup
    {
        public static void Register(ITypeContainer typeContainer)
        {
            typeContainer.Register<GlobalChunkCache, GlobalChunkCache>(InstanceBehaviour.Instance);
            typeContainer.Register<IGlobalChunkCache, GlobalChunkCache>(InstanceBehaviour.Instance);
        }

        public static void ConfigureLogger(ClientType clientType)
        {
            var config = new LoggingConfiguration();

            switch (clientType)
            {
                case ClientType.DesktopClient:
                    config.AddRule(LogLevel.Debug, LogLevel.Fatal, new FileTarget("octoawesome.logfile")
                    {
                        FileName = $"./logs/octoClient-{DateTime.Now.ToString("ddMMyy_hhmmss")}.log"
                    });
                    break;
                case ClientType.GameServer:
                    config.AddRule(LogLevel.Debug, LogLevel.Fatal, new ColoredConsoleTarget("octoawesome.logconsole"));
                    config.AddRule(LogLevel.Debug, LogLevel.Fatal, new FileTarget("octoawesome.logfile")
                    {
                        FileName = $"./logs/server-{DateTime.Now.ToString("ddMMyy_hhmmss")}.log"
                    });
                    break;
                default:
                    config.AddRule(LogLevel.Trace, LogLevel.Fatal, new FileTarget("octoawesome.logfile")
                    {
                        FileName = $"./logs/generic-{DateTime.Now.ToString("ddMMyy_hhmmss")}.log"
                    });
                    break;
            }            

            LogManager.Configuration = config;
        }
    }
}
