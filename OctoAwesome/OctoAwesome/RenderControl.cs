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

namespace OctoAwesome
{
    internal partial class RenderControl : UserControl
    {
        private int SPRITE_WIDTH = 57;
        private int SPRITE_HEIGHT = 57;

        private Stopwatch watch = new Stopwatch();

        private readonly Game game;
        private readonly Image grass;
        private readonly Image sand_center;
        private readonly Image sand_left;
        private readonly Image sand_right;
        private readonly Image sand_upper;
        private readonly Image sand_lower;
        private readonly Image sand_upperLeft_concave;
        private readonly Image sand_upperRight_concave;
        private readonly Image sand_lowerLeft_concave;
        private readonly Image sand_lowerRight_concave;
        private readonly Image sand_upperLeft_convex;
        private readonly Image sand_upperRight_convex;
        private readonly Image sand_lowerLeft_convex;
        private readonly Image sand_lowerRight_convex;
        private readonly Image water;
        private readonly Image sprite;

        public RenderControl(Game game)
        {
            InitializeComponent();

            this.game = game;

            game.Camera.SetRenderSize(new Vector2(ClientSize.Width, ClientSize.Height));

            grass = Image.FromFile("Assets/grass_center.png");
            sand_center = Image.FromFile("Assets/sand_center.png");
            sand_left = Image.FromFile("Assets/sand_left.png");
            sand_right = Image.FromFile("Assets/sand_right.png");
            sand_upper = Image.FromFile("Assets/sand_upper.png");
            sand_lower = Image.FromFile("Assets/sand_lower.png");
            sand_upperLeft_concave = Image.FromFile("Assets/sand_upperLeft_concave.png");
            sand_upperRight_concave = Image.FromFile("Assets/sand_upperRight_concave.png");
            sand_lowerLeft_concave = Image.FromFile("Assets/sand_lowerLeft_concave.png");
            sand_lowerRight_concave = Image.FromFile("Assets/sand_lowerRight_concave.png");
            sand_upperLeft_convex = Image.FromFile("Assets/sand_upperLeft_convex.png");
            sand_upperRight_convex = Image.FromFile("Assets/sand_upperRight_convex.png");
            sand_lowerLeft_convex = Image.FromFile("Assets/sand_lowerLeft_convex.png");
            sand_lowerRight_convex = Image.FromFile("Assets/sand_lowerRight_convex.png");
            water = Image.FromFile("Assets/water_center.png");
            sprite = Image.FromFile("Assets/sprite.png");

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
                            DrawSand(e.Graphics, x, y);
                            break;
                        case CellType.Water:
                            e.Graphics.DrawImage(water, new Rectangle(
                                (int)(x * game.Camera.SCALE - game.Camera.ViewPort.X),
                                (int)(y * game.Camera.SCALE - game.Camera.ViewPort.Y),
                                (int)game.Camera.SCALE,
                                (int)game.Camera.SCALE));
                            break;
                    }
                }
            }

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

            // e.Graphics.FillEllipse(brush, new Rectangle(Game.Position.X, Game.Position.Y, 100, 100));
        }

        private void DrawSand(Graphics g, int x, int y)
        {
            g.DrawImage(sand_center, new Rectangle(
                (int)(x * game.Camera.SCALE - game.Camera.ViewPort.X),
                (int)(y * game.Camera.SCALE - game.Camera.ViewPort.Y),
                (int)game.Camera.SCALE,
                (int)game.Camera.SCALE));

            if (x > 0)
            {
                if (game.Map.GetCell(x - 1, y) != CellType.Sand)
                {
                    g.DrawImage(sand_left, new Rectangle(
                        (int)(x * game.Camera.SCALE - game.Camera.ViewPort.X),
                        (int)(y * game.Camera.SCALE - game.Camera.ViewPort.Y),
                        (int)game.Camera.SCALE,
                        (int)game.Camera.SCALE));
                }
            }

            if (y > 0)
            {
                if (game.Map.GetCell(x, y-1) != CellType.Sand)
                {
                    g.DrawImage(sand_upper, new Rectangle(
                        (int)(x * game.Camera.SCALE - game.Camera.ViewPort.X),
                        (int)(y * game.Camera.SCALE - game.Camera.ViewPort.Y),
                        (int)game.Camera.SCALE,
                        (int)game.Camera.SCALE));
                }
            }

            if  (x > 0 && y > 0)
            {
                if (game.Map.GetCell(x - 1, y) != CellType.Sand &&
                    game.Map.GetCell(x, y - 1) != CellType.Sand)
                {
                    g.DrawImage(sand_upperLeft_convex, new Rectangle(
                        (int)(x * game.Camera.SCALE - game.Camera.ViewPort.X),
                        (int)(y * game.Camera.SCALE - game.Camera.ViewPort.Y),
                        (int)game.Camera.SCALE,
                        (int)game.Camera.SCALE));
                }
            }
        }
    }
}
