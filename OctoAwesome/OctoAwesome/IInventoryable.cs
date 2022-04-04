namespace OctoAwesome
{
    /// <summary>
    /// Basis-Interface für alle im Inventar-Verwaltbaren Definitionen.
    /// </summary>
    public interface IInventoryable
    {
        /// <summary>
        /// Gibt das Volumen für eine Einheit an.
        /// </summary>
        int VolumePerUnit { get; }

        /// <summary>
        /// Gibt an, wie viele dieses Items im Inventar in einem Slot gestapelt werden können (in Anzahl der Blöcke, nicht in Vielfachen der <see cref="VolumePerUnit"/>!).
        /// </summary>
        int StackLimit { get; }
    }
}
