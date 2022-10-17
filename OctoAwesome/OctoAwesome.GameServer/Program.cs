using OctoAwesome.Logging;
using OctoAwesome.Network;

using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO;
using System.Threading;

namespace OctoAwesome.GameServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            /*
             *  StartParameter:
             *  - start -> runs immediately
             *      * port
             *  - execute (Run Command immediately after start)
             *  - wizard (setup)
             *
             *  In-game (Additional):
             *  - stop / shutdown / restart
             *  - world
             *      * Save
             *      * create
             *      * load
             *      * delete
             */

            var portOption = new Option<bool>("--port", "Defines the port where the server listen on");
            var ingameRoot = new Command("execute");
            var root = new RootCommand("starts the server with default options")
            {
                ingameRoot
            };

            var stopCommand = new Command("stop");
            stopCommand.SetHandler(() => Console.WriteLine("Stopped"));
            ingameRoot.Add(stopCommand);

            root
                .Add("start", "this starts the server immediately", ExecuteStart, portOption)
                .Add("wizard", "guides through the creation of a new world", ExecuteStart, portOption)
                .Add("execute", "Run Command immediately after start", ExecuteStart, portOption)
                ;


            ingameRoot
                .Add("stop", "stops the server and saves the world", ExecuteStart, portOption)
                .Add("restart", "restarts the server after saving the world", ExecuteStart, portOption)
                .Add("shutdown", "force stops the server without saving", ExecuteStart, portOption);

            ingameRoot
                .Create("world", "world related commands")
                    .Add("save", "saves the world", ExecuteStart, portOption)
                    .Add("create", "created a whole new world, with a fantastic point of view", ExecuteStart, portOption)
                    .Add("load", "loads an existing world", ExecuteStart, portOption)
                    .Add("delete", "deletes an existing world", ExecuteStart, portOption);



            var builder = new CommandLineBuilder(root).UseDefaults().Build();
            var ingameCommands = new CommandLineBuilder(ingameRoot).UseDefaults().Build();
            var res = builder.Parse("execute stop");
            if (res.Errors.Count == 0)
                res.Invoke();

            res = ingameCommands.Parse("stop");
            if (res.Errors.Count == 0)
                res.Invoke();

            using (var typeContainer = TypeContainer.Get<ITypeContainer>())
            {
                Startup.Register(typeContainer);
                Startup.ConfigureLogger(ClientType.GameServer);

                Network.Startup.Register(typeContainer);

                var logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As("OctoAwesome.GameServer");
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    File.WriteAllText(
                        Path.Combine(".", "logs", $"server-dump-{DateTime.Now:ddMMyy_hhmmss}.txt"),
                        e.ExceptionObject.ToString());

                    string message = $"Unhandled Exception: {e.ExceptionObject}";
                    if (e.ExceptionObject is Exception ex)
                        logger.Fatal(message, ex);
                    else
                        logger.Fatal(message);

                    logger.Flush();
                };


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
                typeContainer.Register<ServerHandler>(InstanceBehavior.Singleton);
                typeContainer.Get<ServerHandler>().Start();

                using var manualResetEvent = new ManualResetEvent(false);
                Console.CancelKeyPress += (s, e) => manualResetEvent.Set();
                manualResetEvent.WaitOne();
                settings.Save();
            }
        }

        private static void ExecuteStart(bool obj)
        {
            throw new NotImplementedException();
        }


    }
}
