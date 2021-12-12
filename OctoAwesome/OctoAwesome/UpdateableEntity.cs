using engenious;

namespace OctoAwesome
{
    /// <summary>
    /// Entity die regelmäßig eine Updateevent bekommt
    /// </summary>
    public abstract class UpdateableEntity : Entity
    {
        /// <summary>
        /// Updatemethode der Entity
        /// </summary>
        /// <param name="gameTime">Spielzeit</param>
        public abstract void Update(GameTime gameTime);

    }
}
