using Microsoft.Xna.Framework;
using OctoAwesome.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    public sealed class World
    {
        public Chunk Chunk { get; private set; }

        public Player Player { get; private set; }

        public World(IInputSet input)
        {
            Chunk = new Model.Chunk();
            Player = new Player(input);
        }

        public void Update(GameTime frameTime)
        {
            Player.Update(frameTime);

            // Ermittlung der Oberflächenbeschaffenheit
            int cellX = (int)Player.Position.X;
            int cellY = (int)Player.Position.Y;

            // Modifikation der Geschwindigkeit
            Vector2 velocity = Player.Velocity;
            // velocity *= cell.VelocityFactor;

            Vector2 newPosition = Player.Position + (velocity * (float)frameTime.ElapsedGameTime.TotalSeconds);

            // Block nach links (Kartenrand + nicht begehbare Zellen)
            if (velocity.X < 0)
            {
                float posLeft = newPosition.X - Player.Radius;
                cellX = (int)posLeft;
                cellY = (int)Player.Position.Y;

                if (posLeft < 0)
                {
                    newPosition = new Vector2(cellX + Player.Radius, newPosition.Y);
                }

                if (cellX < 0)
                {
                    newPosition = new Vector2((cellX + 1) + Player.Radius, newPosition.Y);
                }
            }

            // Block nach oben (Kartenrand + nicht begehbare Zellen)
            if (velocity.Y < 0)
            {
                float posTop = newPosition.Y - Player.Radius;
                cellX = (int)Player.Position.X;
                cellY = (int)posTop;

                if (posTop < 0)
                {
                    newPosition = new Vector2(newPosition.X, cellY + Player.Radius);
                }

                if (cellY < 0)
                {
                    newPosition = new Vector2(newPosition.X, cellY + 1 + Player.Radius);
                }
            }

            if (velocity.X > 0)
            {
                float posRight = newPosition.X + Player.Radius;
                cellX = (int)posRight;
                cellY = (int)Player.Position.Y;

                if (cellX >= Chunk.CHUNKSIZE_X)
                {
                    newPosition = new Vector2(cellX - Player.Radius, newPosition.Y);
                }
            }

            if (velocity.Y > 0)
            {
                float posBottom = newPosition.Y + Player.Radius;
                cellX = (int)Player.Position.X;
                cellY = (int)posBottom;

                if (cellY >= Chunk.CHUNKSIZE_Y)
                {
                    newPosition = new Vector2(newPosition.X, cellY - Player.Radius);
                }
            }

            Player.Position = newPosition;
        }
    }
}
