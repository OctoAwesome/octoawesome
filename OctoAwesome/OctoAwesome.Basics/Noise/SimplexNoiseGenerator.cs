using System;
using System.Linq;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.Noise
{
    /// <summary>
    /// Noise generator implementation using simplex noise.
    /// </summary>
    public class SimplexNoiseGenerator : INoise
    {

        #region Props & Fields

        private byte[] permutations;
        private static readonly byte[] range = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255 };
        private int octaves;
        private float persistence;

        /// <inheritdoc />
        public int Seed { get; }

        /// <summary>
        /// Gets or sets the frequency to apply on the x axis.
        /// </summary>
        public float FrequencyX { get; set; }

        /// <summary>
        /// Gets or sets the frequency to apply on the y axis.
        /// </summary>
        public float FrequencyY { get; set; }

        /// <summary>
        /// Gets or sets the frequency to apply on the z axis.
        /// </summary>
        public float FrequencyZ { get; set; }

        /// <summary>
        /// Gets or sets the frequency to apply on the w axis.
        /// </summary>
        public float FrequencyW { get; set; }

        /// <summary>
        /// Gets or sets the factor to multiply the noise output by.
        /// </summary>
        /// <remarks>Normalizes noise output to be between [-<see cref="Factor"/>..<see cref="Factor"/>].</remarks>
        public float Factor { get; set; }

        /// <summary>
        /// Gets or sets the number of octaves to overlay.
        /// </summary>
        public int Octaves
        {
            get => octaves;
            set
            {
                octaves = value;
                RecalcMax();
            }
        }

        /// <summary>
        /// Gets or sets the persistence for the octave, by which factor each successive octave amplitude is multiplied.
        /// </summary>
        /// <remarks>
        /// Meaning first max amplitude is 1.0f, for the second octave it is <see cref="Persistence"/>,
        /// for the third one it is <see cref="Persistence"/>^2 etc.
        /// </remarks>
        public float Persistence
        {
            get => persistence;
            set
            {
                persistence = value;
                RecalcMax();
            }
        }

        private void CreatePermutations()
        {
            var rnd = new Random(Seed);
            byte[] temp = range.OrderBy(a => rnd.Next()).ToArray();
            permutations = temp.Concat(temp).ToArray();

        }

        private float MaxValue { get; set; }
        private void RecalcMax()
        {
            MaxValue = 0f;
            for (int i = 0; i < Octaves; i++)
            {
                MaxValue += (float)Math.Pow(Persistence, i);
            }
        }


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SimplexNoiseGenerator"/> class.
        /// </summary>
        /// <param name="seed">The random seed for the noise generator.</param>
        /// <param name="frequencyX">The frequency to apply on the x axis.</param>
        /// <param name="frequencyY">The frequency to apply on the y axis.</param>
        /// <param name="frequencyZ">The frequency to apply on the z axis.</param>
        /// <param name="frequencyW">The frequency to apply on the w axis.</param>
        public SimplexNoiseGenerator(int seed, float frequencyX = 1f, float frequencyY = 1f, float frequencyZ = 1f, float frequencyW = 1f)
        {
            Seed = seed;
            Octaves = 5;
            Persistence = 0.5f;
            FrequencyX = frequencyX;
            FrequencyY = frequencyY;
            FrequencyZ = frequencyZ;
            FrequencyW = frequencyW;
            Factor = 1;
            CreatePermutations();
        }

        #region NoiseMaps

        /// <inheritdoc />
        public float[] GetNoiseMap(int startX, int width)
        {
            float[] noise = new float[width];
            for (int x = 0; x < width; x++)
            {
                float frequencyX = FrequencyX;
                float amplitude = 1f;
                for (int i = 0; i < Octaves; i++)
                {
                    noise[x] += Noise((x + startX) * frequencyX) * amplitude;
                    amplitude *= Persistence;
                    frequencyX *= 2f;
                }
                noise[x] = noise[x] * Factor / MaxValue;
            }
            return noise;
        }

        /// <inheritdoc />
        public float[,] GetNoiseMap2D(int startX, int startY, int width, int height)
        {
            float[,] noise = new float[width, height];

            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++)
                {
                    float frequencyX = FrequencyX;
                    float frequencyZ = FrequencyZ;
                    float amplitude = 1f;
                    for (int i = 0; i < Octaves; i++)
                    {
                        noise[x, y] += Noise2D((x + startX) * frequencyX, (y + startY) * frequencyZ) * amplitude;
                        amplitude *= Persistence;
                        frequencyX *= 2f;
                        frequencyZ *= 2f;
                    }
                    noise[x, y] = noise[x, y] * Factor / MaxValue;
                }
            });
            return noise;
        }

        /// <inheritdoc />
        public void FillTileableNoiseMap2D(int startX, int startY, int sizeX, int sizeY, int tileSizeX, int tileSizeY,
            float[] array)
        {
            Parallel.For(0, sizeX, x =>
            {
                for (int y = 0; y < sizeY; y++)
                {
                    float frequencyX = FrequencyX;
                    float frequencyY = FrequencyY;
                    float amplitude = 1;

                    float u = (float)(x + startX) / tileSizeX;
                    float v = (float)(y + startY) / tileSizeY;

                    float nx = (float)(Math.Cos(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
                    float ny = (float)(Math.Cos(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));
                    float nz = (float)(Math.Sin(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
                    float nw = (float)(Math.Sin(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));

                    for (int i = 0; i < Octaves; i++)
                    {
                        array[y * Chunk.CHUNKSIZE_X + x] += Noise4D(nx * frequencyX, ny * frequencyY, nz * frequencyX, nw * frequencyY) * amplitude;

                        amplitude *= Persistence;
                        frequencyX *= 2f;
                        frequencyY *= 2f;
                    }
                    array[y * Chunk.CHUNKSIZE_X + x] = array[y * Chunk.CHUNKSIZE_X + x] * Factor / MaxValue;
                }
            });
        }

        /// <inheritdoc />
        public float[,,] GetNoiseMap3D(int startX, int startY, int startZ, int width, int height, int depth)
        {
            float[,,] noise = new float[width, height, depth];


            Parallel.For(0, width, x =>
            //for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        float frequencyX = FrequencyX;
                        float frequencyY = FrequencyY;
                        float frequencyZ = FrequencyZ;
                        float amplitude = 1f;
                        for (int i = 0; i < Octaves; i++)
                        {

                            noise[x, y, z] += Noise3D((x + startX) * frequencyX, (y + startY) * frequencyY, (z + startZ) * frequencyZ) * amplitude;
                            amplitude *= Persistence;
                            frequencyX *= 2;
                            frequencyY *= 2;
                            frequencyZ *= 2;

                        }
                        noise[x, y, z] = noise[x, y, z] * Factor / MaxValue;
                    }
                }
            });
            return noise;
        }

        /// <inheritdoc />
        public float[,,] GetTileableNoiseMap3D(int startX, int startY, int startZ, int width, int height, int depth, int tileSizeX, int tileSizeY)
        {
            float[,,] noise = new float[width, height, depth];

            Parallel.For(0, width, x =>
            //for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {

                        float frequencyX = FrequencyX;
                        float frequencyY = FrequencyY;
                        float frequencyZ = FrequencyZ;
                        float amplitude = 1;

                        float u = (float)(x + startX) / tileSizeX;
                        float v = (float)(y + startY) / tileSizeY;

                        float nx = (float)(Math.Cos(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
                        float ny = (float)(Math.Cos(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));
                        float nw = (float)(Math.Sin(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
                        float nv = (float)(Math.Sin(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));

                        for (int i = 0; i < Octaves; i++)
                        {
                            noise[x, y, z] += Noise5D(nx * frequencyX, ny * frequencyY, z * frequencyZ, nw * frequencyX, nv * frequencyY) * amplitude;

                            amplitude *= Persistence;
                            frequencyX *= 2f;
                            frequencyY *= 2f;
                            frequencyZ *= 2f;

                        }
                        noise[x, y, z] = noise[x, y, z] * Factor / MaxValue;
                    }
                }
            });

            return noise;
        }

        /// <inheritdoc />
        public float[,,,] GetNoiseMap4D(int startX, int startY, int startZ, int startW, int width, int height, int depth, int wDepth)
        {
            float[,,,] noise = new float[width, height, depth, wDepth];

            Parallel.For(0, width, x =>
            //for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        for (int w = 0; w < wDepth; w++)
                        {
                            float frequencyX = FrequencyX;
                            float frequencyY = FrequencyY;
                            float frequencyZ = FrequencyZ;
                            float frequencyW = FrequencyW;
                            float amplitude = 1f;
                            for (int i = 0; i < Octaves; i++)
                            {
                                noise[x, y, z, w] += Noise4D((x + startX) * frequencyX, (y + startY) * frequencyY, (z + startZ) * frequencyZ, (w + startW) * frequencyW) * amplitude;
                                amplitude *= Persistence;
                                frequencyX *= 2;
                                frequencyY *= 2;
                                frequencyZ *= 2;
                                frequencyW *= 2;
                            }
                            noise[x, y, z, w] = noise[x, y, z, w] * Factor / MaxValue;
                        }
                    }
                }
            });
            return noise;
        }

        #endregion

        #region SingleNoise

        /// <inheritdoc />
        public float GetNoise(int x)
        {
            float noise = 0;
            float frequencyX = FrequencyX;
            float amplitude = 1f;
            for (int i = 0; i < Octaves; i++)
            {
                noise += Noise(x * frequencyX) * amplitude;
                amplitude *= Persistence;
                frequencyX *= 2;

            }
            return noise * Factor / MaxValue;
        }

        /// <inheritdoc />
        public float GetNoise2D(int x, int y)
        {
            float noise = 0;
            float frequencyX = FrequencyX;
            float frequencyY = FrequencyY;
            float amplitude = 1;
            for (int i = 0; i < Octaves; i++)
            {
                noise += Noise2D(x * frequencyX, y * frequencyY) * amplitude;
                amplitude *= Persistence;
                frequencyX *= 2;
                frequencyY *= 2;

            }
            return noise * Factor / MaxValue;
        }

        /// <inheritdoc />
        public float GetTileableNoise2D(int x, int y, int tileSizeX, int tileSizeY)
        {
            float noise = 0;
            float frequencyX = FrequencyX;
            float frequencyY = FrequencyY;
            float amplitude = 1;

            float u = (float)x / tileSizeX;
            float v = (float)y / tileSizeY;

            float nx = (float)(Math.Cos(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
            float ny = (float)(Math.Cos(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));
            float nz = (float)(Math.Sin(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
            float nw = (float)(Math.Sin(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));

            for (int i = 0; i < Octaves; i++)
            {
                noise += Noise4D(nx * frequencyX, ny * frequencyY, nz * frequencyX, nw * frequencyY) * amplitude;

                amplitude *= Persistence;
                frequencyX *= 2f;
                frequencyY *= 2f;

            }
            return noise * Factor / MaxValue;
        }

        /// <inheritdoc />
        public float GetNoise3D(int x, int y, int z)
        {
            float noise = 0;
            float frequencyX = FrequencyX;
            float frequencyY = FrequencyY;
            float frequencyZ = FrequencyZ;
            float amplitude = 1;

            for (int i = 0; i < Octaves; i++)
            {
                noise += Noise3D(x * frequencyX, y * frequencyY, z * frequencyZ) * amplitude;

                amplitude *= Persistence;
                frequencyX *= 2f;
                frequencyY *= 2f;
                frequencyZ *= 2f;
            }
            return noise * Factor / MaxValue;
        }

        /// <inheritdoc />
        public float GetTileableNoise3D(int x, int y, int z, int tileSizeX, int tileSizeY)
        {
            float noise = 0;
            float frequencyX = FrequencyX;
            float frequencyY = FrequencyY;
            float frequencyZ = FrequencyZ;
            float amplitude = 1;

            float u = (float)x / tileSizeX;
            float v = (float)y / tileSizeY;

            float nx = (float)(Math.Cos(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
            float ny = (float)(Math.Cos(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));
            float nw = (float)(Math.Sin(u * 2 * Math.PI) * tileSizeX / (2 * Math.PI));
            float nv = (float)(Math.Sin(v * 2 * Math.PI) * tileSizeY / (2 * Math.PI));

            for (int i = 0; i < Octaves; i++)
            {
                noise += Noise5D(nx * frequencyX, ny * frequencyY, z * frequencyZ, nw * frequencyX, nv * frequencyY) * amplitude;

                amplitude *= Persistence;
                frequencyX *= 2f;
                frequencyY *= 2f;
                frequencyZ *= 2f;

            }
            return noise * Factor / MaxValue;
        }

        /// <inheritdoc />
        public float GetNoise4D(int x, int y, int z, int w)
        {
            float noise = 0;
            float frequencyX = FrequencyX;
            float frequencyY = FrequencyY;
            float frequencyZ = FrequencyZ;
            float frequencyW = FrequencyW;
            float amplitude = 1;

            for (int i = 0; i < Octaves; i++)
            {
                noise += Noise4D(x * frequencyX, y * frequencyY, z * frequencyZ, w * frequencyW) * amplitude;

                amplitude *= Persistence;
                frequencyX *= 2f;
                frequencyY *= 2f;
                frequencyZ *= 2f;
                frequencyW *= 2f;
            }
            return noise * Factor / MaxValue;
        }

        #endregion

        #region Noise Implementation

        private static readonly int[] Grad3 = { 1, 1, 0, -1, 1, 0, 1, -1, 0, -1, -1, 0, 1, 0, 1, -1, 0, 1, 1, 0, -1, -1, 0, -1, 0, 1, 1, 0, -1, 1, 0, 1, -1, 0, -1, -1 };
        private static readonly int[] Grad4 = { 0, 1, 1, 1, 0, 1, 1, -1, 0, 1, -1, 1, 0, 1, -1, -1, 0, -1, 1, 1, 0, -1, 1, -1, 0, -1, -1, 1, 0, -1, -1, -1, 1, 0, 1, 1, 1, 0, 1, -1, 1, 0, -1, 1, 1, 0, -1, -1, -1, 0, 1, 1, -1, 0, 1, -1, -1, 0, -1, 1, -1, 0, -1, -1, 1, 1, 0, 1, 1, 1, 0, -1, 1, -1, 0, 1, 1, -1, 0, -1, -1, 1, 0, 1, -1, 1, 0, -1, -1, -1, 0, 1, -1, -1, 0, -1, 1, 1, 1, 0, 1, 1, -1, 0, 1, -1, 1, 0, 1, -1, -1, 0, -1, 1, 1, 0, -1, 1, -1, 0, -1, -1, 1, 0, -1, -1, -1, 0 };
        private static readonly int[] Grad5 = { 0, -1, -1, -1, -1, 0, -1, -1, -1, 1, 0, -1, -1, 1, -1, 0, -1, -1, 1, 1, 0, -1, 1, -1, -1, 0, -1, 1, -1, 1, 0, -1, 1, 1, -1, 0, -1, 1, 1, 1, 0, 1, -1, -1, -1, 0, 1, -1, -1, 1, 0, 1, -1, 1, -1, 0, 1, -1, 1, 1, 0, 1, 1, -1, -1, 0, 1, 1, -1, 1, 0, 1, 1, 1, -1, 0, 1, 1, 1, 1, -1, 0, -1, -1, -1, -1, 0, -1, -1, 1, -1, 0, -1, 1, -1, -1, 0, -1, 1, 1, -1, 0, 1, -1, -1, -1, 0, 1, -1, 1, -1, 0, 1, 1, -1, -1, 0, 1, 1, 1, 1, 0, -1, -1, -1, 1, 0, -1, -1, 1, 1, 0, -1, 1, -1, 1, 0, -1, 1, 1, 1, 0, 1, -1, -1, 1, 0, 1, -1, 1, 1, 0, 1, 1, -1, 1, 0, 1, 1, 1, -1, -1, 0, -1, -1, -1, -1, 0, -1, 1, -1, -1, 0, 1, -1, -1, -1, 0, 1, 1, -1, 1, 0, -1, -1, -1, 1, 0, -1, 1, -1, 1, 0, 1, -1, -1, 1, 0, 1, 1, 1, -1, 0, -1, -1, 1, -1, 0, -1, 1, 1, -1, 0, 1, -1, 1, -1, 0, 1, 1, 1, 1, 0, -1, -1, 1, 1, 0, -1, 1, 1, 1, 0, 1, -1, 1, 1, 0, 1, 1, -1, -1, -1, 0, -1, -1, -1, -1, 0, 1, -1, -1, 1, 0, -1, -1, -1, 1, 0, 1, -1, 1, -1, 0, -1, -1, 1, -1, 0, 1, -1, 1, 1, 0, -1, -1, 1, 1, 0, 1, 1, -1, -1, 0, -1, 1, -1, -1, 0, 1, 1, -1, 1, 0, -1, 1, -1, 1, 0, 1, 1, 1, -1, 0, -1, 1, 1, -1, 0, 1, 1, 1, 1, 0, -1, 1, 1, 1, 0, 1, -1, -1, -1, -1, 0, -1, -1, -1, 1, 0, -1, -1, 1, -1, 0, -1, -1, 1, 1, 0, -1, 1, -1, -1, 0, -1, 1, -1, 1, 0, -1, 1, 1, -1, 0, -1, 1, 1, 1, 0, 1, -1, -1, -1, 0, 1, -1, -1, 1, 0, 1, -1, 1, -1, 0, 1, -1, 1, 1, 0, 1, 1, -1, -1, 0, 1, 1, -1, 1, 0, 1, 1, 1, -1, 0, 1, 1, 1, 1, 0 };

        private static float DotProduct(int dIndex, float x, float y)
        {
            return (Grad3[dIndex] * x) + (Grad3[dIndex + 1] * y);
        }
        private static float DotProduct(int dIndex, float x, float y, float z)
        {
            return (Grad3[dIndex] * x) + (Grad3[dIndex + 1] * y) + (Grad3[dIndex + 2] * z);
        }
        private static float DotProduct(int dIndex, float x, float y, float z, float w)
        {
            return (Grad4[dIndex] * x) + (Grad4[dIndex + 1] * y) + (Grad4[dIndex + 2] * z) + (Grad4[dIndex + 3] * w);
        }
        private static float DotProduct(int dIndex, float x, float y, float z, float w, float v)
        {
            return (Grad5[dIndex] * x) + (Grad5[dIndex + 1] * y) + (Grad5[dIndex + 2] * z) + (Grad5[dIndex + 3] * w) + (Grad5[dIndex + 4] * v);
        }
        private static readonly float F2 = (float)(0.5f * (Math.Sqrt(3.0f) - 1.0f));
        private static readonly float G2 = (float)(3.0f - Math.Sqrt(3.0f)) / 6.0f;
        private static readonly float F3 = 1.0f / 3.0f;
        private static readonly float G3 = 1.0f / 6.0f;
        private static readonly float F4 = (float)(Math.Sqrt(5.0) - 1.0) / 4.0f;
        private static readonly float G4 = (float)(5.0 - Math.Sqrt(5.0)) / 20.0f;
        private static readonly float F5 = (float)((Math.Sqrt(6.0) - 1) / 5.0);
        private static readonly float G5 = (float)((6.0 - Math.Sqrt(6.0)) / 30.0);

        private static int FastFloor(float val)
        {
            return val > 0 ? (int)val : (int)val - 1;
        }

        private float NoiseFunction(int x)
        {
            unchecked
            {
                int n = x * Seed;
                n = n << 13 ^ n;
                n *= n * 15731;
                n += 789221;
                n *= n;
                n += 1376312589;
                n = n & 0x7fffffff;

                return (float)(1.0 - n / 1073741824.0);
            }
        }
        private float LinearInterpolation(float a, float b, float x)
        {
            return a + (b - a) * x;
        }
        private float Noise(float x)
        {

            int integer_X = (int)x;
            float fractional_X = x - integer_X;

            float v1 = NoiseFunction(integer_X);
            float v2 = NoiseFunction(integer_X + 1);


            return LinearInterpolation(v1, v2, fractional_X);
        }

        private float Noise2D(float xin, float yin)
        {
            float n0, n1, n2;

            float s = (xin + yin) * F2;

            int i = FastFloor(xin + s);
            int j = FastFloor(yin + s);

            float t = (i + j) * G2;
            float X0 = i - t;

            float Y0 = j - t;
            float x0 = xin - X0;

            float y0 = yin - Y0;

            int i1, j1;

            if (x0 > y0)
            { i1 = 1; j1 = 0; }

            else
            { i1 = 0; j1 = 1; }

            float x1 = x0 - i1 + G2;

            float y1 = y0 - j1 + G2;
            float x2 = x0 - 1.0f + 2.0f * G2;

            float y2 = y0 - 1.0f + 2.0f * G2;

            int ii = i & 255;
            int jj = j & 255;
            int gi0 = permutations[ii + permutations[jj]] % 12;
            int gi1 = permutations[ii + i1 + permutations[jj + j1]] % 12;
            int gi2 = permutations[ii + 1 + permutations[jj + 1]] % 12;

            float t0 = 0.5f - x0 * x0 - y0 * y0;
            if (t0 <= 0)
                n0 = 0.0f;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * DotProduct(gi0 * 3, x0, y0);
            }
            float t1 = 0.5f - x1 * x1 - y1 * y1;
            if (t1 <= 0)
                n1 = 0.0f;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * DotProduct(gi1 * 3, x1, y1);
            }
            float t2 = 0.5f - x2 * x2 - y2 * y2;
            if (t2 <= 0)
                n2 = 0.0f;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * DotProduct(gi2 * 3, x2, y2);
            }
            return 70.0f * (n0 + n1 + n2);
        }

        private float Noise3D(float x, float y, float z)
        {
            float n0, n1, n2, n3;
            float s = (x + y + z) * F3;
            int i = FastFloor(x + s);
            int j = FastFloor(y + s);
            int k = FastFloor(z + s);
            float t = (i + j + k) * G3;
            float X0 = i - t;
            float Y0 = j - t;
            float Z0 = k - t;
            float x0 = x - X0;
            float y0 = y - Y0;
            float z0 = z - Z0;

            int i1, j1, k1;
            int i2, j2, k2;
            if (x0 >= y0)
            {
                if (y0 >= z0)
                { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 1; k2 = 0; }
                else if (x0 >= z0)
                { i1 = 1; j1 = 0; k1 = 0; i2 = 1; j2 = 0; k2 = 1; }
                else
                { i1 = 0; j1 = 0; k1 = 1; i2 = 1; j2 = 0; k2 = 1; }
            }
            else
            {
                if (y0 < z0)
                { i1 = 0; j1 = 0; k1 = 1; i2 = 0; j2 = 1; k2 = 1; }
                else if (x0 < z0)
                { i1 = 0; j1 = 1; k1 = 0; i2 = 0; j2 = 1; k2 = 1; }
                else
                { i1 = 0; j1 = 1; k1 = 0; i2 = 1; j2 = 1; k2 = 0; }
            }

            float x1 = x0 - i1 + G3;
            float y1 = y0 - j1 + G3;
            float z1 = z0 - k1 + G3;
            float x2 = x0 - i2 + 2.0f * G3;
            float y2 = y0 - j2 + 2.0f * G3;
            float z2 = z0 - k2 + 2.0f * G3;
            float x3 = x0 - 1.0f + 3.0f * G3;
            float y3 = y0 - 1.0f + 3.0f * G3;
            float z3 = z0 - 1.0f + 3.0f * G3;

            int ii = i & 255;
            int jj = j & 255;
            int kk = k & 255;

            int gi0 = permutations[ii + permutations[jj + permutations[kk]]] % 12;
            int gi1 = permutations[ii + i1 + permutations[jj + j1 + permutations[kk + k1]]] % 12;
            int gi2 = permutations[ii + i2 + permutations[jj + j2 + permutations[kk + k2]]] % 12;
            int gi3 = permutations[ii + 1 + permutations[jj + 1 + permutations[kk + 1]]] % 12;

            float t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0;
            if (t0 <= 0)
                n0 = 0.0f;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * DotProduct(gi0 * 3, x0, y0, z0);
            }
            float t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1;
            if (t1 <= 0)
                n1 = 0.0f;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * DotProduct(gi1 * 3, x1, y1, z1);
            }
            float t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2;
            if (t2 <= 0)
                n2 = 0.0f;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * DotProduct(gi2 * 3, x2, y2, z2);
            }
            float t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3;
            if (t3 <= 0)
                n3 = 0.0f;
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * DotProduct(gi3 * 3, x3, y3, z3);
            }

            return 32.0f * (n0 + n1 + n2 + n3);
        }

        private float Noise4D(float x, float y, float z, float w)
        {

            float n0, n1, n2, n3, n4;

            float s = (x + y + z + w) * F4;
            int i = FastFloor(x + s);
            int j = FastFloor(y + s);
            int k = FastFloor(z + s);
            int l = FastFloor(w + s);
            float t = (i + j + k + l) * G4;
            float X0 = i - t;
            float Y0 = j - t;
            float Z0 = k - t;
            float W0 = l - t;
            float x0 = x - X0;
            float y0 = y - Y0;
            float z0 = z - Z0;
            float w0 = w - W0;

            int rankx = 0;
            int ranky = 0;
            int rankz = 0;
            int rankw = 0;
            if (x0 > y0)
                rankx++;
            else
                ranky++;
            if (x0 > z0)
                rankx++;
            else
                rankz++;
            if (x0 > w0)
                rankx++;
            else
                rankw++;
            if (y0 > z0)
                ranky++;
            else
                rankz++;
            if (y0 > w0)
                ranky++;
            else
                rankw++;
            if (z0 > w0)
                rankz++;
            else
                rankw++;
            int i1, j1, k1, l1;
            int i2, j2, k2, l2;
            int i3, j3, k3, l3;

            i1 = rankx >= 3 ? 1 : 0;
            j1 = ranky >= 3 ? 1 : 0;
            k1 = rankz >= 3 ? 1 : 0;
            l1 = rankw >= 3 ? 1 : 0;

            i2 = rankx >= 2 ? 1 : 0;
            j2 = ranky >= 2 ? 1 : 0;
            k2 = rankz >= 2 ? 1 : 0;
            l2 = rankw >= 2 ? 1 : 0;

            i3 = rankx >= 1 ? 1 : 0;
            j3 = ranky >= 1 ? 1 : 0;
            k3 = rankz >= 1 ? 1 : 0;
            l3 = rankw >= 1 ? 1 : 0;

            float x1 = x0 - i1 + G4;
            float y1 = y0 - j1 + G4;
            float z1 = z0 - k1 + G4;
            float w1 = w0 - l1 + G4;
            float x2 = x0 - i2 + 2.0f * G4;
            float y2 = y0 - j2 + 2.0f * G4;
            float z2 = z0 - k2 + 2.0f * G4;
            float w2 = w0 - l2 + 2.0f * G4;
            float x3 = x0 - i3 + 3.0f * G4;
            float y3 = y0 - j3 + 3.0f * G4;
            float z3 = z0 - k3 + 3.0f * G4;
            float w3 = w0 - l3 + 3.0f * G4;
            float x4 = x0 - 1.0f + 4.0f * G4;
            float y4 = y0 - 1.0f + 4.0f * G4;
            float z4 = z0 - 1.0f + 4.0f * G4;
            float w4 = w0 - 1.0f + 4.0f * G4;

            int ii = i & 255;
            int jj = j & 255;
            int kk = k & 255;
            int ll = l & 255;
            int gi0 = permutations[ii + permutations[jj + permutations[kk + permutations[ll]]]] % 32;
            int gi1 = permutations[ii + i1 + permutations[jj + j1 + permutations[kk + k1 + permutations[ll + l1]]]] % 32;
            int gi2 = permutations[ii + i2 + permutations[jj + j2 + permutations[kk + k2 + permutations[ll + l2]]]] % 32;
            int gi3 = permutations[ii + i3 + permutations[jj + j3 + permutations[kk + k3 + permutations[ll + l3]]]] % 32;
            int gi4 = permutations[ii + 1 + permutations[jj + 1 + permutations[kk + 1 + permutations[ll + 1]]]] % 32;

            float t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0 - w0 * w0;
            if (t0 < 0)
                n0 = 0.0f;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * DotProduct(gi0 * 4, x0, y0, z0, w0);
            }
            float t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1 - w1 * w1;
            if (t1 < 0)
                n1 = 0.0f;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * DotProduct(gi1 * 4, x1, y1, z1, w1);
            }
            float t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2 - w2 * w2;
            if (t2 < 0)
                n2 = 0.0f;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * DotProduct(gi2 * 4, x2, y2, z2, w2);
            }
            float t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3 - w3 * w3;
            if (t3 < 0)
                n3 = 0.0f;
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * DotProduct(gi3 * 4, x3, y3, z3, w3);
            }
            float t4 = 0.6f - x4 * x4 - y4 * y4 - z4 * z4 - w4 * w4;
            if (t4 < 0)
                n4 = 0.0f;
            else
            {
                t4 *= t4;
                n4 = t4 * t4 * DotProduct(gi4 * 4, x4, y4, z4, w4);
            }

            return 27.0f * (n0 + n1 + n2 + n3 + n4);
        }

        private float Noise5D(float x, float y, float z, float w, float v)
        {
            float n0, n1, n2, n3, n4, n5;

            float s = (x + y + z + w + v) * F5;
            int i = FastFloor(x + s);
            int j = FastFloor(y + s);
            int k = FastFloor(z + s);
            int l = FastFloor(w + s);
            int m = FastFloor(v + s);
            float t = (i + j + k + l + m) * G5;
            float X0 = i - t;
            float Y0 = j - t;
            float Z0 = k - t;
            float W0 = l - t;
            float V0 = m - t;
            float x0 = x - X0;
            float y0 = y - Y0;
            float z0 = z - Z0;
            float w0 = w - W0;
            float v0 = v - V0;

            int rankx = 0;
            int ranky = 0;
            int rankz = 0;
            int rankw = 0;
            int rankv = 0;
            if (x0 > y0)
                rankx++;
            else
                ranky++;
            if (x0 > z0)
                rankx++;
            else
                rankz++;
            if (x0 > w0)
                rankx++;
            else
                rankw++;
            if (x0 > v0)
                rankx++;
            else
                rankv++;

            if (y0 > z0)
                ranky++;
            else
                rankz++;
            if (y0 > w0)
                ranky++;
            else
                rankw++;
            if (y0 > v0)
                ranky++;
            else
                rankv++;

            if (z0 > w0)
                rankz++;
            else
                rankw++;
            if (z0 > v0)
                rankz++;
            else
                rankv++;

            int i1, j1, k1, l1, m1;
            int i2, j2, k2, l2, m2;
            int i3, j3, k3, l3, m3;
            int i4, j4, k4, l4, m4;

            i1 = rankx >= 4 ? 1 : 0;
            j1 = ranky >= 4 ? 1 : 0;
            k1 = rankz >= 4 ? 1 : 0;
            l1 = rankw >= 4 ? 1 : 0;
            m1 = rankv >= 4 ? 1 : 0;

            i2 = rankx >= 3 ? 1 : 0;
            j2 = ranky >= 3 ? 1 : 0;
            k2 = rankz >= 3 ? 1 : 0;
            l2 = rankw >= 3 ? 1 : 0;
            m2 = rankv >= 3 ? 1 : 0;

            i3 = rankx >= 2 ? 1 : 0;
            j3 = ranky >= 2 ? 1 : 0;
            k3 = rankz >= 2 ? 1 : 0;
            l3 = rankw >= 2 ? 1 : 0;
            m3 = rankv >= 2 ? 1 : 0;

            i4 = rankx >= 1 ? 1 : 0;
            j4 = ranky >= 1 ? 1 : 0;
            k4 = rankz >= 1 ? 1 : 0;
            l4 = rankw >= 1 ? 1 : 0;
            m4 = rankv >= 1 ? 1 : 0;

            float x1 = x0 - i1 + G5;
            float y1 = y0 - j1 + G5;
            float z1 = z0 - k1 + G5;
            float w1 = w0 - l1 + G5;
            float v1 = v0 - m1 + G5;

            float x2 = x0 - i2 + 2.0f * G5;
            float y2 = y0 - j2 + 2.0f * G5;
            float z2 = z0 - k2 + 2.0f * G5;
            float w2 = w0 - l2 + 2.0f * G5;
            float v2 = v0 - m2 + 2.0f * G5;

            float x3 = x0 - i3 + 3.0f * G5;
            float y3 = y0 - j3 + 3.0f * G5;
            float z3 = z0 - k3 + 3.0f * G5;
            float w3 = w0 - l3 + 3.0f * G5;
            float v3 = v0 - m3 + 3.0f * G5;

            float x4 = x0 - i4 + 4.0f * G5;
            float y4 = y0 - j4 + 4.0f * G5;
            float z4 = z0 - k4 + 4.0f * G5;
            float w4 = w0 - l4 + 4.0f * G5;
            float v4 = v0 - m4 + 4.0f * G5;

            float x5 = x0 - 1.0f + 5.0f * G5;
            float y5 = y0 - 1.0f + 5.0f * G5;
            float z5 = z0 - 1.0f + 5.0f * G5;
            float w5 = w0 - 1.0f + 5.0f * G5;
            float v5 = v0 - 1.0f + 5.0f * G5;

            int ii = i & 255;
            int jj = j & 255;
            int kk = k & 255;
            int ll = l & 255;
            int mm = m & 255;
            int gi0 = permutations[ii + permutations[jj + permutations[kk + permutations[ll + permutations[mm]]]]] % 80;
            int gi1 = permutations[ii + i1 + permutations[jj + j1 + permutations[kk + k1 + permutations[ll + l1 + permutations[mm + m1]]]]] % 80;
            int gi2 = permutations[ii + i2 + permutations[jj + j2 + permutations[kk + k2 + permutations[ll + l2 + permutations[mm + m2]]]]] % 80;
            int gi3 = permutations[ii + i3 + permutations[jj + j3 + permutations[kk + k3 + permutations[ll + l3 + permutations[mm + m3]]]]] % 80;
            int gi4 = permutations[ii + i4 + permutations[jj + j4 + permutations[kk + k4 + permutations[ll + l4 + permutations[mm + m4]]]]] % 80;
            int gi5 = permutations[ii + 1 + permutations[jj + 1 + permutations[kk + 1 + permutations[ll + 1 + permutations[mm + 1]]]]] % 80;

            float t0 = 0.6f - x0 * x0 - y0 * y0 - z0 * z0 - w0 * w0 - v0 * v0;
            if (t0 < 0)
                n0 = 0.0f;
            else
            {
                t0 *= t0;
                n0 = t0 * t0 * DotProduct(gi0 * 5, x0, y0, z0, w0, v0);
            }
            float t1 = 0.6f - x1 * x1 - y1 * y1 - z1 * z1 - w1 * w1 - v1 * v1;
            if (t1 < 0)
                n1 = 0.0f;
            else
            {
                t1 *= t1;
                n1 = t1 * t1 * DotProduct(gi1 * 5, x1, y1, z1, w1, v1);
            }
            float t2 = 0.6f - x2 * x2 - y2 * y2 - z2 * z2 - w2 * w2 - v2 * v2;
            if (t2 < 0)
                n2 = 0.0f;
            else
            {
                t2 *= t2;
                n2 = t2 * t2 * DotProduct(gi2 * 5, x2, y2, z2, w2, v2);
            }
            float t3 = 0.6f - x3 * x3 - y3 * y3 - z3 * z3 - w3 * w3 - v3 * v3;
            if (t3 < 0)
                n3 = 0.0f;
            else
            {
                t3 *= t3;
                n3 = t3 * t3 * DotProduct(gi3 * 5, x3, y3, z3, w3, v3);
            }
            float t4 = 0.6f - x4 * x4 - y4 * y4 - z4 * z4 - w4 * w4 - v4 * v4;
            if (t4 < 0)
                n4 = 0.0f;
            else
            {
                t4 *= t4;
                n4 = t4 * t4 * DotProduct(gi4 * 5, x4, y4, z4, w4, v4);
            }
            float t5 = 0.6f - x5 * x5 - y5 * y5 - z5 * z5 - w5 * w5 - v5 * v5;
            if (t5 < 0)
                n5 = 0.0f;
            else
            {
                t5 *= t5;
                n5 = t5 * t5 * DotProduct(gi5 * 5, x5, y5, z5, w5, v5);
            }
            return 18.0f * (n0 + n1 + n2 + n3 + n4 + n5);

        }

        #endregion
    }
}
