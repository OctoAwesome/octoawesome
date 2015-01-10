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
    internal sealed class Player : Item, IHaveInventory
    {
        private Input2 input;

        private Map map;

        public readonly float MAXSPEED = 2f;

        public Vector2 Velocity { get; set; }

        public float Radius { get; private set; }

        public float Angle { get; private set; }

        public PlayerState State { get; private set; }

        public IHaveInventory InteractionPartner { get; set; }

        public List<InventoryItem> InventoryItems { get; private set; }

        public Player(Input2 input, Map map)
        {
            this.input = input;
            this.map = map;
            Position = new Vector2(0, 0);
            Velocity = new Vector2(0, 0);
            Radius = 0.1f;
            InventoryItems = new List<InventoryItem>();
            InventoryItems.Add(new InventoryItem() { Name = "Apfel" });
        }

        public void Update(GameTime frameTime)
        {
            // Bewegungsrichtung laut Input
            Velocity = new Vector2(
                (input.Left ? -1f : 0f) + (input.Right ? 1f : 0f),
                (input.Up ? -1f : 0f) + (input.Down ? 1f : 0f));

            // Bewegungsberechnung
            if (Velocity.Length() > 0f)
            {
                Velocity.Normalize();
                Velocity *= MAXSPEED;
                State = PlayerState.Walk;
                Angle = (float)Math.Atan2(Velocity.Y, Velocity.X);
            }
            else
            {
                State = PlayerState.Idle;
            }

            int cellX = (int)Position.X;
            int cellY = (int)Position.Y;

            // Umrechung in Grad
            float direction = (Angle * 360f) / (float)(2 * Math.PI);
            direction += 180;
            direction += 45;
            int sector = (int)(direction / 90);

            switch (sector)
            {
                case 1: // oben
                    cellY -= 1;
                    break;
                case 2: // rechts
                    cellX += 1;
                    break;
                case 3: // unten
                    cellY += 1;
                    break;
                case 4: // links
                    cellX -= 1;
                    break;
            }

            // Interaktion überprüfen
            if (input.Interact && InteractionPartner == null)
            {
                input.Interact = false;
                InteractionPartner = map.Items.
                    Where(i => (int)i.Position.X == cellX && (int)i.Position.Y == cellY).
                    OfType<IHaveInventory>().
                    FirstOrDefault();
            }

            if (InteractionPartner != null)
            {
                var partner = map.Items.
                    Where(i => (int)i.Position.X == cellX && (int)i.Position.Y == cellY).
                    OfType<IHaveInventory>().
                    FirstOrDefault();

                if (InteractionPartner != partner)
                    InteractionPartner = null;
            }
        }
    }

    internal enum PlayerState
    {
        Idle,
        Walk
    }
}
