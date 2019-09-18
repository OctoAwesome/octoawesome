using CommandManagementSystem;
using Newtonsoft.Json;
using OctoAwesome.Logging;
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
        private static ManualResetEvent manualResetEvent;
        private static ILogger logger;

        private static void Main(string[] args)
        {
            using (var typeContainer = TypeContainer.Get<ITypeContainer>())
            {
                Startup.Register(typeContainer);
                Startup.ConfigureLogger(ClientType.GameServer);

                Network.Startup.Register(typeContainer);

                logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As("OctoAwesome.GameServer");
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    File.WriteAllText(
                        Path.Combine(".", "logs", $"server-dump-{DateTime.Now.ToString("ddMMyy_hhmmss")}.txt"), 
                        e.ExceptionObject.ToString());

                    logger.Fatal($"Unhandled Exception: {e.ExceptionObject}", e.ExceptionObject as Exception);
                    logger.Flush();
                };

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
                

                typeContainer.Register(settings);
                typeContainer.Register<ISettings, Settings>(settings);
                typeContainer.Register<ServerHandler>(InstanceBehaviour.Singleton);
                typeContainer.Get<ServerHandler>().Start();

                Console.CancelKeyPress += (s, e) => manualResetEvent.Set();
                manualResetEvent.WaitOne();
                settings.Save();
            }
        }


    }
}
