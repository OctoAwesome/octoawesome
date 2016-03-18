namespace OctoAwesome
{
    /// <summary>
    /// Basisklasse für MapPopulators (diese erzeugen Dinge in der Welt)
    /// </summary>
    public abstract class MapPopulator : IMapPopulator
    {
        /// <summary>
        /// Gibt die Rangposition des Populators an [0...99]
        /// </summary>
        public int Order { get; protected set; }

        /// <summary>
        /// Versieht einen Chunk mit Items
        /// </summary>
        /// <param name="definitionManager">Definition Manager</param>
        /// <param name="planet">Index des Planeten</param>
        /// <param name="column00">TODO: Kommentieren</param>
        /// <param name="column01">TODO: Kommentieren</param>
        /// <param name="column10">TODO: Kommentieren</param>
        /// <param name="column11">TODO: Kommentieren</param>
        public abstract void Populate(IDefinitionManager definitionManager, IPlanet planet, IChunkColumn column00, IChunkColumn column01, IChunkColumn column10, IChunkColumn column11);
    }
}
