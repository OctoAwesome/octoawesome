using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Client.Components.Hud;
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
        private SpriteBatch batch;
        private SpriteFont font;
        private Texture2D pix;

        private int buffersize = 10;
        private float[] framebuffer;
        private int bufferindex = 0;

        private int framecount = 0;
        private double seconds = 0;
        private double lastfps = 0f;

        private List<Control> controls = new List<Control>();
        private Toolbar toolbar;

        public PlayerComponent Player { get; private set; }

        public HudComponent(Game game, PlayerComponent player)
            : base(game)
        {
            Player = player;

            framebuffer = new float[buffersize];

            toolbar = new Toolbar(this);
            controls.Add(toolbar);
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

            toolbar.Position = new Index2(0, GraphicsDevice.Viewport.Height - 100);
            toolbar.Size = new Index2(GraphicsDevice.Viewport.Width, 100);

            foreach (var control in controls)
                control.LoadContent();

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

            // SortMode auf BackToFront BlendState.NonPremultiplied

            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            batch.DrawString(font, "Development Version", new Vector2(5, 5), Color.White);

            string pos = "pos: " + Player.Player.Position.ToString();
            var size = font.MeasureString(pos);
            batch.DrawString(font, pos, new Vector2(GraphicsDevice.Viewport.Width - size.X - 5, 5), Color.White);

            float grad = (Player.Player.Angle / MathHelper.TwoPi) * 360;

            string rot = "rot: " +
                (((Player.Player.Angle / MathHelper.TwoPi) * 360) % 360).ToString("0.00") + " / " +
                ((Player.Player.Tilt / MathHelper.TwoPi) * 360).ToString("0.00");

            size = font.MeasureString(rot);
            batch.DrawString(font, rot, new Vector2(GraphicsDevice.Viewport.Width - size.X - 5, 25), Color.White);

            string fps = "fps: " + (1f / lastfps).ToString("0.00");
            size = font.MeasureString(fps);
            batch.DrawString(font, fps, new Vector2(GraphicsDevice.Viewport.Width - size.X - 5, 45), Color.White);

            if (Player.SelectedBox.HasValue)
            {
                string selection = "box: " + 
                    Player.SelectedBox.Value.ToString() + " on " + 
                    Player.SelectedSide.ToString() + " (" + 
                    Player.SelectedPoint.Value.X.ToString("0.00") + "/" + 
                    Player.SelectedPoint.Value.Y.ToString("0.00") + ") -> " + 
                    Player.SelectedEdge.ToString() + " -> " + Player.SelectedCorner.ToString();
                size = font.MeasureString(selection);
                batch.DrawString(font, selection, new Vector2(5, GraphicsDevice.Viewport.Height - size.Y - 5), Color.White);
            }

            int centerX = GraphicsDevice.Viewport.Width / 2;
            int centerY = GraphicsDevice.Viewport.Height / 2;

            batch.Draw(pix, new Rectangle(centerX - 1, centerY - 15, 2, 30), Color.White * 0.5f);
            batch.Draw(pix, new Rectangle(centerX - 15, centerY - 1, 30, 2), Color.White * 0.5f);

            foreach (var control in controls)
                control.Draw(batch);

            batch.End();
        }
    }
}
