using OctoAwesome.Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Biomes
{
    public abstract class SuperBiomeBase : IBiome
    {

        public IPlanet Planet { get; private set; }

        public List<IBiome> SubBiomes { get; private set; }

        public INoise BiomeNoiseGenerator { get; protected set; }

        public float MinValue { get; protected set; }

        public float MaxValue { get; protected set; }

        public float ValueRangeOffset { get; protected set; }

        public float ValueRange { get; protected set; }

        public abstract float[,] GetHeightmap(Index2 chunkIndex);

        public SuperBiomeBase(IPlanet planet, float valueRangeOffset, float valueRange)
        {
            this.Planet = planet;
            this.SubBiomes = new List<IBiome>();
            this.ValueRangeOffset = valueRangeOffset;
            this.ValueRange = valueRange;
        }

        protected void SortSubBiomes()
        {
            SubBiomes = SubBiomes.OrderBy(a => a.MinValue).ToList();

            if (SubBiomes.Count > 0 && (SubBiomes.First().MinValue > 0f || SubBiomes.Last().MaxValue < 1f))
            {
                throw new InvalidOperationException("MinValue oder MaxValue der Biome nicht in gültigem Bereich");
            }
        }

        protected IBiome ChooseBiome(float value, out IBiome secondBiome)
        {

            secondBiome = null;
            bool betweenPossible = false;
            for (int i = 0; i < SubBiomes.Count; i++)
            {
                if (betweenPossible && value < SubBiomes[i].MinValue)
                {
                    secondBiome = SubBiomes[i];
                    return SubBiomes[i - 1];
                }
                else if (SubBiomes[i].MaxValue >= value && SubBiomes[i].MinValue <= value)
                    return SubBiomes[i];
                betweenPossible = (value > SubBiomes[i].MaxValue);
            }
            return null;
        }

        protected int ChooseBiome(float value, out int secondBiome)
        {
            secondBiome = -1;
            bool betweenPossible = false;
            for (int i = 0; i < SubBiomes.Count; i++)
            {
                if (betweenPossible && value < SubBiomes[i].MinValue)
                {
                    secondBiome = i;
                    return i - 1;
                }
                else if (SubBiomes[i].MaxValue >= value && SubBiomes[i].MinValue <= value)
                    return i;
                betweenPossible = (value > SubBiomes[i].MaxValue);
            }
            return -1;
        }

        protected float CalculateInterpolationValue(float region, out IBiome biome1, out IBiome biome2)
        {
            biome1 = ChooseBiome(region, out biome2);

            return CalculateInterpolationValue(region, biome1, biome2);
        }

        protected float CalculateInterpolationValue(float region, IBiome biome1, IBiome biome2)
        {
            if (biome2 != null)
            {
                float diff = biome2.MinValue - biome1.MaxValue;
                region -= biome1.MaxValue;
                region /= diff;
                return CurveFunction(region);
            }
            else if (biome1 != null)
            {
                return 0f;
            }
            return 0f;
        }

        protected virtual float CurveFunction(float inputValue)
        {
            return inputValue;
        }
    }
}
