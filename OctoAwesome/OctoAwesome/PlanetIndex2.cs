namespace OctoAwesome
{
    /// <summary>
    /// Datenstruktur zur genauen bestimmung einer Column und ihres Planeten
    /// </summary>
    public struct PlanetIndex2
    {
        /// <summary>
        /// Die Planeten-ID
        /// </summary>
        public int Planet;

        /// <summary>
        /// Die Position des Chunks
        /// </summary>
        public Index2 ColumnIndex;

        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse PlanetIndex2
        /// </summary>
        /// <param name="planet">Der Index des Planeten</param>
        /// <param name="columnIndex">Der <see cref="Index2"/> des Chunks</param>
        public PlanetIndex2(int planet, Index2 columnIndex)
        {
            Planet = planet;
            ColumnIndex = columnIndex;
        }

        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse PlanetIndex2
        /// </summary>
        /// <param name="planet">Der Index des Planeten</param>
        /// <param name="x">X-Anteil des Indexes des Chunks</param>
        /// <param name="y">Y-Anteil des Indexes des Chunks</param>
        public PlanetIndex2(int planet, int x, int y) : this(planet, new Index2(x, y)) { }

        /// <summary>
        /// Überprüft, ob beide gegebenen PlanetIndex2 den gleichen Wert aufweisen.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator ==(PlanetIndex2 i1, PlanetIndex2 i2)
        {
            return i1.Equals(i2);
        }

        /// <summary>
        /// Überprüft, ob beide gegebenen PlanetIndex2 nicht den gleichen Wert aufweisen.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator !=(PlanetIndex2 i1, PlanetIndex2 i2)
        {
            return !i1.Equals(i2);
        }

        /// <summary>
        /// Überprüft, ob der gegebene PlanetIndex2 den gleichen Wert aufweist, wie das gegebene Objekt.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is PlanetIndex2))
                return false;

            PlanetIndex2 other = (PlanetIndex2)obj;
            return (
                other.Planet == Planet &&
                other.ColumnIndex.X == ColumnIndex.X &&
                other.ColumnIndex.Y == ColumnIndex.Y);
        }

        /// <summary>
        /// Erzeugt einen möglichst eindeutigen Hashcode des PlanetIndex2s
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return
                (Planet << 24) +
               (ColumnIndex.X << 16) +
               ColumnIndex.Y;
        }
    }
}
