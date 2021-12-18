namespace OctoAwesome
{
    /// <summary>
    /// The possible simulation game states.
    /// </summary>
    public enum SimulationState
    {
        /// <summary>
        /// The game is in an undefined state.
        /// </summary>
        Undefined,

        /// <summary>
        /// The game is ready.
        /// </summary>
        Ready,

        /// <summary>
        /// The game is running.
        /// </summary>
        Running,

        /// <summary>
        /// The game is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The game was exited.
        /// </summary>
        Finished
    }
}
