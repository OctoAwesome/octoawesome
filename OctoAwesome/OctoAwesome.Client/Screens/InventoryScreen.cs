using Microsoft.Xna.Framework.Graphics;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Screens
{
    internal sealed class InventoryScreen : Screen
    {
        public InventoryScreen(ScreenComponent manager) : base(manager)
        {
            IsOverlay = true;

            Texture2D panelBackground = manager.Content.Load<Texture2D>("Textures/panel");
            Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30);

            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
            Width = 600;
            Height = 400;

            Button closeButton = Button.TextButton(Manager, "Close");
            closeButton.LeftMouseClick += (s, e) => { Manager.NavigateBack(); };
            Controls.Add(closeButton);
        }

        protected override void OnKeyPress(KeyEventArgs args)
        {
            if (args.Key == Microsoft.Xna.Framework.Input.Keys.Escape)
            {
                args.Handled = true;
                Manager.NavigateBack();
            }
        }
    }
}
