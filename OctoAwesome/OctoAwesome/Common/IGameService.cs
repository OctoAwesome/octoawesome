using engenious;

using OctoAwesome.Chunking;
using OctoAwesome.Definitions;
using OctoAwesome.Location;

using System;

namespace OctoAwesome.Common
{
    /// <summary>
    /// Common services for extensions.
    /// </summary>
    public interface IGameService : IServiceProvider
    {
        /// <summary>
        /// Gets the <see cref="IDefinitionManager"/> of the local data.
        /// </summary>
        IDefinitionManager DefinitionManager { get; }

        /// <summary>
        /// Calculates an entity's velocity after collision with the world. (Original Lassi)
        /// </summary>
        /// <param name="gameTime">The simulation time.</param>
        /// <param name="position">Position of the <see cref="Entity"/>.</param>
        /// <param name="cache"><see cref="ILocalChunkCache"/> to calculate the collisions in.</param>
        /// <param name="radius">The bounding collision radius of <see cref="Entity"/>.</param>
        /// <param name="height">The height of the <see cref="Entity"/>.</param>
        /// <param name="deltaPosition">The position change between two simulation steps.</param>
        /// <param name="velocity">The velocity of the <see cref="Entity"/>.</param>
        /// <returns>Calculated velocity <see cref="Entity"/> after the collision test.</returns>
        Vector3 WorldCollision(GameTime gameTime, Coordinate position, ILocalChunkCache cache, float radius, float height,
            Vector3 deltaPosition, Vector3 velocity);

    }
}
