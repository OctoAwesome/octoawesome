using Microsoft.Xna.Framework;

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
        /// Bewegungsvektor des Spielers.
        /// </summary>
        Vector2 Move { get; set; }

        /// <summary>
        /// Kopfbewegeungsvektor des Spielers.
        /// </summary>
        Vector2 Head { get; set; }

        /// <summary>
        /// Den Spieler hüpfen lassen.
        /// </summary>
        void Jump();

        /// <summary>
        /// Lässt den Spieler einen Block entfernen.
        /// </summary>
        /// <param name="blockIndex"></param>
        void Interact(Index3 blockIndex);

        /// <summary>
        /// Setzt einen neuen Block.
        /// </summary>
        /// <param name="blockIndex"></param>
        /// <param name="orientation"></param>
        void Apply(Index3 blockIndex, OrientationFlags orientation);
    }
}
