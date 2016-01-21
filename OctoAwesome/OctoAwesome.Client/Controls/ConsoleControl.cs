using Microsoft.Xna.Framework;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OctoAwesome.Client.Controls
{
    class ConsoleControl : StackPanel
    {
        //Kein Singleton, speichert aber immer die aktuelle Instanz
        private static ConsoleControl Instance;

        Label consoleText;
        Textbox consoleInput;
        ScrollContainer consoleTextScroll;

        ScreenComponent Manager;

        private int maxConsoleHeight;

        public int MaxConsoleHeight
        {
            get { return maxConsoleHeight; }
            set
            {
                maxConsoleHeight = value;
                this.MaxHeight = maxConsoleHeight + 30;
                consoleTextScroll.MaxHeight = maxConsoleHeight;
            }
        }

        public ConsoleControl(ScreenComponent manager) : base(manager)
        {
            Manager = manager;

            consoleTextScroll = new ScrollContainer(manager);
            consoleTextScroll.HorizontalAlignment = HorizontalAlignment.Stretch;
            consoleTextScroll.VerticalScrollbarVisible = false;
            Controls.Add(consoleTextScroll);

            MaxConsoleHeight = 300;

            consoleText = new Label(manager);
            consoleText.HorizontalAlignment = HorizontalAlignment.Stretch;
            consoleText.TextColor = Color.White;
            consoleText.HorizontalTextAlignment = HorizontalAlignment.Left;
            consoleTextScroll.Content = consoleText;

            consoleInput = new Textbox(manager);
            consoleInput.HorizontalAlignment = HorizontalAlignment.Stretch;
            consoleInput.Height = 30;
            consoleInput.Background = new BorderBrush(Color.Gray * 0.2f);
            consoleInput.TextColor = Color.White;
            Controls.Add(consoleInput);

            Instance = this;
        }

        protected override void OnVisibleChanged(PropertyEventArgs<bool> args)
        {
            base.OnVisibleChanged(args);
            if (args.NewValue == true)
            {
                consoleInput.Focus();
            }
            else
            {
                ClearInput();
                consoleInput.Unfocus();
            }
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (consoleInput.Focused == TreeState.Active)
            {
                if (args.Key == Microsoft.Xna.Framework.Input.Keys.Escape)
                {
                    Visible = false;
                    args.Handled = true;
                }

                if (args.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                {
                    HandleInput(consoleInput.Text);
                    args.Handled = true;

                }
                base.OnKeyPress(args);

                args.Handled = true;
            }
        }

        private void HandleInput(string input)
        {
            //Command
            if (input[0] == '/')
            {
                input = input.Substring(1);
                var parts = Regex.Matches(input, @"[\""].+?[\""]|[^ ]+")
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList();
                
                CommandManager.Instance.ExecuteCommand(parts[0], parts.Skip(1).ToArray());
            }
            //Kein Command
            else
            {
                //Kein Command, tu was auch immer...
                WriteMessage(input);
            }
            ClearInput();
        }

        private void ClearInput()
        {
            consoleInput.SelectionStart = 0;
            consoleInput.CursorPosition = 0;
            consoleInput.Text = "";
        }


        public void WriteMessage(string message)
        {
            if (Instance == null) return;
            consoleText.Text += message + "\n";
            consoleTextScroll.VerticalScrollDown();
        }

        public static void WriteLine(string message)
        {
            Instance?.WriteMessage(message);
        }
    }
}