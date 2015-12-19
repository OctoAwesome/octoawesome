using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OctoAwesome.Server
{
   public static class ServerConsole
    {
        private static RichTextBox consoleTextBox;

        public static RichTextBox ConsoleTextbox { get
            {
                return consoleTextBox;
            }
            set
            {
                consoleTextBox = value;
            }
        }

        public static void Log(string Message)
        {
            DateTime time = DateTime.Now;
            string timeString = time.ToString("HH:mm:ss");
            string timedMessage = $"[{timeString}] {Message} \n";
            WriteToConsole(timedMessage);
        }

        private static void WriteToConsole(string Message)
        {
            if (consoleTextBox == null)
                return;

            MethodInvoker myDelegate = delegate() {
                consoleTextBox.AppendText(Message);
                consoleTextBox.SelectionStart = consoleTextBox.Text.Length;
                consoleTextBox.ScrollToCaret();
             };

            if (consoleTextBox.InvokeRequired)
            {
                consoleTextBox.Invoke(myDelegate);
            }
            else
                myDelegate();
        }
    }
}
