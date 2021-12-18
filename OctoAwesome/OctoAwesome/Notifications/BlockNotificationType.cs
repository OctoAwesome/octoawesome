namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Enumeration of block notification types.
    /// </summary>
    public enum BlockNotificationType : byte
    {
        /// <summary>
        /// A single block changed.
        /// </summary>
        BlockChanged = 1,

        /// <summary>
        /// Multiple blocks changed.
        /// </summary>
        BlocksChanged = 2
    }
}
