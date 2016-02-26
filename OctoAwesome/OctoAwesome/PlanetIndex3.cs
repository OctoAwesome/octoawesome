namespace OctoAwesome
{
    /// <summary>
    /// Datenstruktur zur genauen bestimmung eines Chunks und seinen Planeten
    /// </summary>
    public struct PlanetIndex3
    {
        /// <summary>
        /// Die Planeten-ID
        /// </summary>
        public int Planet;

        /// <summary>
        /// Die Position des Chunks
        /// </summary>
        public Index3 ChunkIndex;

        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse PlanetIndex3
        /// </summary>
        /// <param name="planet">Der Index des Planeten</param>
        /// <param name="chunkIndex">Der <see cref="Index3"/> des Chunks</param>
        public PlanetIndex3(int planet, Index3 chunkIndex)
        {
            Planet = planet;
            ChunkIndex = chunkIndex;
        }

        /// <summary>
        /// Erzeugt eine neue Instanz der Klasse PlanetIndex3
        /// </summary>
        /// <param name="planet">Der Index des Planeten</param>
        /// <param name="x">X-Anteil des Indexes des Chunks</param>
        /// <param name="y">Y-Anteil des Indexes des Chunks</param>
        /// <param name="z">Z-Anteil des Indexes des Chunks</param>
        public PlanetIndex3(int planet, int x, int y, int z) : this(planet, new Index3(x, y, z)) { }

        /// <summary>
        /// Überprüft, ob beide gegebenen PlanetIndex3 den gleichen Wert aufweisen.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator ==(PlanetIndex3 i1, PlanetIndex3 i2)
        {
            return i1.Equals(i2);
        }

        /// <summary>
        /// Überprüft, ob beide gegebenen PlanetIndex3 nicht den gleichen Wert aufweisen.
        /// </summary>
        /// <param name="i1"></param>
        /// <param name="i2"></param>
        /// <returns></returns>
        public static bool operator !=(PlanetIndex3 i1, PlanetIndex3 i2)
        {
            return !i1.Equals(i2);
        }

        /// <summary>
        /// Überprüft, ob der gegebene PlanetIndex3 den gleichen Wert aufweist, wie das gegebene Objekt.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (!(obj is PlanetIndex3))
                return false;

            PlanetIndex3 other = (PlanetIndex3)obj;
            return (
                other.Planet == Planet &&
                other.ChunkIndex.X == ChunkIndex.X &&
                other.ChunkIndex.Y == ChunkIndex.Y &&
                other.ChunkIndex.Z == ChunkIndex.Z);
        }

        /// <summary>
        /// Erzeugt einen möglichst eindeutigen Hashcode des PlanetIndex3s
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return
                (Planet << 24) +
               (ChunkIndex.X << 16) +
               (ChunkIndex.Y << 8) +
               ChunkIndex.Z;
        }
    }
}
