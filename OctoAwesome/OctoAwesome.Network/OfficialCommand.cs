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
        /// For requesting a chunkChannel column.
        /// </summary>
        LoadColumn = 104,

        /// <summary>
        /// For sending a chunkChannel column.
        /// </summary>
        SaveColumn = 105,

    }
}
