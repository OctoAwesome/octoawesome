using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OctoAwesome.Runtime
{
    public class ActorProxy : IPlayerController
    {
        private IClient client;

        private List<InventorySlot> inventory = new List<InventorySlot>();

        public ActorProxy(IClient client)
        {
            this.client = client;
        }

        public float Angle { get; set; }

        private bool flyMode = false;

        public bool FlyMode
        {
            get
            {
                return flyMode;
            }

            set
            {
                if (flyMode != value)
                {
                    flyMode = value;
                    try
                    {
                        client.SetFlyMode(value);
                    }
                    catch (Exception ex) { }
                }
            }
        }

        private Vector2 head = Vector2.Zero;

        public Vector2 Head
        {
            get
            {
                return head;
            }

            set
            {
                if (head != value)
                {
                    head = value;
                    try
                    {
                        client.SetHead(value);
                    }
                    catch (Exception ex) { }
                }
            }
        }

        public float Height { get; set; }

        public IEnumerable<InventorySlot> Inventory
        {
            get
            {
                return inventory;
            }
        }

        private Vector2 move = Vector2.Zero;

        public Vector2 Move
        {
            get
            {
                return move;
            }

            set
            {
                if (move != value)
                {
                    move = value;
                    try
                    {
                        client.SetMove(value);
                    }
                    catch (Exception ex) { }
                }
            }
        }

        public bool OnGround { get; set; }

        public Coordinate Position { get; set; }

        public float Radius { get; set; }

        public float Tilt { get; set; }

        public void Apply(Index3 blockIndex, InventorySlot tool, OrientationFlags orientation)
        {
            try
            {
                client.Apply(blockIndex, tool, orientation);
            }
            catch (Exception ex)
            {

            }
        }

        public void Interact(Index3 blockIndex)
        {
            try
            {
                client.Interact(blockIndex);
            }
            catch (Exception ex)
            {

            }
        }

        public void Jump()
        {
            try
            {
                client.Jump();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
