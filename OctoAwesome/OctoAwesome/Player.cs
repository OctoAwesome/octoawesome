using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public sealed class Player : Item
    {
        public const float POWER = 600f;

        public const float JUMPPOWER = 800000f;

        public const float FRICTION = 60f;

        public float Radius { get; set; }

        // TODO: Angle immer hübsch kürzen
        public float Angle { get; set; }

        public float Height { get; set; }

        public bool OnGround { get; set; }

        public float Tilt { get; set; }

        public Player()
        {
            Position = new Coordinate(0, new Index3(16, 16, 100), Vector3.Zero);
            Velocity = new Vector3(0, 0, 0);
            Radius = 0.75f;
            Angle = 0f;
            Height = 3.5f;
            Mass = 100;
        }
    }
}
