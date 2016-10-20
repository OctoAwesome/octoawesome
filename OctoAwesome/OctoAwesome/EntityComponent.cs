namespace OctoAwesome
{
    /// <summary>
    /// Base Class for all Entity Components.
    /// </summary>
    public abstract class EntityComponent : Component
    {
        /// <summary>
        /// Reference to the Entity.
        /// </summary>
        public Entity Entity { get; set; }
    }
}
