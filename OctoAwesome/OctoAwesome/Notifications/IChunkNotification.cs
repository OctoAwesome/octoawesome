namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Interface for chunk notifications.
    /// </summary>
    public interface IChunkNotification
    {
        /// <summary>
        /// Gets the position of the chunk that caused the notification.
        /// </summary>
        Index3 ChunkPos { get; }

        /// <summary>
        /// Gets the planet id of the planet the chunk resides in that caused the notification.
        /// </summary>
        int Planet { get; }
    }
}
