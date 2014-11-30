using OctoAwesome.Components;
using OctoAwesome.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Rendering
{
    internal class CellTypeRenderer
    {
        private readonly Image center;
        private readonly Image left;
        private readonly Image right;
        private readonly Image upper;
        private readonly Image lower;
        private readonly Image upperLeft_concave;
        private readonly Image upperRight_concave;
        private readonly Image lowerLeft_concave;
        private readonly Image lowerRight_concave;
        private readonly Image upperLeft_convex;
        private readonly Image upperRight_convex;
        private readonly Image lowerLeft_convex;
        private readonly Image lowerRight_convex;

        public CellTypeRenderer(string name)
        {
            center = Image.FromFile(string.Format("Assets/{0}_center.png", name));
            left = Image.FromFile(string.Format("Assets/{0}_left.png", name));
            right = Image.FromFile(string.Format("Assets/{0}_right.png", name));
            upper = Image.FromFile(string.Format("Assets/{0}_upper.png", name));
            lower = Image.FromFile(string.Format("Assets/{0}_lower.png", name));
            upperLeft_concave = Image.FromFile(string.Format("Assets/{0}_upperLeft_concave.png", name));
            upperRight_concave = Image.FromFile(string.Format("Assets/{0}_upperRight_concave.png", name));
            lowerLeft_concave = Image.FromFile(string.Format("Assets/{0}_lowerLeft_concave.png", name));
            lowerRight_concave = Image.FromFile(string.Format("Assets/{0}_lowerRight_concave.png", name));
            upperLeft_convex = Image.FromFile(string.Format("Assets/{0}_upperLeft_convex.png", name));
            upperRight_convex = Image.FromFile(string.Format("Assets/{0}_upperRight_convex.png", name));
            lowerLeft_convex = Image.FromFile(string.Format("Assets/{0}_lowerLeft_convex.png", name));
            lowerRight_convex = Image.FromFile(string.Format("Assets/{0}_lowerRight_convex.png", name));
        }

        public void Draw(Graphics g, Game game, int x, int y)
        {
            CellType centerType = game.Map.GetCell(x, y);

            g.DrawImage(center, new Rectangle(
                (int)(x * game.Camera.SCALE - game.Camera.ViewPort.X),
                (int)(y * game.Camera.SCALE - game.Camera.ViewPort.Y),
                (int)game.Camera.SCALE,
                (int)game.Camera.SCALE));

            bool emptyLeft =
                x > 0 &&
                game.Map.GetCell(x - 1, y) != centerType;
            bool emptyTop =
                y > 0 &&
                game.Map.GetCell(x, y - 1) != centerType;
            bool emptyRight =
                (x + 1) < game.Map.Columns &&
                game.Map.GetCell(x + 1, y) != centerType;
            bool emptyBottom =
                (y + 1) < game.Map.Rows &&
                game.Map.GetCell(x, y + 1) != centerType;

            bool emptyUpperLeft =
                x > 0 &&
                y > 0 &&
                game.Map.GetCell(x - 1, y - 1) != centerType;
            bool emptyUpperRight =
                (x + 1) < game.Map.Columns &&
                y > 0 &&
                game.Map.GetCell(x + 1, y - 1) != centerType;
            bool emptyLowerLeft =
                x > 0 &&
                (y + 1) < game.Map.Rows &&
                game.Map.GetCell(x - 1, y + 1) != centerType;
            bool emptyLowerRight =
                (x + 1) < game.Map.Columns &&
                (y + 1) < game.Map.Rows &&
                game.Map.GetCell(x + 1, y + 1) != centerType;

            // Gerade Kanten
            if (emptyLeft) DrawTexture(g, game.Camera, x, y, left);
            if (emptyRight) DrawTexture(g, game.Camera, x, y, right);
            if (emptyTop) DrawTexture(g, game.Camera, x, y, upper);
            if (emptyBottom) DrawTexture(g, game.Camera, x, y, lower);

            // Konvexe Ecken
            if (emptyLeft && emptyTop) DrawTexture(g, game.Camera, x, y, upperLeft_convex);
            if (emptyLeft && emptyBottom) DrawTexture(g, game.Camera, x, y, lowerLeft_convex);
            if (emptyRight && emptyTop) DrawTexture(g, game.Camera, x, y, upperRight_convex);
            if (emptyRight && emptyBottom) DrawTexture(g, game.Camera, x, y, lowerRight_convex);

            if (emptyUpperLeft && !emptyLeft && !emptyTop) DrawTexture(g, game.Camera, x, y, upperLeft_concave);
            if (emptyUpperRight && !emptyRight && !emptyTop) DrawTexture(g, game.Camera, x, y, upperRight_concave);
            if (emptyLowerLeft && !emptyLeft && !emptyBottom) DrawTexture(g, game.Camera, x, y, lowerLeft_concave);
            if (emptyLowerRight && !emptyRight && !emptyBottom) DrawTexture(g, game.Camera, x, y, lowerRight_concave);
        }

        private static void DrawTexture(Graphics g, Camera camera, int x, int y, Image image)
        {
            g.DrawImage(image, new Rectangle(
                (int)(x * camera.SCALE - camera.ViewPort.X),
                (int)(y * camera.SCALE - camera.ViewPort.Y),
                (int)camera.SCALE,
                (int)camera.SCALE));
        }
    }
}
