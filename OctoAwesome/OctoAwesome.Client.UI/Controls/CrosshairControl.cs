using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using engenious.UI;
using engenious;
using engenious.Graphics;
using OctoAwesome.Client.UI.Components;

namespace OctoAwesome.Client.UI.Controls
{
    public class CrosshairControl : Control
    {
        public Texture2D Texture;
        public float Transparency;
        public Color Color;

        AssetComponent assets;

        private static int crosshairSize = 8;

        /// <summary>
        /// Die Größe des Crosshair
        /// </summary>
        public static int CrosshairSize
        {
            get => crosshairSize;
            set => crosshairSize = Math.Clamp(value, 0, MaxSize);
        }

        /// <summary>
        /// Die Farbe des Crosshair
        /// </summary>
        public static Color CrosshairColor { get; set; } = Color.White;


        /// <summary>
        /// Maximum Größe des Crosshair
        /// </summary>
        public const int MaxSize = 100;


        public CrosshairControl(BaseScreenComponent manager, AssetComponent asset) : base(manager)
        {
            assets = asset;

            Transparency = 0.5f;

            Texture = assets.LoadTexture(GetType(), "octocross");
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (!assets.Ready)
                return;

            Color = CrosshairColor;
            Width = Height = CrosshairSize;

            batch.Draw(Texture, contentArea, Color * Transparency);
        }
    }
}
