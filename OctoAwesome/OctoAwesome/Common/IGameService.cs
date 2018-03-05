using engenious;
using OctoAwesome.Entities;
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
        /// Nimmt einen Block aus der Welt.
        /// </summary>
        /// <param name="controller"><see cref="IEntityController"/> of the <see cref="Entity"/></param>
        /// <param name="cache"><see cref="ILocalChunkCache"/> of the <see cref="Entity"/></param>
        /// <param name="block"><see cref="IInventory"/> of the <see cref="Entity"/></param>
        bool TakeBlock(IEntityController controller, ILocalChunkCache cache, out IInventoryableDefinition block);
        /// <summary>
        /// Nimmt einen Block aus der Welt und steckt ihn in ein Inventar.
        /// </summary>
        /// <param name="controller"><see cref="IEntityController"/> der <see cref="Entity"/></param>
        /// <param name="cache"><see cref="ILocalChunkCache"/> der <see cref="Entity"/></param>
        /// <param name="inventory">Returned <see cref="IInventoryableDefinition"/> on success.</param>
        bool TakeBlock(IEntityController controller, ILocalChunkCache cache, IInventory inventory);
        /// <summary>
        /// Gibt einen <see cref="ILocalChunkCache"/> zurück
        /// </summary>
        /// <param name="passive">Gibt an ob der Cache passiv ist</param>
        /// <param name="dimensions">Dimensionen des Caches</param>
        /// <param name="range">Ausdehnung des Caches</param>
        /// <returns></returns>
        ILocalChunkCache GetLocalCache(bool passive, int dimensions, int range);
        /// <summary>
        /// Setzt einen Block in die Welt oder interagiert mit einem Werkzeug und dem Block.
        /// </summary>
        /// <param name="position"><see cref="Coordinate"/> of the <see cref="Entity"/></param>
        /// <param name="controller"><see cref="IEntityController"/> of the <see cref="Entity"/></param>
        /// <param name="cache"><see cref="ILocalChunkCache"/> of the <see cref="Entity"/></param>
        /// <param name="slot">Used <see cref="InventorySlot"/></param>
        /// <param name="inventory"><see cref="IInventory"/> of the <see cref="Entity"/></param>
        /// <param name="height">Height of the <see cref="Entity"/></param>
        /// <param name="radius">Radius of the <see cref="Entity"/></param>
        bool InteractBlock(Coordinate position, float radius, float height, IEntityController controller,
            ILocalChunkCache cache, InventorySlot slot, IInventory inventory);
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
