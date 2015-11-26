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
        public ConnectScreen(ScreenComponent manager) : base(manager)
        {
            Padding = Border.All(0);

            Image background = new Image(manager);
            background.Texture = Manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/background_notext.png", Manager.GraphicsDevice);
            background.VerticalAlignment = VerticalAlignment.Stretch;
            background.HorizontalAlignment = HorizontalAlignment.Stretch;
            Controls.Add(background);

            ////////////////////////////////////////////Back Button////////////////////////////////////////////
            Button backButton = Button.TextButton(manager, "Back");
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateBack();
            };
            backButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(backButton);

            StackPanel stack = new StackPanel(manager);
            Controls.Add(stack);

            Textbox ipBox = new Textbox(manager);
            ipBox.MinWidth = 500;
            ipBox.MinHeight = 30;
            ipBox.Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Black);
            stack.Controls.Add(ipBox);

            Button connect = Button.TextButton(manager, "Connect");
            connect.HorizontalAlignment = HorizontalAlignment.Stretch;
            stack.Controls.Add(connect);
                
                
        }
    }
}
