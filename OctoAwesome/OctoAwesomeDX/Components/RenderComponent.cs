using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    internal sealed class RenderComponent : DrawableGameComponent
    {
        private int SPRITE_WIDTH = 57;
        private int SPRITE_HEIGHT = 57;

        private SpriteBatch spriteBatch;

        private Texture2D grass;
        private Texture2D sprite;
        private Texture2D tree;
        private Texture2D box;

        private CellTypeRenderer sandRenderer;
        private CellTypeRenderer waterRenderer;

        private CameraComponent camera;
        private WorldComponent world;

        public RenderComponent(Game game, WorldComponent world, CameraComponent camera)
            : base(game)
        {
            this.camera = camera;
            this.world = world;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Create a new SpriteBatch, which can be used to draw textures.
            grass = Game.Content.Load<Texture2D>("Textures/grass_center");

            sandRenderer = new CellTypeRenderer(Game.Content, "sand");
            waterRenderer = new CellTypeRenderer(Game.Content, "water");

            sprite = Game.Content.Load<Texture2D>("Textures/sprite");
            tree = Game.Content.Load<Texture2D>("Textures/tree");
            box = Game.Content.Load<Texture2D>("Textures/box");
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(0.2470f, 0.0980f, 0f));

            spriteBatch.Begin();

            if (world == null)
                return;

            int cellX1 = Math.Max(0, (int)(camera.ViewPort.X / camera.SCALE));
            int cellY1 = Math.Max(0, (int)(camera.ViewPort.Y / camera.SCALE));

            int cellCountX = (int)(GraphicsDevice.Viewport.Width / camera.SCALE) + 2;
            int cellCountY = (int)(GraphicsDevice.Viewport.Height / camera.SCALE) + 2;

            int cellX2 = Math.Min(cellX1 + cellCountX, (int)(world.World.PlaygroundSize.X));
            int cellY2 = Math.Min(cellY1 + cellCountY, (int)(world.World.PlaygroundSize.Y));

            for (int x = cellX1; x < cellX2; x++)
            {
                for (int y = cellY1; y < cellY2; y++)
                {
                    OctoAwesome.Model.CellCache cell = world.World.Map.CellCache[x, y];


                    switch (cell.CellType)
                    {
                        case OctoAwesome.Model.CellType.Gras:
                            spriteBatch.Draw(grass, new Rectangle(
                                (int)(x * camera.SCALE - camera.ViewPort.X),
                                (int)(y * camera.SCALE - camera.ViewPort.Y),
                                (int)camera.SCALE,
                                (int)camera.SCALE), Color.White);
                            break;
                        case OctoAwesome.Model.CellType.Sand:
                            sandRenderer.Draw(spriteBatch, camera, world.World, x, y);
                            break;
                        case OctoAwesome.Model.CellType.Water:
                            waterRenderer.Draw(spriteBatch, camera, world.World, x, y);
                            break;
                    }
                }
            }

            foreach (var item in world.World.Map.Items.OrderBy(t => t.Position.Y))
            {
                if (item is OctoAwesome.Model.TreeItem)
                {
                    spriteBatch.Draw(tree, new Rectangle(
                                    (int)(item.Position.X * camera.SCALE - camera.ViewPort.X) - 30,
                                    (int)(item.Position.Y * camera.SCALE - camera.ViewPort.Y) - 118,
                                    (int)camera.SCALE,
                                    (int)camera.SCALE * 2), Color.White);
                }

                if (item is OctoAwesome.Model.BoxItem)
                {
                    spriteBatch.Draw(box, new Rectangle(
                                    (int)(item.Position.X * camera.SCALE - camera.ViewPort.X) - 32,
                                    (int)(item.Position.Y * camera.SCALE - camera.ViewPort.Y) - 35,
                                    (int)camera.SCALE,
                                    (int)camera.SCALE), Color.White);
                }

                if (item is OctoAwesome.Model.Player)
                {
                    int frame = (int)((gameTime.TotalGameTime.TotalMilliseconds / 250) % 4);

                    int offsetx = 0;
                    if (world.World.Player.State == OctoAwesome.Model.PlayerState.Walk)
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
                    float direction = (world.World.Player.Angle * 360f) / (float)(2 * Math.PI);

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
                            ((int)(world.World.Player.Position.X * camera.SCALE) - camera.ViewPort.X - spriteCenter.X),
                            ((int)(world.World.Player.Position.Y * camera.SCALE) - camera.ViewPort.Y - spriteCenter.Y), SPRITE_WIDTH, SPRITE_HEIGHT),
                        new Rectangle(offsetx, offsety, SPRITE_WIDTH, SPRITE_HEIGHT),
                        Color.White);
                }
            }

            spriteBatch.End();
        }
    }
}
