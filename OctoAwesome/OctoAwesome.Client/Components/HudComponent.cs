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
        private MiniMap miniMap;

        private InventoryScreen inventory;
        private bool inventoryVisible = false;

        public PlayerComponent Player { get; private set; }

        public InputComponent Input { get; private set; }

        public SceneComponent Scene { get; set; }

        public HudComponent(Game game, PlayerComponent player, SceneComponent scene, InputComponent input)
            : base(game)
        {
            Player = player;
            Scene = scene;
            Input = input;

            controls.Add(toolbar = new Toolbar(this));
            controls.Add(debugInfos = new DebugInfos(this));
            controls.Add(compass = new Compass(this));
            controls.Add(miniMap = new MiniMap(this));

            inventory = new InventoryScreen(this);
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
            
            compass.Position = new Index2((GraphicsDevice.Viewport.Width - 300) / 2, 10);
            compass.Size = new Index2(300, 20);

            miniMap.Size = new Index2(128, 128);
            miniMap.Position = new Index2(GraphicsDevice.Viewport.Width - 135, GraphicsDevice.Viewport.Height - 135);

            foreach (var control in controls)
                control.LoadContent();

            inventory.LoadContent();

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (Input.InventoryTrigger)
                inventoryVisible = !inventoryVisible;
            Input.ScreenMode = inventoryVisible;
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

            if (inventoryVisible)
            {
                inventory.Draw(batch, gameTime);
            }
        }
    }
}
