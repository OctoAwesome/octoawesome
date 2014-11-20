using OctoAwesome.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    internal sealed class Player
    {
        private Input input;

        public readonly float MAXSPEED = 100f;

        public Vector2 Position { get; set; }

        public float Radius { get; private set; }

        public float Angle { get; private set; }

        public PlayerState State { get; private set; }

        public Player(Input input)
        {
            this.input = input;
            Radius = 10;
        }

        public void Update(TimeSpan frameTime)
        {
            Vector2 velocity = new Vector2(
                (input.Left ? -1f : 0f) + (input.Right ? 1f : 0f), 
                (input.Up ? -1f : 0f) + (input.Down ? 1f : 0f));

            velocity = velocity.Normalized();

            if (velocity.Length() > 0f)
            {
                State = PlayerState.Walk;
                Angle = velocity.Angle();
                Position += (velocity * MAXSPEED * (float)frameTime.TotalSeconds);
            }
            else
            {
                State = PlayerState.Idle;
            }
        }
    }

    internal enum PlayerState
    {
        Idle,
        Walk
    }
}
