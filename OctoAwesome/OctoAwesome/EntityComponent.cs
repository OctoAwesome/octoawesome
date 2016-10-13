namespace OctoAwesome
{
    /// <summary>
    /// Base Class for all Entity Components.
    /// </summary>
    public abstract class EntityComponent
    {
        /// <summary>
        /// Reference to the Entity.
        /// </summary>
        public Entity Entity { get; set; }
    }
}
