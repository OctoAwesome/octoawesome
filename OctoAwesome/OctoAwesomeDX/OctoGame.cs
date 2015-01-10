using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OctoAwesome.Components;
using OctoAwesome.Rendering;
using System;
using System.Linq;

namespace OctoAwesomeDX
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class OctoGame : Game
    {
        private int SPRITE_WIDTH = 57;
        private int SPRITE_HEIGHT = 57;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Input2 input;

        private Texture2D grass;
        private Texture2D sprite;
        private Texture2D tree;
        private Texture2D box;

        private CellTypeRenderer sandRenderer;
        private CellTypeRenderer waterRenderer;


        OctoAwesome.Model.Game game;

        public OctoGame()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            input = new Input2(this);
            Components.Add(input);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            game = new OctoAwesome.Model.Game(input);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            grass = Content.Load<Texture2D>("Textures/grass_center");

            sandRenderer = new CellTypeRenderer(Content, "sand");
            waterRenderer = new CellTypeRenderer(Content, "water");

            sprite = Content.Load<Texture2D>("Textures/sprite");
            tree = Content.Load<Texture2D>("Textures/tree");
            box = Content.Load<Texture2D>("Textures/box");

            game.Camera.SetRenderSize(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));
        }

        protected override void Update(GameTime gameTime)
        {
            game.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0.2470f, 0.0980f, 0f));

            spriteBatch.Begin();

            if (game == null)
                return;

            int cellX1 = Math.Max(0, (int)(game.Camera.ViewPort.X / game.Camera.SCALE));
            int cellY1 = Math.Max(0, (int)(game.Camera.ViewPort.Y / game.Camera.SCALE));

            int cellCountX = (int)(GraphicsDevice.Viewport.Width / game.Camera.SCALE) + 2;
            int cellCountY = (int)(GraphicsDevice.Viewport.Height / game.Camera.SCALE) + 2;

            int cellX2 = Math.Min(cellX1 + cellCountX, (int)(game.PlaygroundSize.X));
            int cellY2 = Math.Min(cellY1 + cellCountY, (int)(game.PlaygroundSize.Y));

            for (int x = cellX1; x < cellX2; x++)
            {
                for (int y = cellY1; y < cellY2; y++)
                {
                    OctoAwesome.Model.CellCache cell = game.Map.CellCache[x, y];


                    switch (cell.CellType)
                    {
                        case OctoAwesome.Model.CellType.Gras:
                            spriteBatch.Draw(grass, new Rectangle(
                                (int)(x * game.Camera.SCALE - game.Camera.ViewPort.X),
                                (int)(y * game.Camera.SCALE - game.Camera.ViewPort.Y),
                                (int)game.Camera.SCALE,
                                (int)game.Camera.SCALE), Color.White);
                            break;
                        case OctoAwesome.Model.CellType.Sand:
                            sandRenderer.Draw(spriteBatch, game, x, y);
                            break;
                        case OctoAwesome.Model.CellType.Water:
                            waterRenderer.Draw(spriteBatch, game, x, y);
                            break;
                    }
                }
            }

            foreach (var item in game.Map.Items.OrderBy(t => t.Position.Y))
            {
                if (item is OctoAwesome.Model.TreeItem)
                {
                    spriteBatch.Draw(tree, new Rectangle(
                                    (int)(item.Position.X * game.Camera.SCALE - game.Camera.ViewPort.X) - 30,
                                    (int)(item.Position.Y * game.Camera.SCALE - game.Camera.ViewPort.Y) - 118,
                                    (int)game.Camera.SCALE,
                                    (int)game.Camera.SCALE * 2), Color.White);
                }

                if (item is OctoAwesome.Model.BoxItem)
                {
                    spriteBatch.Draw(box, new Rectangle(
                                    (int)(item.Position.X * game.Camera.SCALE - game.Camera.ViewPort.X) - 32,
                                    (int)(item.Position.Y * game.Camera.SCALE - game.Camera.ViewPort.Y) - 35,
                                    (int)game.Camera.SCALE,
                                    (int)game.Camera.SCALE), Color.White);
                }

                if (item is OctoAwesome.Model.Player)
                {
                    int frame = (int)((gameTime.TotalGameTime.TotalMilliseconds / 250) % 4);

                    int offsetx = 0;
                    if (game.Player.State == OctoAwesome.Model.PlayerState.Walk)
                    {
                        switch (frame)
                        {
                            case 0: offsetx = 0; break;
                            case 1: offsetx = SPRITE_WIDTH; break;
                            case 2: offsetx = 2 * SPRITE_WIDTH; break;
                            case 3: offsetx = SPRITE_WIDTH; break;
                        }
                    }
                    else
                    {
                        offsetx = SPRITE_WIDTH;
                    }

                    // Umrechung in Grad
                    float direction = (game.Player.Angle * 360f) / (float)(2 * Math.PI);

                    // In positiven Bereich
                    direction += 180;

                    // Offset
                    direction += 45;

                    int sector = (int)(direction / 90);

                    int offsety = 0;
                    switch (sector)
                    {
                        case 1: offsety = 3 * SPRITE_HEIGHT; break;
                        case 2: offsety = 2 * SPRITE_HEIGHT; break;
                        case 3: offsety = 0 * SPRITE_HEIGHT; break;
                        case 4: offsety = 1 * SPRITE_HEIGHT; break;
                    }

                    Point spriteCenter = new Point(27, 48);

                    spriteBatch.Draw(sprite,
                        new Rectangle(
                            ((int)(game.Player.Position.X * game.Camera.SCALE) - game.Camera.ViewPort.X - spriteCenter.X),
                            ((int)(game.Player.Position.Y * game.Camera.SCALE) - game.Camera.ViewPort.Y - spriteCenter.Y), SPRITE_WIDTH, SPRITE_HEIGHT),
                        new Rectangle(offsetx, offsety, SPRITE_WIDTH, SPRITE_HEIGHT),
                        Color.White);
                }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
