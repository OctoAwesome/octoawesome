#region Using Statements
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
            using (var typeContainer = TypeContainer.Get<ITypeContainer>())
            {
                Startup.Register(typeContainer);
                Startup.ConfigureLogger(ClientType.DesktopClient);

                Network.Startup.Register(typeContainer);

                using (game = new OctoGame())
                    game.Run(60, 60);
            }
        }

        public static void Restart()
        {
            game.Exit();
            using (game = new OctoGame())
                game.Run(60, 60);
        }
    }
#endif
}
