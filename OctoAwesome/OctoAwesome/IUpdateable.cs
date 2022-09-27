using engenious;

namespace OctoAwesome
{
    /// <summary>
    /// Base interface for types that needs updating.
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        /// Gets called to update the updateable.
        /// </summary>
        /// <param name="gameTime">The <see cref="GameTime"/>.</param>
        public abstract void Update(GameTime gameTime);

    }
}
