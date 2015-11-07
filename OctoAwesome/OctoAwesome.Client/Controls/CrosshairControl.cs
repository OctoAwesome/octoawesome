using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUi;
using OctoAwesome.Client.Components;

namespace OctoAwesome.Client.Controls
{
    class CrosshairControl : Control
    {

        ScreenComponent Manager;
        Texture2D Texture;

        public float Transparency;

        public Color Color;

        public CrosshairControl(ScreenComponent manager) : base(manager)
        {
            Manager = manager;
            Texture = manager.Content.Load<Texture2D>("Textures/crossocto");
            Transparency = 0.5f;
            Color = Color.WhiteSmoke;
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            batch.Draw(Texture,contentArea, Color * Transparency);
        }

    }
}
