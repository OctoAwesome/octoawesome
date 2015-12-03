using System.Windows.Forms;
using System;

namespace OctoAwesome.Server
{
    public static class ServerConsole
    {
        private static RichTextBox console;

        //Zur späteren Einschränkung
        public static RichTextBox Console
        {
            get
            {
                return console;
            }
            set
            {
                console = value;
            }
        }

        /// <summary>
        /// Log Text mit Zeit
        /// </summary>
        /// <param name="message">Text</param>
        public static void Log(string message)
        {
            //Prüfe ob Konsole gesetzt
            if (console == null)
                return;

            var timeNow = DateTime.Now;                 //Aktuelle Zeit
            string time = timeNow.ToString("HH:mm:ss"); //Zeit formatieren
            string text = String.Format("[{0}] {1}\n", time, message); //Message erstellen
            WriteText(text);                         //Message in die Konsole schreiben
        }

        /// <summary>
        /// Text in die Konsole schreiben
        /// </summary>
        /// <param name="text">Der Text</param>
        private static void WriteText(string text)
        {
            console.AppendText(text);                   //Text zur Konsole hinzufügen
            console.SelectionStart = console.Text.Length; //Caret ans Ende setzen - für autoscroll wenn kein fokus
            console.ScrollToCaret();                    //Zum Caret scrollen - für autoscroll wenn kein fokus
        }
    }
}
