using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Interface zur Player-Steuerung.
    /// </summary>
    public interface IPlayerController
    {
        /// <summary>
        /// Position des Spielers.
        /// </summary>
        Coordinate Position { get; }

        /// <summary>
        /// Radius des Spielers.
        /// </summary>
        float Radius { get; }

        /// <summary>
        /// Winkel des Spielers (Standposition).
        /// </summary>
        float Angle { get; }

        /// <summary>
        /// Höhe des Spielers.
        /// </summary>
        float Height { get; }

        /// <summary>
        /// Gibt an, ob der Spieler auf dem Boden steht.
        /// </summary>
        bool OnGround { get; }

        /// <summary>
        /// Winkel der Kopfstellung.
        /// </summary>
        float Tilt { get; }

        /// <summary>
        /// Gibt an, ob der Flugmodus aktiviert ist.
        /// </summary>
        bool FlyMode { get; set; }

        /// <summary>
        /// Gibt an, ob der Spieler ?.
        /// </summary>
        bool Crouch { get; set; }

        /// <summary>
        /// Gibt an, ob der Spieler sprintet.
        /// </summary>
        bool Sprint { get; set; }

        /// <summary>
        /// Bewegungsvektor des Spielers.
        /// </summary>
        Vector2 Move { get; set; }

        /// <summary>
        /// Kopfbewegeungsvektor des Spielers.
        /// </summary>
        Vector2 Head { get; set; }

        /// <summary>
        /// Das Inventar des Spielers.
        /// </summary>
        IEnumerable<InventorySlot> Inventory { get; }

        /// <summary>
        /// Den Spieler hüpfen lassen.
        /// </summary>
        void Jump();

        /// <summary>
        /// Lässt den Spieler mit einem Block Interagieren. (zur Zeit Block löschen)
        /// </summary>
        /// <param name="blockIndex">Die Position des Blocks.</param>
        void Interact(Index3 blockIndex);

        /// <summary>
        /// Lässt den Spieler ein Werkzeug auf einen Block anwenden. (zur Zeit Block setzen)
        /// </summary>
        /// <param name="blockIndex"></param>
        /// <param name="tool">Das anzuwendende Wekzeug.</param>
        /// <param name="orientation"></param>
        void Apply(Index3 blockIndex, InventorySlot tool, OrientationFlags orientation);
    }
}