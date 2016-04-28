using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoGameUi;
using OctoAwesome.Client.Components;
using engenious;
using engenious.Graphics;

namespace OctoAwesome.Client.Controls
{
    class CrosshairControl : Control
    {
        public Texture2D Texture;
        public float Transparency;
        public Color Color;

        public CrosshairControl(ScreenComponent manager) : base(manager)
        {
            Texture = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/octocross.png", manager.GraphicsDevice);
            Transparency = 0.5f;
            Color = Color.White;
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            batch.Draw(Texture, contentArea, Color * Transparency);
        }
    }
}
