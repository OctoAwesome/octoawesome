using engenious;

using OctoAwesome.Location;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Interface for controlling players.
    /// </summary>
    public interface IPlayerController
    {
        /// <summary>
        /// Gets the position of the player.
        /// </summary>
        Coordinate Position { get; }

        /// <summary>
        /// Gets the radius of the player.
        /// </summary>
        float Radius { get; }

        /// <summary>
        /// Gets the planar player rotation.
        /// </summary>
        float Angle { get; }

        /// <summary>
        /// Gets the height of the player.
        /// </summary>
        float Height { get; }

        /// <summary>
        /// Gets a value indicating whether the player is on the ground.
        /// </summary>
        bool OnGround { get; }

        /// <summary>
        /// Gets the tilting angle of the player.
        /// </summary>
        float Tilt { get; }

        /// <summary>
        /// Gets or sets the movement vector of the player.
        /// </summary>
        Vector2 Move { get; set; }

        /// <summary>
        /// Gets or sets the head movement vector of the player.
        /// </summary>
        Vector2 Head { get; set; }

        /// <summary>
        /// Let the player character jump.
        /// </summary>
        void Jump();

        /// <summary>
        /// Starts player interaction with a block at a specified index.
        /// </summary>
        /// <param name="blockIndex">The index of the block to interact with</param>
        void Interact(Index3 blockIndex);

        /// <summary>
        /// Applies a block at a specific index.
        /// </summary>
        /// <param name="blockIndex">The index to apply the block at.</param>
        /// <param name="orientation">The orientation to apply the block with.</param>
        void Apply(Index3 blockIndex, OrientationFlags orientation);
    }
}
