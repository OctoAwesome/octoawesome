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

        public HudComponent(Game game, WorldComponent world) : base(game)
        {
            this.world = world;
        }

        public override void Initialize()
        {
            batch = new SpriteBatch(Game.GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            font = Game.Content.Load<SpriteFont>("hud");
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            batch.Begin();
            batch.DrawString(font, "Development Version", new Vector2(5, 5), Color.White);

            string pos = "pos: " + world.World.Player.Position.ToString();
            var size = font.MeasureString(pos);
            batch.DrawString(font, pos, new Vector2(GraphicsDevice.Viewport.Width - size.X - 5, 5), Color.White);

            string rot = "rot: " + world.World.Player.Angle.ToString("0.00") + " / " + world.World.Player.Tilt.ToString("0.00");
            size = font.MeasureString(rot);
            batch.DrawString(font, rot, new Vector2(GraphicsDevice.Viewport.Width - size.X - 5, 25), Color.White);

            batch.End();
        }
    }
}
