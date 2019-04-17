using CommandManagementSystem;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using OctoAwesome.Network;
using System;
using System.Collections.Generic;
using System.IO;
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
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, new FileTarget("octoawesome.logfile")
            {
                FileName = $"./logs/server-{DateTime.Now.ToString("ddMMyy_hhmmss")}.log"
            });

            LogManager.Configuration = config;
            logger = LogManager.GetCurrentClassLogger();

            manualResetEvent = new ManualResetEvent(false);

            logger.Info("Server start");

            var fileInfo = new FileInfo(Path.Combine(".", "settings.json"));
            Settings settings;


            if (!fileInfo.Exists)
            {
                logger.Debug("Create new Default Settings");
                settings = new Settings()
                {
                    FileInfo = fileInfo
                };
                settings.Save();
            }
            else
            {
                logger.Debug("Load Settings");
                settings = new Settings(fileInfo);                
            }

            ServerHandler = new ServerHandler(settings);
            ServerHandler.Start();

            Console.CancelKeyPress += (s, e) => manualResetEvent.Set();
            manualResetEvent.WaitOne();
            settings.Save();
        }


    }
}
