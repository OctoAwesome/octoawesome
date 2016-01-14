using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public interface IPlayerController
    {
        Coordinate Position { get; }

        float Radius { get; }

        float Angle { get; }

        float Height { get; }

        bool OnGround { get; }

        bool FlyMode { get; set; }

        float Tilt { get; }

        bool Crouch { get; set; }

        bool Sprint { get; set; }

        Vector2 Move { get; set; }

        Vector2 Head { get; set; }

        IEnumerable<InventorySlot> Inventory { get; }

        void Jump();

        void Interact(Index3 blockIndex);

        void Apply(Index3 blockIndex, InventorySlot tool, OrientationFlags orientation);
    }
}