using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client
{
    internal class CustomSkin : Skin
    {
        public CustomSkin(ContentManager content, GraphicsDevice device, ScreenComponent manager) : base(content)
        {
            Texture2D button_texture = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/button.png",
                device);
            Texture2D buttonHovered_texture =
                manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/button_hovered.png", device);
            Texture2D buttonPressed_texture =
                manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/button_pressed.png", device);

            NineTileBrush button_brush = NineTileBrush.FromSingleTexture(button_texture, 10, 10);
            NineTileBrush buttonHovered_brush = NineTileBrush.FromSingleTexture(buttonHovered_texture, 10, 10);
            NineTileBrush buttonPressed_brush = NineTileBrush.FromSingleTexture(buttonPressed_texture, 10, 10);

            SoundEffect click = content.Load<SoundEffect>("click");
            SoundEffect hover = content.Load<SoundEffect>("rollover");

            StyleSkins.Add("button", (c) =>
            {
                if (c is Button)
                {
                    //c.Width = 200;
                    c.Height = 55;
                    c.Padding = new Border(0, 0, 0, 2);
                    Button button = c as Button;
                    button.ClickSound = click;
                    button.HoverSound = hover;
                    button.Background = button_brush;
                    button.HoveredBackground = buttonHovered_brush;
                    button.PressedBackground = buttonPressed_brush;
                }
            });
        }
    }
}