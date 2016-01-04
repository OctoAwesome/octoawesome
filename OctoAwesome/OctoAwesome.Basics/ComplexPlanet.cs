using OctoAwesome.Basics.Biomes;

namespace OctoAwesome.Basics
{
    public class ComplexPlanet : Planet
    {
        public int HEIGHTMAPDETAILS = 8;

        public float[,] Heightmap { get; private set; }

        public float[,,] CloudMap { get; private set; }

        public SurfaceBiomeGenerator BiomeGenerator { get; private set; }

        /// <summary>
        /// Konstruktor des komplexen Map-Generators
        /// </summary>
        /// <param name="id">ID des Planeten</param>
        /// <param name="universe">ID des Universums</param>
        /// <param name="size">Größe des Planeten in Zweierpotenzen Chunks</param>
        /// <param name="generator">Instanz des Map-Generators</param>
        /// <param name="seed">Seed des Zufallsgenerators</param>
        public ComplexPlanet(int id, int universe, Index3 size, IMapGenerator generator, int seed)
            : base(id, universe, size, seed)
        {
            BiomeGenerator = new SurfaceBiomeGenerator(this, 40);
            this.Heightmap = null;
            ClimateMap = new Climate.ComplexClimateMap(this);
        }
    }
}