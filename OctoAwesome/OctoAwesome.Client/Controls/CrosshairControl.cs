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

        AssetComponent assets;

        public CrosshairControl(ScreenComponent manager) : base(manager)
        {
            assets = manager.Game.Assets;

            Transparency = 0.5f;
            Color = Color.White;

            Texture = assets.LoadTexture(GetType(), "octocross");
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (!assets.Ready)
                return;

            batch.Draw(Texture, contentArea, Color * Transparency);
        }
    }
}
