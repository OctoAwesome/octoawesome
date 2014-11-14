using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public class Game
    {
        public Point Position { get; set; }

        public Point Velocity { get; set; }

        public Point PlaygroundSize { get; set; }

        public Game()
        {
            Position = new Point(0, 0);
            Velocity = new Point(100, 80);
        }

        public void Update(TimeSpan frameTime)
        {
            Position = new Point(
                Position.X + (int)(Velocity.X * frameTime.TotalSeconds),
                Position.Y + (int)(Velocity.Y * frameTime.TotalSeconds));

            if (Position.X < 0)
            {
                Velocity = new Point(-Velocity.X, Velocity.Y);
                Position = new Point(0, Position.Y);
            }

            if ((Position.X + 100) > PlaygroundSize.X)
            {
                Velocity = new Point(-Velocity.X, Velocity.Y);
                Position = new Point(PlaygroundSize.X - 100, Position.Y);
            }

            if (Position.Y < 0)
            {
                Velocity = new Point(Velocity.X, -Velocity.Y);
                Position = new Point(Position.X, 0);
            }

            if ((Position.Y + 100) > PlaygroundSize.Y)
            {
                Velocity = new Point(Velocity.X, -Velocity.Y);
                Position = new Point(Position.X, PlaygroundSize.Y - 100);
            }
        }
    }
}
