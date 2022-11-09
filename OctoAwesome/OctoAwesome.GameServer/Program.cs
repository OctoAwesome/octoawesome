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

    internal class ServerContext : IDisposable
    {
        public Parser IngameCommandParser { get; set; }
        public Parser StartupCommandParser { get; set; }
        public ITypeContainer TypeContainer { get; set; }
        public ServerHandler ServerHandler { get; set; }
        public ISettings Settings { get; set; }

        private bool disposedValue;

        public ServerContext(Parser ingameCommandParser, Parser startupCommandParser, ITypeContainer typeContainer, ServerHandler serverHandler, ISettings settings)
        {
            IngameCommandParser = ingameCommandParser;
            StartupCommandParser = startupCommandParser;
            TypeContainer = typeContainer;
            ServerHandler = serverHandler;
            Settings = settings;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    TypeContainer.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

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

            //var res = builder.Parse("execute stop");
            //if (res.Errors.Count == 0)
            //    res.Invoke();

            //res = ingameCommands.Parse("stop");
            //if (res.Errors.Count == 0)
            //    res.Invoke();

            Startup.ConfigureLogger(ClientType.GameServer);
            var typeContainer = CreateTypeContainer();

            var logger = (TypeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As("OctoAwesome.GameServer");

            using ServerContext context = CreateServerContext(logger, typeContainer);

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

            Run(context, args);
        }

        private static void Run(ServerContext context, string[] args)
        {
            string? command = string.Join(' ', args);
            while (true)
            {
                ExecuteCommand(
                    context.ServerHandler.SimulationManager.IsRunning
                        ? context.IngameCommandParser
                        : context.StartupCommandParser,
                    command);

                command = Console.ReadLine();

            }
        }

        private static int ExecuteCommand(Parser parser, string? command)
        {
            if (command is null)
                return -1;

            var commandResult = parser.Parse(command);

            if (commandResult.Errors.Count > 0)
            {
                //Write the fucking manual and then read it!
                //TODO
            }

            return commandResult.Invoke();
        }

        private static Settings GetSettings(ILogger logger)
        {
            Settings settings;
            var fileInfo = new FileInfo(Path.Combine(".", "settings.json"));

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

            return settings;
        }

        private static ServerContext CreateServerContext(ILogger logger, ITypeContainer typeContainer)
        {
            Settings settings = GetSettings(logger);

            typeContainer.Register(settings);
            typeContainer.Register<ISettings, Settings>(settings);


            var sh = new ServerHandler(typeContainer);
            typeContainer.Register(sh);

            var portOption = new Option<ushort>("--port", ()=>8888, "Defines the port where the server listen on");
            var ingameRoot = new Command("execute");
            var root = new RootCommand("starts the server with default options")
            {
                ingameRoot
            };
            root.SetHandler(() => logger.Debug("Initial Root Command"));

            var stopCommand = new Command("stop");
            stopCommand.SetHandler(() => Console.WriteLine("Stopped"));
            ingameRoot.Add(stopCommand);

            root
                .Add("start", "this starts the server immediately", sh.Start, portOption)
                //.Add("wizard", "guides through the creation of a new world", ExecuteStart, portOption)
                //.Add("execute", "Run Command immediately after start", ExecuteStart, portOption)
                ;

            ingameRoot
                .Add("stop", "stops the server and saves the world", sh.Stop)
                .Add("start", "starts a previously stopped server", sh.Start, portOption)
                //    .Add("restart", "restarts the server after saving the world", ExecuteStart, portOption)
            //    .Add("shutdown", "force stops the server without saving", ExecuteStart, portOption)
                ;

            //ingameRoot
            //    .Create("world", "world related commands")
            //        .Add("save", "saves the world", ExecuteStart, portOption)
            //        .Add("create", "created a whole new world, with a fantastic point of view", ExecuteStart, portOption)
            //        .Add("load", "loads an existing world", ExecuteStart, portOption)
            //        .Add("delete", "deletes an existing world", ExecuteStart, portOption);

            var builder = new CommandLineBuilder(root).UseDefaults().Build();
            var ingameCommands = new CommandLineBuilder(ingameRoot).UseDefaults().Build();

            return new ServerContext
            (
                ingameCommands,
                builder,
                typeContainer,
                sh,
                settings
            );
        }

        private static ITypeContainer CreateTypeContainer()
        {
            var typeContainer = TypeContainer.Get<ITypeContainer>();
            Startup.Register(typeContainer);
            Network.Startup.Register(typeContainer);
            return typeContainer;
        }

        private static void ExecuteStart(bool obj)
        {
            throw new NotImplementedException();
        }


    }
}
