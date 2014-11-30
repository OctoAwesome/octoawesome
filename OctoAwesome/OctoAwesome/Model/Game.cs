using OctoAwesome.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    internal sealed class Game
    {
        private Input input;

        public Camera Camera { get; private set; }

        public Vector2 PlaygroundSize
        {
            get
            {
                return new Vector2(Map.Columns, Map.Rows);
            }
        }

        public Map Map { get; private set; }

        public Player Player { get; private set; }

        public Game(Input input)
        {
            // Map = Map.Generate(20, 20, CellType.Gras);
            Map = Map.Load(@"C:\Users\Tom\Desktop\test.map");
            Player = new Player(input, Map);
            Camera = new Camera(this, input);
        }

        public void Update(TimeSpan frameTime)
        {
            Player.Update(frameTime);

            // Ermittlung der Oberflächenbeschaffenheit
            int cellX = (int)Player.Position.X;
            int cellY = (int)Player.Position.Y;
            CellType cellType = Map.GetCell(cellX, cellY);

            // Modifikation der Geschwindigkeit
            Vector2 velocity = Player.Velocity;
            switch (cellType)
            {
                case CellType.Gras:
                    velocity *= 0.8f;
                    break;
                case CellType.Sand:
                    velocity *= 1f;
                    break;
            }

            Player.Position += (velocity * (float)frameTime.TotalSeconds);

            // Block nach links (Kartenrand + nicht begehbare Zellen)
            if (velocity.X < 0)
            {
                float posLeft = Player.Position.X - Player.Radius;
                cellX = (int)posLeft;
                cellY = (int)Player.Position.Y;

                if (posLeft < 0)
                {
                    Player.Position = new Vector2(cellX + Player.Radius, Player.Position.Y);
                }

                if (cellX < 0 || Map.GetCell(cellX, cellY) == CellType.Water)
                {
                    Player.Position = new Vector2((cellX + 1) + Player.Radius, Player.Position.Y);
                }
            }

            // Block nach oben (Kartenrand + nicht begehbare Zellen)
            if (velocity.Y < 0)
            {
                float posTop = Player.Position.Y - Player.Radius;
                cellX = (int)Player.Position.X;
                cellY = (int)posTop;

                if (posTop < 0)
                {
                    Player.Position = new Vector2(Player.Position.X, cellY + Player.Radius);
                }

                if (cellY < 0 || Map.GetCell(cellX, cellY) == CellType.Water)
                {
                    Player.Position = new Vector2(Player.Position.X, cellY + 1 + Player.Radius);
                }
            }

            if (velocity.X > 0)
            {
                float posRight = Player.Position.X + Player.Radius;
                cellX = (int)posRight;
                cellY = (int)Player.Position.Y;

                if (cellX >= Map.Columns || Map.GetCell(cellX, cellY) == CellType.Water)
                {
                    Player.Position = new Vector2(cellX - Player.Radius, Player.Position.Y);
                }
            }

            if (velocity.Y > 0)
            {
                float posBottom = Player.Position.Y + Player.Radius;
                cellX = (int)Player.Position.X;
                cellY = (int)posBottom;

                if (cellY >= Map.Rows || Map.GetCell(cellX, cellY) == CellType.Water)
                {
                    Player.Position = new Vector2(Player.Position.X, cellY - Player.Radius);
                }
            }

            Camera.Update(frameTime);
        }
    }
}
