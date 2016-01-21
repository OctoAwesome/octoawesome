using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OctoAwesome.Client.Controls;

namespace OctoAwesome.Client.Components
{
    public class CommandManager
    {

        #region Singleton

        private static CommandManager _instance;

        public static CommandManager Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new CommandManager();
                return _instance;
            }
        }
        #endregion

        private Dictionary<string, CommandDelegate> commands = new Dictionary<string, CommandDelegate>();

        public Dictionary<string, CommandDelegate> Commands => commands;

        public void RegisterCommand(string command, CommandDelegate action)
        {
            commands.Add(command, action);
        }

        public void ExecuteCommand(string command, string[] args)
        {
            if(!commands.ContainsKey(command))
                ConsoleControl.WriteLine("Command not found.");
            else
                commands[command]?.Invoke(args);
        }

        public delegate void CommandDelegate(String[] args);
    }


}
