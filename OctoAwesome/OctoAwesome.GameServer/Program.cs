using CommandManagementSystem;
using NLog;
using NLog.Config;
using NLog.Targets;
using OctoAwesome.Network;
using System;
using System.Net;
using System.Threading;

namespace OctoAwesome.GameServer
{
    internal class Program
    {
        public static ServerHandler ServerHandler { get; set; }

        private static ManualResetEvent manualResetEvent;
        private static Logger logger;

        private static void Main(string[] args)
        {
            var config = new LoggingConfiguration();

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, new ColoredConsoleTarget("octoawesome.logconsole"));
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, new FileTarget("octoawesome.logfile") { FileName = "server.log" });

            LogManager.Configuration = config;
            logger = LogManager.GetCurrentClassLogger(typeof(Program));
            
            manualResetEvent = new ManualResetEvent(false);
                        
            logger.Info("Server start");
            ServerHandler = new ServerHandler();
            ServerHandler.Start();

            Console.CancelKeyPress += (s, e) => manualResetEvent.Set();
            manualResetEvent.WaitOne();
        }

        
    }
}
