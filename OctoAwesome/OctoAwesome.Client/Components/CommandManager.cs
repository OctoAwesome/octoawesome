using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OctoAwesome.Client.Controls;

namespace OctoAwesome.Client.Components
{
    static class CommandManager
    {
        private static Dictionary<string, CommandDelegate> commands = new Dictionary<string, CommandDelegate>();

        public static Dictionary<string, CommandDelegate> Commands => commands;

        public static void RegisterCommand(string command, CommandDelegate action)
        {
            commands.Add(command, action);
        }

        public static void ExecuteCommand(string command, string[] args)
        {
            if(!commands.ContainsKey(command))
                ConsoleControl.WriteLine("Command not found.");
            else
                commands[command]?.Invoke(args);
        }

        public delegate void CommandDelegate(String[] args);
    }


}
