using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    internal sealed class HudComponent : DrawableGameComponent
    {
        private WorldComponent world;

        private SpriteBatch batch;
        private SpriteFont font;
        private Texture2D pix;

        private int buffersize = 10;
        private float[] framebuffer;
        private int bufferindex = 0;

        public HudComponent(Game game, WorldComponent world) : base(game)
        {
            this.world = world;

            framebuffer = new float[buffersize];
        }

        public override void Initialize()
        {
            batch = new SpriteBatch(Game.GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = Game.Content.Load<SpriteFont>("hud");
            pix = Game.Content.Load<Texture2D>("Textures/pix");
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            framebuffer[bufferindex++] = (float)gameTime.ElapsedGameTime.TotalSeconds;
            bufferindex %= buffersize;

            batch.Begin();
            batch.DrawString(font, "Development Version", new Vector2(5, 5), Color.White);

            string pos = "pos: " + world.World.Player.Position.ToString();
            var size = font.MeasureString(pos);
            batch.DrawString(font, pos, new Vector2(GraphicsDevice.Viewport.Width - size.X - 5, 5), Color.White);

            string rot = "rot: " + world.World.Player.Angle.ToString("0.00") + " / " + world.World.Player.Tilt.ToString("0.00");
            size = font.MeasureString(rot);
            batch.DrawString(font, rot, new Vector2(GraphicsDevice.Viewport.Width - size.X - 5, 25), Color.White);

            string fps = "fps: " + (1f / (framebuffer.Sum() / buffersize)).ToString("0.00");
            size = font.MeasureString(fps);
            batch.DrawString(font, fps, new Vector2(GraphicsDevice.Viewport.Width - size.X - 5, 45), Color.White);

            int centerX = GraphicsDevice.Viewport.Width / 2;
            int centerY = GraphicsDevice.Viewport.Height / 2;

            batch.Draw(pix, new Rectangle(centerX - 1, centerY - 15, 2, 30), Color.White * 0.5f);
            batch.Draw(pix, new Rectangle(centerX - 15, centerY - 1, 30, 2), Color.White * 0.5f);

            batch.End();
        }
    }
}
