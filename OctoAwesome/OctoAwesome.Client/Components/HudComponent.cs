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
        private Texture2D pix;


        private List<Control> controls = new List<Control>();
        private Toolbar toolbar;
        private DebugInfos debugInfos;
        private Compass compass;

        public PlayerComponent Player { get; private set; }

        public HudComponent(Game game, PlayerComponent player)
            : base(game)
        {
            Player = player;

            controls.Add(toolbar = new Toolbar(this));
            controls.Add(debugInfos = new DebugInfos(this));
            controls.Add(compass = new Compass(this));
        }

        public override void Initialize()
        {
            batch = new SpriteBatch(Game.GraphicsDevice);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            pix = Game.Content.Load<Texture2D>("Textures/pix");

            toolbar.Position = new Index2(0, GraphicsDevice.Viewport.Height - 100);
            toolbar.Size = new Index2(GraphicsDevice.Viewport.Width, 100);
            debugInfos.Position = new Index2();
            debugInfos.Size = new Index2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            compass.Position = new Index2();
            compass.Size = new Index2(GraphicsDevice.Viewport.Width, 100);

            foreach (var control in controls)
                control.LoadContent();

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var control in controls)
                control.Draw(batch, gameTime);

            batch.Begin();

            int centerX = GraphicsDevice.Viewport.Width / 2;
            int centerY = GraphicsDevice.Viewport.Height / 2;

            batch.Draw(pix, new Rectangle(centerX - 1, centerY - 15, 2, 30), Color.White * 0.5f);
            batch.Draw(pix, new Rectangle(centerX - 15, centerY - 1, 30, 2), Color.White * 0.5f);

            batch.End();
        }
    }
}
