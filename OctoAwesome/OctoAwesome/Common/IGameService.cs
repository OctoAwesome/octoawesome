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
        /// DefinitionManager
        /// </summary>
        IDefinitionManager DefinitionManager { get; }
        /// <summary>
        /// Take a Block from the World.
        /// </summary>
        /// <param name="controller"><see cref="IEntityController"/> of the <see cref="Entity"/></param>
        /// <param name="cache"><see cref="ILocalChunkCache"/> of the <see cref="Entity"/></param>
        /// <param name="block"><see cref="IInventory"/> of the <see cref="Entity"/></param>
        /// <returns>true if the block was successully taken</returns>
        bool TakeBlock(IEntityController controller, ILocalChunkCache cache, out IInventoryableDefinition block);
        /// <summary>
        /// Take a Block from the World and add it to the invetory.
        /// </summary>
        /// <param name="controller"><see cref="IEntityController"/> of the <see cref="Entity"/></param>
        /// <param name="cache"><see cref="ILocalChunkCache"/> of the <see cref="Entity"/></param>
        /// <param name="inventory">Inventory to store the item</param>
        /// <returns>true if the block was successully taken</returns>
        bool TakeBlock(IEntityController controller, ILocalChunkCache cache, IInventory inventory);
        /// <summary>
        /// Instanziert einen neuen local Chunk Cache.
        /// </summary>
        /// <param name="dimensions">Größe des Caches in Zweierpotenzen</param>
        /// <param name="range">Gibt die Range in alle Richtungen an.</param>
        /// <param name="passive">Indicats that the Cache is passive (no active load)</param>
        ILocalChunkCache GetLocalCache(bool passive, int dimensions, int range);
        /// <summary>
        /// Set a block or interact with a Tool.
        /// </summary>
        /// <param name="position"><see cref="Coordinate"/> of the <see cref="Entity"/></param>
        /// <param name="controller"><see cref="IEntityController"/> of the <see cref="Entity"/></param>
        /// <param name="cache"><see cref="ILocalChunkCache"/> of the <see cref="Entity"/></param>
        /// <param name="slot">Used <see cref="InventorySlot"/></param>
        /// <param name="inventory"><see cref="IInventory"/> of the <see cref="Entity"/></param>
        /// <param name="height">Height of the <see cref="Entity"/></param>
        /// <param name="radius">Radius of the <see cref="Entity"/></param>
        /// <returns>true if the interaction was successful</returns>
        bool InteractBlock(Coordinate position, float radius, float height, IEntityController controller,
            ILocalChunkCache cache, InventorySlot slot, IInventory inventory);
        /// <summary>
        /// Calculates the collison between an <see cref="Entity"/> and the world.
        /// </summary>
        /// <param name="gameTime">Simulation time</param>
        /// <param name="position">Position of the <see cref="Entity"/></param>
        /// <param name="cache"><see cref="ILocalChunkCache"/> as workspace</param>
        /// <param name="radius">radius of the <see cref="Entity"/></param>
        /// <param name="height">height of the <see cref="Entity"/></param>
        /// <param name="deltaPosition">Calculated delta position for this cycle</param>
        /// <param name="velocity">claculated velocity for this cycle</param>
        /// <returns>the velocity of the <see cref="Entity"/> after collision checks</returns>
        Vector3 WorldCollision(GameTime gameTime, Coordinate position, ILocalChunkCache cache, float radius, float height,
            Vector3 deltaPosition, Vector3 velocity);

    }
}
