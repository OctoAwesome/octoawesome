using engenious.UI;
using System;
using engenious;
using engenious.Graphics;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Client.UI.Controls
{
    /// <summary>
    /// A control for displaying a dynamic compass.
    /// </summary>
    public class CompassControl : Control
    {
        private readonly Texture2D compassTexture;

        private readonly AssetComponent assets;

        private readonly HeadComponent headComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompassControl"/> class.
        /// </summary>
        /// <param name="screenManager">The <see cref="T:engenious.UI.BaseScreenComponent" />.</param>
        /// <param name="assets">The asset component to load resource assets from.</param>
        /// <param name="headComponent">The head component which determines the compass heading.</param>
        public CompassControl(BaseScreenComponent screenManager, AssetComponent assets, HeadComponent headComponent) : base(screenManager)
        {
            this.assets = assets;

            this.headComponent = headComponent;
            Padding = Border.All(7);

            Texture2D background = assets.LoadTexture("buttonLong_brown_pressed");
            Background = NineTileBrush.FromSingleTexture(background, 7, 7);
            compassTexture = assets.LoadTexture(GetType(), "compass");
        }

        /// <inheritdoc />
        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (headComponent is null || !assets.Ready)
                return;

            float compassValue = headComponent.Angle / (float)(2 * Math.PI);
            compassValue %= 1f;
            if (compassValue < 0)
                compassValue += 1f;

            int offset = (int)(compassTexture.Width * compassValue);
            offset -= contentArea.Width / 2;
            int offsetY = (compassTexture.Height - contentArea.Height) / 2;

            batch.Draw(compassTexture, new Rectangle(contentArea.X, contentArea.Y - offsetY, contentArea.Width, contentArea.Height), new Rectangle(offset, 0, contentArea.Width, contentArea.Height + offsetY), Color.White * alpha);
        }
    }
}
