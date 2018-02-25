using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using engenious;
using engenious.Graphics;

namespace OctoAwesome.Client.Controls
{
    internal class CompassControl : Control
    {
        private Texture2D compassTexture;

        private AssetComponent assets;

        public PlayerComponent Player { get; set; }

        public CompassControl(ScreenComponent screenManager) : base(screenManager)
        {
            assets = screenManager.Game.Assets;

            Player = screenManager.Player;
            Padding = Border.All(7);

            Texture2D background = assets.LoadTexture(typeof(ScreenComponent), "buttonLong_brown_pressed");
            Background = NineTileBrush.FromSingleTexture(background, 7, 7);
            compassTexture = assets.LoadTexture(GetType(), "compass");
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (Player == null || Player.CurrentEntity == null || !assets.Ready)
                return;

            float compassValue = Player.HeadYaw / (float)(2 * Math.PI);
            compassValue %= 1f;
            if (compassValue < 0)
                compassValue += 1f;

            int offset = (int)(compassTexture.Width * compassValue);
            offset -= contentArea.Width / 2;
            int offsetY = (compassTexture.Height -contentArea.Height) / 2;

            batch.Draw(compassTexture, new Rectangle(contentArea.X,contentArea.Y-offsetY,contentArea.Width,contentArea.Height), new Rectangle(offset, 0, contentArea.Width, contentArea.Height+offsetY), Color.White * alpha);
        }
    }
}
