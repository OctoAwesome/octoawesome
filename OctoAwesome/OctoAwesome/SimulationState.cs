namespace OctoAwesome
{
    /// <summary>
    /// Auflistung der Spielstatusse.
    /// </summary>
    public enum SimulationState
    {
        /// <summary>
        /// Undefinierter Spielstatus.
        /// </summary>
        Undefined,

        /// <summary>
        /// Das Spiel ist bereit.
        /// </summary>
        Ready,

        /// <summary>
        /// Das Spiel läuft.
        /// </summary>
        Running,

        /// <summary>
        /// Das Spiel ist pausiert.
        /// </summary>
        Paused,

        /// <summary>
        /// Das Spiel ist beendet.
        /// </summary>
        Finished
    }
}
