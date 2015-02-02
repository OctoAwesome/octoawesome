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

        public Vector2 Velocity { get; set; }

        public float Radius { get; private set; }

        public float Angle { get; private set; }

        public float Jaw { get; private set; }

        public PlayerState State { get; private set; }

        public IHaveInventory InteractionPartner { get; set; }

        public List<InventoryItem> InventoryItems { get; private set; }

        public Player(IInputSet input)
        {
            this.input = input;
            Position = new Vector2(0, 0);
            Velocity = new Vector2(0, 0);
            Radius = 0.1f;
            Angle = 0f;
            InventoryItems = new List<InventoryItem>();
            InventoryItems.Add(new InventoryItem() { Name = "Apfel" });
        }

        public void Update(GameTime frameTime)
        {
            Angle += (float)frameTime.ElapsedGameTime.TotalSeconds * input.HeadX;

            float lookX = (float)Math.Cos(Angle);
            float lookY = (float)Math.Sin(Angle);

            Velocity = new Vector2(lookX, lookY) * input.MoveY;

            float stafeX = (float)Math.Cos(Angle + MathHelper.PiOver2);
            float stafeY = (float)Math.Sin(Angle + MathHelper.PiOver2);

            Velocity += new Vector2(stafeX, stafeY) * input.MoveX;

            Jaw += (float)frameTime.ElapsedGameTime.TotalSeconds * input.HeadY;
            Jaw = Math.Min(MathHelper.PiOver4, Math.Max(-MathHelper.PiOver4, Jaw));

            // Bewegungsberechnung
            if (Velocity.Length() > 0f)
            {
                Velocity *= MAXSPEED;
                State = PlayerState.Walk;
            }
            else
            {
                State = PlayerState.Idle;
            }

            int cellX = (int)Position.X;
            int cellY = (int)Position.Y;

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
