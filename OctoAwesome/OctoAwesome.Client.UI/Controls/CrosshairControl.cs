using System;
using engenious.UI;
using engenious;
using engenious.Graphics;
using OctoAwesome.Client.UI.Components;

namespace OctoAwesome.Client.UI.Controls
{
    /// <summary>
    /// Control for displaying a crosshair.
    /// </summary>
    public class CrosshairControl : Control
    {
        private readonly Texture2D texture;
        private readonly float transparency;

        private readonly AssetComponent assets;

        private static int crosshairSize = 8;

        /// <summary>
        /// Gets or sets the size of the crosshair cursor.
        /// </summary>
        public static int CrosshairSize
        {
            get => crosshairSize;
            set => crosshairSize = Math.Clamp(value, 0, MaxSize);
        }

        /// <summary>
        /// Gets or sets the color of the crosshair cursor.
        /// </summary>
        public static Color CrosshairColor { get; set; } = Color.White;


        /// <summary>
        /// Maximum size of the crosshair cursor.
        /// </summary>
        public const int MaxSize = 100;

        /// <summary>
        /// Initializes a ne instance of the <see cref="CrosshairControl"/> control.
        /// </summary>
        /// <param name="manager">The <see cref="engenious.UI.BaseScreenComponent" />.</param>
        /// <param name="asset">The asset component to load resources from.</param>
        public CrosshairControl(BaseScreenComponent manager, AssetComponent asset) : base(manager)
        {
            assets = asset;

            transparency = 0.5f;

            texture = assets.LoadTexture(GetType(), "octocross");
        }

        /// <inheritdoc />
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
