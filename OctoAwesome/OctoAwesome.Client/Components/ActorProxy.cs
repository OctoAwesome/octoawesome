using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OctoAwesome.Runtime
{
    public class ActorProxy : IPlayerController
    {
        public float Angle
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool FlyMode
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public Vector2 Head
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public float Height
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<InventorySlot> Inventory
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Vector2 Move
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool OnGround
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public Coordinate Position
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float Radius
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float Tilt
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Apply(Index3 blockIndex, InventorySlot tool, OrientationFlags orientation)
        {
            throw new NotImplementedException();
        }

        public void Interact(Index3 blockIndex)
        {
            throw new NotImplementedException();
        }

        public void Jump()
        {
            throw new NotImplementedException();
        }
    }
}
