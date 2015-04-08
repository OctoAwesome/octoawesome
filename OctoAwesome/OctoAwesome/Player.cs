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
        private IInputSet input;

        public const float POWER = 600f;

        public const float JUMPPOWER = 800000f;

        public const float FRICTION = 60f;

        public float Radius { get; private set; }

        // TODO: Angle immer hübsch kürzen
        public float Angle { get; private set; }

        public float Height { get; private set; }

        public bool OnGround { get; set; }

        public float Tilt { get; private set; }

        public PlayerState State { get; private set; }

        public Player(IInputSet input)
        {
            this.input = input;
            Position = new Coordinate(0, new Index3(16, 16, 100), Vector3.Zero);
            Velocity = new Vector3(0, 0, 0);
            Radius = 0.75f;
            Angle = 0f;
            Height = 3.5f;
            Mass = 100;
        }

        public void Update(GameTime frameTime)
        {
            Vector3 externalPower = ((ExternalForce * ExternalForce) / (2 * Mass)) * (float)frameTime.ElapsedGameTime.TotalSeconds;

            // Input verarbeiten
            Angle += (float)frameTime.ElapsedGameTime.TotalSeconds * input.HeadX;
            Tilt += (float)frameTime.ElapsedGameTime.TotalSeconds * input.HeadY;
            Tilt = Math.Min(1.5f, Math.Max(-1.5f, Tilt));

            float lookX = (float)Math.Cos(Angle);
            float lookY = -(float)Math.Sin(Angle);
            var VelocityDirection = new Vector3(lookX, lookY, 0) * input.MoveY;

            float stafeX = (float)Math.Cos(Angle + MathHelper.PiOver2);
            float stafeY = -(float)Math.Sin(Angle + MathHelper.PiOver2);
            VelocityDirection += new Vector3(stafeX, stafeY, 0) * input.MoveX;

            Vector3 Friction = new Vector3(1, 1, 0.1f) * FRICTION;
            Vector3 powerdirection = new Vector3();

            powerdirection += ExternalForce;
            powerdirection += (POWER * VelocityDirection);
            // if (OnGround && input.JumpTrigger)
            if (input.JumpTrigger)
            {
                Vector3 jumpDirection = new Vector3(lookX, lookY, 0f) * input.MoveY * 0.1f;
                jumpDirection.Z = 1f;
                jumpDirection.Normalize();
                powerdirection += jumpDirection * JUMPPOWER;
            }

            Vector3 VelocityChange = (2.0f / Mass * (powerdirection - Friction * Velocity)) *
                (float)frameTime.ElapsedGameTime.TotalSeconds;

            Velocity += new Vector3(
                (float)(VelocityChange.X < 0 ? -Math.Sqrt(-VelocityChange.X) : Math.Sqrt(VelocityChange.X)),
                (float)(VelocityChange.Y < 0 ? -Math.Sqrt(-VelocityChange.Y) : Math.Sqrt(VelocityChange.Y)),
                (float)(VelocityChange.Z < 0 ? -Math.Sqrt(-VelocityChange.Z) : Math.Sqrt(VelocityChange.Z)));
        }
    }

    public enum PlayerState
    {
        Idle,
        Walk
    }
}
