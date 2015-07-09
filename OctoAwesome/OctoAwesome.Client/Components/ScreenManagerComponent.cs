using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Client.Components.Hud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components
{
    internal class ScreenManagerComponent : DrawableGameComponent, IScreenManager
    {
        private InputComponent input;

        private Dictionary<string, Screen> screens = new Dictionary<string, Screen>();

        private Screen ActiveScreen = null;

        public ScreenManagerComponent(Game game, InputComponent input)
            : base(game)
        {
            this.input = input;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Pix = Game.Content.Load<Texture2D>("Textures/pix");
            NormalText = Game.Content.Load<SpriteFont>("hud");

            screens.Add("inventory", new InventoryScreen(this));
        }

        public override void Update(GameTime gameTime)
        {
            //if (Input.InventoryTrigger)
            //    inventoryVisible = !inventoryVisible;
            //Input.ScreenMode = inventoryVisible;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        public Texture2D Pix { get; private set; }

        public SpriteFont NormalText { get; private set; }


        public Index2 ScreenSize
        {
            get
            {
                return new Index2(
                    (int)GraphicsDevice.Viewport.Width,
                    (int)GraphicsDevice.Viewport.Height);
            }
        }


        public ContentManager Content
        {
            get { return Game.Content; }
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return base.GraphicsDevice; }
        }
    }
}
