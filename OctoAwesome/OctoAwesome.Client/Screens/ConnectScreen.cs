using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoGameUi;
using OctoAwesome.Client.Components;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace OctoAwesome.Client.Screens
{
    class ConnectScreen : Screen
    {
        private Textbox ipBox, nameBox;

        public ConnectScreen(ScreenComponent manager) : base(manager)
        {
            Padding = Border.All(0);

            Image background = new Image(manager);
            background.Texture =
                Manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/background_notext.png",
                    Manager.GraphicsDevice);
            background.VerticalAlignment = VerticalAlignment.Stretch;
            background.HorizontalAlignment = HorizontalAlignment.Stretch;
            Controls.Add(background);

            ////////////////////////////////////////////Back Button////////////////////////////////////////////
            Button backButton = Button.TextButton(manager, "Back");
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.LeftMouseClick += (s, e) => { manager.NavigateBack(); };
            backButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(backButton);

            Texture2D panelTexture = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/panel.png",
                manager.GraphicsDevice);

            StackPanel stack = new StackPanel(manager);
            stack.Background = NineTileBrush.FromSingleTexture(panelTexture, 30, 30);
            stack.Padding = Border.All(30);
            Controls.Add(stack);

            StackPanel namePanel = new StackPanel(manager);
            namePanel.Orientation = Orientation.Horizontal;
            namePanel.Margin = new Border(0, 0, 0, 10);
            namePanel.Controls.Add(new Label(manager) {Text = "Username: "});

            nameBox = new Textbox(manager);
            nameBox.MinWidth = 500;
            nameBox.MinHeight = 30;
            nameBox.Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Black);
            namePanel.Controls.Add(nameBox);

            stack.Controls.Add(namePanel);

            StackPanel ipPanel = new StackPanel(manager);
            ipPanel.Orientation = Orientation.Horizontal;
            ipPanel.HorizontalAlignment = HorizontalAlignment.Right;
            ipPanel.Margin = new Border(0, 0, 0, 10);
            ipPanel.Controls.Add(new Label(manager) {Text = "IP: "});

            ///Die IP Box
            ipBox = new Textbox(manager);
            ipBox.MinWidth = 500;
            ipBox.MinHeight = 30;
            ipBox.Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Black);
            ipPanel.Controls.Add(ipBox);

            stack.Controls.Add(ipPanel);

            Button connect = Button.TextButton(manager, "Connect");
            connect.HorizontalAlignment = HorizontalAlignment.Stretch;
            connect.LeftMouseClick += Connect_LeftMouseClick;
            stack.Controls.Add(connect);
        }

        /// <summary>
        /// Wird aufgerufen on connect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Connect_LeftMouseClick(Control sender, MouseEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}