using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal class DebugInfos : Control
    {
        private int buffersize = 10;
        private float[] framebuffer;
        private int bufferindex = 0;

        private int framecount = 0;
        private double seconds = 0;
        private double lastfps = 0f;

        public PlayerComponent Player { get; set; }

        private Trigger<bool> debugTrigger = new Trigger<bool>();

        public DebugInfos(IScreenManager screenManager, PlayerComponent player)
            : base(screenManager)
        {
            framebuffer = new float[buffersize];
            Player = player;
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
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

            batch.Begin();

            batch.DrawString(ScreenManager.NormalText, "Development Version", new Vector2(5, 5), Color.White);

            string pos = "pos: " + Player.ActorHost.Position.ToString();
            var size = ScreenManager.NormalText.MeasureString(pos);
            batch.DrawString(ScreenManager.NormalText, pos, new Vector2(Size.X - size.X - 5, 5), Color.White);

            float grad = (Player.ActorHost.Angle / MathHelper.TwoPi) * 360;

            string rot = "rot: " +
                (((Player.ActorHost.Angle / MathHelper.TwoPi) * 360) % 360).ToString("0.00") + " / " +
                ((Player.ActorHost.Tilt / MathHelper.TwoPi) * 360).ToString("0.00");

            size = ScreenManager.NormalText.MeasureString(rot);
            batch.DrawString(ScreenManager.NormalText, rot, new Vector2(Size.X - size.X - 5, 25), Color.White);

            string fps = "fps: " + (1f / lastfps).ToString("0.00");
            size = ScreenManager.NormalText.MeasureString(fps);
            batch.DrawString(ScreenManager.NormalText, fps, new Vector2(Size.X - size.X - 5, 45), Color.White);

            if (Player.SelectedBox.HasValue)
            {
                string selection = "box: " +
                    Player.SelectedBox.Value.ToString() + " on " +
                    Player.SelectedSide.ToString() + " (" +
                    Player.SelectedPoint.Value.X.ToString("0.00") + "/" +
                    Player.SelectedPoint.Value.Y.ToString("0.00") + ") -> " +
                    Player.SelectedEdge.ToString() + " -> " + Player.SelectedCorner.ToString();
                size = ScreenManager.NormalText.MeasureString(selection);
                batch.DrawString(ScreenManager.NormalText, selection, new Vector2(5, Size.Y - size.Y - 5), Color.White);
            }

            batch.End();
        }
    }
}
