#region Using Statements

using OctoAwesome.Logging;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

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
            string jsonString = "{\"a\":1, \"b\":12.5, \"c\":\"q\", \"d\":{ \"a\":[1,2,3,4,5,12.5] },\"e\":12,\"MetaData\":{  \"a\":[1,2,3,4,5,12.5]} }";

            var recipe = JsonSerializer.Deserialize<Crafting.Recipe>(jsonString);

            var res = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);

            var d = res["d"].Deserialize<Dictionary<string, JsonElement>>();
            
            var je = JsonSerializer.Deserialize<JsonElement>(jsonString);

            foreach (var item in je.EnumerateObject())
            {
                if (item.Value.ValueKind == JsonValueKind.Object)
                {
                    foreach (var item2 in item.Value.EnumerateObject())
                    {
                        var elem = item2.Value;
                        foreach (var item3 in elem.EnumerateArray())
                        {

                        }
                    }

                }
                else
                    ;
            }


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

        /// <summary>
        /// Restarts OctoAwesome.
        /// </summary>
        public static void Restart()
        {
            game?.Exit();
            using (game = new OctoGame())
                game.Run(60, 60);
        }
    }
}
