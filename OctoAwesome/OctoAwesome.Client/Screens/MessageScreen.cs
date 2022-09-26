using engenious;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.UI.Components;
using System;
using System.Diagnostics;

namespace OctoAwesome.Client.Screens
{
    internal sealed class MessageScreen : OctoScreen
    {
        private readonly Panel panel;

        public MessageScreen(AssetComponent assets, string title, string content, string buttonText = "OK", Action<Control, MouseEventArgs>? buttonClick = null)
            : base(assets)
        {
            IsOverlay = true;
            Background = new BorderBrush(Color.Black * 0.5f);
            Title = title;

            panel = new Panel()
            {
                Padding = Border.All(20),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            Controls.Add(panel);

            StackPanel spanel = new StackPanel();
            panel.Controls.Add(spanel);

            Label headLine = new Label()
            {
                Text = title,
                Font = Skin.Current.HeadlineFont,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            spanel.Controls.Add(headLine);

            Label contentLabel = new Label()
            {
                Text = content,
                Font = Skin.Current.TextFont,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            spanel.Controls.Add(contentLabel);

            Button closeButton = new TextButton(buttonText);
            closeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
            closeButton.LeftMouseClick += (s, e) =>
            {
                if (buttonClick != null)
                    buttonClick(s, e);
                else
                    ScreenManager.NavigateBack();
            };
            spanel.Controls.Add(closeButton);

            var panelText = assets.LoadTexture("panel");

            Debug.Assert(panelText != null, nameof(panelText) + " != null");
            panel.Background = NineTileBrush.FromSingleTexture(panelText, 30, 30);
        }
    }
}
