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

        public PlayerComponent Player { get; set; }

        public CompassControl(ScreenComponent screenManager) : base(screenManager)
        {
            Player = screenManager.Player;
            Padding = Border.All(7);

            Background = NineTileBrush.FromSingleTexture(
                    screenManager.Game.Screen.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/UI/buttonLong_brown_pressed.png",
                        screenManager.GraphicsDevice), 7, 7);

            compassTexture = ScreenManager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/compass.png", ScreenManager.GraphicsDevice);
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (Player == null || Player.ActorHost == null)
                return;

            float compassValue = Player.ActorHost.Angle / (float)(2 * Math.PI);
            compassValue %= 1f;
            if (compassValue < 0)
                compassValue += 1f;

            int offset = (int)(compassTexture.Width * compassValue);
            offset -= contentArea.Width / 2;
            int offsetY = (-compassTexture.Height - contentArea.Height) / 2;

            batch.Draw(compassTexture, contentArea, new Rectangle(offset, offsetY, contentArea.Width, contentArea.Height), Color.White * alpha);
        }
    }
}
