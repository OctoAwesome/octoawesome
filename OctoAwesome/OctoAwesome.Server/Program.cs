using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OctoAwesome.Server
{
    static class Program
    {
        [DllImport("kernel32.dll", EntryPoint = "AllocConsole", SetLastError = true, CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int AllocConsole();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            bool isgui = true;
            if (args.Contains("-nogui"))
                isgui = false;
            if (isgui)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            else
            {
                if (!IsLinux)
                    AllocConsole();

                Runtime.Server.Instance.OnJoin += Instance_OnJoin;
                Runtime.Server.Instance.OnLeave += Instance_OnLeave;

                Runtime.Server.Instance.Open();

                while (Console.ReadKey() != new ConsoleKeyInfo('c', ConsoleKey.C, false, false, false))
                {

                }

                Runtime.Server.Instance.Close();
            }
        }

        private static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        private static void Instance_OnJoin(Client info)
        {
            Console.WriteLine("Client joined: " + info.Playername + " / Guid: " + info.ConnectionId.ToString());
        }

        private static void Instance_OnLeave(Client info)
        {
            Console.WriteLine("Client leaved: " + info.Playername + " / Guid: " + info.ConnectionId.ToString());
        }
    }
}
