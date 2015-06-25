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

        private SpriteFont font;

        private Trigger<bool> debugTrigger = new Trigger<bool>();

        public DebugInfos(HudComponent hud) : base(hud)
        {
            framebuffer = new float[buffersize];
        }

        public override void LoadContent()
        {
            font = Hud.Game.Content.Load<SpriteFont>("Hud");
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

            batch.DrawString(font, "Development Version", new Vector2(5, 5), Color.White);

            string pos = "pos: " + Hud.Player.Player.Position.ToString();
            var size = font.MeasureString(pos);
            batch.DrawString(font, pos, new Vector2(Size.X - size.X - 5, 5), Color.White);

            float grad = (Hud.Player.Player.Angle / MathHelper.TwoPi) * 360;

            string rot = "rot: " +
                (((Hud.Player.Player.Angle / MathHelper.TwoPi) * 360) % 360).ToString("0.00") + " / " +
                ((Hud.Player.Player.Tilt / MathHelper.TwoPi) * 360).ToString("0.00");

            size = font.MeasureString(rot);
            batch.DrawString(font, rot, new Vector2(Size.X - size.X - 5, 25), Color.White);

            string fps = "fps: " + (1f / lastfps).ToString("0.00");
            size = font.MeasureString(fps);
            batch.DrawString(font, fps, new Vector2(Size.X - size.X - 5, 45), Color.White);

            if (Hud.Player.SelectedBox.HasValue)
            {
                string selection = "box: " +
                    Hud.Player.SelectedBox.Value.ToString() + " on " +
                    Hud.Player.SelectedSide.ToString() + " (" +
                    Hud.Player.SelectedPoint.Value.X.ToString("0.00") + "/" +
                    Hud.Player.SelectedPoint.Value.Y.ToString("0.00") + ") -> " +
                    Hud.Player.SelectedEdge.ToString() + " -> " + Hud.Player.SelectedCorner.ToString();
                size = font.MeasureString(selection);
                batch.DrawString(font, selection, new Vector2(5, Size.Y - size.Y - 5), Color.White);
            }

            batch.End();
        }
    }
}
