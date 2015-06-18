using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components
{
    internal sealed class HudComponent : DrawableGameComponent
    {
        private PlayerComponent player;

        private SpriteBatch batch;
        private SpriteFont font;
        private Texture2D pix;

        private int buffersize = 10;
        private float[] framebuffer;
        private int bufferindex = 0;

        private int framecount = 0;
        private double seconds = 0;
        private double lastfps = 0f;

        private Texture2D[] toolTextures;

        public HudComponent(Game game, PlayerComponent player)
            : base(game)
        {
            this.player = player;

            framebuffer = new float[buffersize];
        }

        public override void Initialize()
        {
            batch = new SpriteBatch(Game.GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = Game.Content.Load<SpriteFont>("Hud");
            pix = Game.Content.Load<Texture2D>("Textures/pix");

            toolTextures = new Texture2D[player.Tools.Length];
            int index = 0;
            foreach (var tool in player.Tools)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    System.Drawing.Bitmap bitmap = tool.Textures.First();
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Seek(0, SeekOrigin.Begin);

                    toolTextures[index++] = Texture2D.FromStream(GraphicsDevice, stream);
                }
            }

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
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

            string pos = "pos: " + player.Player.Position.ToString();
            var size = font.MeasureString(pos);
            batch.DrawString(font, pos, new Vector2(GraphicsDevice.Viewport.Width - size.X - 5, 5), Color.White);

            float grad = (player.Player.Angle / MathHelper.TwoPi) * 360;

            string rot = "rot: " +
                (((player.Player.Angle / MathHelper.TwoPi) * 360) % 360).ToString("0.00") + " / " +
                ((player.Player.Tilt / MathHelper.TwoPi) * 360).ToString("0.00");

            size = font.MeasureString(rot);
            batch.DrawString(font, rot, new Vector2(GraphicsDevice.Viewport.Width - size.X - 5, 25), Color.White);

            string fps = "fps: " + (1f / lastfps).ToString("0.00");
            size = font.MeasureString(fps);
            batch.DrawString(font, fps, new Vector2(GraphicsDevice.Viewport.Width - size.X - 5, 45), Color.White);

            if (player.SelectedBox.HasValue)
            {
                string selection = "box: " + 
                    player.SelectedBox.Value.ToString() + " on " + 
                    player.SelectedSide.ToString() + " (" + 
                    player.SelectedPoint.Value.X.ToString("0.00") + "/" + 
                    player.SelectedPoint.Value.Y.ToString("0.00") + ") -> " + 
                    player.SelectedEdge.ToString() + " -> " + player.SelectedCorner.ToString();
                size = font.MeasureString(selection);
                batch.DrawString(font, selection, new Vector2(5, GraphicsDevice.Viewport.Height - size.Y - 5), Color.White);
            }

            if (player.Tools != null && player.Tools.Length > 0)
            {
                int width = player.Tools.Length * 32 + (player.Tools.Length - 1) * 10;
                int offset = (GraphicsDevice.Viewport.Width - width) / 2;
                int index = 0;
                foreach (var definition in BlockDefinitionManager.GetBlockDefinitions())
                {
                    batch.Draw(pix, new Rectangle(offset + (index * 42) - 2, GraphicsDevice.Viewport.Height - 60 - 2, 36, 36), 
                        player.Player.ActiveTool == definition ? Color.White : Color.DarkGray);
                    batch.Draw(toolTextures[index], new Rectangle(offset + (index * 42), GraphicsDevice.Viewport.Height - 60, 32, 32), Color.White);

                    index++;
                }
            }

            int centerX = GraphicsDevice.Viewport.Width / 2;
            int centerY = GraphicsDevice.Viewport.Height / 2;

            batch.Draw(pix, new Rectangle(centerX - 1, centerY - 15, 2, 30), Color.White * 0.5f);
            batch.Draw(pix, new Rectangle(centerX - 15, centerY - 1, 30, 2), Color.White * 0.5f);

            batch.End();
        }
    }
}
