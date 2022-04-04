#region Using Statements

using OctoAwesome.Logging;
using System;
using System.IO;

#endregion

namespace OctoAwesome.Client
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        static OctoGame? game;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var typeContainer = TypeContainer.Get<ITypeContainer>())
            {
                Startup.Register(typeContainer);
                Startup.ConfigureLogger(ClientType.DesktopClient);
                Network.Startup.Register(typeContainer);

                var logger = (typeContainer.GetOrNull<ILogger>() ?? NullLogger.Default).As("OctoAwesome.Client");
                AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                {
                    File.WriteAllText(
                        Path.Combine(".", "logs", $"client-dump-{DateTime.Now:ddMMyy_hhmmss}.txt"),
                        e.ExceptionObject.ToString());

                    string message = $"Unhandled Exception: {e.ExceptionObject}";
                    if (e.ExceptionObject is Exception exception)
                        logger.Fatal(message, exception);
                    else
                        logger.Fatal(message);

                    logger.Flush();
                };

                using (game = new OctoGame())
                    game.Run(60, 60);
            }
        }
        public static void Restart()
        {
            game?.Exit();
            using (game = new OctoGame())
                game.Run(60, 60);
        }
    }
}
