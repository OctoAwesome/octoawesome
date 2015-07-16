using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OctoAwesome.Client.Components.Hud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components
{
    internal class ScreenManagerComponent : DrawableGameComponent, IScreenManager
    {
        private SpriteBatch batch;
        private InputComponent input;

        private Dictionary<string, Screen> screens = new Dictionary<string, Screen>();

        private Screen ActiveScreen = null;

        public ScreenManagerComponent(Game game, InputComponent input)
            : base(game)
        {
            this.input = input;

            this.input.OnKeyDown += input_OnKeyDown;
            this.input.OnKeyUp += input_OnKeyUp;

            screens.Add("inventory", new InventoryScreen(this));
        }

        void input_OnKeyUp(Keys key)
        {
            if (key == Keys.Escape)
            {
                Close();
            }
        }

        void input_OnKeyDown(Keys key)
        {
            if (ActiveScreen != null)
            {
                foreach (var control in ActiveScreen.Controls)
                {
                    if (input.PointerPosition.X >= control.Position.X &&
                        input.PointerPosition.X <= control.Position.X + control.Size.X &&
                        input.PointerPosition.Y >= control.Position.Y &&
                        input.PointerPosition.Y <= control.Position.Y + control.Size.Y)
                    {
                        control.FireMouseUp();
                    }
                }
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Pix = Game.Content.Load<Texture2D>("Textures/pix");
            NormalText = Game.Content.Load<SpriteFont>("hud");
            batch = new SpriteBatch(GraphicsDevice);

            foreach (var screen in screens.Values)
                screen.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (input.InventoryTrigger)
            {
                ActiveScreen = screens["inventory"];
                input.ScreenMode = true;
            }

            if (ActiveScreen != null)
            {
                foreach (var control in ActiveScreen.Controls)
                {
                    control.IsHovered = (input.PointerPosition.X >= control.Position.X && 
                        input.PointerPosition.X <= control.Position.X + control.Size.X && 
                        input.PointerPosition.Y >= control.Position.Y && 
                        input.PointerPosition.Y <= control.Position.Y + control.Size.Y);
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            if (ActiveScreen != null)
                ActiveScreen.Draw(batch, gameTime);

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


        public void Close()
        {
            ActiveScreen = null;
            input.PointerPosition = ScreenSize / 2;
            input.ScreenMode = false;
        }
    }
}
