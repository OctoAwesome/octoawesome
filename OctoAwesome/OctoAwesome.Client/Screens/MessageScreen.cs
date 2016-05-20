using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Screens
{
    internal sealed class MessageScreen : Screen
    {
        public MessageScreen(ScreenComponent manager, string title, string content, string button = "OK", Action<Control, MouseEventArgs> click = null) : base(manager)
        {
            IsOverlay = true;

            Texture2D panelBackground = manager.Game.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/panel.png", manager.GraphicsDevice);
            Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30);

            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;

            StackPanel spanel = new StackPanel(manager);
            Controls.Add(spanel);

            Label headLine = new Label(manager);
            headLine.Text = title;
            headLine.Font = Skin.Current.HeadlineFont;
            headLine.HorizontalAlignment = HorizontalAlignment.Stretch;
            spanel.Controls.Add(headLine);

            Label contentLabel = new Label(manager);
            contentLabel.Text = content;
            contentLabel.Font = Skin.Current.TextFont;
            contentLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
            spanel.Controls.Add(contentLabel);

            Button closeButton = Button.TextButton(manager, button);
            closeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            closeButton.LeftMouseClick += (s, e) => 
            {
                if (click != null)
                    click(s, e);
                else
                    manager.NavigateBack();
            };
            spanel.Controls.Add(closeButton);
        }
    }
}
