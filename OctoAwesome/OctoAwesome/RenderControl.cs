using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using OctoAwesome.Model;
using OctoAwesome.Components;
using OctoAwesome.Rendering;

namespace OctoAwesome
{
    internal partial class RenderControl : UserControl
    {
        private int SPRITE_WIDTH = 57;
        private int SPRITE_HEIGHT = 57;

        private Stopwatch watch = new Stopwatch();

        private readonly Game game;
        private readonly Image grass;
        private readonly Image sprite;
        private readonly Image tree;

        private readonly CellTypeRenderer sandRenderer;
        private readonly CellTypeRenderer waterRenderer;

        public RenderControl(Game game)
        {
            InitializeComponent();

            this.game = game;

            game.Camera.SetRenderSize(new Vector2(ClientSize.Width, ClientSize.Height));

            grass = Image.FromFile("Assets/grass_center.png");

            sandRenderer = new CellTypeRenderer("sand");
            waterRenderer = new CellTypeRenderer("water");

            sprite = Image.FromFile("Assets/sprite.png");
            tree = Image.FromFile("Assets/tree.png");

            watch.Start();
        }

        protected override void OnResize(EventArgs e)
        {
            if (game != null)
            {
                game.Camera.SetRenderSize(new Vector2(ClientSize.Width, ClientSize.Height));
            }
            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(Color.FromArgb(63, 25, 0));

            if (game == null)
                return;

            int cellX1 = Math.Max(0, (int)(game.Camera.ViewPort.X / game.Camera.SCALE));
            int cellY1 = Math.Max(0, (int)(game.Camera.ViewPort.Y / game.Camera.SCALE));

            int cellCountX = (int)(ClientSize.Width / game.Camera.SCALE) + 2;
            int cellCountY = (int)(ClientSize.Height / game.Camera.SCALE) + 2;

            int cellX2 = Math.Min(cellX1 + cellCountX, (int)(game.PlaygroundSize.X));
            int cellY2 = Math.Min(cellY1 + cellCountY, (int)(game.PlaygroundSize.Y));

            for (int x = cellX1; x < cellX2; x++)
            {
                for (int y = cellY1; y < cellY2; y++)
                {
                    switch (game.Map.GetCell(x, y))
                    {
                        case CellType.Gras:
                            e.Graphics.DrawImage(grass, new Rectangle(
                                (int)(x * game.Camera.SCALE - game.Camera.ViewPort.X),
                                (int)(y * game.Camera.SCALE - game.Camera.ViewPort.Y),
                                (int)game.Camera.SCALE,
                                (int)game.Camera.SCALE));
                            break;
                        case CellType.Sand:
                            sandRenderer.Draw(e.Graphics, game, x, y);
                            break;
                        case CellType.Water:
                            waterRenderer.Draw(e.Graphics, game, x, y);
                            break;
                    }
                }
            }

            foreach (var item in game.Map.Items.OrderBy(t => t.Position.Y))
            {
                if (item is TreeItem)
                {
                    e.Graphics.DrawImage(tree, new Rectangle(
                                    (int)(item.Position.X * game.Camera.SCALE - game.Camera.ViewPort.X) - 30,
                                    (int)(item.Position.Y * game.Camera.SCALE - game.Camera.ViewPort.Y) - 118,
                                    (int)game.Camera.SCALE,
                                    (int)game.Camera.SCALE * 2));
                }

                if (item is Player)
                {
                    int frame = (int)((watch.ElapsedMilliseconds / 250) % 4);

                    int offsetx = 0;
                    if (game.Player.State == PlayerState.Walk)
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

                    e.Graphics.DrawImage(sprite,
                        new RectangleF(
                            (game.Player.Position.X * game.Camera.SCALE) - game.Camera.ViewPort.X - spriteCenter.X,
                            (game.Player.Position.Y * game.Camera.SCALE) - game.Camera.ViewPort.Y - spriteCenter.Y, SPRITE_WIDTH, SPRITE_HEIGHT),
                        new RectangleF(offsetx, offsety, SPRITE_WIDTH, SPRITE_HEIGHT),
                        GraphicsUnit.Pixel);
                }
            }
        }      
    }
}
