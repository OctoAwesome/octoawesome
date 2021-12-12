using OctoAwesome.Basics.Noise;

namespace OctoAwesome.Basics.Biomes
{

    public interface IBiome
    {

        IPlanet Planet { get; }
        float MinValue { get; }
        float MaxValue { get; }
        float ValueRangeOffset { get; }
        float ValueRange { get; }
        
        INoise BiomeNoiseGenerator { get; }

        void GetHeightmap(Index2 chunkIndex, float[] heightmap);
    }
}
