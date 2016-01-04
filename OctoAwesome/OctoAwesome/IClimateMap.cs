using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IClimateMap
    {
        IPlanet Planet { get; }

        float GetTemperature(Index3 blockIndex);
        int GetPrecipitation(Index3 blockIndex);
    }
}