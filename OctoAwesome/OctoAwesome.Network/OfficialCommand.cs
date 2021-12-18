namespace OctoAwesome.Network
{
    /// <summary>
    /// Enumeration of official networking commands.
    /// </summary>
    public enum OfficialCommand : ushort
    {
        //0 - 100 System Commands
        //100 - 200 General Commands
        /// <summary>
        /// For identifying clients.
        /// </summary>
        Whoami = 101,

        /// <summary>
        /// For requesting the universe.
        /// </summary>
        GetUniverse = 102,

        /// <summary>
        /// For requesting a planet.
        /// </summary>
        GetPlanet = 103,

        /// <summary>
        /// For requesting a chunk column.
        /// </summary>
        LoadColumn = 104,

        /// <summary>
        /// For sending a chunk column.
        /// </summary>
        SaveColumn = 105,


        //400 - 500 Notifications
        /// <summary>
        /// For notification of entity changes.
        /// </summary>
        EntityNotification = 401,

        /// <summary>
        /// For notification of chunk changes.
        /// </summary>
        ChunkNotification = 402
    }
}
