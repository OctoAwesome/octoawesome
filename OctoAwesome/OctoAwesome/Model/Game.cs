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

        public PointF PlaygroundSize { get; set; }

        public Player Player { get; private set; }

        public Game(Input input)
        {
            Player = new Player(input);
        }

        public void Update(TimeSpan frameTime)
        {
            Player.Update(frameTime);

            if (Player.Position.X < 0)
            {
                Player.Position = new Vector2(0, Player.Position.Y);
            }

            if ((Player.Position.X + 100) > PlaygroundSize.X)
            {
                Player.Position = new Vector2(PlaygroundSize.X - 100, Player.Position.Y);
            }

            if (Player.Position.Y < 0)
            {
                Player.Position = new Vector2(Player.Position.X, 0);
            }

            if ((Player.Position.Y + 100) > PlaygroundSize.Y)
            {
                Player.Position = new Vector2(Player.Position.X, PlaygroundSize.Y - 100);
            }
        }
    }
}
