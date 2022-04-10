using engenious;

namespace OctoAwesome
{
    /// <summary>
    /// Base class for entities that are updated.
    /// </summary>
    public abstract class UpdateableEntity : Entity
    {
        /// <summary>
        /// Gets called to update the entity.
        /// </summary>
        /// <param name="gameTime">The <see cref="GameTime"/>.</param>
        public abstract void Update(GameTime gameTime);

    }
}
