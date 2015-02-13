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
    public sealed class Player : Item, IHaveInventory
    {
        private IInputSet input;

        public readonly float MAXSPEED = 10f;

        public float Radius { get; private set; }

        public float Angle { get; private set; }

        public bool OnGround { get; set; }

        public float Tilt { get; private set; }

        public PlayerState State { get; private set; }

        public IHaveInventory InteractionPartner { get; set; }

        public List<InventoryItem> InventoryItems { get; private set; }

        public Player(IInputSet input)
        {
            this.input = input;
            Position = new Vector3(50, 70, 50);
            Velocity = new Vector3(0, 0, 0);
            Radius = 0.75f;
            Angle = 0f;
            Mass = 100;
            InventoryItems = new List<InventoryItem>();
            InventoryItems.Add(new InventoryItem() { Name = "Apfel" });
        }

        public void Update(GameTime frameTime)
        {
            float Power = 50f;
            float JumpPower = 150000f;

            // Input verarbeiten
            Angle += (float)frameTime.ElapsedGameTime.TotalSeconds * input.HeadX;
            Tilt += (float)frameTime.ElapsedGameTime.TotalSeconds * input.HeadY;
            Tilt = Math.Min(1.5f, Math.Max(-1.5f, Tilt));

            float lookX = (float)Math.Cos(Angle);
            float lookY = (float)Math.Sin(Angle);
            var VelocityDirection = new Vector3(lookX, 0, lookY) * input.MoveY;

            float stafeX = (float)Math.Cos(Angle + MathHelper.PiOver2);
            float stafeY = (float)Math.Sin(Angle + MathHelper.PiOver2);
            VelocityDirection += new Vector3(stafeX, 0, stafeY) * input.MoveX;

            Vector3 Friction = new Vector3(1, 0, 1) * 10f;
            Vector3 powerdirection = new Vector3();

            if (OnGround)
            {
                powerdirection += Power * VelocityDirection;
                if (input.JumpTrigger)
                    powerdirection += new Vector3(0, JumpPower, 0);
            }

            Vector3 VelocityChange = 2.0f / Mass * (powerdirection - Friction * Velocity);

            Velocity += new Vector3(
                (float)(VelocityChange.X < 0 ? -Math.Sqrt(-VelocityChange.X) : Math.Sqrt(VelocityChange.X)),
                (float)(VelocityChange.Y < 0 ? -Math.Sqrt(-VelocityChange.Y) : Math.Sqrt(VelocityChange.Y)),
                (float)(VelocityChange.Z < 0 ? -Math.Sqrt(-VelocityChange.Z) : Math.Sqrt(VelocityChange.Z)));

            //Vector3 force = new Vector3();
            //force += new Vector3(lookX, 0, lookY) * input.MoveY;
            //force += new Vector3(stafeX, 0, stafeY) * input.MoveX;
            //force -= Velocity * 0.7f;
            //force += new Vector3(0, -10, 0);
            // Vector3 acceleration = force / Mass;

            // Velocity += acceleration * (float)frameTime.ElapsedGameTime.TotalSeconds;

            // Bewegungsberechnung
            //if (Velocity.Length() > 0f)
            //{
            //    Velocity *= MAXSPEED;
            //    State = PlayerState.Walk;
            //}
            //else
            //{
            //    State = PlayerState.Idle;
            //}

            //int cellX = (int)Position.X;
            //int cellY = (int)Position.Y;

            //// Umrechung in Grad
            //float direction = (Angle * 360f) / (float)(2 * Math.PI);
            //direction += 180;
            //direction += 45;
            //int sector = (int)(direction / 90);

            //switch (sector)
            //{
            //    case 1: // oben
            //        cellY -= 1;
            //        break;
            //    case 2: // rechts
            //        cellX += 1;
            //        break;
            //    case 3: // unten
            //        cellY += 1;
            //        break;
            //    case 4: // links
            //        cellX -= 1;
            //        break;
            //}

            //// Interaktion überprüfen
            //if (input.Interact && InteractionPartner == null)
            //{
            //    InteractionPartner = map.Items.
            //        Where(i => (int)i.Position.X == cellX && (int)i.Position.Y == cellY).
            //        OfType<IHaveInventory>().
            //        FirstOrDefault();
            //}

            //if (InteractionPartner != null)
            //{
            //    var partner = map.Items.
            //        Where(i => (int)i.Position.X == cellX && (int)i.Position.Y == cellY).
            //        OfType<IHaveInventory>().
            //        FirstOrDefault();

            //    if (InteractionPartner != partner)
            //        InteractionPartner = null;
            //}
        }
    }

    public enum PlayerState
    {
        Idle,
        Walk
    }
}
