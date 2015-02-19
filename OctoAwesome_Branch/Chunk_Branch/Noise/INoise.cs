using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Noises
{
    public interface INoise
    {

        int Seed { get; }

        float[] GetNoise(int x, int width);

        float[,] GetNoise2(int x, int y, int width, int heigth);

        float[,,] GetNoise3(int x, int y, int z, int width, int heigth, int depth);

    }
}
