using engenious;
using engenious.Graphics;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;

namespace OctoAwesome.Client.Screens
{
    internal sealed class MessageScreen : Screen
    {
        public MessageScreen(ScreenComponent manager, string title, string content, string buttonText = "OK", Action<Control, MouseEventArgs> buttonClick = null) : base(manager)
        {
            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.5f);
            Title = title;

            Texture2D panelBackground = manager.Game.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/panel.png", manager.GraphicsDevice);
            Panel panel = new Panel(manager)
            {
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = Border.All(20),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Controls.Add(panel);

            StackPanel spanel = new StackPanel(manager);
            panel.Controls.Add(spanel);

            Label headLine = new Label(manager)
            {
                Text = title,
                Font = Skin.Current.HeadlineFont,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            spanel.Controls.Add(headLine);

            Label contentLabel = new Label(manager)
            {
                Text = content,
                Font = Skin.Current.TextFont,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            spanel.Controls.Add(contentLabel);

            Button closeButton = Button.TextButton(manager, buttonText);
            closeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            closeButton.LeftMouseClick += (s, e) => 
            {
                if (buttonClick != null)
                    buttonClick(s, e);
                else
                    manager.NavigateBack();
            };
            spanel.Controls.Add(closeButton);
        }
    }
}
