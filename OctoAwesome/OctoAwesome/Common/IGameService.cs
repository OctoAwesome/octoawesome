using engenious;

using OctoAwesome.Chunking;
using OctoAwesome.Definitions;
using OctoAwesome.Location;

using System;

namespace OctoAwesome.Common
{
    /// <summary>
    /// Common Services for Extensions
    /// </summary>
    public interface IGameService : IServiceProvider
    {
        /// <summary>
        /// <see cref="IDefinitionManager"/> der lokalen Daten.
        /// </summary>
        IDefinitionManager DefinitionManager { get; }
        /// <summary>
        /// Berechnet die Geschwindigkeit einer <see cref="Entity"/> nach der Kollision mit der Welt.
        /// </summary>
        /// <param name="gameTime">Simulation time</param>
        /// <param name="position">Position der <see cref="Entity"/></param>
        /// <param name="cache"><see cref="ILocalChunkCache"/> as workspace</param>
        /// <param name="radius">Radius der <see cref="Entity"/></param>
        /// <param name="height">Höhe der <see cref="Entity"/></param>
        /// <param name="deltaPosition">Positionsänderung zwischen zwei Simulationsdurchläufen</param>
        /// <param name="velocity">Berechnete Geschwindigkeit</param>
        /// <exception cref="ArgumentNullException">Cache</exception>
        /// <returns>Geschwindigkeit der <see cref="Entity"/> nach der Killisionsprüfung</returns>
        Vector3 WorldCollision(GameTime gameTime, Coordinate position, ILocalChunkCache cache, float radius, float height,
            Vector3 deltaPosition, Vector3 velocity);

    }
}
