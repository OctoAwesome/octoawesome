using System;
using engenious.UI;
using engenious;
using engenious.Graphics;
using OctoAwesome.Client.UI.Components;

namespace OctoAwesome.Client.UI.Controls
{

    public class CrosshairControl : Control
    {
        private readonly Texture2D texture;
        private readonly float transparency;

        private readonly AssetComponent assets;

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

            transparency = 0.5f;

            texture = assets.LoadTexture(GetType(), "octocross");
        }
        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (!assets.Ready)
                return;

            var color = CrosshairColor;
            Width = Height = CrosshairSize;

            batch.Draw(texture, contentArea, color * transparency);
        }
    }
}
