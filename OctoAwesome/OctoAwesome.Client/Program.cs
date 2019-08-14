#region Using Statements
using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace OctoAwesome.Client
{
#if WINDOWS || LINUX
    /// <summary>
    /// The main class.
    /// </summary>
    public static class Program
    {
        static OctoGame game;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            var config = new LoggingConfiguration();

            config.AddRule(LogLevel.Trace, LogLevel.Fatal, new FileTarget("octoClient.logfile")
            {
                FileName = $"./logs/octoClient-{DateTime.Now.ToString("ddMMyy_hhmmss")}.log"
            });

            LogManager.Configuration = config;

            using (game = new OctoGame())
                game.Run(60,60);
        }

        public static void Restart()
        {
            game.Exit();
            using (game = new OctoGame())
                game.Run(60,60);
        }
    }
#endif
}
