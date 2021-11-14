using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public class TreePopulator : MapPopulator
    {
        private IEnumerable<ITreeDefinition> treeDefinitions = null;

        public TreePopulator()
        {
            Order = 10;
        }

        private static IChunkColumn getColumn(IChunkColumn column00, IChunkColumn column10, IChunkColumn column01, IChunkColumn column11, int x, int y)
        {
            IChunkColumn column;
            if (x >= Chunk.CHUNKSIZE_X && y >= Chunk.CHUNKSIZE_Y)
                column = column11;
            else if (x < Chunk.CHUNKSIZE_X && y >= Chunk.CHUNKSIZE_Y)
                column = column01;
            else if (x >= Chunk.CHUNKSIZE_X && y < Chunk.CHUNKSIZE_Y)
                column = column10;
            else
                column = column00;


            return column;
        }

        public override void Populate(IResourceManager resourceManager, IPlanet planet, IChunkColumn column00, IChunkColumn column10, IChunkColumn column01, IChunkColumn column11)
        {
            // Tree Definitions initialisieren
            if (treeDefinitions == null)
            {
                treeDefinitions = resourceManager.DefinitionManager.GetDefinitions<ITreeDefinition>().OrderBy(d => d.Order).ToArray();
                foreach (var treeDefinition in treeDefinitions)
                    treeDefinition.Init(resourceManager.DefinitionManager);
            }

            int salt = (column00.Index.X & 0xffff) + ((column00.Index.Y & 0xffff) << 16);
            Random random = new Random(planet.Seed + salt);

            Index3 sample = new Index3(column00.Index.X * Chunk.CHUNKSIZE_X, column00.Index.Y * Chunk.CHUNKSIZE_Y, column00.Heights[0, 0]);
            
            foreach (var treeDefinition in treeDefinitions)
            {
                int density = treeDefinition.GetDensity(planet, sample);
                if (density <= 0) continue;

                for (int i = 0; i < density; i++)
                {
                    int x = random.Next(Chunk.CHUNKSIZE_X / 2, Chunk.CHUNKSIZE_X * 3 / 2);
                    int y = random.Next(Chunk.CHUNKSIZE_Y / 2, Chunk.CHUNKSIZE_Y * 3 / 2);
                    int z = LocalBuilder.GetSurfaceHeight(column00, column10, column01, column11, x, y);

                    float blocktemp = planet.ClimateMap.GetTemperature(new Index3(column00.Index.X * Chunk.CHUNKSIZE_X,
                    column00.Index.Y * Chunk.CHUNKSIZE_X, z));

                    if (blocktemp > treeDefinition.MaxTemperature || blocktemp < treeDefinition.MinTemperature)
                        continue;

                    LocalBuilder builder = new LocalBuilder(x, y, z + 1, column00, column10, column01, column11);
                    treeDefinition.PlantTree(planet, new Index3(x, y, z), builder, random.Next(int.MaxValue));
                }
            }
        }
    }
}
