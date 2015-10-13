using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameUi;
using OctoAwesome.Client.Components;

namespace OctoAwesome.Client.Controls
{
    internal class DebugControl : Control
    {
        private int buffersize = 10;
        private float[] framebuffer;
        private int bufferindex = 0;

        private int framecount = 0;
        private double seconds = 0;
        private double lastfps = 0f;

        public PlayerComponent Player { get; set; }

        private Trigger<bool> debugTrigger = new Trigger<bool>();

        public DebugControl(ScreenComponent screenManager)
            : base(screenManager)
        {
            framebuffer = new float[buffersize];
            Player = screenManager.Player;
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            debugTrigger.Value = Keyboard.GetState().IsKeyDown(Keys.F10);
            if (debugTrigger)
                Visible = !Visible;

            if (!Visible || !Enabled)
                return;

            framecount++;
            seconds += gameTime.ElapsedGameTime.TotalSeconds;
            if (framecount == 10)
            {
                lastfps = seconds / framecount;
                framecount = 0;
                seconds = 0;
            }

            framebuffer[bufferindex++] = (float)gameTime.ElapsedGameTime.TotalSeconds;
            bufferindex %= buffersize;

            batch.DrawString(Skin.Current.TextFont, "Development Version", new Vector2(5, 5), Color.White);

            string pos = "pos: " + Player.ActorHost.Position.ToString();
            var size = Skin.Current.TextFont.MeasureString(pos);
            batch.DrawString(Skin.Current.TextFont, pos, new Vector2(contentArea.Width - size.X - 5, 5), Color.White);

            float grad = (Player.ActorHost.Angle / MathHelper.TwoPi) * 360;

            string rot = "rot: " +
                (((Player.ActorHost.Angle / MathHelper.TwoPi) * 360) % 360).ToString("0.00") + " / " +
                ((Player.ActorHost.Tilt / MathHelper.TwoPi) * 360).ToString("0.00");

            size = Skin.Current.TextFont.MeasureString(rot);
            batch.DrawString(Skin.Current.TextFont, rot, new Vector2(contentArea.Width - size.X - 5, 25), Color.White);

            string fps = "fps: " + (1f / lastfps).ToString("0.00");
            size = Skin.Current.TextFont.MeasureString(fps);
            batch.DrawString(Skin.Current.TextFont, fps, new Vector2(contentArea.Width - size.X - 5, 45), Color.White);

            if (Player.SelectedBox.HasValue)
            {
                string selection = "box: " +
                    Player.SelectedBox.Value.ToString() + " on " +
                    Player.SelectedSide.ToString() + " (" +
                    Player.SelectedPoint.Value.X.ToString("0.00") + "/" +
                    Player.SelectedPoint.Value.Y.ToString("0.00") + ") -> " +
                    Player.SelectedEdge.ToString() + " -> " + Player.SelectedCorner.ToString();
                size = Skin.Current.TextFont.MeasureString(selection);
                batch.DrawString(Skin.Current.TextFont, selection, new Vector2(5, contentArea.Height - size.Y - 5), Color.White);
            }
        }
    }
}
