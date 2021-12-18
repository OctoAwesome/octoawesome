using System;
using System.Diagnostics;
using System.Linq;
using OctoAwesome.Basics.Noise;

namespace OctoAwesome.Basics.Biomes
{
    /// <summary>
    /// Base class for biomes with sub biomes to choose from.
    /// </summary>
    public abstract class LargeBiomeBase : BiomeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LargeBiomeBase"/> class.
        /// </summary>
        /// <param name="planet">The planet the biome should be generated on.</param>
        /// <param name="minValue">The minimum mapping value where the biome is generated.</param>
        /// <param name="maxValue">The maximum mapping value where the biome is generated.</param>
        /// <param name="valueRangeOffset">The value offset the biome height starts at.</param>
        /// <param name="valueRange">The value range the biome height has.</param>
        /// <param name="biomeNoiseGenerator">The noise generator used for generating the biome features.</param>
        public LargeBiomeBase(IPlanet planet, float minValue, float maxValue, float valueRangeOffset, float valueRange, INoise biomeNoiseGenerator)
            : base(planet, minValue, maxValue, valueRangeOffset, valueRange, biomeNoiseGenerator)
        {
        }

        /// <summary>
        /// Sorts the sub biomes byt their min and max values.
        /// </summary>
        /// <remarks>Needs to be called for <see cref="ChooseBiome(float,out OctoAwesome.Basics.Biomes.IBiome?)"/> to function.</remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown if <see cref="IBiome.MinValue"/> or <see cref="IBiome.MaxValue"/> are out of range.
        /// </exception>
        protected void SortSubBiomes()
        {
            SubBiomes.Sort((a, b) => a.MinValue.CompareTo(b.MinValue));

            if (SubBiomes.Count > 0 && (SubBiomes.First().MinValue > 0f || SubBiomes.Last().MaxValue < 1f))
            {
                throw new InvalidOperationException("MinValue oder MaxValue der Biome nicht in gültigem Bereich");
            }
        }

        /// <summary>
        /// Choose a biome corresponding to a float mapping value.
        /// </summary>
        /// <param name="value">The mapping value to get the biome for.</param>
        /// <param name="secondBiome">The second biome for interpolation; <c>null</c> for no interpolation.</param>
        /// <returns>The biome best matching the mapping value;<c>null</c> if no matching biome was found.</returns>
        /// <seealso cref="ChooseBiome(float,out int)"/>
        protected IBiome? ChooseBiome(float value, out IBiome? secondBiome)
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
                betweenPossible = value > SubBiomes[i].MaxValue;
            }
            return null;
        }

        /// <summary>
        /// Choose a biome corresponding to a float mapping value.
        /// </summary>
        /// <param name="value">The mapping value to get the biome for.</param>
        /// <param name="secondBiome">The second biome index for interpolation; <c>-1</c> for no interpolation.</param>
        /// <returns>The biome index best matching the mapping value;<c>-1</c> if no matching biome was found.</returns>
        /// <seealso cref="ChooseBiome(float,out OctoAwesome.Basics.Biomes.IBiome?)"/>
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
                betweenPossible = value > SubBiomes[i].MaxValue;
            }
            return -1;
        }

        /// <summary>
        /// Calculates an interpolated value for a mapping value.
        /// </summary>
        /// <param name="region">The mapping value to choose the biome for.</param>
        /// <param name="biome1">The matching primary biome found;c<c>null</c> if no biome was matched.</param>
        /// <param name="biome2">The matching secondary biome found;c<c>null</c> if no biome was matched.</param>
        /// <returns>The interpolated value.</returns>
        protected float CalculateInterpolationValue(float region, out IBiome? biome1, out IBiome? biome2)
        {
            biome1 = ChooseBiome(region, out biome2);

            return CalculateInterpolationValue(region, biome1, biome2);
        }

        /// <summary>
        /// Calculates an interpolated value for a mapping value.
        /// </summary>
        /// <param name="region">The mapping value to choose the interpolation value with.</param>
        /// <param name="biome1">The matching primary biome to use for interpolation.</param>
        /// <param name="biome2">The matching secondary biome to use for interpolation.</param>
        /// <returns>The interpolated value.</returns>
        protected float CalculateInterpolationValue(float region, IBiome? biome1, IBiome? biome2)
        {
            if (biome2 != null)
            {
                Debug.Assert(biome1 != null, nameof(biome1) + " != null");
                float diff = biome2.MinValue - biome1.MaxValue;
                region -= biome1.MaxValue;
                region /= diff;
                return CurveFunction(region);
            }

            return 0f;
        }

        /// <summary>
        /// The curve function applied before sampling values.
        /// </summary>
        /// <param name="inputValue">The input value for the curve function [0..1].</param>
        /// <returns>The mapped value result [0..1].</returns>
        protected virtual float CurveFunction(float inputValue)
        {
            return inputValue;
        }
    }
}
